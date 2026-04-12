using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FotoFlow.Core
{
    public class FotoFlowService : IFotoFlowService
    {
        private CancellationTokenSource cts;
        private HashSet<string> procesados = new HashSet<string>();
        private string adbPath;
        private bool errorMostrado = false;

        public event System.Action<int> ProgressChanged;
        public event System.Action<string> StatusChanged;
        public event System.Action<string> ErrorOccurred;

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
                var inicial = EjecutarADB("shell ls /sdcard/DCIM/Camera");
                var listaInicial = inicial.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var file in listaInicial)
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
                    var archivos = EjecutarADB("shell ls /sdcard/DCIM/Camera");

                    var lista = archivos.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var file in lista)
                    {
                        if (procesados.Contains(file))
                            continue;
                        string rutaLocal = Path.Combine(destino, file);

                        int progresoPorPaso = validateDelete ? 50 : 100;
                        string mensaje = validateDelete ? $"Transferiendo {file} (50%)" : $"Transferiendo {file}";

                        StatusChanged?.Invoke(mensaje);
                        SetProgress(0);

                        var progresoTask = Task.Run(() => SimularProgreso(token));

                        EjecutarADB($"pull /sdcard/DCIM/Camera/{file} \"{rutaLocal}\"");
                        progresoTask.Wait();

                        if (File.Exists(rutaLocal))
                        {
                            procesados.Add(file);
                            StatusChanged?.Invoke($"Eliminando {file} del dispositivo(50 %)");
                            SetProgress(progresoPorPaso);
                        }
                        if (validateDelete)
                        {
                            EjecutarADB($"shell rm /sdcard/DCIM/Camera/{file}");
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

        private string EjecutarADB(string argumentos)
        {
            var proceso = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = adbPath,
                    Arguments = argumentos,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            proceso.Start();
            string resultado = proceso.StandardOutput.ReadToEnd();
            proceso.WaitForExit();
            return resultado;
        }
    }
}
