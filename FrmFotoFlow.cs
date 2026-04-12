using System.Diagnostics;

namespace FotoFlow
{
    public partial class FrmFotoFlow : Form
    {
        private CancellationTokenSource cts;
        private HashSet<string> procesados = new HashSet<string>();
        private string adbPath;
        private bool errorMostrado = false;

        public FrmFotoFlow()
        {
            InitializeComponent();
            adbPath = Path.Combine(Application.StartupPath, "adb", "adb.exe");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            btnIniciar.Enabled = true;
            btnDetener.Enabled = false;
        }

        private async void btnIniciar_Click(object sender, EventArgs e)
        {
            string destino = txtRuta.Text;
            if (string.IsNullOrEmpty(destino))
            {
                MessageBox.Show("Por favor, ingresa una ruta de destino.");
                return;
            }

            if (!File.Exists(adbPath))
            {
                MessageBox.Show("No se encontró ADB en la carpeta /adb");
                return;
            }

            if (!Directory.Exists(destino))
            {
                Directory.CreateDirectory(destino);
            }
            cts = new CancellationTokenSource();
            btnIniciar.Enabled = false;
            btnDetener.Enabled = true;
            await Task.Run(() => LoopFotos(destino, cts.Token));
        }

        private void btnDetener_Click(object sender, EventArgs e)
        {
            cts?.Cancel();
            btnIniciar.Enabled = true;
            btnDetener.Enabled = false;
        }
        private void LoopFotos(string destino, CancellationToken token)
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
                MostrarErrorUnaVez("Error inicial: " + ex.Message);
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

                        EjecutarADB($"pull /sdcard/DCIM/Camera/{file} \"{rutaLocal}\"");

                        if (File.Exists(rutaLocal))
                        {
                            procesados.Add(file);
                        }
                        if (chbxValidateDelete.Checked == true)
                        {
                            EjecutarADB($"shell rm /sdcard/DCIM/Camera/{file}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MostrarErrorUnaVez(ex.Message);
                }

                Thread.Sleep(2000);
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

        private void MostrarErrorUnaVez(string mensaje)
        {
            if (errorMostrado) return;

            errorMostrado = true;

            Invoke(new Action(() =>
            {
                MessageBox.Show(mensaje, "Error");
            }));
        }


        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {

        }

        private void btnSelectPath_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select the destination folder";
                folderDialog.UseDescriptionForTitle = true;
                folderDialog.ShowNewFolderButton = true;

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedPath = folderDialog.SelectedPath;
                    txtRuta.Text = selectedPath;
                }
            }
        }

        private void chbxValidateDelete_CheckedChanged(object sender, EventArgs e)
        {
            MessageBox.Show("Si marcas esta opción, las fotos se eliminarán del dispositivo después de ser copiadas. Asegúrate de que las fotos se hayan copiado correctamente antes de habilitar esta opción.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //Debugg
            //MessageBox.Show($"Haz seleccionado {chbxValidateDelete.Checked}", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
