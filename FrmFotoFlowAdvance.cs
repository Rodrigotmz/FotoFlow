using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using FotoFlow.Core;

namespace FotoFlow
{
    public partial class FrmFotoFlowAdvance : Form
    {
        private readonly Color backgroundStart = Color.YellowGreen;
        private readonly Color backgroundStop = Color.Red;
        private readonly Color backgroundDisabled = Color.Gainsboro;
        private readonly Color textStart = SystemColors.ControlLightLight;
        private readonly Color textDisabled = Color.Black;

        private readonly IFotoFlowService _service;
        private readonly AdbRunner _adbRunner;

        private FileSystemWatcher? _incomingWatcher;
        private string? _incomingFolder;

        private readonly ConcurrentQueue<string> _pendingIncomingFiles = new();
        private readonly HashSet<string> _seenIncomingFiles = new(StringComparer.OrdinalIgnoreCase);
        private readonly SemaphoreSlim _incomingLock = new(1, 1);

        private volatile bool _receiveMultipleEnabled = true;

        private string? _currentIncomingPath;
        private string? _currentDeviceFileName;
        private string? _currentOriginalFileName;
        private string? _currentOriginalExtension;
        private bool _loadedSettings;

        public FrmFotoFlowAdvance()
        {
            InitializeComponent();
            _service = new FotoFlowService();
            _adbRunner = new AdbRunner();
            _service.ProgressChanged += OnServiceProgress;
            _service.StatusChanged += OnServiceStatus;
            _service.ErrorOccurred += OnServiceError;
            this.AcceptButton = btnSaveFile;

            Activated += (_, _) => AppRuntimeState.LastMode = FotoFlowMode.Advance;
        }

