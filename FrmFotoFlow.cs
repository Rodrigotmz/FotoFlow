using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FotoFlow.Core;

namespace FotoFlow
{
    public partial class FrmFotoFlow : Form
    {
        private IFotoFlowService _service;
        private bool _loadedSettings;

        public FrmFotoFlow()
        {
            InitializeComponent();
            _service = new FotoFlowService();
            _service.ProgressChanged += OnServiceProgress;
            _service.StatusChanged += OnServiceStatus;
            _service.ErrorOccurred += OnServiceError;

            Activated += (_, _) => AppRuntimeState.LastMode = FotoFlowMode.Basic;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            lblStatusPhoto.Visible = true;
            lblStatusPhoto.Text = "Listo. Selecciona una ruta y presiona Iniciar.";
            // Ensure progress bar is configured
            pgrbrStatusPhoto.InitConfigurationProgressBar();
            btnIniciar.StyleButtonEnabled(true, Color.YellowGreen, SystemColors.ControlLightLight);
            btnDetener.StyleButtonEnabled(false, Color.Gainsboro, Color.Black);
            txtPath.TrySelectFolder(promptUser: false);

            LoadLastPathIfAny();
            txtPath.Leave += (_, _) => PersistPathIfValid();
        }

        private async void btnIniciar_Click(object sender, EventArgs e)
        {
            string destino = txtPath.Text;
            lblStatusPhoto.Text = "Iniciando transferencia automática...";
            if (string.IsNullOrEmpty(destino))
            {
                MessageBox.Show("Por favor, ingresa una ruta de destino.");
                lblStatusPhoto.Text = "Listo. Ingresa una ruta para guardar.";
                return;
            }
            PersistPathIfValid();
            btnIniciar.StyleButtonEnabled(false, Color.Gainsboro, Color.Black);
            btnDetener.StyleButtonEnabled(true, Color.Red, SystemColors.ControlLightLight);

            try
            {
                await _service.StartAsync(destino, chbxValidateDelete.Checked);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
                lblStatusPhoto.Text = "Listo. Ocurrió un error al iniciar.";
                btnIniciar.StyleButtonEnabled(true, Color.YellowGreen, SystemColors.ControlLightLight);
                btnDetener.StyleButtonEnabled(false, Color.Gainsboro, Color.Black);
            }
        }

        private void btnDetener_Click(object sender, EventArgs e)
        {
            _service.Stop();
            btnIniciar.StyleButtonEnabled(true, Color.YellowGreen, SystemColors.ControlLightLight);
            btnDetener.StyleButtonEnabled(false, Color.Gainsboro, Color.Black);
            UpdateUI(() =>
            {
                pgrbrStatusPhoto.Value = 0;
                pgrbrStatusPhoto.Visible = false;
            });
            lblStatusPhoto.Text = "Transferencia detenida.";
        }

        private void SetProgress(int value)
        {
            // Ensure updates happen on the UI thread and control visibility follows progress.
            try
            {
                UpdateUI(() =>
                {
                    // Clamp value to valid range
                    int v = Math.Max(pgrbrStatusPhoto.Minimum, Math.Min(value, pgrbrStatusPhoto.Maximum));
                    pgrbrStatusPhoto.Value = v;
                    // Show progress bar while work is in progress (non-zero). Hide when idle (0).
                    pgrbrStatusPhoto.Visible = v != 0;
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

        // Logic moved to FotoFlowService in FotoFlow.Core

        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {

        }

        private void btnSelectPath_Click(object sender, EventArgs e)
        {
            txtPath.TrySelectFolder(promptUser: true);
            PersistPathIfValid();
        }

        private void chbxValidateDelete_CheckedChanged(object sender, EventArgs e)
        {
#if DEBUG
            if (chbxValidateDelete.Checked == true)
            {
                MessageBox.Show("Si marcas esta opción, las fotos se eliminarán del dispositivo después de ser copiadas en tu PC. Asegúrate de que las fotos se hayan copiado correctamente antes de habilitar esta opción.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
#else
            if(chbxValidateDelete.Checked == true)
            {
                MessageBox.Show("Si marcas esta opción, las fotos se eliminarán del dispositivo después de ser copiadas en tu PC. Asegúrate de que las fotos se hayan copiado correctamente antes de habilitar esta opción.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
#endif
        }

        private void OnServiceProgress(int value)
        {
            SetProgress(value);
        }

        private void OnServiceStatus(string status)
        {
            UpdateUI(() =>
            {
                lblStatusPhoto.Visible = true;
                lblStatusPhoto.Text = MapBasicStatus(status);
            });
        }

        private void OnServiceError(string message)
        {
            UpdateUI(() => MessageBox.Show(message, "Error"));
        }

        private static string MapBasicStatus(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return "Listo.";

            if (status.StartsWith("Esperando", StringComparison.OrdinalIgnoreCase))
                return "Esperando nuevas fotos en el dispositivo...";

            if (status.StartsWith("Transferiendo", StringComparison.OrdinalIgnoreCase))
            {
                string file = status.Substring("Transferiendo".Length).Trim();
                return $"Copiando: {file}";
            }

            if (status.StartsWith("Archivo ", StringComparison.OrdinalIgnoreCase) && status.Contains("transferido", StringComparison.OrdinalIgnoreCase))
                return status.Replace("Archivo", "Guardado", StringComparison.OrdinalIgnoreCase);

            return status;
        }

        private void btnAdvance_Click(object sender, EventArgs e)
        {
            var advanceForm = new FrmFotoFlowAdvance();
            this.Hide();
            if (advanceForm.ShowDialog() == DialogResult.OK)
            {
                this.Show();

            }
        }

        private void FrmFotoFlow_FormClosed(object sender, FormClosedEventArgs e)
        {
            PersistPathIfValid();
            Application.Exit();
        }

        private void LoadLastPathIfAny()
        {
            if (_loadedSettings)
                return;

            _loadedSettings = true;
            var settings = AppUserSettings.Load();
            if (string.IsNullOrWhiteSpace(settings.LastPathBasic))
                return;

            if (!Directory.Exists(settings.LastPathBasic))
                return;

            txtPath.Text = settings.LastPathBasic;
        }

        private void PersistPathIfValid()
        {
            string path = txtPath.Text.Trim();
            if (string.IsNullOrWhiteSpace(path))
                return;
            if (!Directory.Exists(path))
                return;

            AppUserSettings.Update(s => s.LastPathBasic = path);
        }
    }
}
