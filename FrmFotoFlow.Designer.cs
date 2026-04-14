namespace FotoFlow
{
    partial class FrmFotoFlow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmFotoFlow));
            txtPath = new TextBox();
            btnIniciar = new Button();
            btnDetener = new Button();
            btnSelectPath = new Button();
            chbxValidateDelete = new CheckBox();
            fbdSelectPath = new FolderBrowserDialog();
            pgrbrStatusPhoto = new ProgressBar();
            lblStatusPhoto = new Label();
            btnAdvance = new Button();
            SuspendLayout();
            // 
            // txtPath
            // 
            txtPath.Location = new Point(9, 50);
            txtPath.Name = "txtPath";
            txtPath.Size = new Size(303, 23);
            txtPath.TabIndex = 0;
            // 
            // btnIniciar
            // 
            btnIniciar.BackColor = Color.YellowGreen;
            btnIniciar.FlatAppearance.BorderSize = 0;
            btnIniciar.FlatStyle = FlatStyle.Flat;
            btnIniciar.ForeColor = SystemColors.ControlLightLight;
            btnIniciar.Location = new Point(6, 143);
            btnIniciar.Name = "btnIniciar";
            btnIniciar.Size = new Size(105, 23);
            btnIniciar.TabIndex = 1;
            btnIniciar.Text = "Iniciar";
            btnIniciar.UseVisualStyleBackColor = false;
            btnIniciar.Click += btnIniciar_Click;
            // 
            // btnDetener
            // 
            btnDetener.BackColor = Color.Red;
            btnDetener.FlatAppearance.BorderSize = 0;
            btnDetener.FlatStyle = FlatStyle.Flat;
            btnDetener.ForeColor = SystemColors.ControlLightLight;
            btnDetener.Location = new Point(126, 143);
            btnDetener.Name = "btnDetener";
            btnDetener.Size = new Size(103, 23);
            btnDetener.TabIndex = 2;
            btnDetener.Text = "Detener";
            btnDetener.UseVisualStyleBackColor = false;
            btnDetener.Click += btnDetener_Click;
            // 
            // btnSelectPath
            // 
            btnSelectPath.Location = new Point(318, 50);
            btnSelectPath.Name = "btnSelectPath";
            btnSelectPath.Size = new Size(38, 23);
            btnSelectPath.TabIndex = 3;
            btnSelectPath.Text = "...";
            btnSelectPath.UseVisualStyleBackColor = true;
            btnSelectPath.Click += btnSelectPath_Click;
            // 
            // chbxValidateDelete
            // 
            chbxValidateDelete.AutoSize = true;
            chbxValidateDelete.Location = new Point(9, 96);
            chbxValidateDelete.Name = "chbxValidateDelete";
            chbxValidateDelete.Size = new Size(197, 19);
            chbxValidateDelete.TabIndex = 4;
            chbxValidateDelete.Text = "Borrar las fotos una vez tomadas";
            chbxValidateDelete.UseVisualStyleBackColor = true;
            chbxValidateDelete.CheckedChanged += chbxValidateDelete_CheckedChanged;
            // 
            // fbdSelectPath
            // 
            fbdSelectPath.InitialDirectory = "C:/";
            fbdSelectPath.RootFolder = Environment.SpecialFolder.MyPictures;
            // 
            // pgrbrStatusPhoto
            // 
            pgrbrStatusPhoto.Location = new Point(6, 182);
            pgrbrStatusPhoto.Name = "pgrbrStatusPhoto";
            pgrbrStatusPhoto.Size = new Size(350, 23);
            pgrbrStatusPhoto.TabIndex = 5;
            // 
            // lblStatusPhoto
            // 
            lblStatusPhoto.AutoSize = true;
            lblStatusPhoto.Location = new Point(9, 244);
            lblStatusPhoto.Name = "lblStatusPhoto";
            lblStatusPhoto.Size = new Size(35, 15);
            lblStatusPhoto.TabIndex = 6;
            lblStatusPhoto.Text = "Listo.";
            // 
            // btnAdvance
            // 
            btnAdvance.Location = new Point(285, 12);
            btnAdvance.Name = "btnAdvance";
            btnAdvance.Size = new Size(71, 23);
            btnAdvance.TabIndex = 7;
            btnAdvance.Text = "Avanzado";
            btnAdvance.UseVisualStyleBackColor = true;
            btnAdvance.Click += btnAdvance_Click;
            // 
            // FrmFotoFlow
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(372, 268);
            Controls.Add(btnAdvance);
            Controls.Add(lblStatusPhoto);
            Controls.Add(pgrbrStatusPhoto);
            Controls.Add(chbxValidateDelete);
            Controls.Add(btnSelectPath);
            Controls.Add(btnDetener);
            Controls.Add(btnIniciar);
            Controls.Add(txtPath);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximumSize = new Size(388, 307);
            MinimumSize = new Size(388, 307);
            Name = "FrmFotoFlow";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "FotoFlow";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtPath;
        private Button btnIniciar;
        private Button btnDetener;
        private Button btnSelectPath;
        private CheckBox chbxValidateDelete;
        private FolderBrowserDialog fbdSelectPath;
        private ProgressBar pgrbrStatusPhoto;
        private Label lblStatusPhoto;
        private Button btnAdvance;
    }
}
