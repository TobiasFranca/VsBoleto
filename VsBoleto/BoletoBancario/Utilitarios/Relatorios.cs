using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FastReport.Export;
using FastReport.Export.Pdf;
using FastReport;
using System.Data;
using BoletoBancario.Conta;
using System.IO;

namespace BoletoBancario.Utilitarios
{
    public static class Relatorios
    {
        /// <summary>
        /// Monta o DataSet para o boleto desejado.
        /// </summary>
        /// <param name="b">Boleto com as informações</param>
        /// <returns>Dataset com os valores do boleto</returns>
        public static DataSet GetDataSetBoleto(Boleto b)
        {
            return GetDataSetContaCorrente(b.ContaCorrente, b);
        }
        /// <summary>
        /// Monta o DataSet para todos os boletos da conta corrente.
        /// </summary>
        /// <param name="cc">Conta corrente com as informações</param>
        /// <returns>Dataset com todos os valores de cada boleto</returns>
        public static DataSet GetDataSetContaCorrente(ContaCorrente cc)
        {
            return GetDataSetContaCorrente(cc, null);
        }

        /// <summary>
        /// Monta o dataset da conta corrente e boleto.
        /// </summary>
        /// <param name="cc">Conta corrente desejada</param>
        /// <param name="bol">boleto da conta corrente. se for nulo utiliza todos os boletos da conta corrente passada</param>
        /// <returns>DataSet com os valores</returns>
        public static DataSet GetDataSetContaCorrente(ContaCorrente cc, Boleto bol)
        {
            DataSet ds = new DataSet("Boletos");

            DataTable dt = new DataTable("Boletos");

            DataColumn[] colunas = new DataColumn[]{
            new DataColumn("Cedente_Nome"),
            new DataColumn("Cedente_CpfCnpj"),
            new DataColumn("Cedente_Endereco"),
            new DataColumn("Cedente_Bairro"),
            new DataColumn("Cedente_Cidade"),
            new DataColumn("Cedente_Cep"),
            new DataColumn("Cedente_Estado"),

            new DataColumn("CC_Agencia"),
            new DataColumn("CC_CodCedente"),
            new DataColumn("CC_NumeroBanco"),
            new DataColumn("CC_Outros1"),// carteira sicoob
			new DataColumn("CC_Outros2"),// modalidade sicoob

			new DataColumn("Boleto_CodigoBarras"),
            new DataColumn("Boleto_LinhaDigitavel"),
            new DataColumn("Boleto_DataDocumento"),
            new DataColumn("Boleto_DataVencimento"),
            new DataColumn("Boleto_NossoNumeroComDV"),
            new DataColumn("Boleto_ParcelaAtual"),
            new DataColumn("Boleto_PercDesconto"),
            new DataColumn("Boleto_PercJurosDiaAtraso"),
            new DataColumn("Boleto_PercMultaAtraso"),
            new DataColumn("Boleto_OutrosAcrecimos"),
            new DataColumn("Boleto_ValorDesconto"),
            new DataColumn("Boleto_ValorJurosDiaAtraso"),
            new DataColumn("Boleto_ValorMultaAtraso"),
            new DataColumn("Boleto_ValorOutrosAcrecimos"),
            new DataColumn("Boleto_ValorAbatimento"),
            new DataColumn("Boleto_ValorDocumento"),

            new DataColumn("Sacado_Nome"),
            new DataColumn("Sacado_CpfCnpj"),
            new DataColumn("Sacado_Endereco"),
            new DataColumn("Sacado_Bairro"),
            new DataColumn("Sacado_Cidade"),
            new DataColumn("Sacado_Cep"),
            new DataColumn("Sacado_Estado"),

            new DataColumn("Boleto_NumeroDocumento"),
            new DataColumn("Boleto_Aceite"),
            new DataColumn("Boleto_Moeda"),
            new DataColumn("Boleto_Especie"),

            new DataColumn("CC_Instrucoes"),
            new DataColumn("CC_Demonstrativo"),

            new DataColumn("Boleto_PathImgCodBarras"),
            new DataColumn("Sacado_Codigo"),

            new DataColumn("Sacado_Numero_Endereco"),
            new DataColumn("Cedente_Numero_Endereco"),
            new DataColumn("Boleto_Numero_Parcelas"),
            new DataColumn("Banco_Carteira"),
            new DataColumn("CC_NumeroConta"),
            new DataColumn("CC_DV_NumeroBanco")
            };

            dt.Columns.AddRange(colunas);

            IList<Boleto> boletos = new List<Boleto>();
            if (bol == null)
            {
                foreach (Boleto b in cc.Boletos)
                {
                    boletos.Add(b);
                }
            }
            else
            {
                boletos.Add(bol);
            }

            foreach (Boleto b in boletos)
            {
                DataRow dr = dt.NewRow();
                dr[0] = b.ContaCorrente.Cedente.NomeCedente;
                dr[1] = b.ContaCorrente.Cedente.CpfCnpj;
                dr[2] = b.ContaCorrente.Cedente.Endereco;
                dr[3] = b.ContaCorrente.Cedente.Bairro;
                dr[4] = b.ContaCorrente.Cedente.Cidade;
                dr[5] = b.ContaCorrente.Cedente.Cep;
                dr[6] = b.ContaCorrente.Cedente.Estado;

                string teste = b.ContaCorrente.Banco.MascaraAgencia;
                dr[7] = cc.Agencia;
                dr[8] = cc.CodigoCedente;
                dr[9] = cc.Banco.NumeroBanco;
                dr[10] = cc.Outros1;
                dr[11] = cc.Outros2;

                dr[12] = b.CodigoBarras;
                dr[13] = b.LinhaDigitavel;
                dr[14] = b.DataDocumento.ToShortDateString();
                dr[15] = b.DataVencimento.ToShortDateString();
                dr[16] = b.NossoNumero + "-" + b.DigVerNossoNumero;
                dr[17] = b.NumeroParcelaAtual;
                dr[18] = b.PercentualDesconto;
                dr[19] = b.PercentualJurosDiaAtraso;
                dr[20] = b.PercentualMultaAtraso;
                dr[21] = b.PercentualOutrosAcrescimos;
                dr[22] = b.ValorDesconto;
                dr[23] = b.ValorJurosDiaAtraso;
                dr[24] = b.ValorMultaAtraso;
                dr[25] = b.ValorOutrosAcrescimos;
                dr[26] = b.ValorAbatimento;
                dr[27] = b.ValorDocumento.ToString("N2");

                dr[28] = b.Sacado.Razao.Trim().Length > 3 ? b.Sacado.Razao : b.Sacado.Nome;
                dr[29] = b.Sacado.CpfCnpj;
                dr[30] = b.Sacado.Endereco;
                dr[31] = b.Sacado.Bairro;
                dr[32] = b.Sacado.Cidade;
                dr[33] = b.Sacado.Cep;
                dr[34] = b.Sacado.Estado;

                dr[35] = b.NumeroDocumento;
                dr[36] = b.Aceite;
                dr[37] = EnumHelper.GetEspecieMoeda(b.Moeda);
                dr[38] = b.EspecieBoleto;

                string inst = "";

                if (b.ValorJurosDiaAtraso > 0)
                {
                    inst += "Cobrar Juros de " + EnumHelper.GetEspecieMoeda(b.Moeda) + " " + b.ValorJurosDiaAtraso.ToString("N2") + " ao dia" + Environment.NewLine;
                }

                if (b.ValorMultaAtraso > 0)
                {
                    inst += "Cobrar Multa de " + EnumHelper.GetEspecieMoeda(b.Moeda) + " " + b.ValorMultaAtraso.ToString("N2") + Environment.NewLine;
                }

                inst += cc.Instrucoes;

                dr[39] = inst;
                dr[40] = cc.Demonstrativo;

                dr[41] = b.PathImgCodBarras;
                dr[42] = b.Sacado.CodigoSistema;

                dr[43] = b.Sacado.NumEndereco;
                dr[44] = b.ContaCorrente.Cedente.NumEndereco;
                dr[45] = b.NumeroParcelas;
                dr[46] = b.ContaCorrente.Banco.Carteira;
                dr[47] = b.ContaCorrente.NumeroConta;
                dr[48] = b.ContaCorrente.Banco.DvNumeroBanco;

                dt.Rows.Add(dr);
            }

            ds.Tables.Add(dt);

            return ds;
        }
        /// <summary>
        /// Instancia um novo relatório do Fast Report.
        /// </summary>
        /// <param name="path">Path do arquivo .frx</param>
        /// <param name="ds">Dataset para popular o relatório</param>
        /// <returns>Nova instancia do Relatório do Fast Report</returns>
        public static Report PrepararRelatório(string path, DataSet ds)
        {
            Report r = new Report();
            try
            {
                if (!File.Exists(path))
                {
                    throw new Exception("Arquivo .frx não encontrado");
                }

                r.Load(path);
                r.RegisterData(ds);
                r.GetDataSource("Boletos").Enabled = true;

                r.Prepare();

                FastReport.Utils.Config.PreviewSettings.Buttons =
                    PreviewButtons.All & ~PreviewButtons.Edit & ~PreviewButtons.Open
                    & ~PreviewButtons.Outline & ~PreviewButtons.PageSetup;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return r;
        }
        /// <summary>
        /// Deleta arquivos criados.
        /// </summary>
        /// <param name="path">path do arquivo a ser deletado</param>
        internal static void DeletarImagensCodBarras(string path)
        {
            try
            {
                FileInfo f = new FileInfo(path);
                if (f.Exists)
                {
                    f.Delete();
                }

                //DirectoryInfo d = new DirectoryInfo(Configuracoes.CaminhoArquivosPadroes);

                //FileInfo[] fs = d.GetFiles();

                //for (int i = fs.Length - 1; i >= 0; i--)
                //{
                //    if (fs[i].FullName.ToLower() == f.FullName.ToLower())
                //        continue;
                //    if (fs[i].Name.ToLower().Contains("barcode"))
                //        fs[i].Delete();
                //}
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// Mostra o relatorio na janela do FastReport. 
        /// Além de opções de impressão e exportação.
        /// </summary>
        /// <param name="r">Instancia do Relatório já com o dataset. Utilize PrepararRelatorio()</param>
        /// <returns>Se ocorreu erros ou não durante a visualização</returns>
        public static bool VisualizarBoleto(Report r)
        {
            try
            {
                r.Show();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }
        /// <summary>
        /// Imprime um relatório.
        /// </summary>
        /// <param name="r">Instancia do Relatório já com o dataset. Utilize PrepararRelatorio()</param>
        /// <param name="impressora">impressora padrão para impressão</param>
        /// <param name="mostrarDialog">mostrar caixa de dialogo de impressão (deixar false para impressão automática)</param>
        /// <returns>Se ocorreu erros ou não durante a impressão</returns>
        public static bool ImprimirBoleto(Report r, string impressora, bool mostrarDialog = false)
        {
            try
            {
                r.PrintSettings.ShowDialog = mostrarDialog;
                r.PrintSettings.Printer = impressora;
                r.Print();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }
        /// <summary>
        /// Utilizado para desenhar o arquivo .frx
        /// </summary>
        /// <param name="r">Instancia do Relatório já com o dataset.
        /// Utilize PrepararRelatorio()</param>
        /// <returns>Se ocorreu erros ou não durante o design</returns>
        public static bool DesenharRelatório(Report r)
        {
            try
            {
                r.Design();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }
        /// <summary>
        /// Exporta o relatório para PDF
        /// </summary>
        /// <param name="r">Instancia do Relatório já com o dataset.
        /// Utilize PrepararRelatorio()</param>
        /// <param name="file">Local que o arquivo será salvo</param>
        /// <returns>Se ocorreu erros ou não durante a exportação</returns>
        public static bool ExportarRelatorioPDF(Report r, string file)
        {
            try
            {
                ExportBase export = new PDFExport();
                r.Export(export, file);
                export.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }
    }
}
