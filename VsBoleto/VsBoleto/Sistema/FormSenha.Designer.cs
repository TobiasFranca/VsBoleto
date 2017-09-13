namespace VsBoleto.Sistema
{
    partial class FormSenha
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
            this.tbxSenha = new DevExpress.XtraEditors.TextEdit();
            this.chkPw = new DevExpress.XtraEditors.CheckEdit();
            this.btnConfirmar = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.tbxSenha.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPw.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // tbxSenha
            // 
            this.tbxSenha.EnterMoveNextControl = true;
            this.tbxSenha.Location = new System.Drawing.Point(12, 15);
            this.tbxSenha.Name = "tbxSenha";
            this.tbxSenha.Size = new System.Drawing.Size(200, 20);
            this.tbxSenha.TabIndex = 0;
            // 
            // chkPw
            // 
            this.chkPw.Location = new System.Drawing.Point(12, 41);
            this.chkPw.Name = "chkPw";
            this.chkPw.Properties.Caption = "Mostrar Caracteres";
            this.chkPw.Size = new System.Drawing.Size(119, 19);
            this.chkPw.TabIndex = 2;
            this.chkPw.CheckedChanged += new System.EventHandler(this.chkPw_CheckedChanged);
            // 
            // btnConfirmar
            // 
            this.btnConfirmar.Location = new System.Drawing.Point(137, 41);
            this.btnConfirmar.Name = "btnConfirmar";
            this.btnConfirmar.Size = new System.Drawing.Size(75, 23);
            this.btnConfirmar.TabIndex = 1;
            this.btnConfirmar.Text = "Confirmar";
            this.btnConfirmar.Click += new System.EventHandler(this.btnConfirmar_Click);
            // 
            // FormSenha
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(220, 74);
            this.Controls.Add(this.btnConfirmar);
            this.Controls.Add(this.chkPw);
            this.Controls.Add(this.tbxSenha);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSenha";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Senha";
            ((System.ComponentModel.ISupportInitialize)(this.tbxSenha.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPw.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.TextEdit tbxSenha;
        private DevExpress.XtraEditors.CheckEdit chkPw;
        private DevExpress.XtraEditors.SimpleButton btnConfirmar;
    }
}