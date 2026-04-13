namespace FotoFlow
{
    partial class FrmFotoFlowAdvance
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmFotoFlowAdvance));
            pnlImageContainer = new Panel();
            pcbxPreview = new PictureBox();
            pnlForm = new Panel();
            btnBasic = new Button();
            pgrbrStatus = new ProgressBar();
            lblSatus = new Label();
            groupBox1 = new GroupBox();
            chbxReceiveMultiple = new CheckBox();
            chbxValidateDelete = new CheckBox();
            btnStop = new Button();
            btnStart = new Button();
            grpbxData = new GroupBox();
            txtNewNameFile = new TextBox();
            label3 = new Label();
            txtPath = new TextBox();
            btnSaveFile = new Button();
            btnSelectPath = new Button();
            txtNameFilePrimary = new TextBox();
            label2 = new Label();
            label1 = new Label();
            pnlImageContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pcbxPreview).BeginInit();
            pnlForm.SuspendLayout();
            groupBox1.SuspendLayout();
            grpbxData.SuspendLayout();
            SuspendLayout();
            // 
            // pnlImageContainer
            // 
            pnlImageContainer.BackColor = SystemColors.ControlDark;
            pnlImageContainer.Controls.Add(pcbxPreview);
            pnlImageContainer.Dock = DockStyle.Left;
            pnlImageContainer.Location = new Point(0, 0);
            pnlImageContainer.Name = "pnlImageContainer";
            pnlImageContainer.Size = new Size(438, 672);
            pnlImageContainer.TabIndex = 0;
            // 
            // pcbxPreview
            // 
            pcbxPreview.Dock = DockStyle.Fill;
            pcbxPreview.Location = new Point(0, 0);
            pcbxPreview.Name = "pcbxPreview";
            pcbxPreview.Size = new Size(438, 672);
            pcbxPreview.SizeMode = PictureBoxSizeMode.Zoom;
            pcbxPreview.TabIndex = 0;
            pcbxPreview.TabStop = false;
            // 
            // pnlForm
            // 
            pnlForm.BackColor = Color.Transparent;
            pnlForm.Controls.Add(btnBasic);
            pnlForm.Controls.Add(pgrbrStatus);
            pnlForm.Controls.Add(lblSatus);
            pnlForm.Controls.Add(groupBox1);
            pnlForm.Controls.Add(grpbxData);
            pnlForm.Dock = DockStyle.Fill;
            pnlForm.Location = new Point(438, 0);
            pnlForm.Name = "pnlForm";
            pnlForm.Size = new Size(488, 672);
            pnlForm.TabIndex = 1;
            // 
            // btnBasic
            // 
            btnBasic.Location = new Point(391, 12);
            btnBasic.Name = "btnBasic";
            btnBasic.Size = new Size(75, 23);
            btnBasic.TabIndex = 9;
            btnBasic.Text = "Básico";
            btnBasic.UseVisualStyleBackColor = true;
            btnBasic.Click += btnBasic_Click;
            // 
            // pgrbrStatus
            // 
            pgrbrStatus.Location = new Point(30, 521);
            pgrbrStatus.Name = "pgrbrStatus";
            pgrbrStatus.Size = new Size(436, 23);
            pgrbrStatus.TabIndex = 7;
            pgrbrStatus.Visible = false;
            // 
            // lblSatus
            // 
            lblSatus.AutoSize = true;
            lblSatus.Location = new Point(30, 491);
            lblSatus.Name = "lblSatus";
            lblSatus.Size = new Size(32, 15);
            lblSatus.TabIndex = 8;
            lblSatus.Text = "Listo";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(chbxReceiveMultiple);
            groupBox1.Controls.Add(chbxValidateDelete);
            groupBox1.Controls.Add(btnStop);
            groupBox1.Controls.Add(btnStart);
            groupBox1.Location = new Point(30, 52);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(436, 118);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "Controles";
            // 
            // chbxReceiveMultiple
            // 
            chbxReceiveMultiple.AutoSize = true;
            chbxReceiveMultiple.Location = new Point(5, 93);
            chbxReceiveMultiple.Name = "chbxReceiveMultiple";
            chbxReceiveMultiple.Size = new Size(168, 19);
            chbxReceiveMultiple.TabIndex = 3;
            chbxReceiveMultiple.Text = "Recibir múltiples imágenes";
            chbxReceiveMultiple.UseVisualStyleBackColor = true;
            // 
            // chbxValidateDelete
            // 
            chbxValidateDelete.AutoSize = true;
            chbxValidateDelete.Location = new Point(5, 68);
            chbxValidateDelete.Name = "chbxValidateDelete";
            chbxValidateDelete.Size = new Size(197, 19);
            chbxValidateDelete.TabIndex = 2;
            chbxValidateDelete.Text = "Borrar las fotos una vez tomadas";
            chbxValidateDelete.UseVisualStyleBackColor = true;
            chbxValidateDelete.CheckedChanged += chbxValidateDelete_CheckedChanged;
            // 
            // btnStop
            // 
            btnStop.BackColor = Color.Red;
            btnStop.FlatAppearance.BorderSize = 0;
            btnStop.FlatStyle = FlatStyle.Flat;
            btnStop.ForeColor = SystemColors.ControlLightLight;
            btnStop.Location = new Point(249, 28);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(144, 30);
            btnStop.TabIndex = 1;
            btnStop.Text = "Detener trasnferencia";
            btnStop.UseVisualStyleBackColor = false;
            btnStop.Click += btnStop_Click;
            // 
            // btnStart
            // 
            btnStart.BackColor = Color.YellowGreen;
            btnStart.FlatAppearance.BorderSize = 0;
            btnStart.FlatStyle = FlatStyle.Flat;
            btnStart.ForeColor = SystemColors.ControlLightLight;
            btnStart.Location = new Point(47, 28);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(142, 30);
            btnStart.TabIndex = 0;
            btnStart.Text = "Iniciar transferencia";
            btnStart.UseVisualStyleBackColor = false;
            btnStart.Click += btnStart_Click;
            // 
            // grpbxData
            // 
            grpbxData.Controls.Add(txtNewNameFile);
            grpbxData.Controls.Add(label3);
            grpbxData.Controls.Add(txtPath);
            grpbxData.Controls.Add(btnSaveFile);
            grpbxData.Controls.Add(btnSelectPath);
            grpbxData.Controls.Add(txtNameFilePrimary);
            grpbxData.Controls.Add(label2);
            grpbxData.Controls.Add(label1);
            grpbxData.Location = new Point(30, 190);
            grpbxData.Name = "grpbxData";
            grpbxData.Size = new Size(436, 289);
            grpbxData.TabIndex = 0;
            grpbxData.TabStop = false;
            grpbxData.Text = "Información del archivo";
            // 
            // txtNewNameFile
            // 
            txtNewNameFile.Location = new Point(6, 154);
            txtNewNameFile.MaxLength = 350;
            txtNewNameFile.Name = "txtNewNameFile";
            txtNewNameFile.Size = new Size(424, 23);
            txtNewNameFile.TabIndex = 8;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(6, 136);
            label3.Name = "label3";
            label3.Size = new Size(108, 15);
            label3.TabIndex = 7;
            label3.Text = "Renombrar archivo";
            // 
            // txtPath
            // 
            txtPath.Location = new Point(6, 97);
            txtPath.Name = "txtPath";
            txtPath.Size = new Size(375, 23);
            txtPath.TabIndex = 6;
            // 
            // btnSaveFile
            // 
            btnSaveFile.Location = new Point(320, 251);
            btnSaveFile.Name = "btnSaveFile";
            btnSaveFile.Size = new Size(110, 29);
            btnSaveFile.TabIndex = 5;
            btnSaveFile.Text = "Guardar imagen";
            btnSaveFile.UseVisualStyleBackColor = true;
            btnSaveFile.Click += btnSaveFile_Click;
            // 
            // btnSelectPath
            // 
            btnSelectPath.Location = new Point(387, 96);
            btnSelectPath.Name = "btnSelectPath";
            btnSelectPath.Size = new Size(43, 23);
            btnSelectPath.TabIndex = 4;
            btnSelectPath.Text = "...";
            btnSelectPath.UseVisualStyleBackColor = true;
            // 
            // txtNameFilePrimary
            // 
            txtNameFilePrimary.Location = new Point(6, 37);
            txtNameFilePrimary.Name = "txtNameFilePrimary";
            txtNameFilePrimary.ReadOnly = true;
            txtNameFilePrimary.Size = new Size(424, 23);
            txtNameFilePrimary.TabIndex = 2;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(5, 19);
            label2.Name = "label2";
            label2.Size = new Size(115, 15);
            label2.TabIndex = 1;
            label2.Text = "Nombre del archivo:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(6, 76);
            label1.Name = "label1";
            label1.Size = new Size(142, 15);
            label1.TabIndex = 0;
            label1.Text = "Ruta a guardar la imagen:";
            // 
            // FrmFotoFlowAdvance
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(926, 672);
            Controls.Add(pnlForm);
            Controls.Add(pnlImageContainer);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximumSize = new Size(942, 711);
            MinimumSize = new Size(942, 711);
            Name = "FrmFotoFlowAdvance";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "FotoFlow Advance Mode";
            FormClosed += FrmFotoFlowAdvance_FormClosed;
            Load += FrmFotoFlowAdvance_Load;
            pnlImageContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pcbxPreview).EndInit();
            pnlForm.ResumeLayout(false);
            pnlForm.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            grpbxData.ResumeLayout(false);
            grpbxData.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlImageContainer;
        private PictureBox pcbxPreview;
        private Panel pnlForm;
        private GroupBox grpbxData;
        private Label label2;
        private Label label1;
        private Button btnSelectPath;
        private TextBox textBox2;
        private TextBox txtNameFilePrimary;
        private Label lblSatus;
        private ProgressBar pgrbrStatus;
        private GroupBox groupBox1;
        private Button btnSaveFile;
        private Button btnStop;
        private Button btnStart;
        private TextBox txtNewNameFile;
        private Label label3;
        private TextBox txtPath;
        private CheckBox chbxValidateDelete;
        private CheckBox chbxReceiveMultiple;
        private Button btnBasic;
    }
}
