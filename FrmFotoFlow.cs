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

        public FrmFotoFlow()
        {
            InitializeComponent();
            _service = new FotoFlowService();
            _service.ProgressChanged += OnServiceProgress;
            _service.StatusChanged += OnServiceStatus;
            _service.ErrorOccurred += OnServiceError;
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
            lblStatusPhoto.Visible = true;
            lblStatusPhoto.Text = "Listo.";
            // Ensure progress bar is configured
            pgrbrStatusPhoto.Minimum = 0;
            pgrbrStatusPhoto.Maximum = 100;
            pgrbrStatusPhoto.Style = ProgressBarStyle.Continuous;
            pgrbrStatusPhoto.Value = 0;
            pgrbrStatusPhoto.Visible = false;
            StyleButtonEnabled(btnIniciar, true, Color.YellowGreen, SystemColors.ControlLightLight);
            StyleButtonEnabled(btnDetener, false, Color.Gainsboro, Color.Black);
            TrySelectFolder(promptUser: false);
        }

        private void StyleButtonEnabled(Button button, bool init, Color colorBackground, Color textColor)
        {
            button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderSize = 0;
            if (init)
            {
                button.BackColor = colorBackground;
                button.ForeColor = textColor;
                button.Enabled = init;
                return;
            }
            button.Enabled = init;
            button.BackColor = colorBackground;
            button.ForeColor = textColor;

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
            StyleButtonEnabled(btnIniciar, false, Color.Gainsboro, Color.Black);
            StyleButtonEnabled(btnDetener, true, Color.Red, SystemColors.ControlLightLight);

            try
            {
                await _service.StartAsync(destino, chbxValidateDelete.Checked);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
                lblStatusPhoto.Text = "Listo.";
                btnIniciar.Enabled = true;
                btnDetener.Enabled = false;
            }
        }

        private void btnDetener_Click(object sender, EventArgs e)
        {
            _service.Stop();
            StyleButtonEnabled(btnIniciar, true, Color.YellowGreen, SystemColors.ControlLightLight);
            StyleButtonEnabled(btnDetener, false, Color.Gainsboro, Color.Black);
            UpdateUI(() =>
            {
                pgrbrStatusPhoto.Value = 0;
                pgrbrStatusPhoto.Visible = false;
            });
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
            TrySelectFolder(promptUser: true);
        }

        private void chbxValidateDelete_CheckedChanged(object sender, EventArgs e)
        {
#if DEBUG
            //MessageBox.Show($"Haz seleccionado {chbxValidateDelete.Checked}", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Information);
            if (chbxValidateDelete.Checked == true)
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

        private void OnServiceProgress(int value)
        {
            SetProgress(value);
        }

        private void OnServiceStatus(string status)
        {
            UpdateUI(() =>
            {
                lblStatusPhoto.Visible = true;
                lblStatusPhoto.Text = status;
            });
        }

        private void OnServiceError(string message)
        {
            UpdateUI(() => MessageBox.Show(message, "Error"));
        }

        private void btnAdvance_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Funcionalidad avanzada en desarrollo.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnDetener_Enter(object sender, EventArgs e)
        {

        }

        private void btnDetener_MouseHover(object sender, EventArgs e)
        {
        }
    }
}
