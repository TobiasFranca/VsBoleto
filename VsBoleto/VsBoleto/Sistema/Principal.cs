using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using FirebirdSql.Data.FirebirdClient;
using BoletoBancario.Conta;
using BoletoBancario.Utilitarios;
using VsBoleto.Utilitarios;
using System.IO;
using System.Timers;
using BoletoBancario.Bancos;

namespace VsBoleto.Sistema
{
    public partial class Principal : XtraForm
    {
        private string usuarioRET = "", senhaRET = "", pathRET = "", portaRet = "", serverRet = "";
        FbConnection cnRET;

        private string pathIni = Application.StartupPath;
        private string pathLog = Application.StartupPath + "\\Log.txt";
        private string pathRetorno;
        private string pathPosicao = "";
        private string pathConfig = Application.StartupPath + "\\Config.ini";
        private System.Timers.Timer timer2 = new System.Timers.Timer();        

        decimal jurosMes = 0;
        decimal multa = 0;
        decimal desconto = 0;
        string inst1 = "00", inst2 = "00", protesto = "00";
        string data = "";

        private bool editando = true;
        private bool Editando
        {
            get { return editando; }
            set
            {
                editando = value;
                if (!editando)
                {
                    conta = null;
                }
                btnCriarEditar.Enabled = !editando;
                btnCancelar.Enabled = editando;
                btnLimpar.Enabled = editando;
                btnSalvar.Enabled = editando;
                tbxAgencia.Enabled = tbxConta.Enabled = tbxCodigo.Enabled = tbxOutros1.Enabled =
                tbxOutros2.Enabled = ddlNomeCedente.Enabled = tbxInicioNossoN.Enabled = tbxFimNossoN.Enabled = tbxNNAtual.Enabled = editando;
                tbxDemonstrativo.Enabled = tbxInstrucoes.Enabled = tbxPathLayoutBoleto.Enabled =
                tbxMulta.Enabled = tbxJurosMes.Enabled = tbxInstrucao1.Enabled = tbxInstrucao2.Enabled = editando;
                tbxArquivoRemessa.Enabled = ddlLayoutRemessa.Enabled = chkDesconto.Enabled = ddlEspecieTitulo.Enabled = tbxProtesto.Enabled = editando;
                ddlBanco.Enabled = tbxCarteira.Enabled = chkUtilizaNumBanco.Enabled = editando;
            }
        }

        ContaCorrente conta = null;

        public Principal()
        {
            InitializeComponent();
            Text = "VsBoleto - Versão: " + Application.ProductVersion.ToString();

            GravarLog("Iniciando Programa.");

            if (!LeConfigBase())
            {
                Close();
                return;
            }

            if (!Conecta())
            {
                Close();
                return;
            }

            dtpDe.DateTime = DateTime.Now;
            dtpAte.DateTime = DateTime.Now;
            timer2.Elapsed += Timer2_Elapsed;
            timer2.Interval = int.Parse(ArquivoINI.LeString(pathConfig, "CONFIG", "INTERVALO", valorDefault: "10")) * 1000;
            xTabRetorno.PageVisible = false;
            ExcluirImagensPng();

            ddlBanco.Properties.Items.Clear();
            foreach (EnumBanco b in Enum.GetValues(typeof(EnumBanco)))
            {
                ddlBanco.Properties.Items.Add(new ListItem(b.ToString(), b.ToString()));
            }

            LoadPosicoesEFiliais();

            CarregarGrids();


            foreach (DevExpress.XtraGrid.Columns.GridColumn item in gridNotas.Columns)
            {
                if (!item.FieldName.Equals("check"))
                {
                    item.OptionsColumn.AllowEdit = false;
                }
            }                        

        }

        private bool LeConfigBase()
        {

            try
            {
                pathRET = ArquivoINI.LeString(pathConfig, "BANCO", "CAMINHO", valorDefault: "");
                serverRet = ArquivoINI.LeString(pathConfig, "BANCO", "SERVIDOR", valorDefault: "");
                portaRet = ArquivoINI.LeString(pathConfig, "BANCO", "PORTA", valorDefault: "");
                usuarioRET = ArquivoINI.LeString(pathConfig, "BANCO", "USUARIO", valorDefault: "");
                senhaRET = ArquivoINI.LeString(pathConfig, "BANCO", "SENHA", criptografado: true, valorDefault: "");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar as configurações do banco: " + ex.Message);
                return false;
            }

            return true;
        }

        private bool Conecta()
        {
            if (usuarioRET == "" || senhaRET == "" || pathRET == "" || serverRet == "" || portaRet == "")
            {
                MessageBox.Show("Informções de conexão com o banco de dados inválida, deseja ");
                return false;
            }

            string connectionString = "DataSource=" + serverRet + ";Port=" + portaRet + ";User=" + usuarioRET + ";Password=" + senhaRET + ";DataBase=" + pathRET + ";Dialect=3";

            try
            {
                cnRET = new FbConnection(connectionString);
                cnRET.Open();
                //Globais.ConexaoRET = cnRET;                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao abrir conexão com RET:\n" + pathRET + "\n\nO programa será fechado." + ex);
                return false;
            }

            return true;
        }

        private void LoadPosicoesEFiliais()
        {
            try
            {
                lbxPosicoes.Items.Clear();
                ddlPosicoes.Properties.Items.Clear();

                string sql = @"SELECT * FROM CDPOSIC WHERE POSICAO > 0 AND ST2 = 'S'";
                DataSet ds = new DataSet();
                DataTable dtP = ds.Tables.Add("Posicoes");
                DataTable dtF = ds.Tables.Add("Filiais");
                FbDataAdapter daP = new FbDataAdapter(sql, cnRET);
                daP.Fill(dtP);

                foreach (DataRow dr in dtP.Rows)
                {
                    lbxPosicoes.Items.Add(new ListItem(dr["POSICAO"].ToString() + " - " + dr["DESCRICAO"].ToString(), dr["POSICAO"].ToString()));
                    ddlPosicoes.Properties.Items.Add(new ListItem(dr["POSICAO"].ToString() + " - " + dr["DESCRICAO"].ToString(), dr["POSICAO"].ToString()));
                }

                ddlNomeCedente.Properties.Items.Clear();

                FbDataAdapter daF = new FbDataAdapter(@"SELECT * FROM CDFILIAL", cnRET);
                daF.Fill(dtF);

                foreach (DataRow dr in dtF.Rows)
                {
                    if (dr["RAZAO"].ToString().IsNotNullOrEmpty())
                    {
                        ddlNomeCedente.Properties.Items.Add(new ListItem(dr["RAZAO"].ToString(), dr["FILIAL"].ToString()));
                    }
                }

                ddlNomeCedente.SelectedIndex = -1;
            }
            catch (Exception)
            {
                MostrarMensagem("Erro ao carregar posições/filiais. Tente Novamente.", true);
            }

        }

        private void CarregarConfiguracoes()
        {
            LimparCamposConfiguracoes();
            groupBanco.Text = "Banco (" + ((ListItem)lbxPosicoes.SelectedItem).Nome + ")";
            btnCriarEditar.Enabled = true;

            string pathPosicao = pathIni + "/Posicao/Pos" + ((ListItem)lbxPosicoes.SelectedItem).Value + ".ini";
            FileInfo f = new FileInfo(pathPosicao);
            if (!f.Exists)
            {
                MessageBox.Show("Não existe configuração para esta posição.");
                return;
            }

            string item = ArquivoINI.LeString(pathPosicao, "BANCO", "NOME", valorDefault: "");
            ddlBanco.SelectedItem = new ListItem(item, item);
            tbxCarteira.Text = ArquivoINI.LeString(pathPosicao, "BANCO", "CARTEIRA", valorDefault: "");

            ddlNomeCedente.SelectedItem = ArquivoINI.LeString(pathPosicao, "CEDENTE", "FILIAL", valorDefault: "");

            tbxCodigo.Text = ArquivoINI.LeString(pathPosicao, "CEDENTE", "CODIGO", valorDefault: "");
            tbxAgencia.Text = ArquivoINI.LeString(pathPosicao, "CEDENTE", "AGENCIA", valorDefault: "");
            tbxConta.Text = ArquivoINI.LeString(pathPosicao, "CEDENTE", "CONTA", valorDefault: "");
            tbxInicioNossoN.Text = ArquivoINI.LeString(pathPosicao, "CEDENTE", "NOSSONROI", valorDefault: "");
            tbxFimNossoN.Text = ArquivoINI.LeString(pathPosicao, "CEDENTE", "NOSSONROF", valorDefault: "");
            tbxOutros1.Text = ArquivoINI.LeString(pathPosicao, "CEDENTE", "OUTRODADO1", valorDefault: "");
            tbxOutros2.Text = ArquivoINI.LeString(pathPosicao, "CEDENTE", "OUTRODADO2", valorDefault: "");

            if (CarregarNossoNumeroDoBD(((ListItem)lbxPosicoes.SelectedItem).Value.ToString(), out string num))
            {
                tbxNNAtual.Text = num;
            }
            else
            {
                MessageBox.Show("Não foi possível carregar Nosso Número, tente novamente.\nCaso continue com erro, contate o suporte.");
            }

            tbxDemonstrativo.Text = ArquivoINI.LeString(pathPosicao, "BOLETO", "DEMONSTRATIVO", valorDefault: "", multiplasLinhas: true).Replace("\n", "\n\r");
            tbxInstrucoes.Text = ArquivoINI.LeString(pathPosicao, "BOLETO", "INSTRUCOES", valorDefault: "");
            tbxJurosMes.Text = ArquivoINI.LeString(pathPosicao, "BOLETO", "JUROS", valorDefault: "");
            tbxMulta.Text = ArquivoINI.LeString(pathPosicao, "BOLETO", "MULTA", valorDefault: "");
            tbxPathLayoutBoleto.Text = ArquivoINI.LeString(pathPosicao, "BOLETO", "LAYOUT", valorDefault: "");
            tbxInstrucao1.Text = ArquivoINI.LeString(pathPosicao, "BOLETO", "INSTRUCAO1", valorDefault: "00");
            tbxInstrucao2.Text = ArquivoINI.LeString(pathPosicao, "BOLETO", "INSTRUCAO2", valorDefault: "00");
            tbxProtesto.Text = ArquivoINI.LeString(pathPosicao, "BOLETO", "DIASPROTESTO", valorDefault: "00");
            tbxArquivoRemessa.Text = ArquivoINI.LeString(pathPosicao, "REMESSA", "CAMINHO", valorDefault: "");
            ddlLayoutRemessa.SelectedItem = ArquivoINI.LeString(pathPosicao, "REMESSA", "LAYOUT", valorDefault: "");
            ddlEspecieTitulo.SelectedItem = ArquivoINI.LeString(pathPosicao, "BOLETO", "ESPECIE", valorDefault: "");
            txtDescontoVencimento.Text = ArquivoINI.LeString(pathPosicao, "BOLETO", "DESCVENCIMENTO", valorDefault: "0");
        }

