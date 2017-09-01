using System;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Drawing.Printing;
using VsBoleto.Utilitarios;
using FirebirdSql.Data.FirebirdClient;
using System.Net.Mail;
using System.Net;

namespace VsBoleto.Sistema
{
    public partial class FormConfig : XtraForm
    {
        private string pathConfig = Application.StartupPath + "\\Config.ini";

        private bool testado = false;

        public bool Testado
        {
            get
            {
                TestarEnvio();
                return testado;
            }
            set { testado = value; }
        }

        public FormConfig()
        {
            InitializeComponent();
            CarregarConfiguracao();
            tbxSenhaEmail.Properties.UseSystemPasswordChar = !chkMostrarSenha.Checked;
        }

        private void CarregarConfiguracao()
        {
            foreach (string s in PrinterSettings.InstalledPrinters)
            {
                ddlImpressora.Properties.Items.Add(s);
            }

            string impressoraPadraoPC = new PrinterSettings().PrinterName;
            string impPadrao = ArquivoINI.LeString(pathConfig, "CONFIG", "IMPRESSORA", valorDefault: "");

            if (impPadrao.IsNotNullOrEmpty())
            {
                ddlImpressora.SelectedItem = impPadrao;
            }
            else
            {
                ddlImpressora.SelectedItem = impressoraPadraoPC;
            }

            radioGroup1.SelectedIndex = int.Parse(ArquivoINI.LeString(pathConfig, "CONFIG", "ROTINA", valorDefault: "1"));
            tbxAtualizar.Text = ArquivoINI.LeString(pathConfig, "CONFIG", "INTERVALO", valorDefault: "10");            

            tbxArquivo.Text = ArquivoINI.LeString(pathConfig, "BANCO", "CAMINHO", valorDefault: "");
            tbxSource.Text = ArquivoINI.LeString(pathConfig, "BANCO", "SERVIDOR", valorDefault: "");
            tbxPort.Text = ArquivoINI.LeString(pathConfig, "BANCO", "PORTA", valorDefault: "");
            tbxUser.Text = ArquivoINI.LeString(pathConfig, "BANCO", "USUARIO", valorDefault: "");
            tbxPW.Text = ArquivoINI.LeString(pathConfig, "BANCO", "SENHA", criptografado: true, valorDefault: "");

            tbxServerEmail.Text = ArquivoINI.LeString(pathConfig, "EMAIL", "SERVER", valorDefault: "");
            tbxPortEmail.Text = ArquivoINI.LeString(pathConfig, "EMAIL", "PORTA", valorDefault: "");
            tbxUserEmail.Text = ArquivoINI.LeString(pathConfig, "EMAIL", "USUARIO", valorDefault: "");
            tbxSenhaEmail.Text = ArquivoINI.LeString(pathConfig, "EMAIL", "SENHA", true, valorDefault: "");
            tbxAssuntoEmail.Text = ArquivoINI.LeString(pathConfig, "EMAIL", "ASSUNTO", valorDefault: "");
            tbxNomeEmail.Text = ArquivoINI.LeString(pathConfig, "EMAIL", "NOME", valorDefault: "");
            tbxCorpo.Text = ArquivoINI.LeString(pathConfig, "EMAIL", "CORPO", valorDefault: "", multiplasLinhas: true);
            chkCorpoHtml.Checked = ArquivoINI.LeBool(pathConfig, "EMAIL", "HTML", false);
            chkSSL.Checked = ArquivoINI.LeBool(pathConfig, "EMAIL", "SSL", false);
        }

        private void BtnSalvarConfigImpressao_Click(object sender, EventArgs e)
        {
            try
            {
                if (ddlImpressora.SelectedIndex < 0)
                {
                    MessageBox.Show("Selecione uma impressora padrão");
                    return;
                }
                ArquivoINI.EscreveString(pathConfig, "CONFIG", "ROTINA", radioGroup1.SelectedIndex.ToString());
                ArquivoINI.EscreveString(pathConfig, "CONFIG", "IMPRESSORA", ddlImpressora.SelectedItem.ToString());
                ArquivoINI.EscreveString(pathConfig, "CONFIG", "INTERVALO", tbxAtualizar.Text);               
               
                MessageBox.Show("Configurações de impressão salvas");

                if (radioGroup1.SelectedIndex == 0)
                {
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar configurações de impressão. Tente novamente. \n\r" + ex.Message);
            }
        }

        private void TbxArquivo_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog
            {
                Filter = "Firebird (*.fdb)|*.fdb|Interbase (*.gdb)|*.gdb"
            };

            if (file.ShowDialog() == DialogResult.OK)
            {
                tbxArquivo.Text = file.FileName;
            }
        }

