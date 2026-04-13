using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace FotoFlow.Core
{
    public sealed class AdbRunner
    {
        private readonly string _adbPath;

        public AdbRunner()
        {
            _adbPath = Path.Combine(AppContext.BaseDirectory, "adb", "adb.exe");
        }

        public AdbRunner(string adbPath)
        {
            _adbPath = adbPath ?? throw new ArgumentNullException(nameof(adbPath));
        }

        public AdbResult Execute(string arguments)
        {
            if (!File.Exists(_adbPath))
            {
                throw new FileNotFoundException("No se encontró ADB en la carpeta /adb", _adbPath);
            }

            var proceso = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _adbPath,
                    Arguments = arguments,
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
            proceso.WaitForExit();

            return new AdbResult(proceso.ExitCode, stdout, stderr);
        }
    }

    public sealed record AdbResult(int ExitCode, string StdOut, string StdErr)
    {
        public bool IsSuccess => ExitCode == 0;
    }
}