        private bool CarregarNossoNumeroDoBD(string pos, out string numero)
        {
            numero = "";
            try
            {
                string sql = "select * from gavar where variavel = 'Boleto.Pos" + pos + "'";
                DataSet ds = new DataSet();
                DataTable dt = ds.Tables.Add("GaVar");
                FbDataAdapter da = new FbDataAdapter(sql, cnRET);
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];
                    numero = dr["str30"].ToString();
                    return true;
                }
            }
            catch (Exception ex)
            {
                GravarLog("Erro ao recuperrar nosso numero do banco de dados: " + ex.Message);
            }

            return false;
        }

        private bool GravarNossoNumeroDoBD(string pos, string numero)
        {
            try
            {
                string sql = "select * from gavar where variavel = 'Boleto.Pos" + pos + "'";
                DataSet ds = new DataSet();
                DataTable dt = ds.Tables.Add("GaVar");
                FbDataAdapter da = new FbDataAdapter(sql, cnRET);
                FbCommandBuilder cb = new FbCommandBuilder(da);
                da.Fill(dt);

                if (dt.Rows.Count > 0 && numero.IsNotNullOrEmpty())
                {
                    string sqlU = "update gavar set str30='" + numero + "' where variavel = 'Boleto.Pos" + pos + "'";
                    FbCommand fc = new FbCommand(sqlU, cnRET);
                    fc.ExecuteNonQuery();
                    return true;
                }
                else if (dt.Rows.Count <= 0 && !numero.IsNotNullOrEmpty())
                {
                    string sqlI = "insert into gavar(variavel, str30) values ('Boleto.Pos" + pos + "','" + numero + "')";
                    FbCommand fc = new FbCommand(sqlI, cnRET);
                    fc.ExecuteNonQuery();
                    return true;
                }

            }
            catch (Exception ex)
            {
                GravarLog("Erro ao recuperrar nosso numero do banco de dados." + ex.Message);
            }

            return false;
        }

        private void CarregarGrids()
        {
            try
            {
                controlParcelas.DataSource = controlNotas.DataSource = null;

                string sql = SqlHelper.GetSelectNotasSaida(dtpDe.DateTime.ToString("MM/dd/yyyy"), dtpAte.DateTime.ToString("MM/dd/yyyy"));
                DataSet ds = new DataSet();
                DataTable dt = ds.Tables.Add("Notas");
                FbDataAdapter da = new FbDataAdapter(sql, cnRET);
                da.Fill(dt);

                dt.Columns.Add("check", typeof(bool));

                if (ds.Tables[0].Rows.Count > 0)
                {
                    controlNotas.DataSource = dt;
                }
                else
                {
                    controlParcelas.DataSource = controlNotas.DataSource = null;
                }

                //if (gridNotas != null)
                //{
                //    barStaticItem1.Caption = "Qtd Notas: " + gridNotas.DataRowCount;
                //}

                gridParcelas.BestFitColumns();
                gridNotas.BestFitColumns();
                chkSelecionarRemessa.Checked = true;
                ChkSelecionarRemessa_CheckedChanged(null, null);
            }
            catch (Exception)
            {
                MostrarMensagem("Erro ao carregar grids. Tente Novamente.", true);
            }

        }

        private void ExibirParcelas()
        {
            string filial = gridNotas.GetFocusedRowCellValue("FILIAL").ToString();
            string ordem = gridNotas.GetFocusedRowCellValue("ORDEM").ToString();

            if (!filial.IsNotNullOrEmpty() || !ordem.IsNotNullOrEmpty())
            {
                return;
            }

            string sql = SqlHelper.GetSelectCRMVINT(filial, ordem);
            DataSet ds = new DataSet();
            DataTable dt = ds.Tables.Add("Parcelas");
            FbDataAdapter da = new FbDataAdapter(sql, cnRET);
            da.Fill(dt);
            dt.Columns.Add("remessaCheck", typeof(bool));
            dt.Columns.Add("impressoCheck", typeof(bool));
            dt.Columns.Add("emailCheck", typeof(bool));

            auxNumParcelas = dt.Rows.Count;
            if (auxNumParcelas > 0)
            {
                controlParcelas.DataSource = dt;
            }
            else
            {
                controlParcelas.DataSource = null;
            }

            for (int i = 0; i < gridNotas.DataRowCount; i++)
            {
                DataRowView dtrw = (DataRowView)(gridNotas.GetRow(i));
                if (dtrw.Row["check"] != null)
                {
                    gridNotas.SetRowCellValue(i, "check", chkSelecionarRemessa.Checked);
                }
            }

            int cont = 0;
            foreach (DataRow row in dt.Rows)
            {
                int posic = gridParcelas.RowCount;
                if (row[dt.Columns["st3"]].ToString() == "I")
                {
                    gridParcelas.SetRowCellValue(cont, colunaRemessa, false);
                    gridParcelas.SetRowCellValue(cont, colunaImpresso, true);
                }
                else if (row[dt.Columns["st3"]].ToString() == "R")
                {
                    gridParcelas.SetRowCellValue(cont, colunaRemessa, false);
                    gridParcelas.SetRowCellValue(cont, colunaImpresso, true);
                }
                else if (row[dt.Columns["st3"]].ToString() == "A")
                {
                    gridParcelas.SetRowCellValue(cont, colunaRemessa, true);
                    gridParcelas.SetRowCellValue(cont, colunaImpresso, true);
                }
                else
                {
                    gridParcelas.SetRowCellValue(cont, colunaRemessa, false);
                    gridParcelas.SetRowCellValue(cont, colunaImpresso, false);
                }
                if (row[dt.Columns["st2"]].ToString() == "E")
                {
                    gridParcelas.SetRowCellValue(cont, colunaEmail, true);
                }
                else
                {
                    gridParcelas.SetRowCellValue(cont, colunaEmail, false);
                }
                cont++;

            }

            gridParcelas.BestFitColumns();
        }

        private void PrepararConfiguracoes()
        {
            if (!ArquivoINI.LeString(pathConfig, "CONFIG", "IMPRESSORA", valorDefault: "").IsNotNullOrEmpty())
            {
                throw new ArgumentException("Impressora padrão não selecionada.");
            }

            Configuracoes.ImpressoraPadrao = ArquivoINI.LeString(pathConfig, "CONFIG", "IMPRESSORA", valorDefault: "");

            Configuracoes.LayoutArquivoRemessa = ArquivoINI.LeString(pathPosicao, "REMESSA", "LAYOUT", valorDefault: "");
            Configuracoes.LayoutBoleto = ArquivoINI.LeString(pathPosicao, "BOLETO", "LAYOUT", valorDefault: "");

            Configuracoes.Email_Porta = ArquivoINI.LeString(pathConfig, "EMAIL", "PORTA", valorDefault: "");
            Configuracoes.Email_Servidor = ArquivoINI.LeString(pathConfig, "EMAIL", "SERVER", valorDefault: "");
            Configuracoes.Email_Usuario = ArquivoINI.LeString(pathConfig, "EMAIL", "USUARIO", valorDefault: "");
            Configuracoes.Email_Senha = ArquivoINI.LeString(pathConfig, "EMAIL", "SENHA", true, valorDefault: "");
            Configuracoes.Email_Assunto_Padrao = ArquivoINI.LeString(pathConfig, "EMAIL", "ASSUNTO", valorDefault: "");
            Configuracoes.Email_Corpo_Padrao = ArquivoINI.LeString(pathConfig, "EMAIL", "CORPO", valorDefault: "", multiplasLinhas: true);
            Configuracoes.Email_De = ArquivoINI.LeString(pathConfig, "EMAIL", "NOME", valorDefault: "");
            Configuracoes.Email_Html = ArquivoINI.LeBool(pathConfig, "EMAIL", "HTML", false);
            Configuracoes.Email_Usar_SSL = ArquivoINI.LeBool(pathConfig, "EMAIL", "SSL", false);
        }

        private void PrepararMeuBoleto(DataRow detalhe, string tipo)
        {
            PrepararMeuBoleto(null, detalhe, tipo);
        }

        private IList<string> posicIgnoradas = new List<string>();

        private bool PrepararMinhaConta(string posic)
        {
            string path = pathIni + "/Posicao/Pos" + posic + ".ini";
            FileInfo f = new FileInfo(path);
            if (!f.Exists)
            {
                if (posicIgnoradas.Contains(posic))
                {
                    return false;
                }

                posicIgnoradas.Add(posic);
                pathPosicao = "";
                MostrarMensagem("Arquivo de configuração não encontrado. Crie uma nova configuração para a posição " + posic +
                                ".\nAs notas com esta posição serão ignoradas.", true);
                return false;
            }
            pathPosicao = path;

            string banco = ArquivoINI.LeString(pathPosicao, "BANCO", "NOME", valorDefault: "");
            string carteira = ArquivoINI.LeString(pathPosicao, "BANCO", "CARTEIRA", valorDefault: "");
            string filial = ArquivoINI.LeString(pathPosicao, "CEDENTE", "FILIAL", valorDefault: "");

            if (!filial.IsNotNullOrEmpty() || !carteira.IsNotNullOrEmpty() || !banco.IsNotNullOrEmpty())
            {
                MostrarMensagem("As informações necessarias para criação da conta estão incorretas.", true);
                return false;
            }

            string sql = @"SELECT * FROM CDFILIAL WHERE FILIAL = " + filial;
            DataSet ds = new DataSet();
            DataTable dt = ds.Tables.Add("Filiais");
            FbDataAdapter da = new FbDataAdapter(sql, cnRET);
            da.Fill(dt);

            if (dt.Rows.Count <= 0)
            {
                MostrarMensagem("Filial não encontrada no banco de dados.", true);
                return false;
            }

            EnumBanco bank;

            switch (banco.ToLower())
            {
                case "itau": bank = EnumBanco.Itau; break;
                case "sicoob": bank = EnumBanco.Sicoob; break;
                case "bradesco": bank = EnumBanco.Bradesco; break;
                case "caixasr": bank = EnumBanco.CaixaSR; break;
                case "banestes": bank = EnumBanco.Banestes; break;
                case "santander": bank = EnumBanco.Santander; break;
                case "bancobrasil": bank = EnumBanco.BancoBrasil; break;
                default: return false;
            }

            if (conta == null)
            {
                conta = ContaCorrente.NovaContaCorrente(bank, carteira, ds.Tables[0].Rows[0]["RAZAO"].ToString(), ds.Tables[0].Rows[0]["CGC"].ToString());
            }

            conta.Cedente.Bairro = ds.Tables[0].Rows[0]["BAIRRO"].ToString();
            conta.Cedente.Cep = ds.Tables[0].Rows[0]["CEP"].ToString();
            conta.Cedente.Cidade = ds.Tables[0].Rows[0]["CIDADE"].ToString();
            conta.Cedente.Email = ds.Tables[0].Rows[0]["EMAIL"].ToString();
            conta.Cedente.Endereco = ds.Tables[0].Rows[0]["ENDERECO"].ToString();
            conta.Cedente.Estado = ds.Tables[0].Rows[0]["ESTADO"].ToString();
            conta.Cedente.NumEndereco = ds.Tables[0].Rows[0]["NUMERO"].ToString();

            conta.Agencia = ArquivoINI.LeString(pathPosicao, "CEDENTE", "AGENCIA", valorDefault: "");
            conta.NumeroConta = ArquivoINI.LeString(pathPosicao, "CEDENTE", "CONTA", valorDefault: "");
            conta.CodigoCedente = ArquivoINI.LeString(pathPosicao, "CEDENTE", "CODIGO", valorDefault: "");
            conta.Outros1 = ArquivoINI.LeString(pathPosicao, "CEDENTE", "OUTRODADO1", valorDefault: "");
            conta.Outros2 = ArquivoINI.LeString(pathPosicao, "CEDENTE", "OUTRODADO2", valorDefault: "");
            conta.EspecieTitulo = ArquivoINI.LeString(pathPosicao, "BOLETO", "ESPECIE").Substring(0, 2);
            conta.InicioNossoNumero = ArquivoINI.LeString(pathPosicao, "CEDENTE", "NOSSONROI", valorDefault: "").ToLong();
            conta.FimNossoNumero = ArquivoINI.LeString(pathPosicao, "CEDENTE", "NOSSONROF", valorDefault: "").ToLong();

            long nn = 0;
            if (CarregarNossoNumeroDoBD(posic, out string num))
            {
                nn = num.ToLong();
            }
            else
            {
                MostrarMensagem("Erro ao carregar Nosso Número do banco de dados. Confira se há posição criada. Caso o erro persista, contate o adminitrador do sitema.", true);
                return false;
            }

            if (nn < conta.InicioNossoNumero)
            {
                GravarLog("Proximo nosso número anterior ao limite permitido. Alterando de: " + nn + " para: " + conta.InicioNossoNumero);
                nn = conta.InicioNossoNumero;
            }
            if (nn > conta.FimNossoNumero)
            {
                throw new ArgumentException("Nosso número ultrapassou o limite permitido para esta conta. Verifique com o banco como proceder.");
            }

            conta.ProximoNossoNumero = nn;

            conta.Demonstrativo = ArquivoINI.LeString(pathPosicao, "BOLETO", "DEMONSTRATIVO", valorDefault: "", multiplasLinhas: true).Replace("\n", "\n\r");
            conta.Instrucoes = ArquivoINI.LeString(pathPosicao, "BOLETO", "INSTRUCOES", valorDefault: "");
            jurosMes = ArquivoINI.LeString(pathPosicao, "BOLETO", "JUROS", valorDefault: "").ToDecimal();
            multa = ArquivoINI.LeString(pathPosicao, "BOLETO", "MULTA", valorDefault: "").ToDecimal();
            desconto = ArquivoINI.LeString(pathPosicao, "BOLETO", "DESCVENCIMENTO", valorDefault: "").ToDecimal();
            inst1 = ArquivoINI.LeString(pathPosicao, "BOLETO", "INSTRUCAO1", valorDefault: "00");
            inst2 = ArquivoINI.LeString(pathPosicao, "BOLETO", "INSTRUCAO2", valorDefault: "00");
            protesto = ArquivoINI.LeString(pathPosicao, "BOLETO", "DIASPROTESTO", valorDefault: "00");
            return true;
        }

        private int auxNumParcelas = 1;

        private void PrepararMeuBoleto(DataRow master, DataRow detalhe, string tipo)
        {
            try
            {
                if (master == null)
                {
                    master = gridNotas.GetFocusedDataRow();
                }

                if (!PrepararMinhaConta(master["POSICAO"].ToString()))
                {
                    return;
                }

                PrepararConfiguracoes();

                Boleto bol = conta.NovoBoleto();
                bol.Sacado.CodigoSistema = master["CLIENTE"].ToString();
                bol.Sacado.Nome = master["NOME"].ToString();
                bol.Sacado.Razao = master["RAZAO"].ToString();
                bol.Sacado.CpfCnpj = master["CGCCPF"].ToString().Trim();
                bol.Sacado.Endereco = master["ENDERECO"].ToString();
                bol.Sacado.Bairro = master["BAIRRO"].ToString();
                bol.Sacado.Cidade = master["CIDADE"].ToString();
                bol.Sacado.Estado = master["ESTADO"].ToString();
                bol.Sacado.Cep = master["CEP"].ToString();
                bol.Sacado.NumEndereco = master["NUMERO"].ToString();

                bol.DataDocumento = master["DATA"].ToString().ToDateTime();
                bol.DataVencimento = detalhe["VENCIMENTO"].ToString().ToDateTime();
                bol.ValorDocumento = detalhe["VALOR"].ToString().ToDecimal();
                bol.NumeroParcelaAtual = detalhe["PARCELA"].ToString().ToInt32();
                bol.NumeroParcelas = master["NUMPARCELAS"].ToString().ToInt32();

                string nfiscal = master["NFISCAL"].ToString();
                string nro_ecf = master["NRO_ECF"].ToString();

                if (nro_ecf.IsNotNullOrEmpty())
                {
                    bol.NumeroDocumento = nro_ecf + "/" + detalhe["PARCELA"].ToString();
                }
                else if (nfiscal.IsNotNullOrEmpty())
                {
                    bol.NumeroDocumento = nfiscal + "/" + detalhe["PARCELA"].ToString();
                }
                else
                {
                    GravarLog("NFISCAL ou NRO_ECF inexistente para ORDEM " + detalhe["ORDEM"].ToString());
                    return;
                }

                bol.Instrucao1 = inst1;
                bol.Instrucao2 = inst2;
                bol.DiasProtesto = protesto;
                bol.PercentualMultaAtraso = multa;
                bol.PercentualDesconto = desconto;
                bol.ValorDesconto = bol.ValorDocumento * bol.PercentualDesconto / 100;
                bol.ValorMultaAtraso = bol.ValorDocumento * bol.PercentualMultaAtraso / 100;
                bol.PercentualJurosMesAtraso = jurosMes;
                bol.ValorJurosDiaAtraso = bol.ValorDocumento * bol.PercentualJurosDiaAtraso / 100;

                if (bol.ValorDesconto <= 0)
                {
                    bol.DataLimiteDesconto = bol.DataVencimento;
                }

                string nossoNum = detalhe["NROBOLETO"].ToString().Trim();
                if (!nossoNum.IsNotNullOrEmpty())
                {
                    bol.NossoNumero = bol.GerarProximoNossoNumero();

                    if (!GravarNossoNumeroDoBD(master["POSICAO"].ToString(), bol.NossoNumero))
                    {
                        throw new ArgumentException("Falha ao gravar nosso número no banco de dados");
                    }
                    ArquivoINI.EscreveString(pathPosicao, "CEDENTE", "NOSSONUMERO", bol.NossoNumero);
                }
                else
                {
                    bol.NossoNumero = nossoNum;
                }

                bol.CalcularDadosBoleto();

                try
                {
                    string emailDestino = master["EMAIL"].ToString();
                    string[] emails = emailDestino.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    if (emails != null)
                    {
                        foreach (string e in emails)
                        {
                            if (!string.IsNullOrEmpty(e.Trim()) && e.Contains("@"))
                            {
                                bol.EmailsDestinatarios.Add(e.Trim());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MostrarMensagem("Erro ao preparar o boleto: " + ex.Message, true);                    
                }

                string st3 = detalhe["ST3"].ToString();

                string flag = st3;

                string flagSt2 = "";

                if (tipo.ToLower() == "email")
                {
                    flagSt2 = "E";
                }

                else if (tipo.ToLower() == "imprimir")
                {
                    flag = "I";
                    if (st3.ToUpper() == "R" || st3.ToUpper() == "A")
                    {
                        flag = "A";
                        flagSt2 = "E";
                    }
                }
                else if (tipo.ToLower() == "visualizar")
                {
                    flag = st3;
                }
                else if (tipo.ToLower() == "remessa")
                {
                    flag = "R";
                    if (st3.ToUpper() == "I" || st3.ToUpper() == "A")
                    {
                        flag = "A";
                        flagSt2 = "E";
                    }
                }

                string sql = "UPDATE CRMVINT SET ST2='" + flagSt2 + "', ST3='" + flag + "', NROBOLETO='" + bol.NossoNumero + "' WHERE FILIAL = '" + detalhe["FILIAL"].ToString() + "' AND ORDEM = '" + detalhe["ORDEM"].ToString() + "' AND PARCELA = '" + detalhe["PARCELA"].ToString() + "';";
                FbCommand fc = new FbCommand(sql, cnRET);
                fc.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                MostrarMensagem(ex.Message, true);
            }
        }

        private void VisualizarBoletos(bool imprimir = false)
        {
            try
            {
                if (string.IsNullOrEmpty(Configuracoes.LayoutBoleto))
                {
                    return;
                }

                if (!imprimir)
                {
                    conta.VisualizarBoleto(Configuracoes.LayoutBoleto);
                }
                else
                {
                    conta.ImprimirBoletos(Configuracoes.LayoutBoleto);
                }
            }
            catch (Exception ex)
            {
                MostrarMensagem("Erro durante a preparação dos boletos: " + ex.Message, true);
            }
        }

        private void LimparCamposConfiguracoes()
        {
            Util.LimparCampos(xtraTabConfig);

            ddlLayoutRemessa.Properties.Items.Clear();
            ddlLayoutRemessa.SelectedItem = null;

            ddlEspecieTitulo.Properties.Items.Clear();
            ddlEspecieTitulo.SelectedItem = null;

            tbxNNAtual.Text = "";
            tbxPathLayoutBoleto.Text = "";
            tbxIdentificacao.Text = "";

            lblOD1.Text = "Outros Dados 1";
            lblOD2.Text = "Outros Dados 2";
            lblInicioNN.Text = "Início nosso Nº";
            lblFimNN.Text = "Fim nosso nº";
            lblAgencia.Text = "Agência";
            lblCodigo.Text = "Código";
            lblConta.Text = "Conta";

            tbxAgencia.Text = tbxCodigo.Text = tbxConta.Text = tbxFimNossoN.Text = tbxInicioNossoN.Text = tbxOutros1.Text = tbxOutros2.Text = tbxNNAtual.Text = tbxCarteira.Text = "";

            tbxAgencia.Properties.Mask.EditMask = "";
            tbxCodigo.Properties.Mask.EditMask = "";
            tbxConta.Properties.Mask.EditMask = "";
            tbxFimNossoN.Properties.Mask.EditMask = "";
            tbxInicioNossoN.Properties.Mask.EditMask = "";
            tbxOutros1.Properties.Mask.EditMask = "";
            tbxOutros2.Properties.Mask.EditMask = "";
        }

        private void MostrarMensagem(string msg, bool log = false)
        {
            MessageBox.Show(msg, "VsBoletos");
            if (log)
            {
                GravarLog(msg);
            }
        }

        private void GravarLog(string msg)
        {
            if (msg.IsNotNullOrEmpty())
            {
                Arquivo.EscreveConteudo(pathLog, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff") + "  " + msg, true);
            }
        }

        private void Principal_FormClosed(object sender, FormClosedEventArgs e)
        {
            ArquivoINI.EscreveString(pathConfig, "CONFIG", "ROTINA", "1");
            ExcluirImagensPng();
            GravarLog("Finalizando Programa.");
        }

        private static void ExcluirImagensPng()
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(Configuracoes.CaminhoArquivosGerados);
                FileInfo[] fis = di.GetFiles();
                foreach (FileInfo f in fis)
                {
                    if (f.Extension.ToLower() == ".png" || f.Extension.ToLower() == ".gif")
                    {
                        f.Delete();                        
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Erro ao Excluir arquivos temporarios: " + ex.Message);               
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                if (data.ToLower() == "rgbsistemas")
                {
                    AbrirDesignerRelatorios();
                }
            }
            else
            {
                data += keyData.ToString();
                while (data.Length > 11)
                {
                    data = data.Remove(0, 1);
                }
            }
            switch (keyData)
            {
                case Keys.F3: dtpDe.Focus(); break;
                default: break;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void AbrirDesignerRelatorios()
        {
            if (conta == null)
            {
                conta = ContaCorrente.NovaContaCorrente(EnumBanco.Sicoob, "00000", "Cedente Teste", "000.000.000-00");
            }
            OpenFileDialog fd = new OpenFileDialog()
            {
                Filter = "FastReport (*.frx) | *.frx"
            };
            fd.ShowDialog(this);

            Relatorios.DesenharRelatório(Relatorios.PrepararRelatório(fd.FileName, Relatorios.GetDataSetContaCorrente(conta)));
        }

        private void Principal_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {                
                Hide();
            }
        }

        private void BtnGerarRemessas_Click(object sender, EventArgs e)
        {
            string erros = "";
            bool gerou = false;
            try
            {
                gridNotas.ClearSorting();
                Dictionary<int, List<DataRowView>> remessas = new Dictionary<int, List<DataRowView>>();

                string pathremessa = pathIni + "/Remessa/";

                if (gridNotas.DataRowCount > 0)
                {
                    for (int i = 0; i < gridNotas.DataRowCount; i++)
                    {
                        DataRowView dtrw = (DataRowView)(gridNotas.GetRow(i));

                        if (dtrw.Row["check"].ToString().ToLower() == "false")
                        {
                            continue;
                        }

                        int posicao = dtrw.Row["POSICAO"].ToString().ToInt();

                        if (!remessas.ContainsKey(posicao))
                        {
                            remessas.Add(posicao, new List<DataRowView>());
                        }

                        remessas[posicao].Add(dtrw);
                    }

                    foreach (var item in remessas)
                    {
                        string nomeArquivo = "remessa" + DateTime.Now.ToString("ddMMyyhhmmss") + "_" + item.Key + ".txt";
                        foreach (var dtrw in item.Value)
                        {
                            string ordem = dtrw.Row["ORDEM"].ToString();
                            string filial = dtrw.Row["FILIAL"].ToString();
                            int cupom = dtrw.Row["NRO_ECF"].ToString().ToInt();
                            int nfiscal = dtrw.Row["NFISCAL"].ToString().ToInt();
                            string cancel = dtrw.Row["INDCANC"].ToString();
                            string retorno = dtrw.Row["RETORNONFE"].ToString();

                            string pathAuxiliar = pathIni + "/Posicao/Pos" + dtrw.Row["POSICAO"].ToString() + ".ini";
                            if (File.Exists(pathAuxiliar))
                            {
                                pathremessa = ArquivoINI.LeString(pathAuxiliar, "REMESSA", "CAMINHO", valorDefault: "");
                            }

                            if (string.IsNullOrEmpty(pathremessa))
                            {
                                pathremessa = pathIni + "/Remessa/";
                            }

                            if (cupom > 0 && cancel.ToUpper() != "N")
                            {
                                continue;
                            }
                            else if (nfiscal > 0 && retorno.IsNotNullOrEmpty() && retorno.Substring(0, 3).ToUpper() != "100")
                            {
                                continue;
                            }

                            string sql = SqlHelper.GetSelectCRMVINT(filial, ordem);
                            DataSet ds = new DataSet();
                            DataTable dt = ds.Tables.Add("Parcela");
                            FbDataAdapter da = new FbDataAdapter(sql, cnRET);
                            da.Fill(dt);

                            if (dt.Rows.Count > 0)
                            {
                                foreach (DataRow dr in dt.Rows)
                                {
                                    string parcela = "";
                                    string nro = "";
                                    try
                                    {
                                        parcela = dr["PARCELA"].ToString();
                                        nro = dr["NROBOLETO"].ToString();
                                        string st3 = dr["ST3"].ToString();
                                        if (st3.ToUpper() != "R" || st3.ToUpper() != "A")
                                        {
                                            PrepararMeuBoleto(dtrw.Row, dr, "remessa");
                                        }

                                        gerou = true;
                                    }
                                    catch (Exception ex)
                                    {
                                        erros += nro + " " + ordem + filial + parcela + "\n";
                                        GravarLog(ordem + " " + filial + " " + parcela + " \n" + ex.Message + "\n" + ex.ToString());
                                    }
                                }
                            }
                        }
                        if (conta != null)
                        {
                            conta.GerarArquivoRemessa(Configuracoes.LayoutArquivoRemessa, pathremessa, ref nomeArquivo);
                            conta = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarMensagem("Erro geração das remessas: " + ex.Message, true);
            }
            finally
            {
                MostrarMensagem(erros.IsNotNullOrEmpty() ? "Remessa gerada. Erro nos seguintes parcelas: \n" + erros : gerou ? "Remessa dos itens gerada com sucesso." : "Não há remessas a serem geradas", true);
                conta = null;
            }
        }

        private void BkW_DoWork(object sender, DoWorkEventArgs e) { }

        private void ImpressaoAuto2()
        {
            string erros = "";
            string impressos = "";
            if (chkBtnImpressaoAutomatica.Checked)
            {
                timer2.Stop();
            }

            try
            {
                string sql = SqlHelper.GetSelectNotasSaidaImpressaoAuto(dtpDe.DateTime.ToString("MM/dd/yyyy"), dtpAte.DateTime.ToString("MM/dd/yyyy"));
                DataSet ds = new DataSet();
                DataTable dt = ds.Tables.Add("Nota");
                FbDataAdapter da = new FbDataAdapter(sql, cnRET);
                da.Fill(dt);

                Dictionary<int, List<DataRow>> dicNotas = new Dictionary<int, List<DataRow>>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dtrw = dt.Rows[i];

                    int posicao = dtrw["POSICAO"].ToString().ToInt();

                    if (!dicNotas.ContainsKey(posicao))
                    {
                        dicNotas.Add(posicao, new List<DataRow>());
                    }

                    dicNotas[posicao].Add(dtrw);
                }

                foreach (var item in dicNotas)
                {
                    foreach (DataRow drMaster in item.Value)
                    {
                        string ordem = drMaster["ORDEM"].ToString();
                        string filial = drMaster["FILIAL"].ToString();

                        string sqlP = SqlHelper.GetSelectCRMVINT(filial, ordem);
                        DataSet dsP = new DataSet();
                        DataTable dtP = dsP.Tables.Add("Parcela");
                        FbDataAdapter daP = new FbDataAdapter(sqlP, cnRET);
                        daP.Fill(dtP);

                        foreach (DataRow dr in dtP.Rows)
                        {
                            string parcela = "";
                            string nro = "";
                            try
                            {
                                parcela = dr["PARCELA"].ToString();
                                nro = dr["NROBOLETO"].ToString();
                                string st3 = dr["ST3"].ToString();
                                if (st3.ToUpper() != "I" && st3.ToUpper() != "A")
                                {
                                    PrepararMeuBoleto(drMaster, dr, "imprimir");
                                    impressos += nro + " " + ordem + "/" + filial + "/" + parcela + "\n";
                                }
                            }
                            catch (Exception exbb)
                            {
                                erros += nro + " " + ordem + " " + filial + " " + parcela + "\n" + exbb.Message + "\n\n";
                            }
                        }
                        if (conta != null)
                        {
                            if (chkPDF.Checked)
                            {
                                conta.GerarPDFs("", Configuracoes.LayoutBoleto);
                            }

                            conta.ImprimirBoletos(Configuracoes.LayoutBoleto);
                            try
                            {
                                conta.EnviarBoletosPorEmail(Configuracoes.LayoutBoleto);
                            }
                            catch (Exception emx)
                            {
                                GravarLog("Falha ao enviar boleto por email. " + conta.Cedente.NomeCedente + " - " + emx.Message);
                            }

                            conta = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                GravarLog("Erro durante a impressão automática" + ex);
                erroduranteimpressaoautomatica = true;
                chkBtnImpressaoAutomatica.Checked = false;
                CheckButton1_CheckedChanged(null, null);
                return;
            }
            finally
            {
                GravarLog((erros.IsNotNullOrEmpty() ? "Erro nas seguintes parcelas: \n" + erros : "") +
                   (impressos.IsNotNullOrEmpty() ? "Boletos impressos com sucesso.\n" + impressos : ""));

                if (chkBtnImpressaoAutomatica.Checked)
                {
                    timer2.Start();
                }
            }
        }

        private void ImpressaoAuto()
        {
            string erros = "";
            string impressos = "";
            if (chkBtnImpressaoAutomatica.Checked)
            {
                timer2.Stop();
            }

            try
            {
                string sql = SqlHelper.GetSelectNotasSaidaImpressaoAuto(dtpDe.DateTime.ToString("MM/dd/yyyy"), dtpAte.DateTime.ToString("MM/dd/yyyy"));
                DataSet ds = new DataSet();
                DataTable dt = ds.Tables.Add("Nota");
                FbDataAdapter da = new FbDataAdapter(sql, cnRET);
                da.Fill(dt);

                foreach (DataRow drM in dt.Rows)
                {
                    string ordem = drM["ORDEM"].ToString();
                    string filial = drM["FILIAL"].ToString();

                    string sqlP = SqlHelper.GetSelectCRMVINT(filial, ordem);
                    DataSet dsP = new DataSet();
                    DataTable dtP = dsP.Tables.Add("Parcela");
                    FbDataAdapter daP = new FbDataAdapter(sqlP, cnRET);
                    daP.Fill(dtP);

                    foreach (DataRow dr in dtP.Rows)
                    {
                        string parcela = "";
                        string nro = "";
                        try
                        {
                            parcela = dr["PARCELA"].ToString();
                            nro = dr["NROBOLETO"].ToString();
                            string st3 = dr["ST3"].ToString();
                            if (st3.ToUpper() != "I" && st3.ToUpper() != "A")
                            {
                                PrepararMeuBoleto(drM, dr, "imprimir");
                                impressos += nro + " " + ordem + "/" + filial + "/" + parcela + "\n";
                            }
                        }
                        catch (Exception)
                        {
                            erros += nro + " " + ordem + filial + parcela + "\n";
                        }
                    }
                }
                if (conta != null)
                {
                    if (chkPDF.Checked)
                    {
                        conta.GerarPDFs("", Configuracoes.LayoutBoleto);
                    }

                    conta.ImprimirBoletos(Configuracoes.LayoutBoleto);
                    try
                    {
                        conta.EnviarBoletosPorEmail(Configuracoes.LayoutBoleto);
                    }
                    catch (Exception ex)
                    {
                        GravarLog("Falha ao enviar boleto por email. " + conta.Cedente.NomeCedente + " : " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                GravarLog("Erro durante a impressão automática" + ex);
                erroduranteimpressaoautomatica = true;
                return;
            }
            finally
            {
                GravarLog((erros.IsNotNullOrEmpty() ? "Erro nas seguintes parcelas: \n" + erros : "") +
                   (impressos.IsNotNullOrEmpty() ? "Boletos impressos com sucesso.\n" + impressos : ""));

                conta = null;
                if (erroduranteimpressaoautomatica)
                {
                    erroduranteimpressaoautomatica = false;
                    MostrarMensagem("Ocorreu um erro durante a impressão automática.A impressão foi pausada.\n\nAtualize o grid, verifique o ultimo boleto e tente emitir manualmente.");
                }
                else
                    if (chkBtnImpressaoAutomatica.Checked)
                {
                    timer2.Start();
                }
            }
        }

        bool erroduranteimpressaoautomatica = false;

        private void ChkSelecionarRemessa_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < gridNotas.DataRowCount; i++)
                {
                    DataRowView dtrw = (DataRowView)(gridNotas.GetRow(i));
                    if (dtrw.Row["check"] != null)
                    {
                        gridNotas.SetRowCellValue(i, "check", (bool)chkSelecionarRemessa.Checked);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Timer2_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                timer2.Stop();
                ImpressaoAuto2();
            }
            finally
            {
                timer2.Start();
            }
        }

        private void CheckButton1_CheckedChanged(object sender, EventArgs e)
        {
            bool aux = chkBtnImpressaoAutomatica.Checked;

            ArquivoINI.EscreveString(pathConfig, "CONFIG", "ROTINA", chkBtnImpressaoAutomatica.Checked ? "0" : "1");

            timer2.Enabled = chkBtnImpressaoAutomatica.Checked;
            notifyIcon1.Text = chkBtnImpressaoAutomatica.Checked ? "Impressão Automática On" : "Impressão Automática Off";
            notifyIcon1.BalloonTipTitle = "VsBoletos";
            notifyIcon1.BalloonTipText = chkBtnImpressaoAutomatica.Checked ? "Impressão Automática On" : "Impressão Automática Off";
            notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
            notifyIcon1.ShowBalloonTip(500);

            xtraTabMonitor.Select();

            panelControl2.Enabled = groupRemessa.Enabled = controlNotas.Enabled = controlParcelas.Enabled =
                                    barBtnInfoBD.Enabled = xtraTabConfig.PageEnabled = groupPesquisa.Enabled =
                                    xTabRetorno.PageEnabled = chkExibirImpressos.Enabled = barBtnAtualizar.Enabled =
                                    chkPDF.Enabled = !aux;

            BarBtnAtualizar_ItemClick(null, null);

        }

        private void DtpAte_Leave(object sender, EventArgs e)
        {
            DateEdit dt = (DateEdit)sender;
            if (dt.DateTime.IsMinDateTime())
            {
                dt.DateTime = DateTime.Now;
            }

            timer2.Enabled = int.Parse(ArquivoINI.LeString(pathConfig, "CONFIG", "ROTINA")) == 0;
        }

        private void DtpAte_Enter(object sender, EventArgs e)
        {
            timer2.Enabled = false;
        }

        private void BarBtnEmail_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (controlNotas.Focused)
                {
                    if (gridNotas.GetSelectedRows().Length > 0)
                    {
                        if (gridParcelas.DataRowCount > 0)
                        {
                            for (int i = 0; i < gridParcelas.DataRowCount; i++)
                            {
                                PrepararMeuBoleto(null, ((DataRowView)gridParcelas.GetRow(i)).Row, "email");
                            }

                            conta.EnviarBoletosPorEmail(Configuracoes.LayoutBoleto);
                            MostrarMensagem("Email enviado.");
                        }
                    }
                }
                else if (controlParcelas.Focused)
                {
                    if (gridParcelas.GetSelectedRows().Length > 0)
                    {
                        PrepararMeuBoleto(null, gridParcelas.GetFocusedDataRow(), "email");
                        conta.EnviarBoletosPorEmail(Configuracoes.LayoutBoleto);
                        MostrarMensagem("Email enviado");
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarMensagem("Erro durante o envio de email.\n" + ex);
            }
            finally
            {
                conta = null;
            }
        }

        private void BarBtnInfoBD_Click(object sender, EventArgs e)
        {
            FormSenha pw = new FormSenha("dram");
            pw.ShowDialog();
            if (pw.Permitido)
            {
                FormConfig frm = new FormConfig();
                frm.ShowDialog(this);
                chkBtnImpressaoAutomatica.Checked = int.Parse(ArquivoINI.LeString(pathConfig, "CONFIG", "ROTINA", valorDefault: "1")) == 0;
                timer2.Interval = int.Parse(ArquivoINI.LeString(pathConfig, "CONFIG", "INTERVALO", valorDefault: "10")) * 1000;
                timer2.Enabled = int.Parse(ArquivoINI.LeString(pathConfig, "CONFIG", "ROTINA")) == 0;
            }
            
        }

        private void BarBtnSair_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (MessageBox.Show("Sair do programa?", "VsBoleto", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Close();
            }
        }

        private void BarBtnAtualizar_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CarregarGrids();
        }

        private void BarBtnGerarRemessa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                string nomeArquivo = "remessa" + DateTime.Now.ToString("ddMMyyhhmmss") + ".txt";
                string pathremessa = pathIni + "/Remessa/";
                if (controlNotas.Focused)
                {
                    if (gridNotas.GetSelectedRows().Length > 0)
                    {
                        if (gridParcelas.DataRowCount > 0)
                        {
                            for (int i = 0; i < gridParcelas.DataRowCount; i++)
                            {
                                PrepararMeuBoleto(null, ((DataRowView)gridParcelas.GetRow(i)).Row, "remessa");
                            }
                        }

                        string pathAuxiliar = pathIni + "/Posicao/Pos" + ((DataRowView)gridNotas.GetRow(gridNotas.GetSelectedRows()[0]))["POSICAO"].ToString() + ".ini";
                        if (File.Exists(pathAuxiliar))
                        {
                            pathremessa = ArquivoINI.LeString(pathAuxiliar, "REMESSA", "CAMINHO", valorDefault: "");
                        }

                        if (string.IsNullOrEmpty(pathremessa))
                        {
                            pathremessa = pathIni + "/Remessa/";
                        }

                        conta.GerarArquivoRemessa(Configuracoes.LayoutArquivoRemessa,
                            pathremessa, ref nomeArquivo);
                        MostrarMensagem("Arquivos de remessa gerado.", true);
                    }
                }
                else if (controlParcelas.Focused)
                {
                    if (gridParcelas.GetSelectedRows().Length > 0)
                    {
                        PrepararMeuBoleto(null, gridParcelas.GetFocusedDataRow(), "remessa");

                        string pathAuxiliar = pathIni + "/Posicao/Pos" + ((DataRowView)gridNotas.GetRow(gridNotas.GetSelectedRows()[0]))["POSICAO"].ToString() + ".ini";
                        if (File.Exists(pathAuxiliar))
                        {
                            pathremessa = ArquivoINI.LeString(pathAuxiliar, "REMESSA", "CAMINHO", valorDefault: "");
                        }

                        if (string.IsNullOrEmpty(pathremessa))
                        {
                            pathremessa = pathIni + "/Remessa/";
                        }

                        conta.GerarArquivoRemessa(Configuracoes.LayoutArquivoRemessa,
                            pathremessa, ref nomeArquivo);
                        MostrarMensagem("Arquivo de remessa gerado.", true);
                    }
                }
            }
            catch (Exception)
            {
                GravarLog("Erro durante a geração da remessa de um item.");
            }
            finally
            {
                conta = null;
            }
        }

        private void BarBtnDesenharBoleto_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                FormSenha pw = new FormSenha();
                pw.ShowDialog();
                if (!pw.Permitido)
                {
                    return;
                }

                if (controlNotas.Focused)
                {
                    if (gridNotas.GetSelectedRows().Length > 0)
                    {
                        if (gridParcelas.DataRowCount > 0)
                        {
                            for (int i = 0; i < gridParcelas.DataRowCount; i++)
                            {
                                PrepararMeuBoleto(null, ((DataRowView)gridParcelas.GetRow(i)).Row, "");
                            }
                        }

                        conta.DesenharRelatorio(Configuracoes.LayoutBoleto);
                    }
                }
                else if (controlParcelas.Focused)
                {
                    if (gridParcelas.GetSelectedRows().Length > 0)
                    {
                        PrepararMeuBoleto(null, gridParcelas.GetFocusedDataRow(), "");
                        conta.DesenharRelatorio(Configuracoes.LayoutBoleto);
                    }
                }
            }
            catch (Exception ex)
            {
                GravarLog("Erro no desenho do boleto.\n" + ex.ToString());
            }
            finally
            {
                conta = null;
            }
        }

        private void BarBtnVisualizar_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (controlNotas.Focused)
                {
                    if (gridNotas.GetSelectedRows().Length > 0)
                    {
                        if (gridParcelas.DataRowCount > 0)
                        {
                            for (int i = 0; i < gridParcelas.DataRowCount; i++)
                            {
                                PrepararMeuBoleto(null, ((DataRowView)gridParcelas.GetRow(i)).Row, "visualizar");
                            }
                        }

                        VisualizarBoletos();
                    }
                }
                else if (controlParcelas.Focused)
                {
                    if (gridParcelas.GetSelectedRows().Length > 0)
                    {
                        PrepararMeuBoleto(null, gridParcelas.GetFocusedDataRow(), "visualizar");
                        VisualizarBoletos();
                    }
                }
            }
            catch (Exception)
            {
                GravarLog("Erro durante a visualização de um item.");
            }
            finally
            {
                conta = null;
            }
        }

        private void BarBtnImprimir_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (controlNotas.Focused)
                {
                    if (gridNotas.GetSelectedRows().Length > 0)
                    {
                        if (gridParcelas.DataRowCount > 0)
                        {
                            for (int i = 0; i < gridParcelas.DataRowCount; i++)
                            {
                                PrepararMeuBoleto(null, ((DataRowView)gridParcelas.GetRow(i)).Row, "imprimir");
                            }
                        }

                        VisualizarBoletos(true);
                    }
                }
                else if (controlParcelas.Focused)
                {
                    if (gridParcelas.GetSelectedRows().Length > 0)
                    {
                        PrepararMeuBoleto(null, gridParcelas.GetFocusedDataRow(), "imprimir");
                        VisualizarBoletos(true);
                    }
                }
            }
            catch (Exception ex)
            {
                GravarLog("Erro durante a impressão de um item: " + ex.Message);
            }
            finally
            {
                conta = null;
            }
        }

        private void GridNotas_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                if (gridNotas.GetSelectedRows().Length > 0)
                {
                    ExibirParcelas();
                }
            }
            catch (Exception ex)
            {
                MostrarMensagem("Erro na exibição das parcelas: " + ex.Message, true);
            }
        }

        private void GridNotas_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                if (gridNotas.GetSelectedRows().Length > 0)
                {
                    ExibirParcelas();
                }
            }
            catch (Exception ex)
            {
                MostrarMensagem("Erro na exibição das parcelas: " + ex.Message, true);
            }
        }

        private void GridNotas_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
                {
                    if (gridNotas.GetSelectedRows().Length > 0)
                    {
                        ExibirParcelas();
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarMensagem("Erro na exibição das parcelas: " + ex.Message, true);
            }
        }

        private void ControlNotas_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (popupMenu1.CanShowPopup)
                {
                    popupMenu1.ShowPopup(controlNotas.PointToScreen(e.Location));
                }
            }

            controlNotas.Focus();
        }

        private void ControlParcelas_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (popupMenu1.CanShowPopup)
                {
                    popupMenu1.ShowPopup(controlParcelas.PointToScreen(e.Location));
                }
            }

            controlParcelas.Focus();
        }

        private void LbxPosicoes_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (lbxPosicoes.SelectedItem != null)
            {
                CarregarConfiguracoes();
            }
        }

        private void BtnLimpar_Click(object sender, EventArgs e)
        {
            LimparCamposConfiguracoes();
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            LimparCamposConfiguracoes();
            Editando = false;
        }

        private void BtnCriarEditar_Click(object sender, EventArgs e)
        {
            Editando = true;
            DdlBanco_SelectedIndexChanged(null, null);
        }

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                string pathPosicao = pathIni + "/Posicao/";
                if (!Directory.Exists(pathPosicao))
                {
                    Directory.CreateDirectory(pathPosicao);
                }

                pathPosicao += "Pos" + ((ListItem)lbxPosicoes.SelectedItem).Value + ".ini";

                ArquivoINI.EscreveString(pathPosicao, "BANCO", "NOME", ddlBanco.SelectedItem.ToString());
                ArquivoINI.EscreveString(pathPosicao, "BANCO", "CARTEIRA", tbxCarteira.Text);
                ArquivoINI.EscreveString(pathPosicao, "CEDENTE", "FILIAL", ((ListItem)ddlNomeCedente.SelectedItem).Value);
                ArquivoINI.EscreveString(pathPosicao, "CEDENTE", "CODIGO", tbxCodigo.Text);
                ArquivoINI.EscreveString(pathPosicao, "CEDENTE", "AGENCIA", tbxAgencia.Text);
                ArquivoINI.EscreveString(pathPosicao, "CEDENTE", "CONTA", tbxConta.Text);
                ArquivoINI.EscreveString(pathPosicao, "CEDENTE", "NOSSONROI", tbxInicioNossoN.Text);
                ArquivoINI.EscreveString(pathPosicao, "CEDENTE", "NOSSONROF", tbxFimNossoN.Text);
                ArquivoINI.EscreveString(pathPosicao, "CEDENTE", "OUTRODADO1", tbxOutros1.Text);
                ArquivoINI.EscreveString(pathPosicao, "CEDENTE", "OUTRODADO2", tbxOutros2.Text);
                ArquivoINI.EscreveString(pathPosicao, "BOLETO", "DEMONSTRATIVO", tbxDemonstrativo.Text, multiplasLinhas: true);
                ArquivoINI.EscreveString(pathPosicao, "BOLETO", "INSTRUCOES", tbxInstrucoes.Text);
                ArquivoINI.EscreveString(pathPosicao, "BOLETO", "LAYOUT", tbxPathLayoutBoleto.Text);
                ArquivoINI.EscreveString(pathPosicao, "BOLETO", "JUROS", tbxJurosMes.Text);
                ArquivoINI.EscreveString(pathPosicao, "BOLETO", "MULTA", tbxMulta.Text);
                ArquivoINI.EscreveString(pathPosicao, "BOLETO", "INSTRUCAO1", tbxInstrucao1.Text);
                ArquivoINI.EscreveString(pathPosicao, "BOLETO", "INSTRUCAO2", tbxInstrucao2.Text);
                ArquivoINI.EscreveString(pathPosicao, "BOLETO", "DIASPROTESTO", tbxProtesto.Text);
                ArquivoINI.EscreveString(pathPosicao, "BOLETO", "ESPECIE", ddlEspecieTitulo.SelectedItem.ToString());
                ArquivoINI.EscreveString(pathPosicao, "BOLETO", "DESCVENCIMENTO", txtDescontoVencimento.Text);
                ArquivoINI.EscreveString(pathPosicao, "REMESSA", "CAMINHO", tbxArquivoRemessa.Text);
                ArquivoINI.EscreveString(pathPosicao, "REMESSA", "LAYOUT", ddlLayoutRemessa.SelectedItem.ToString());



                string pathConfigBanco = AppDomain.CurrentDomain.BaseDirectory + "\\ConfigBoletosBancos.ini";
                if (ddlBanco.SelectedItem.ToString().ToLower().Equals("bradesco"))
                {
                    ArquivoINI.EscreveBool(pathConfigBanco, "BRADESCO", "UTILIZA_NUM_BANCO", chkUtilizaNumBanco.Checked);
                }

                GravarNossoNumeroDoBD(((ListItem)lbxPosicoes.SelectedItem).Value, "");

                Editando = false;
                txtDescontoVencimento.Enabled = false;
            }
            catch (Exception ex)
            {
                MostrarMensagem("Erro ao salvar as informações: " + ex.Message, true);
            }
        }

        private void LbxPosicoes_Click(object sender, EventArgs e)
        {
            groupBanco.Text = "Banco";
            Editando = false;
            btnCriarEditar.Enabled = false;
            LimparCamposConfiguracoes();
        }

        private void LbxPosicoes_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                CarregarConfiguracoes();
            }
        }

        private void LbxPosicoes_SelectedIndexChanged(object sender, EventArgs e)
        {
            groupBanco.Text = "Banco";
            Editando = false;
            btnCriarEditar.Enabled = false;
            LimparCamposConfiguracoes();
        }

        private void TbxArquivoRemessa_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            if (folder.ShowDialog() == DialogResult.OK)
            {
                tbxArquivoRemessa.Text = folder.SelectedPath;
            }
        }

        private void TbxArquivoRetorno_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            if (file.ShowDialog() == DialogResult.OK)
            {
                pathRetorno = file.FileName;
                tbxArquivoRetorno.Text = file.SafeFileName;
            }
        }

        private void BtnCarregar_Click(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception)
            {
                MostrarMensagem("Erro no arquivo retorno.", true);
            }
            finally
            {
                conta = null;
            }
        }

        private void DdlPosicoes_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception)
            {
                MostrarMensagem("Erro ao carregar Layouts.", true);
            }
            finally
            {
                conta = null;
            }
        }

        private void DdlBanco_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                chkUtilizaNumBanco.Visible = false;
                switch (ddlBanco.SelectedItem.ToString().ToLower())
                {
                    case "sicoob": CarregarItensDoBanco(EnumHelper.GetBanco(EnumBanco.Sicoob)); break;
                    case "itau": CarregarItensDoBanco(EnumHelper.GetBanco(EnumBanco.Itau)); break;
                    case "bradesco":
                        CarregarItensDoBanco(EnumHelper.GetBanco(EnumBanco.Bradesco));
                        chkUtilizaNumBanco.Visible = true; break;
                    case "caixasr": CarregarItensDoBanco(EnumHelper.GetBanco(EnumBanco.CaixaSR)); break;
                    case "banestes": CarregarItensDoBanco(EnumHelper.GetBanco(EnumBanco.Banestes)); break;
                    case "santander": CarregarItensDoBanco(EnumHelper.GetBanco(EnumBanco.Santander)); break;
                    case "bancobrasil": CarregarItensDoBanco(EnumHelper.GetBanco(EnumBanco.BancoBrasil)); break;
                    default: break;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Erro ao carregar itens do banco selecionado.");
            }
        }

        private void CarregarItensDoBanco(Banco banco)
        {
            if (banco == null)
            {
                BtnCancelar_Click(null, null);
                return;
            }

            ddlLayoutRemessa.Properties.Items.Clear();
            ddlEspecieTitulo.Properties.Items.Clear();

            for (int i = 0; i < banco.LayoutsArquivoRemessa.Count; i++)
            {
                ddlLayoutRemessa.Properties.Items.Add(banco.LayoutsArquivoRemessa[i]);
            }

            if (ddlLayoutRemessa.Properties.Items.Count > 0)
            {
                ddlLayoutRemessa.SelectedIndex = 0;
            }

            for (int i = 0; i < banco.EspecieTitulo.Count; i++)
            {
                ddlEspecieTitulo.Properties.Items.Add(banco.EspecieTitulo[i]);
            }

            if (ddlEspecieTitulo.Properties.Items.Count > 0)
            {
                ddlEspecieTitulo.SelectedIndex = 0;
            }

            tbxIdentificacao.Text = banco.Identificacao;
            lblOD1.Text = banco.TituloOutros1;
            lblOD2.Text = banco.TituloOutros2;

            tbxOutros1.Enabled = editando ? banco.HabilitarOutros1 : tbxOutros1.Enabled;
            tbxOutros2.Enabled = editando ? banco.HabilitarOutros2 : tbxOutros2.Enabled;

            tbxAgencia.Properties.MaxLength = banco.MascaraAgencia.ToNoFormated().Length;
            tbxCodigo.Properties.MaxLength = banco.MascaraCodigoCedente.ToNoFormated().Length;
            tbxConta.Properties.MaxLength = banco.MascaraContaCorrente.ToNoFormated().Length;
            tbxInicioNossoN.Properties.MaxLength = banco.MascaraNossoNumero.ToNoFormated().Length;
            tbxFimNossoN.Properties.MaxLength = banco.MascaraNossoNumero.ToNoFormated().Length;
            tbxNNAtual.Properties.MaxLength = banco.MascaraNossoNumero.ToNoFormated().Length;
            tbxOutros1.Properties.MaxLength = banco.MascaraOutros1.ToNoFormated().Length;
            tbxOutros2.Properties.MaxLength = banco.MascaraOutros2.ToNoFormated().Length;
        }

        private void BtnAbrirPasta_Click(object sender, EventArgs e)
        {
            try
            {
                string windir = Environment.GetEnvironmentVariable("WINDIR");
                System.Diagnostics.Process prc = new System.Diagnostics.Process();
                prc.StartInfo.FileName = windir + @"\explorer.exe";
                prc.StartInfo.Arguments = pathIni;
                prc.Start();
            }
            catch (Exception ex)
            {
                MostrarMensagem("Erro ao abrir a pasta: " + ex.Message, true);
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            CarregarGrids();
            tbxPesquisaNN.Text = "";
        }

        private void ChkBtnImpressaoAutomatica_CheckedChanged(object sender, EventArgs e)
        {
            bool aux = chkBtnImpressaoAutomatica.Checked;

            ArquivoINI.EscreveString(pathConfig, "CONFIG", "ROTINA", chkBtnImpressaoAutomatica.Checked ? "0" : "1");            
            timer2.Enabled = chkBtnImpressaoAutomatica.Checked;
            notifyIcon1.Text = chkBtnImpressaoAutomatica.Checked ? "Impressão Automática Ativada" : "Impressão Automática Desativada";
            notifyIcon1.BalloonTipTitle = "VsBoleto";
            notifyIcon1.BalloonTipText = chkBtnImpressaoAutomatica.Checked ? "Impressão Automática Ativada" : "Impressão Automática Desativada";
            notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
            notifyIcon1.ShowBalloonTip(500);

            xtraTabMonitor.Select();

            panelControl2.Enabled = groupRemessa.Enabled = controlNotas.Enabled =
                controlParcelas.Enabled = barBtnInfoBD.Enabled = xtraTabConfig.PageEnabled = groupPesquisa.Enabled =
                xTabRetorno.PageEnabled = chkExibirImpressos.Enabled = barBtnAtualizar.Enabled = chkPDF.Enabled = !aux;

            BarBtnAtualizar_ItemClick(null, null);
        }

        private void NotifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && IsHandleCreated)
            {
                popupMenu2.ShowPopup(MousePosition);
            }
        }

        private void NotifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
            CarregarGrids();
        }

        private void btnIconeSair_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Close();
        }

        private void Principal_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Sair do programa?", "VsBoleto", MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                e.Cancel = true;
            }
        }

        private void btnIconeAbrir_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Show();
                WindowState = FormWindowState.Normal;
            }
        }

        private void TbxPathLayoutBoleto_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            OpenFileDialog o = new OpenFileDialog
            {
                Filter = "FastReport | *.frx"
            };
            if (DialogResult.OK == o.ShowDialog())
            {
                if (o.CheckFileExists)
                {
                    tbxPathLayoutBoleto.Text = o.FileName;
                }
            }
        }

        private void ChkDesconto_CheckedChanged(object sender, EventArgs e)
        {
            if (chkDesconto.Checked == true)
            {
                if (MessageBox.Show("Ao habilitar essa função será aplicado o percentual de desconto até o vencimento para todos os boletos emitidos para essa posição, deseja realmente habilitar essa função?", "Atenção", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                {
                    txtDescontoVencimento.Enabled = true;
                }
                else
                {
                    chkDesconto.Checked = false;
                    txtDescontoVencimento.Text = "0";
                }
            }
            else
            {
                txtDescontoVencimento.Text = "0";
                txtDescontoVencimento.Enabled = false;
            }
        }

        private void XtraTabControl1_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            if (e.Page == xtraTabConfig)
            {
                FormSenha pw = new FormSenha();
                pw.ShowDialog();
                if (!pw.Permitido)
                {
                    xtraTabControl1.SelectedTabPage = xtraTabMonitor;
                }
            }
        }

        private void TbxNNAtual_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                string pathPosicao = pathIni + "/Posicao/";
                pathPosicao += "Pos" + ((ListItem)lbxPosicoes.SelectedItem).Value + ".ini";
                if (!File.Exists(pathPosicao))
                {
                    MostrarMensagem("Arquivo de configuração " + ((ListItem)lbxPosicoes.SelectedItem).Value + " inexistente. Crie um antes de editar este valor.", true);
                    return;
                }
                GravarNossoNumeroDoBD(((ListItem)lbxPosicoes.SelectedItem).Value, tbxNNAtual.Text.Trim());
                ArquivoINI.EscreveString(pathPosicao, "CEDENTE", "NOSSONUMERO", tbxNNAtual.Text.Trim());
                MostrarMensagem("Pos " + ((ListItem)lbxPosicoes.SelectedItem).Value + ": Nosso Número Atual foi modificado para: " + tbxNNAtual.Text.Trim(), true);
            }
            catch (Exception ex)
            {
                MostrarMensagem("Erro ao salvar nosso número no arquivo de configuração." + ex, true);
            }

        }

        private void BtnPesquisaNN_Click(object sender, EventArgs e)
        {
            controlParcelas.DataSource = controlNotas.DataSource = null;
            try
            {
                string sql = SqlHelper.GetSelectNotasSaidaPorNN(tbxPesquisaNN.Text);
                DataSet ds = new DataSet();
                DataTable dt = ds.Tables.Add("Notas");
                FbDataAdapter da = new FbDataAdapter(sql, cnRET);
                da.Fill(dt);

                dt.Columns.Add("check", typeof(bool));

                if (dt.Rows.Count > 0)
                {
                    controlNotas.DataSource = dt;
                }
                else
                {
                    controlParcelas.DataSource = controlNotas.DataSource = null;
                }

                gridParcelas.BestFitColumns();
                gridNotas.BestFitColumns();
                chkSelecionarRemessa.Checked = true;
                ChkSelecionarRemessa_CheckedChanged(null, null);

                if (gridNotas.DataRowCount > 0)
                {
                    ExibirParcelas();
                }
            }
            catch (Exception)
            {
                MostrarMensagem("Erro ao carregar grids. Tente Novamente.", true);
            }
        }

        private void TbxPesquisaNN_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                BtnPesquisaNN_Click(null, null);
            }
        }
    }
}