        private void BtnSalvarConfig_Click(object sender, EventArgs e)
        {
            try
            {
                if (TestarConexaoComBD())
                {
                    ArquivoINI.EscreveString(pathConfig, "BANCO", "CAMINHO", tbxArquivo.Text);
                    ArquivoINI.EscreveString(pathConfig, "BANCO", "SERVIDOR", tbxSource.Text);
                    ArquivoINI.EscreveString(pathConfig, "BANCO", "PORTA", tbxPort.Text);
                    ArquivoINI.EscreveString(pathConfig, "BANCO", "USUARIO", tbxUser.Text);
                    ArquivoINI.EscreveString(pathConfig, "BANCO", "SENHA", tbxPW.Text, true);                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar configurações. Tente novamente. \n\r" + ex.StackTrace);
            }

            
        }

        private bool TestarConexaoComBD()
        {
            bool teste = false;

            if (tbxArquivo.Text.IsNotNullOrEmpty() && tbxUser.Text.IsNotNullOrEmpty() &&
                tbxPW.Text.IsNotNullOrEmpty() && tbxSource.Text.IsNotNullOrEmpty() &&
                tbxPort.Text.IsNotNullOrEmpty())
            {
                try
                {

                    FbConnection fb = new FbConnection(@"User=" + tbxUser.Text + ";Password=" + tbxPW.Text + @";
                Database=" + tbxArquivo.Text + ";DataSource=" + tbxSource.Text + ";Port=" + tbxPort.Text + ";Dialect=3;");
                    fb.Open();
                                        
                    MessageBox.Show("Conexão realizada com sucesso");
                    teste = true;
                }
                catch (FbException ex)
                {
                    MessageBox.Show("Erro na conexão com o BD: " + ex.Message);
                }

            }
            else
            {
                MessageBox.Show("Insira todos os dados pedidos para conexão");
            }

            return teste;
        }

        private void SliderIntervalo_EditValueChanged(object sender, EventArgs e)
        {
            tbxAtualizar.Text = SliderIntervalo.Value.ToString();
        }

        private void TbxAtualizar_EditValueChanged(object sender, EventArgs e)
        {
            SliderIntervalo.Value = int.Parse(tbxAtualizar.Text);
        }

        private void ChkMostrarSenha_CheckedChanged(object sender, EventArgs e)
        {
            tbxSenhaEmail.Properties.UseSystemPasswordChar = !chkMostrarSenha.Checked;
        }

        private void BtnSalvarConfigEmail_Click(object sender, EventArgs e)
        {
            if (!testado)
            {
                MessageBox.Show("Email não configurado corretamente ou não testado.\nVerifique os dados e faça um novo teste antes de salvar.");
                return;
            }

            ArquivoINI.EscreveString(pathConfig, "EMAIL", "SERVER", tbxServerEmail.Text);
            ArquivoINI.EscreveString(pathConfig, "EMAIL", "PORTA", tbxPortEmail.Text);
            ArquivoINI.EscreveString(pathConfig, "EMAIL", "USUARIO", tbxUserEmail.Text);
            ArquivoINI.EscreveString(pathConfig, "EMAIL", "SENHA", tbxSenhaEmail.Text, true);
            ArquivoINI.EscreveString(pathConfig, "EMAIL", "ASSUNTO", tbxAssuntoEmail.Text);
            ArquivoINI.EscreveString(pathConfig, "EMAIL", "NOME", tbxNomeEmail.Text);
            ArquivoINI.EscreveString(pathConfig, "EMAIL", "CORPO", tbxCorpo.Text, multiplasLinhas: true);
            ArquivoINI.EscreveBool(pathConfig, "EMAIL", "HTML", chkCorpoHtml.Checked);
            ArquivoINI.EscreveBool(pathConfig, "EMAIL", "SSL", chkSSL.Checked);

            MessageBox.Show("Configurações de Email salvas");
        }

        private void BtnTestarEnvioEmail_Click(object sender, EventArgs e)
        {
            try
            {
                TestarEnvio();
                MessageBox.Show("Email enviado");
                Testado = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao testar envio de email\n" + ex.Message + ex.InnerException ?? ex.InnerException.Message);
            }
        }

        private bool TestarEnvio()
        {
            try
            {
                MailAddress from = new MailAddress(tbxUserEmail.Text);

                SmtpClient smtp = new SmtpClient()
                {
                    Host = tbxServerEmail.Text,
                    Port = Convert.ToInt32(tbxPortEmail.Text),
                    EnableSsl = chkSSL.Checked,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(tbxUserEmail.Text, tbxSenhaEmail.Text),
                    Timeout = 25000,
                };

                MailMessage message = new MailMessage();

                message.To.Add(tbxUserEmail.Text);

                message.From = from;
                message.IsBodyHtml = chkCorpoHtml.Checked;
                message.Subject = tbxAssuntoEmail.Text;
                message.Body = tbxCorpo.Text;                

                smtp.Send(message);

                message.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}