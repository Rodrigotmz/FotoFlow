namespace FotoFlow
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            txtRuta = new TextBox();
            btnIniciar = new Button();
            btnDetener = new Button();
            fbdSelectPath = new FolderBrowserDialog();
            btnSelectPath = new Button();
            SuspendLayout();
            // 
            // txtRuta
            // 
            txtRuta.Location = new Point(12, 12);
            txtRuta.Name = "txtRuta";
            txtRuta.Size = new Size(303, 23);
            txtRuta.TabIndex = 0;
            // 
            // btnIniciar
            // 
            btnIniciar.BackColor = Color.YellowGreen;
            btnIniciar.FlatAppearance.BorderSize = 0;
            btnIniciar.FlatStyle = FlatStyle.Flat;
            btnIniciar.ForeColor = SystemColors.ControlLightLight;
            btnIniciar.Location = new Point(12, 69);
            btnIniciar.Name = "btnIniciar";
            btnIniciar.Size = new Size(105, 23);
            btnIniciar.TabIndex = 1;
            btnIniciar.Text = "Iniciar";
            btnIniciar.UseVisualStyleBackColor = false;
            btnIniciar.Click += btnIniciar_Click;
            // 
            // btnDetener
            // 
            btnDetener.BackColor = Color.OrangeRed;
            btnDetener.FlatAppearance.BorderSize = 0;
            btnDetener.FlatStyle = FlatStyle.Flat;
            btnDetener.ForeColor = SystemColors.ControlLightLight;
            btnDetener.Location = new Point(132, 69);
            btnDetener.Name = "btnDetener";
            btnDetener.Size = new Size(103, 23);
            btnDetener.TabIndex = 2;
            btnDetener.Text = "Detener";
            btnDetener.UseVisualStyleBackColor = false;
            btnDetener.Click += btnDetener_Click;
            // 
            // fbdSelectPath
            // 
            fbdSelectPath.HelpRequest += folderBrowserDialog1_HelpRequest;
            // 
            // btnSelectPath
            // 
            btnSelectPath.Location = new Point(321, 12);
            btnSelectPath.Name = "btnSelectPath";
            btnSelectPath.Size = new Size(38, 23);
            btnSelectPath.TabIndex = 3;
            btnSelectPath.Text = "...";
            btnSelectPath.UseVisualStyleBackColor = true;
            btnSelectPath.Click += btnSelectPath_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(373, 154);
            Controls.Add(btnSelectPath);
            Controls.Add(btnDetener);
            Controls.Add(btnIniciar);
            Controls.Add(txtRuta);
            Name = "Form1";
            Text = "FotoFlow";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtRuta;
        private Button btnIniciar;
        private Button btnDetener;
        private FolderBrowserDialog fbdSelectPath;
        private Button btnSelectPath;
    }
}
