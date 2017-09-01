namespace VsBoleto.Sistema
{
    partial class Principal
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Principal));
            this.xtraTabControl1 = new DevExpress.XtraTab.XtraTabControl();
            this.xtraTabMonitor = new DevExpress.XtraTab.XtraTabPage();
            this.groupControl2 = new DevExpress.XtraEditors.GroupControl();
            this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.pictureEdit2 = new DevExpress.XtraEditors.PictureEdit();
            this.pictureEdit1 = new DevExpress.XtraEditors.PictureEdit();
            this.xtraTabConfig = new DevExpress.XtraTab.XtraTabPage();
            this.xTabRetorno = new DevExpress.XtraTab.XtraTabPage();
            this.groupControl3 = new DevExpress.XtraEditors.GroupControl();
            this.chkBtnImpressaoAutomatica = new DevExpress.XtraEditors.CheckButton();
            this.chkPDF = new DevExpress.XtraEditors.CheckEdit();
            this.chkExibirImpressos = new DevExpress.XtraEditors.CheckEdit();
            this.groupControl4 = new DevExpress.XtraEditors.GroupControl();
            this.chkExibirRemessa = new DevExpress.XtraEditors.CheckEdit();
            this.chkSelecionarRemessa = new DevExpress.XtraEditors.CheckEdit();
            this.btnGerarRemessas = new DevExpress.XtraEditors.SimpleButton();
            this.btnAbrirPasta = new DevExpress.XtraEditors.SimpleButton();
            this.groupControl5 = new DevExpress.XtraEditors.GroupControl();
            this.btnRefresh = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.dtpDe = new DevExpress.XtraEditors.DateEdit();
            this.dtpAte = new DevExpress.XtraEditors.DateEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.tbxPesquisaNN = new DevExpress.XtraEditors.TextEdit();
            this.groupControl6 = new DevExpress.XtraEditors.GroupControl();
            this.barBtnInfoBD = new DevExpress.XtraEditors.SimpleButton();
            this.btnPesquisaNN = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).BeginInit();
            this.xtraTabControl1.SuspendLayout();
            this.xtraTabMonitor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureEdit2.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureEdit1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl3)).BeginInit();
            this.groupControl3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkPDF.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkExibirImpressos.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl4)).BeginInit();
            this.groupControl4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkExibirRemessa.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkSelecionarRemessa.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl5)).BeginInit();
            this.groupControl5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtpDe.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpDe.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpAte.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpAte.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbxPesquisaNN.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl6)).BeginInit();
            this.groupControl6.SuspendLayout();
            this.SuspendLayout();
            // 
            // xtraTabControl1
            // 
            this.xtraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xtraTabControl1.Location = new System.Drawing.Point(0, 0);
            this.xtraTabControl1.Name = "xtraTabControl1";
            this.xtraTabControl1.SelectedTabPage = this.xtraTabMonitor;
            this.xtraTabControl1.Size = new System.Drawing.Size(835, 584);
            this.xtraTabControl1.TabIndex = 0;
            this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.xtraTabMonitor,
            this.xtraTabConfig,
            this.xTabRetorno});
            // 
            // xtraTabMonitor
            // 
            this.xtraTabMonitor.Controls.Add(this.groupControl2);
            this.xtraTabMonitor.Controls.Add(this.groupControl1);
            this.xtraTabMonitor.Name = "xtraTabMonitor";
            this.xtraTabMonitor.Size = new System.Drawing.Size(829, 556);
            this.xtraTabMonitor.Text = "Monitor";
            // 
            // groupControl2
            // 
            this.groupControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupControl2.Location = new System.Drawing.Point(0, 330);
            this.groupControl2.Name = "groupControl2";
            this.groupControl2.Size = new System.Drawing.Size(829, 226);
            this.groupControl2.TabIndex = 1;
            this.groupControl2.Text = "Parcelas";
            // 
            // groupControl1
            // 
            this.groupControl1.Controls.Add(this.groupControl6);
            this.groupControl1.Controls.Add(this.groupControl5);
            this.groupControl1.Controls.Add(this.panelControl1);
            this.groupControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupControl1.Location = new System.Drawing.Point(0, 0);
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.Size = new System.Drawing.Size(829, 330);
            this.groupControl1.TabIndex = 0;
            this.groupControl1.Text = "Notas";
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.groupControl4);
            this.panelControl1.Controls.Add(this.groupControl3);
            this.panelControl1.Controls.Add(this.pictureEdit2);
            this.panelControl1.Controls.Add(this.pictureEdit1);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl1.Location = new System.Drawing.Point(2, 20);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(825, 94);
            this.panelControl1.TabIndex = 0;
            // 
            // pictureEdit2
            // 
            this.pictureEdit2.Cursor = System.Windows.Forms.Cursors.Default;
            this.pictureEdit2.Dock = System.Windows.Forms.DockStyle.Right;
            this.pictureEdit2.EditValue = global::VsBoleto.Properties.Resources.lmSIG_peq;
            this.pictureEdit2.Location = new System.Drawing.Point(713, 2);
            this.pictureEdit2.Name = "pictureEdit2";
            this.pictureEdit2.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.Auto;
            this.pictureEdit2.Properties.ZoomAccelerationFactor = 1D;
            this.pictureEdit2.Size = new System.Drawing.Size(110, 90);
            this.pictureEdit2.TabIndex = 1;
            // 
            // pictureEdit1
            // 
            this.pictureEdit1.Cursor = System.Windows.Forms.Cursors.Default;
            this.pictureEdit1.Dock = System.Windows.Forms.DockStyle.Left;
            this.pictureEdit1.EditValue = global::VsBoleto.Properties.Resources.lmRGB_peq;
            this.pictureEdit1.Location = new System.Drawing.Point(2, 2);
            this.pictureEdit1.Name = "pictureEdit1";
            this.pictureEdit1.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.Auto;
            this.pictureEdit1.Properties.ZoomAccelerationFactor = 1D;
            this.pictureEdit1.Size = new System.Drawing.Size(100, 90);
            this.pictureEdit1.TabIndex = 0;
            // 
            // xtraTabConfig
            // 
            this.xtraTabConfig.Name = "xtraTabConfig";
            this.xtraTabConfig.Size = new System.Drawing.Size(829, 556);
            this.xtraTabConfig.Text = "Configurações";
            // 
            // xTabRetorno
            // 
            this.xTabRetorno.Name = "xTabRetorno";
            this.xTabRetorno.Size = new System.Drawing.Size(829, 556);
            this.xTabRetorno.Text = "Retorno";
            // 
            // groupControl3
            // 
            this.groupControl3.Controls.Add(this.chkExibirImpressos);
            this.groupControl3.Controls.Add(this.chkPDF);
            this.groupControl3.Controls.Add(this.chkBtnImpressaoAutomatica);
            this.groupControl3.Location = new System.Drawing.Point(105, 0);
            this.groupControl3.Name = "groupControl3";
            this.groupControl3.Size = new System.Drawing.Size(129, 94);
            this.groupControl3.TabIndex = 2;
            this.groupControl3.Text = "Impressão";
            // 
            // chkBtnImpressaoAutomatica
            // 
            this.chkBtnImpressaoAutomatica.Location = new System.Drawing.Point(29, 23);
            this.chkBtnImpressaoAutomatica.Name = "chkBtnImpressaoAutomatica";
            this.chkBtnImpressaoAutomatica.Size = new System.Drawing.Size(75, 23);
            this.chkBtnImpressaoAutomatica.TabIndex = 0;
            this.chkBtnImpressaoAutomatica.Text = "Automática";
            // 
            // chkPDF
            // 
            this.chkPDF.Location = new System.Drawing.Point(5, 52);
            this.chkPDF.Name = "chkPDF";
            this.chkPDF.Properties.Caption = "Gerar PDF das Notas";
            this.chkPDF.Size = new System.Drawing.Size(124, 19);
            this.chkPDF.TabIndex = 1;
            // 
            // chkExibirImpressos
            // 
            this.chkExibirImpressos.EditValue = true;
            this.chkExibirImpressos.Location = new System.Drawing.Point(5, 70);
            this.chkExibirImpressos.Name = "chkExibirImpressos";
            this.chkExibirImpressos.Properties.Caption = "Exibir Impressos";
            this.chkExibirImpressos.Size = new System.Drawing.Size(124, 19);
            this.chkExibirImpressos.TabIndex = 2;
            // 
            // groupControl4
            // 
            this.groupControl4.Controls.Add(this.btnAbrirPasta);
            this.groupControl4.Controls.Add(this.btnGerarRemessas);
            this.groupControl4.Controls.Add(this.chkExibirRemessa);
            this.groupControl4.Controls.Add(this.chkSelecionarRemessa);
            this.groupControl4.Location = new System.Drawing.Point(237, 0);
            this.groupControl4.Name = "groupControl4";
            this.groupControl4.Size = new System.Drawing.Size(146, 94);
            this.groupControl4.TabIndex = 3;
            this.groupControl4.Text = "Remessa";
            // 
            // chkExibirRemessa
            // 
            this.chkExibirRemessa.EditValue = true;
            this.chkExibirRemessa.Location = new System.Drawing.Point(5, 70);
            this.chkExibirRemessa.Name = "chkExibirRemessa";
            this.chkExibirRemessa.Properties.Caption = "Exibir Remessa";
            this.chkExibirRemessa.Size = new System.Drawing.Size(124, 19);
            this.chkExibirRemessa.TabIndex = 2;
            // 
            // chkSelecionarRemessa
            // 
            this.chkSelecionarRemessa.EditValue = true;
            this.chkSelecionarRemessa.Location = new System.Drawing.Point(5, 52);
            this.chkSelecionarRemessa.Name = "chkSelecionarRemessa";
            this.chkSelecionarRemessa.Properties.Caption = "Selecionar Todas";
            this.chkSelecionarRemessa.Size = new System.Drawing.Size(124, 19);
            this.chkSelecionarRemessa.TabIndex = 1;
            // 
            // btnGerarRemessas
            // 
            this.btnGerarRemessas.Location = new System.Drawing.Point(5, 23);
            this.btnGerarRemessas.Name = "btnGerarRemessas";
            this.btnGerarRemessas.Size = new System.Drawing.Size(100, 23);
            this.btnGerarRemessas.TabIndex = 3;
            this.btnGerarRemessas.Text = "Gerar Selecionadas";
            // 
            // btnAbrirPasta
            // 
            this.btnAbrirPasta.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("simpleButton2.ImageOptions.Image")));
            this.btnAbrirPasta.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnAbrirPasta.Location = new System.Drawing.Point(111, 23);
            this.btnAbrirPasta.Name = "btnAbrirPasta";
            this.btnAbrirPasta.Size = new System.Drawing.Size(30, 23);
            this.btnAbrirPasta.TabIndex = 4;
            // 
            // groupControl5
            // 
            this.groupControl5.Controls.Add(this.btnPesquisaNN);
            this.groupControl5.Controls.Add(this.tbxPesquisaNN);
            this.groupControl5.Controls.Add(this.labelControl3);
            this.groupControl5.Controls.Add(this.dtpAte);
            this.groupControl5.Controls.Add(this.dtpDe);
            this.groupControl5.Controls.Add(this.labelControl2);
            this.groupControl5.Controls.Add(this.labelControl1);
            this.groupControl5.Controls.Add(this.btnRefresh);
            this.groupControl5.Location = new System.Drawing.Point(388, 20);
            this.groupControl5.Name = "groupControl5";
            this.groupControl5.Size = new System.Drawing.Size(253, 94);
            this.groupControl5.TabIndex = 4;
            this.groupControl5.Text = "Pesquisa";
            // 
            // btnRefresh
            // 
            this.btnRefresh.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("simpleButton3.ImageOptions.Image")));
            this.btnRefresh.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnRefresh.Location = new System.Drawing.Point(138, 46);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(30, 23);
            this.btnRefresh.TabIndex = 4;
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(5, 38);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(17, 13);
            this.labelControl1.TabIndex = 5;
            this.labelControl1.Text = "De:";
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(5, 71);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(21, 13);
            this.labelControl2.TabIndex = 6;
            this.labelControl2.Text = "Até:";
            // 
            // dtpDe
            // 
            this.dtpDe.EditValue = null;
            this.dtpDe.Location = new System.Drawing.Point(32, 30);
            this.dtpDe.Name = "dtpDe";
            this.dtpDe.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtpDe.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtpDe.Size = new System.Drawing.Size(100, 20);
            this.dtpDe.TabIndex = 7;
            // 
            // dtpAte
            // 
            this.dtpAte.EditValue = null;
            this.dtpAte.Location = new System.Drawing.Point(32, 64);
            this.dtpAte.Name = "dtpAte";
            this.dtpAte.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtpAte.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtpAte.Size = new System.Drawing.Size(100, 20);
            this.dtpAte.TabIndex = 8;
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(174, 19);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(73, 13);
            this.labelControl3.TabIndex = 9;
            this.labelControl3.Text = "Nosso Número:";
            // 
            // tbxPesquisaNN
            // 
            this.tbxPesquisaNN.Location = new System.Drawing.Point(174, 35);
            this.tbxPesquisaNN.Name = "tbxPesquisaNN";
            this.tbxPesquisaNN.Size = new System.Drawing.Size(73, 20);
            this.tbxPesquisaNN.TabIndex = 10;
            // 
            // groupControl6
            // 
            this.groupControl6.Controls.Add(this.barBtnInfoBD);
            this.groupControl6.Location = new System.Drawing.Point(647, 20);
            this.groupControl6.Name = "groupControl6";
            this.groupControl6.Size = new System.Drawing.Size(62, 94);
            this.groupControl6.TabIndex = 5;
            this.groupControl6.Text = "Ajustes";
            // 
            // barBtnInfoBD
            // 
            this.barBtnInfoBD.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("simpleButton4.ImageOptions.Image")));
            this.barBtnInfoBD.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.barBtnInfoBD.Location = new System.Drawing.Point(13, 36);
            this.barBtnInfoBD.Name = "barBtnInfoBD";
            this.barBtnInfoBD.Size = new System.Drawing.Size(35, 38);
            this.barBtnInfoBD.TabIndex = 4;
            // 
            // btnPesquisaNN
            // 
            this.btnPesquisaNN.Location = new System.Drawing.Point(174, 61);
            this.btnPesquisaNN.Name = "btnPesquisaNN";
            this.btnPesquisaNN.Size = new System.Drawing.Size(75, 23);
            this.btnPesquisaNN.TabIndex = 11;
            this.btnPesquisaNN.Text = "Pesquisar";
            // 
            // Principal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(835, 584);
            this.Controls.Add(this.xtraTabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "Principal";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "VsBoleto";
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).EndInit();
            this.xtraTabControl1.ResumeLayout(false);
            this.xtraTabMonitor.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.groupControl2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureEdit2.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureEdit1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl3)).EndInit();
            this.groupControl3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chkPDF.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkExibirImpressos.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl4)).EndInit();
            this.groupControl4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chkExibirRemessa.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkSelecionarRemessa.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl5)).EndInit();
            this.groupControl5.ResumeLayout(false);
            this.groupControl5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtpDe.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpDe.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpAte.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpAte.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbxPesquisaNN.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl6)).EndInit();
            this.groupControl6.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraTab.XtraTabControl xtraTabControl1;
        private DevExpress.XtraTab.XtraTabPage xtraTabMonitor;
        private DevExpress.XtraTab.XtraTabPage xtraTabConfig;
        private DevExpress.XtraTab.XtraTabPage xTabRetorno;
        private DevExpress.XtraEditors.GroupControl groupControl2;
        private DevExpress.XtraEditors.GroupControl groupControl1;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.PictureEdit pictureEdit1;
        private DevExpress.XtraEditors.PictureEdit pictureEdit2;
        private DevExpress.XtraEditors.GroupControl groupControl3;
        private DevExpress.XtraEditors.CheckEdit chkExibirImpressos;
        private DevExpress.XtraEditors.CheckEdit chkPDF;
        private DevExpress.XtraEditors.CheckButton chkBtnImpressaoAutomatica;
        private DevExpress.XtraEditors.GroupControl groupControl5;
        private DevExpress.XtraEditors.TextEdit tbxPesquisaNN;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.DateEdit dtpAte;
        private DevExpress.XtraEditors.DateEdit dtpDe;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.SimpleButton btnRefresh;
        private DevExpress.XtraEditors.GroupControl groupControl6;
        private DevExpress.XtraEditors.SimpleButton barBtnInfoBD;
        private DevExpress.XtraEditors.GroupControl groupControl4;
        private DevExpress.XtraEditors.SimpleButton btnAbrirPasta;
        private DevExpress.XtraEditors.SimpleButton btnGerarRemessas;
        private DevExpress.XtraEditors.CheckEdit chkExibirRemessa;
        private DevExpress.XtraEditors.CheckEdit chkSelecionarRemessa;
        private DevExpress.XtraEditors.SimpleButton btnPesquisaNN;
    }
}