        #region Control Event Form Handlers
        private void FrmFotoFlowAdvance_Load(object sender, EventArgs e)
        {
            pgrbrStatus.InitConfigurationProgressBar();
            btnStart.StyleButtonEnabled(true, backgroundStart, textStart);
            btnStop.StyleButtonEnabled(false, backgroundDisabled, textDisabled);
            txtPath.TrySelectFolder(promptUser: false);
            LoadLastPathIfAny();
            txtPath.Leave += (_, _) => PersistPathIfValid();

            pcbxPreview.SizeMode = PictureBoxSizeMode.Zoom;
            btnSaveFile.Enabled = false;
            txtNameFilePrimary.Text = string.Empty;
            txtNewNameFile.Text = string.Empty;
            txtNewNameFile.Enabled = false;

            _receiveMultipleEnabled = chbxReceiveMultiple.Checked;
            chbxReceiveMultiple.CheckedChanged += chbxReceiveMultiple_CheckedChanged;

            btnSelectPath.Click += btnSelectPath_Click;

            lblSatus.Text = "Listo. Inicia la transferencia para recibir nuevas fotos.";
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            string destination = txtPath.Text;
            lblSatus.Text = "Preparando recepción de nuevas fotos...";
            if (string.IsNullOrEmpty(destination))
            {
                MessageBox.Show("Por favor, ingresa una ruta de destino.");
                lblSatus.Text = "Listo. Ingresa una ruta para guardar.";
                return;
            }
            PersistPathIfValid();

            _incomingFolder = Path.Combine(Path.GetTempPath(), "FotoFlow", "Incoming");
            Directory.CreateDirectory(_incomingFolder);
            ClearIncomingFolder(_incomingFolder);
            StartIncomingWatcher(_incomingFolder);

            btnStart.StyleButtonEnabled(false, backgroundDisabled, textDisabled);
            btnStop.StyleButtonEnabled(true, backgroundStop, textStart);
            try
            {
                // En modo Advance: recibir en carpeta temporal y guardar manualmente después.
                await _service.StartAsync(_incomingFolder, validateDelete: false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
                lblSatus.Text = "Listo. Ocurrió un error al iniciar.";
                btnStart.StyleButtonEnabled(true, backgroundStart, textStart);
                btnStop.StyleButtonEnabled(false, backgroundDisabled, textDisabled);
                StopIncomingWatcher();
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            _service.Stop();
            StopIncomingWatcher();
            btnStart.StyleButtonEnabled(true, backgroundStart, textStart);
            btnStop.StyleButtonEnabled(false, backgroundDisabled, textDisabled);
            UpdateUI(() =>
            {
                pgrbrStatus.Value = 0;
                pgrbrStatus.Visible = false;
            });
            lblSatus.Text = "Transferencia detenida.";
        }
        #endregion

        #region Service Event Handlers
        private void OnServiceProgress(int value)
        {
            SetProgress(value);
        }

        private void OnServiceStatus(string status)
        {
            UpdateUI(() =>
            {
                lblSatus.Visible = true;
                lblSatus.Text = MapAdvanceStatus(status);
            });
        }

        private void OnServiceError(string message)
        {
            UpdateUI(() => MessageBox.Show(message, "Error"));
        }

        private void SetProgress(int value)
        {
            try
            {
                UpdateUI(() =>
                {
                    int v = Math.Max(pgrbrStatus.Minimum, Math.Min(value, pgrbrStatus.Maximum));
                    pgrbrStatus.Value = v;
                    pgrbrStatus.Visible = v != 0;
                });
            }
            catch { }
        }

        private void UpdateUI(Action action)
        {
            try
            {
                if (InvokeRequired)
                    Invoke(action);
                else
                    action();
            }
            catch { }
        }
        #endregion

        private void chbxValidateDelete_CheckedChanged(object sender, EventArgs e)
        {
#if DEBUG
            if (chbxValidateDelete.Checked == true)
            {
                MessageBox.Show("Si marcas esta opción, las fotos se eliminarán del dispositivo después de guardarlas en tu PC. Asegúrate de que la foto se haya guardado correctamente antes de habilitar esta opción.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
#else
            if (chbxValidateDelete.Checked == true)
            {
                MessageBox.Show("Si marcas esta opción, las fotos se eliminarán del dispositivo después de guardarlas en tu PC. Asegúrate de que la foto se haya guardado correctamente antes de habilitar esta opción.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
#endif
        }

        private void btnBasic_Click(object sender, EventArgs e)
        {
            var basicForm = new FrmFotoFlow();
            Hide();
            basicForm.Show();
        }

        private void FrmFotoFlowAdvance_FormClosed(object sender, FormClosedEventArgs e)
        {
            PersistPathIfValid();
            try { _service.Stop(); } catch { }
            StopIncomingWatcher();
            Application.Exit();
        }

        private async void btnSaveFile_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPath.Text))
            {
                MessageBox.Show("Por favor, ingresa una ruta de destino.", "Ruta inválida");
                return;
            }
            PersistPathIfValid();

            string? incomingPath = _currentIncomingPath;
            if (string.IsNullOrWhiteSpace(incomingPath) || !File.Exists(incomingPath))
            {
                MessageBox.Show("No hay archivo para guardar.", "Sin archivo");
                btnSaveFile.Enabled = false;
                txtNewNameFile.Enabled = false;
                return;
            }

            string targetDir = txtPath.Text.Trim();
            try
            {
                Directory.CreateDirectory(targetDir);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "No se pudo crear la carpeta destino");
                return;
            }

            string originalFileName = _currentOriginalFileName ?? Path.GetFileName(incomingPath);
            string originalExtension = _currentOriginalExtension ?? Path.GetExtension(originalFileName);

            string finalFileName;
            if (string.IsNullOrWhiteSpace(txtNewNameFile.Text))
            {
                finalFileName = originalFileName;
            }
            else
            {
                string normalized = NormalizeFileName(txtNewNameFile.Text);
                if (string.IsNullOrWhiteSpace(normalized))
                {
                    MessageBox.Show("El nombre nuevo no puede estar vacío.", "Nombre inválido");
                    return;
                }

                if (normalized.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                {
                    MessageBox.Show("El nombre contiene caracteres inválidos.", "Nombre inválido");
                    return;
                }

                finalFileName = Path.HasExtension(normalized) ? normalized : normalized + originalExtension;

                if (string.IsNullOrWhiteSpace(Path.GetFileNameWithoutExtension(finalFileName)))
                {
                    MessageBox.Show("El nombre del archivo no puede estar vacío.", "Nombre inválido");
                    return;
                }
            }

            string targetPath = GetAvailableTargetPath(targetDir, finalFileName);

            try
            {
                File.Copy(incomingPath, targetPath, overwrite: false);
                TryDeleteFile(incomingPath);
                UpdateUI(() => lblSatus.Text = $"Imagen guardada: {Path.GetFileName(targetPath)}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error al guardar");
                return;
            }

            if (chbxValidateDelete.Checked && !string.IsNullOrWhiteSpace(_currentDeviceFileName))
            {
                string deviceFile = _currentDeviceFileName!;
                try
                {
                    UpdateUI(() => lblSatus.Text = $"Eliminando del dispositivo: {deviceFile}");
                    await Task.Run(() =>
                    {
                        string escaped = EscapeForDoubleQuotes(deviceFile);
                        var adbResult = _adbRunner.Execute($"shell rm \"/sdcard/DCIM/Camera/{escaped}\"");
                        if (!adbResult.IsSuccess)
                        {
                            throw new InvalidOperationException(string.IsNullOrWhiteSpace(adbResult.StdErr)
                                ? $"ADB falló (ExitCode={adbResult.ExitCode})."
                                : adbResult.StdErr.Trim());
                        }
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "No se pudo eliminar del dispositivo");
                }
            }

            ClearCurrentFile();
            _ = TryLoadNextIncomingAsync();
        }

        private void btnSelectPath_Click(object? sender, EventArgs e)
        {
            txtPath.TrySelectFolder(promptUser: true);
            PersistPathIfValid();
        }

        private void StartIncomingWatcher(string incomingFolder)
        {
            StopIncomingWatcher();

            _incomingWatcher = new FileSystemWatcher(incomingFolder)
            {
                IncludeSubdirectories = false,
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.Size | NotifyFilters.CreationTime | NotifyFilters.LastWrite,
                Filter = "*.*",
                EnableRaisingEvents = true
            };

            _incomingWatcher.Created += IncomingWatcherOnFile;
            _incomingWatcher.Renamed += IncomingWatcherOnFileRenamed;
        }

        private void StopIncomingWatcher()
        {
            if (_incomingWatcher == null)
                return;

            try
            {
                _incomingWatcher.EnableRaisingEvents = false;
                _incomingWatcher.Created -= IncomingWatcherOnFile;
                _incomingWatcher.Renamed -= IncomingWatcherOnFileRenamed;
                _incomingWatcher.Dispose();
            }
            catch { }
            finally
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(_incomingFolder))
                        ClearIncomingFolder(_incomingFolder);
                }
                catch { }
                _incomingWatcher = null;
                _incomingFolder = null;
                lock (_seenIncomingFiles) _seenIncomingFiles.Clear();
                while (_pendingIncomingFiles.TryDequeue(out _)) { }
                ClearCurrentFile();
            }
        }

        private void LoadLastPathIfAny()
        {
            if (_loadedSettings)
                return;

            _loadedSettings = true;
            var settings = AppUserSettings.Load();
            if (string.IsNullOrWhiteSpace(settings.LastPathAdvance))
                return;

            if (!Directory.Exists(settings.LastPathAdvance))
                return;

            txtPath.Text = settings.LastPathAdvance;
        }

        private void PersistPathIfValid()
        {
            string path = txtPath.Text.Trim();
            if (string.IsNullOrWhiteSpace(path))
                return;
            if (!Directory.Exists(path))
                return;

            AppUserSettings.Update(s => s.LastPathAdvance = path);
        }

        private void IncomingWatcherOnFile(object? sender, FileSystemEventArgs e)
        {
            EnqueueIncoming(e.FullPath);
        }

        private void IncomingWatcherOnFileRenamed(object? sender, RenamedEventArgs e)
        {
            EnqueueIncoming(e.FullPath);
        }

        private void EnqueueIncoming(string fullPath)
        {
            if (string.IsNullOrWhiteSpace(fullPath))
                return;

            lock (_seenIncomingFiles)
            {
                if (!_seenIncomingFiles.Add(fullPath))
                    return;
            }

            if (!_receiveMultipleEnabled)
            {
                ClearPendingQueue();
                DiscardCurrentIncoming();
                CleanupIncomingFolder(keepPath: fullPath);
            }

            _pendingIncomingFiles.Enqueue(fullPath);
            _ = TryLoadNextIncomingAsync();
        }

        private async Task TryLoadNextIncomingAsync()
        {
            await _incomingLock.WaitAsync();
            try
            {
                if (_receiveMultipleEnabled && !string.IsNullOrWhiteSpace(_currentIncomingPath) && File.Exists(_currentIncomingPath))
                    return; // Ya hay un archivo activo.

                _currentIncomingPath = null;

                while (_pendingIncomingFiles.TryDequeue(out string? next))
                {
                    if (string.IsNullOrWhiteSpace(next) || !File.Exists(next))
                        continue;

                    bool ready = await WaitForFileReadyAsync(next, TimeSpan.FromSeconds(10));
                    if (!ready)
                        continue;

                    SetCurrentFile(next);
                    LoadPreview(next);
                    UpdateUI(() =>
                    {
                        txtNameFilePrimary.Text = _currentOriginalFileName ?? string.Empty;
                        btnSaveFile.Enabled = true;
                        txtNewNameFile.Enabled = true;
                        txtNewNameFile.Text = string.Empty; // solo “nuevos nombres” por archivo recibido
                        lblSatus.Text = _receiveMultipleEnabled
                            ? $"Imagen recibida: {_currentOriginalFileName}. Lista para guardar."
                            : $"Nueva imagen recibida: {_currentOriginalFileName}. Lista para guardar.";
                    });

                    if (!_receiveMultipleEnabled)
                        CleanupIncomingFolder(keepPath: next);
                    return;
                }

                UpdateUI(() =>
                {
                    btnSaveFile.Enabled = false;
                    txtNewNameFile.Enabled = false;
                });
            }
            catch (Exception ex)
            {
                UpdateUI(() => MessageBox.Show(ex.Message, "Error"));
            }
            finally
            {
                _incomingLock.Release();
            }
        }

        private void SetCurrentFile(string incomingPath)
        {
            _currentIncomingPath = incomingPath;
            _currentOriginalFileName = Path.GetFileName(incomingPath);
            _currentOriginalExtension = Path.GetExtension(_currentOriginalFileName);
            _currentDeviceFileName = _currentOriginalFileName;
        }

        private void ClearCurrentFile()
        {
            _currentIncomingPath = null;
            _currentDeviceFileName = null;
            _currentOriginalFileName = null;
            _currentOriginalExtension = null;

            UpdateUI(() =>
            {
                txtNameFilePrimary.Text = string.Empty;
                btnSaveFile.Enabled = false;
                txtNewNameFile.Enabled = false;
                txtNewNameFile.Text = string.Empty;

                if (pcbxPreview.Image != null)
                {
                    var prev = pcbxPreview.Image;
                    pcbxPreview.Image = null;
                    try { prev.Dispose(); } catch { }
                }
            });
        }

        private void LoadPreview(string path)
        {
            UpdateUI(() =>
            {
                if (pcbxPreview.Image != null)
                {
                    var prev = pcbxPreview.Image;
                    pcbxPreview.Image = null;
                    try { prev.Dispose(); } catch { }
                }
            });

            if (!IsLikelyImageFile(path))
                return;

            byte[] bytes = File.ReadAllBytes(path);
            using var ms = new MemoryStream(bytes);
            using var img = Image.FromStream(ms);
            var bmp = new Bitmap(img);

            UpdateUI(() => pcbxPreview.Image = bmp);
        }

        private static bool IsLikelyImageFile(string path)
        {
            string ext = Path.GetExtension(path).ToLowerInvariant();
            return ext is ".jpg" or ".jpeg" or ".png" or ".bmp" or ".gif" or ".tif" or ".tiff" or ".webp";
        }

        private static async Task<bool> WaitForFileReadyAsync(string path, TimeSpan timeout)
        {
            var start = DateTime.UtcNow;
            long lastLength = -1;
            int stableCount = 0;

            while (DateTime.UtcNow - start < timeout)
            {
                if (!File.Exists(path))
                {
                    await Task.Delay(200);
                    continue;
                }

                long length;
                try { length = new FileInfo(path).Length; }
                catch
                {
                    await Task.Delay(200);
                    continue;
                }

                if (length == lastLength && length > 0)
                    stableCount++;
                else
                    stableCount = 0;

                lastLength = length;

                if (stableCount >= 2)
                {
                    try
                    {
                        using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        return true;
                    }
                    catch
                    {
                        // Sigue esperando.
                    }
                }

                await Task.Delay(200);
            }

            return false;
        }

        private static string NormalizeFileName(string raw)
        {
            if (raw == null)
                return string.Empty;

            string normalized = raw.Replace("\r", " ").Replace("\n", " ");
            normalized = Regex.Replace(normalized, "\\s+", " ").Trim();
            normalized = normalized.TrimEnd('.', ' ');
            return normalized;
        }

        private void chbxReceiveMultiple_CheckedChanged(object? sender, EventArgs e)
        {
            _receiveMultipleEnabled = chbxReceiveMultiple.Checked;

            if (!_receiveMultipleEnabled)
            {
                ClearPendingQueue();
                CleanupIncomingFolder(keepPath: _currentIncomingPath);
                UpdateUI(() => lblSatus.Text = "Modo único: se reemplazará la imagen al recibir una nueva.");
            }
            else
            {
                UpdateUI(() => lblSatus.Text = "Modo múltiple: se recibirán varias imágenes para guardarlas una por una.");
            }
        }

        private void ClearPendingQueue()
        {
            while (_pendingIncomingFiles.TryDequeue(out _)) { }
        }

        private void DiscardCurrentIncoming()
        {
            string? current = _currentIncomingPath;
            if (string.IsNullOrWhiteSpace(current))
            {
                ClearCurrentFile();
                return;
            }

            TryDeleteFile(current);
            ClearCurrentFile();
        }

        private void CleanupIncomingFolder(string? keepPath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_incomingFolder) || !Directory.Exists(_incomingFolder))
                    return;

