using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FotoFlow.Core
{
    public class FotoFlowService : IFotoFlowService
    {
        private CancellationTokenSource cts;
        private readonly HashSet<string> procesados = new HashSet<string>(StringComparer.Ordinal);
        private string adbPath;

        public event Action<int> ProgressChanged;
        public event Action<string> StatusChanged;
        public event Action<string> ErrorOccurred;

        public bool IsRunning => cts != null && !cts.IsCancellationRequested;

        public FotoFlowService()
        {
            adbPath = Path.Combine(AppContext.BaseDirectory, "adb", "adb.exe");
        }

        public async Task StartAsync(string destino, bool validateDelete)
        {
            if (string.IsNullOrEmpty(destino))
                throw new ArgumentException("Destino inválido", nameof(destino));

            if (!File.Exists(adbPath))
            {
                throw new FileNotFoundException("No se encontró ADB en la carpeta /adb", adbPath);
            }

            if (!Directory.Exists(destino))
                Directory.CreateDirectory(destino);

            cts = new CancellationTokenSource();

            await Task.Run(() => LoopFotos(destino, validateDelete, cts.Token));
        }

        public void Stop()
        {
            cts?.Cancel();
        }

        private void SetProgress(int value)
        {
            try { ProgressChanged?.Invoke(Math.Min(value, 100)); } catch { }
        }

        private void SimularProgreso(CancellationToken token)
        {
            for (int i = 0; i <= 90; i += 10)
            {
                if (token.IsCancellationRequested) return;
                SetProgress(i);
                Thread.Sleep(100);
            }
        }

        private void LoopFotos(string destino, bool validateDelete, CancellationToken token)
        {
            try
            {
                foreach (var file in ListCameraFilesSafe())
                    procesados.Add(file);
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke("Error inicial: " + ex.Message);
                return;
            }

            while (!token.IsCancellationRequested)
            {
                try
                {
                    foreach (var file in ListCameraFilesSafe())
                    {
                        if (procesados.Contains(file))
                            continue;
                        string rutaLocal = Path.Combine(destino, file);

                        int progresoPorPaso = validateDelete ? 50 : 100;
                        string mensaje = validateDelete ? $"Transferiendo {file} (50%)" : $"Transferiendo {file}";

                        StatusChanged?.Invoke(mensaje);
                        SetProgress(0);

                        var progresoTask = Task.Run(() => SimularProgreso(token));

                        var pullResult = EjecutarADBResult($"pull \"/sdcard/DCIM/Camera/{EscapeForDoubleQuotes(file)}\" \"{rutaLocal}\"", timeoutMs: 5 * 60 * 1000);
                        progresoTask.Wait();
                        if (pullResult.ExitCode != 0)
                        {
                            string err = string.IsNullOrWhiteSpace(pullResult.StdErr) ? pullResult.StdOut : pullResult.StdErr;
                            ErrorOccurred?.Invoke($"Error al transferir {file}: {err}".Trim());
                            continue;
                        }

                        if (File.Exists(rutaLocal))
                        {
                            procesados.Add(file);
                            StatusChanged?.Invoke($"Eliminando {file} del dispositivo(50 %)");
                            SetProgress(progresoPorPaso);
                        }
                        if (validateDelete)
                        {
                            EjecutarADBResult($"shell rm \"/sdcard/DCIM/Camera/{EscapeForDoubleQuotes(file)}\"", timeoutMs: 30 * 1000);
                        }
                        StatusChanged?.Invoke($"Archivo {file} transferido correctamente");
                        SetProgress(100);
                        progresoTask.Wait();
                    }
                }
                catch (Exception ex)
                {
                    ErrorOccurred?.Invoke(ex.Message);
                }

                Thread.Sleep(2000);
                StatusChanged?.Invoke("Esperando archivos.");
                SetProgress(0);
            }
        }

        private static readonly HashSet<string> AllowedImageExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tif", ".tiff", ".webp", ".heic", ".heif", ".dng"
        };

        private static readonly HashSet<string> IgnoredNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "cache", ".cache", "thumbnails", ".thumbnails", "thumb", "tmp", ".tmp"
        };

        private IReadOnlyList<string> ListCameraFilesSafe()
        {
            try
            {
                return ListCameraFiles();
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke("Error al listar archivos: " + ex.Message);
                return Array.Empty<string>();
            }
        }

        private IReadOnlyList<string> ListCameraFiles()
        {
            // -p marca directorios con '/' al final; así podemos ignorarlos y evitar "pull" a carpetas cache.
            var listResult = EjecutarADBResult("shell ls -p /sdcard/DCIM/Camera", timeoutMs: 10 * 1000);
            string output = listResult.ExitCode == 0 ? listResult.StdOut : (string.IsNullOrWhiteSpace(listResult.StdOut) ? listResult.StdErr : listResult.StdOut);

            if (string.IsNullOrWhiteSpace(output))
            {
                // Fallback por compatibilidad con dispositivos que no soporten "-p"
                output = EjecutarADB("shell ls /sdcard/DCIM/Camera");
            }

            var entries = output.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            var files = new List<string>(capacity: entries.Count);
            foreach (var entry in entries)
            {
                // Directorios (por -p) -> ignorar
                if (entry.EndsWith("/", StringComparison.Ordinal))
                    continue;

                if (ShouldIgnoreEntry(entry))
                    continue;

                if (!IsAllowedImage(entry))
                    continue;

                files.Add(entry);
            }

            return files;
        }

        private static bool ShouldIgnoreEntry(string entry)
        {
            // Ignorar ocultos y candidatos de cache.
            if (entry.StartsWith(".", StringComparison.Ordinal))
                return true;

            string nameOnly = entry.TrimEnd('/');
            if (IgnoredNames.Contains(nameOnly))
                return true;

            // Heurística adicional: nombres que contienen "thumb" o "cache" suelen ser cache.
            string lower = nameOnly.ToLowerInvariant();
            if (lower.Contains("thumb") || lower.Contains("cache"))
                return true;

            return false;
        }

        private static bool IsAllowedImage(string entry)
        {
            string ext = Path.GetExtension(entry);
            return !string.IsNullOrWhiteSpace(ext) && AllowedImageExtensions.Contains(ext);
        }

        private static string EscapeForDoubleQuotes(string value)
        {
            return value.Replace("\"", "\\\"");
        }

        private string EjecutarADB(string argumentos)
        {
            return EjecutarADBResult(argumentos, timeoutMs: 30 * 1000).StdOut;
        }

        private sealed record AdbCommandResult(int ExitCode, string StdOut, string StdErr);

        private AdbCommandResult EjecutarADBResult(string argumentos, int timeoutMs)
        {
            var proceso = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = adbPath,
                    Arguments = argumentos,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8
                }
            };

            proceso.Start();
            string stdout = proceso.StandardOutput.ReadToEnd();
            string stderr = proceso.StandardError.ReadToEnd();

            bool exited = proceso.WaitForExit(timeoutMs);
            if (!exited)
            {
                try { proceso.Kill(entireProcessTree: true); } catch { }
                throw new TimeoutException($"Tiempo de espera excedido ejecutando ADB: {argumentos}");
            }

            return new AdbCommandResult(proceso.ExitCode, stdout, stderr);
        }
    }
}
