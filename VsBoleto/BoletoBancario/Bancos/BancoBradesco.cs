using BoletoBancario.Conta;
using BoletoBancario.Utilitarios;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BoletoBancario.Bancos
{
    public class BancoBradesco : Banco
    {
        public const string Cnab400 = "Cnab400";
        public const string BoletoPadrao = "Padrao";
        public const string BoletoPadraoInvertido = "Padrao Invertido";
        private static string campoLivre;
        private static string pathConfigBanco = AppDomain.CurrentDomain.BaseDirectory + "\\ConfigBoletosBancos.ini";

        internal BancoBradesco()
        {
            nomeBanco = "Bradesco";
            numeroBanco = 237;
            dvNumeroBanco = 2;
            identificacao = "Bradesco";

            MascaraCodigoCedente = "000000000000000";
            MascaraContaCorrente = "0000000-0";
            MascaraNossoNumero = "00000000000";
            MascaraAgencia = "0000-0";

            HabilitarOutros1 = this.HabilitarOutros2 = false;
            TituloOutros1 = this.TituloOutros2 = "";

            LayoutsArquivoRemessa.Add(Cnab400);
            LayoutsArquivoRetorno.Add(Cnab400);
            LayoutsBoleto.Add(BoletoPadrao);
            LayoutsBoleto.Add(BoletoPadraoInvertido);

            EspecieTitulo.Add("01 - Duplicata");
            EspecieTitulo.Add("02 - Nota promissória");
            EspecieTitulo.Add("03 - Nota de Seguro");
            EspecieTitulo.Add("04 - Cobrança Seriada");
            EspecieTitulo.Add("05 - Recibo");
            EspecieTitulo.Add("10 - Letra de Câmbio");
            EspecieTitulo.Add("11 - Nota de Débito");
            EspecieTitulo.Add("12 - Duplicata de Serviço");
            EspecieTitulo.Add("31 - Cartão de Crédito");
            EspecieTitulo.Add("32 - Boleto de Proposta");
            EspecieTitulo.Add("99 - Outros");
        }

        internal override void CalcularDados(Boleto b)
        {
            b.DigVerNossoNumero = DigitoVerificadorNossoNumero(b);
            b.NossoNumeroComDV = b.NossoNumero + b.DigVerNossoNumero;

            b.CodigoBarras = MontarCodigoBarras(b);
            b.LinhaDigitavel = InserirDigitosVerificadores(b, MontarLinhaDigitavel(b));
        }

        internal override bool MontarArquivoRemessa(string tipoRemessa, ContaCorrente c, string path, ref string nome)
        {
            try
            {

                int reg = ArquivoINI.LeString(pathConfigBanco, "BRADESCO", "SEQ_NOME").ToInt32();
                if (reg > 99)
                {
                    reg = 01;
                }

                string nomeAux = "CB" + DateTime.Now.ToString("ddMM") + reg.ToString("00") + ".rem";
                reg++;
                ArquivoINI.EscreveString(pathConfigBanco, "BRADESCO", "SEQ_NOME", reg.ToString());

                nome = nomeAux;

                string remessa = "";
                switch (tipoRemessa)
                {
                    case Cnab400:
                        remessa = MontarCnab400(c);
                        break;
                    default: return false;
                }

                if (!remessa.IsNotNullOrEmpty())
                {
                    return false;
                }

                using (StreamWriter sw = new StreamWriter(path + "\\" + nome, true, Encoding.GetEncoding(1252)))
                    sw.Write(remessa);
            }
            catch
            {
                return false;
            }
            return true;
        }

        internal override bool AbrirArquivoRetorno(string tipoRetorno, string path)
        {
            try
            {
                StreamReader sr = new StreamReader(path);

                switch (tipoRetorno)
                {
                    case Cnab400: RetornoCnab400(sr.ReadToEnd()); break;
                    default: return false;
                }
            }
            catch (IOException ioex)
            {
                throw ioex;
            }
            catch
            {
                return false;
            }
            return true;
        }

        private static void RetornoCnab400(string texto)
        {
        }

        private static string InserirDVCodigoBarras(string codBarra)
        {
            int soma = 0;
            for (int cont = 0; cont < 44; cont++)
                if (cont != 4)
                    soma += codBarra[cont].ToString().ToInt() * indiceCodBarra[cont].ToString().ToInt();
            int digito = 11 - (soma % 11);
            if (digito <= 1 || digito > 9)
                digito = 1;

            codBarra = codBarra.Remove(4, 1);
            codBarra = codBarra.Insert(4, digito.ToString()[0].ToString());
            return codBarra;
        }

        private static string MontarLinhaDigitavel(Boleto b)
        {
            string linha = "";

            //Campo 1
            linha += Utils.InserirString(b.ContaCorrente.Banco.NumeroBanco.ToString(), 3);
            linha += Utils.InserirString(EnumHelper.GetNumMoeda(b.Moeda), 1);
            linha += Utils.InserirString("0", 5);
            //Campo 2
            linha += Utils.Insert(campoLivre.Substring(5, 10), 10);
            //Campo 3
            linha += Utils.Insert(campoLivre.Substring(15, 10), 10);

            //Campo 4 == somente 1 em branco para DV do código de barras
            linha += Utils.Insert(b.CodigoBarras.Substring(4, 1), 1);

            //Campo 5
            linha += Utils.InserirString(GetFatorVencimentoDaData(b.DataVencimento).ToString(), 4);
            linha += Utils.InserirString(Utils.FormatNumber(b.ValorDocumento, 10, 2), 10);

            return linha;
        }

        private static string InserirDigitosVerificadores(Boleto b, string linha)
        {
            //Primeiro Digito
            int soma = 0;
            for (int cont = 0; cont < 9; cont++)
            {
                int mult = linha[cont].ToString().ToInt() * indiceLinhaDigitavel[cont].ToString().ToInt();
                if (mult >= 10)
                {
                    soma += mult.ToString()[0].ToString().ToInt() + mult.ToString()[1].ToString().ToInt();
                }
                else
                {
                    soma += mult;
                }
            }
            int digito = Utils.ProximoMultiplo10(soma) - soma;
            linha = linha.Insert(9, digito.ToString()[0].ToString());

            //Segundo Digito
            soma = 0;
            for (int cont = 10; cont < 20; cont++)
            {
                int mult = linha[cont].ToString().ToInt() * indiceLinhaDigitavel[cont].ToString().ToInt();
                if (mult >= 10)
                    soma += mult.ToString()[0].ToString().ToInt() + mult.ToString()[1].ToString().ToInt();
                else
                    soma += mult;
            }
            digito = Utils.ProximoMultiplo10(soma) - soma;
            linha = linha.Insert(20, digito.ToString()[0].ToString());

            //Terceiro Digito
            soma = 0;
            for (int cont = 21; cont < 31; cont++)
            {
                int mult = linha[cont].ToString().ToInt() * indiceLinhaDigitavel[cont].ToString().ToInt();
                if (mult >= 10)
                    soma += mult.ToString()[0].ToString().ToInt() + mult.ToString()[1].ToString().ToInt();
                else
                    soma += mult;
            }
            digito = Utils.ProximoMultiplo10(soma) - soma;
            linha = linha.Insert(31, digito.ToString()[0].ToString());
            //linha = linha.Insert(32, "0");

            //linha = linha.Remove(32, 1);
            //linha = linha.Insert(32, b.CodigoBarras[4].ToString());
            //b.LinhaDigitavel = linha;
            return linha;
        }

        private static string MontarCodigoBarras(Boleto b)
        {
            string cb = Utils.Insert(b.ContaCorrente.Banco.NumeroBanco.ToString(), 3);
            cb += EnumHelper.GetNumMoeda(b.Moeda);
            cb += 0; //DV
            cb += Utils.Insert(GetFatorVencimentoDaData(b.DataVencimento).ToString(), 4, "0", true);
            cb += Utils.InserirString(Utils.FormatNumber(b.ValorDocumento, 10, 2), 10);

            campoLivre = Utils.Insert(b.ContaCorrente.Agencia, 4, "0", true);
            campoLivre += Utils.Insert(b.ContaCorrente.Banco.Carteira, 2, "0", true);
            campoLivre += Utils.Insert(b.NossoNumero, 11, "0", true);
            campoLivre += Utils.Insert(b.ContaCorrente.NumeroConta.Remove(b.ContaCorrente.NumeroConta.Length - 1), 7, "0", true);
            campoLivre += "0";

            cb += campoLivre;
            cb = InserirDVCodigoBarras(cb);

            return cb;
        }

        private static string DigitoVerificadorNossoNumero(Boleto b)
        {
            string seq = Utils.FormatNumber(b.ContaCorrente.Banco.Carteira.ToNoFormated(), 2) +
                Utils.FormatNumber(b.NossoNumero.ToNoFormated(), 11);

            int cont = 0;
            int soma = 0;
            foreach (char c in seq)
            {
                soma += c.ToString().ToInt() * indiceNossoNumero[cont].ToString().ToInt();
                cont++;
            }

            int resto = soma % 11;
            if (resto == 1)
            {
                return "P";
            }

            if (resto == 0)
            {
                return "0";
            }

            return (11 - resto).ToString();
        }

        private static int GetFatorVencimentoDaData(DateTime dataVencimento)
        {
            return dataVencimento.Subtract(new DateTime(2000, 07, 03)).Days + 1000;
        }

        private static DateTime GetDataDoFatorVencimento(int fatorVencimento)
        {
            return new DateTime(2000, 07, 03).AddDays(fatorVencimento - 1000);
        }

        private static string MontarCnab400(ContaCorrente c)
        {

            int sequencial = 1;

            string aux = "";
            string ret = "";

            ret += Utils.Insert("0", 1);
            ret += Utils.Insert("1", 1);
            ret += Utils.Insert("REMESSA", 7);
            ret += Utils.Insert("01", 2);
            ret += Utils.Insert("COBRANCA", 15);
            ret += Utils.Insert(c.CodigoCedente.ToNoFormated(), 20, "0", true);
            ret += Utils.Insert(c.Cedente.NomeCedente, 30);
            ret += Utils.Insert(c.Banco.NumeroBanco.ToString(), 3);
            ret += Utils.Insert(c.Banco.Identificacao.ToUpper(), 15);
            ret += Utils.Insert(DateTime.Now.ToString("ddMMyy"), 6);
            ret += Utils.Insert(Utils.ReplicarChar(" ", 8), 8);
            ret += Utils.Insert("MX", 2);


            int seqRemessa = ArquivoINI.LeString(pathConfigBanco, "BRADESCO", "SEQ_REMESSA").ToInt32();
            ret += Utils.Insert(seqRemessa.ToString(), 7, "0", true);
            seqRemessa++;
            ArquivoINI.EscreveString(pathConfigBanco, "BRADESCO", "SEQ_REMESSA", seqRemessa.ToString());

            ret += Utils.Insert(Utils.ReplicarChar(" ", 277), 277);
            ret += Utils.Insert(sequencial++.ToString("000000"), 6);

            aux += ret + Environment.NewLine;

            foreach (Boleto b in c.Boletos)
            {
                b.CalcularDadosBoleto();

                ret = "1";
                ret += Utils.Insert("0000000000000000000", 19);
                ret += "0";
                ret += Utils.Insert(c.Banco.Carteira, 3, "0", true);
                ret += Utils.Insert(c.Agencia.Remove(c.Agencia.Length - 1), 5, "0", true);
                ret += Utils.Insert(c.NumeroConta, 8, "0", true);
                ret += Utils.Insert("", 25);
                bool usaNumBanco = ArquivoINI.LeBool(pathConfigBanco, "BRADESCO", "UTILIZA_NUM_BANCO", true);
                ret += Utils.Insert(usaNumBanco && c.Banco.GetType() == typeof(BancoBradesco) ? c.Banco.NumeroBanco.ToString() : "000", 3);
                ret += Utils.Insert(b.ValorMultaAtraso > 0 ? "2" : "0", 1);
                ret += Utils.Insert(Utils.FormatNumber(b.PercentualMultaAtraso, 4, 2), 4);
                ret += Utils.Insert(b.NossoNumeroComDV, 12, "0", true);
                ret += Utils.Insert("0000000000", 10);
                ret += Utils.Insert("2", 1);
                ret += Utils.Insert("", 1);
                ret += Utils.Insert("", 10);
                ret += Utils.Insert("", 1);
                ret += Utils.Insert("", 1);
                ret += Utils.Insert("", 2);
                ret += Utils.Insert("01", 2);
                ret += Utils.Insert(b.NumeroDocumento.ToNoFormated(), 10, "0", true);
                ret += b.DataVencimento.ToString("ddMMyy");
                ret += Utils.FormatNumber(b.ValorDocumento, 13, 2);
                ret += Utils.Insert("000", 3);
                ret += Utils.Insert("00000", 5);
                ret += c.EspecieTitulo;
                ret += Utils.Insert("N", 1);
                ret += b.DataDocumento.ToString("ddMMyy");
                ret += Utils.Insert(b.Instrucao1, 2);
                ret += Utils.Insert(b.Instrucao2, 2);
                ret += Utils.FormatNumber(b.ValorJurosDiaAtraso, 13, 2);

                if (b.ValorDesconto > 0)
                {
                    ret += b.DataVencimento.ToString("ddMMyy");
                }
                else
                {
                    ret += "000000";
                    //ret += b.DataLimiteDesconto <= b.DataVencimento ? "000000" : b.DataLimiteDesconto.ToString("ddMMyy");
                }                
                ret += Utils.FormatNumber(b.ValorDesconto, 13, 2);
                ret += Utils.FormatNumber("", 13, 2);
                ret += Utils.FormatNumber(b.ValorAbatimento, 13, 2);
                ret += Utils.Insert(b.Sacado.CpfCnpj.ToNoFormated().IsCpf() ? "01" : "02", 2);
                ret += Utils.Insert(b.Sacado.CpfCnpj.ToNoFormated(), 14, "0", true);
                ret += Utils.Insert(b.Sacado.Razao.Trim().Length > 3 ? b.Sacado.Razao : b.Sacado.Nome, 40);
                string aux_end = b.Sacado.Endereco + " " + b.Sacado.NumEndereco + " " + b.Sacado.Bairro;
                ret += Utils.Insert(aux_end, 40);
                ret += Utils.Insert("", 12);
                ret += Utils.Insert(b.Sacado.Cep.ToNoFormated(), 8);
                ret += Utils.Insert("", 60);
                ret += Utils.Insert(sequencial++.ToString("000000"), 6);

                aux += ret + Environment.NewLine;
            }

            ret = "9";
            ret += Utils.Insert(" ", 393);
            ret += Utils.Insert(sequencial++.ToString("000000"), 6);

            aux += ret + Environment.NewLine;

            return aux;
        }

        private const string indiceNossoNumero = "2765432765432";
        private const string indiceCodBarra = "43290876543298765432987654329876543298765432";
        private const string indiceLinhaDigitavel = "2121212120121212121201212121212";
    }
}
