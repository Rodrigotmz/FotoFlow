using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FotoFlow.Core
{
    public static class ControlsServices
    {
        public static void StyleButtonEnabled(this Button button, bool init, Color colorBackground, Color textColor)
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
        public static void TrySelectFolder(this TextBox txtPath, bool promptUser)
        {
            if (!promptUser && !string.IsNullOrWhiteSpace(txtPath.Text))
                return;

            string defaultPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

            if (!promptUser)
            {
                if (string.IsNullOrWhiteSpace(txtPath.Text))
                    txtPath.Text = defaultPath;
                return;
            }

            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Selecciona la carpeta de destino para las fotos";
                folderDialog.UseDescriptionForTitle = true;
                folderDialog.ShowNewFolderButton = true;
                folderDialog.RootFolder = Environment.SpecialFolder.MyComputer;
                folderDialog.SelectedPath = string.IsNullOrWhiteSpace(txtPath.Text) ? defaultPath : txtPath.Text;

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    txtPath.Text = folderDialog.SelectedPath;
                    return;
                }
            }
        }
        public static void InitConfigurationProgressBar(this ProgressBar progressBar)
        {
            progressBar.Minimum = 0;
            progressBar.Maximum = 100;
            progressBar.Style = ProgressBarStyle.Continuous;
            progressBar.Value = 0;
            progressBar.Visible = false;
        }
    }
}
