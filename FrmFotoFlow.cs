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
            lblStatusPhoto.Visible = true;
            lblStatusPhoto.Text = "Listo.";
            pgrbrStatusPhoto.Value = 0;
            pgrbrStatusPhoto.Visible = false;
        }

        private void TrySelectFolder(bool promptUser)
        {
            if (!promptUser && !string.IsNullOrWhiteSpace(txtRuta.Text))
                return;

            string defaultPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

            if (!promptUser)
            {
                if (string.IsNullOrWhiteSpace(txtRuta.Text))
                    txtRuta.Text = defaultPath;
                return;
            }

            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Selecciona la carpeta de destino para las fotos";
                folderDialog.UseDescriptionForTitle = true;
                folderDialog.ShowNewFolderButton = true;
                folderDialog.RootFolder = Environment.SpecialFolder.MyComputer;
                folderDialog.SelectedPath = string.IsNullOrWhiteSpace(txtRuta.Text) ? defaultPath : txtRuta.Text;

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    txtRuta.Text = folderDialog.SelectedPath;
                    return;
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            btnIniciar.Enabled = true;
            btnDetener.Enabled = false;
            TrySelectFolder(promptUser: false);
        }

        private async void btnIniciar_Click(object sender, EventArgs e)
        {
            string destino = txtRuta.Text;
            lblStatusPhoto.Text = "Iniciando servicio de transferencia.";
            if (string.IsNullOrEmpty(destino))
            {
                MessageBox.Show("Por favor, ingresa una ruta de destino.");
                lblStatusPhoto.Text = "Listo.";
                return;
            }

            if (!File.Exists(adbPath))
            {
                MessageBox.Show("No se encontró ADB en la carpeta /adb");
                lblStatusPhoto.Text = "Listo.";
                return;
            }
            if (!Directory.Exists(destino))
            {
                Directory.CreateDirectory(destino);
            }
            cts = new CancellationTokenSource();
            btnIniciar.Enabled = false;
            btnDetener.Enabled = true;
            lblStatusPhoto.Text = "Esperando archivos.";
            await Task.Run(() => LoopFotos(destino, cts.Token));
        }

        private void btnDetener_Click(object sender, EventArgs e)
        {
            cts?.Cancel();
            btnIniciar.Enabled = true;
            btnDetener.Enabled = false;
        }
        private void SetProgress(int value)
        {
            try
            {
                pgrbrStatusPhoto.Invoke(new Action(() =>
                {
                    pgrbrStatusPhoto.Value = Math.Min(value, pgrbrStatusPhoto.Maximum);
                }));
            }
            catch { }
        }
        private void SimularProgreso(CancellationToken token)
        {
            for (int i = 0; i <= 90; i += 10)
            {
                if (token.IsCancellationRequested) return;

                SetProgress(i);
                Thread.Sleep(100); // velocidad de animación
            }
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

                        int progresoPorPaso = chbxValidateDelete.Checked ? 50 : 100;
                        string mensaje = chbxValidateDelete.Checked ? $"Transferiendo {file} (50%)" : $"Transferiendo {file}";
                        
                        UpdateUI(() =>
                        {
                            SetProgress(0);
                            lblStatusPhoto.Visible = true;
                            lblStatusPhoto.Text = mensaje;
                            pgrbrStatusPhoto.Visible = true;
                        });
                        
                        var progresoTask = Task.Run(() => SimularProgreso(token));

                        EjecutarADB($"pull /sdcard/DCIM/Camera/{file} \"{rutaLocal}\"");
                        progresoTask.Wait();

                        if (File.Exists(rutaLocal))
                        {
                            procesados.Add(file);
                            UpdateUI(() =>
                            {
                            lblStatusPhoto.Text = $"Eliminando {file} del dispositivo(50 %)";
                                SetProgress(progresoPorPaso);
                            });
                        }
                        if (chbxValidateDelete.Checked == true)
                        {
                            EjecutarADB($"shell rm /sdcard/DCIM/Camera/{file}");
                        }
                        UpdateUI(() =>
                        {
                            lblStatusPhoto.Text = $"Archivo {file} transferido correctamente";
                            SetProgress(100);
                        });
                        progresoTask.Wait();
                    }
                }
                catch (Exception ex)
                {
                    MostrarErrorUnaVez(ex.Message);
                }

                Thread.Sleep(2000);
                lblStatusPhoto.Invoke(new Action(() =>
                {
                    lblStatusPhoto.Visible = true;
                    lblStatusPhoto.Text = "Esperando archivos.";
                    pgrbrStatusPhoto.Visible = false;
                    SetProgress(0);
                }));
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
            TrySelectFolder(promptUser: true);
        }

        private void chbxValidateDelete_CheckedChanged(object sender, EventArgs e)
        {
            #if DEBUG
            //MessageBox.Show($"Haz seleccionado {chbxValidateDelete.Checked}", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Information);
            if(chbxValidateDelete.Checked == true) 
            { 
                MessageBox.Show("Si marcas esta opción, las fotos se eliminarán del dispositivo después de ser copiadas. Asegúrate de que las fotos se hayan copiado correctamente antes de habilitar esta opción.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
#else
            if(chbxValidateDelete.Checked == true)
            {
                MessageBox.Show("Si marcas esta opción, las fotos se eliminarán del dispositivo después de ser copiadas. Asegúrate de que las fotos se hayan copiado correctamente antes de habilitar esta opción.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
#endif
        }
    }
}