                foreach (var file in Directory.GetFiles(_incomingFolder))
                {
                    if (!string.IsNullOrWhiteSpace(keepPath) && string.Equals(file, keepPath, StringComparison.OrdinalIgnoreCase))
                        continue;
                    TryDeleteFile(file);
                }
            }
            catch { }
        }

        private static void ClearIncomingFolder(string folder)
        {
            try
            {
                if (!Directory.Exists(folder))
                    return;

                foreach (var file in Directory.GetFiles(folder))
                {
                    try { File.Delete(file); } catch { }
                }
            }
            catch { }
        }

        private static string MapAdvanceStatus(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return "Listo.";

            if (status.StartsWith("Esperando", StringComparison.OrdinalIgnoreCase))
                return "Esperando nuevas fotos en el dispositivo...";

            if (status.StartsWith("Transferiendo", StringComparison.OrdinalIgnoreCase))
            {
                // "Transferiendo file" o "Transferiendo file (50%)"
                string file = status.Substring("Transferiendo".Length).Trim();
                return $"Recibiendo: {file}";
            }

            if (status.StartsWith("Archivo ", StringComparison.OrdinalIgnoreCase) && status.Contains("transferido", StringComparison.OrdinalIgnoreCase))
                return "Imagen recibida. Cargando previsualización...";

            return status;
        }

        private static string GetAvailableTargetPath(string targetDir, string finalFileName)
        {
            string desired = Path.Combine(targetDir, finalFileName);
            if (!File.Exists(desired))
                return desired;

            string baseName = Path.GetFileNameWithoutExtension(finalFileName);
            string ext = Path.GetExtension(finalFileName);

            string root = RemoveTrailingNumericSuffix(baseName);
            string candidate;

            int n = 1;
            while (true)
            {
                string name = $"{root} ({n}){ext}";
                candidate = Path.Combine(targetDir, name);
                if (!File.Exists(candidate))
                    return candidate;
                n++;
            }
        }

        private static string RemoveTrailingNumericSuffix(string baseName)
        {
            // Quita solo el patrón final " (n)" para poder continuar con n incremental.
            var match = Regex.Match(baseName, @"^(?<root>.*?)(?: \((?<n>\d+)\))?$");
            if (!match.Success)
                return baseName;

            string root = match.Groups["root"].Value;
            string n = match.Groups["n"].Value;
            if (string.IsNullOrWhiteSpace(n))
                return baseName;

            root = root.TrimEnd();
            return string.IsNullOrWhiteSpace(root) ? baseName : root;
        }

        private static void TryDeleteFile(string path)
        {
            try
            {
                if (File.Exists(path))
                    File.Delete(path);
            }
            catch { }
        }

        private static string EscapeForDoubleQuotes(string value)
        {
            return value.Replace("\"", "\\\"");
        }

    }
}
