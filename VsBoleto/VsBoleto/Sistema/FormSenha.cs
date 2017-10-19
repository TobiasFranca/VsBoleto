using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace VsBoleto.Sistema
{
    public partial class FormSenha : DevExpress.XtraEditors.XtraForm
    {
        private string senha;

        private bool permitido = false;

        public bool Permitido
        {
            get { return permitido; }
        }

        public FormSenha(string senha = "dram", string titulo = "Senha", bool mascaraPw = true)
        {
            InitializeComponent();
            this.senha = senha.Trim();
            this.Text = titulo;
            tbxSenha.Properties.UseSystemPasswordChar = mascaraPw;
            chkPw.Checked = !mascaraPw;
        }  

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            permitido = senha.Equals(tbxSenha.Text.Trim());
            this.Close();
            this.Dispose();
        }

        private void chkPw_CheckedChanged(object sender, EventArgs e)
        {
            tbxSenha.Properties.UseSystemPasswordChar = !chkPw.Checked;
        }
    }
}