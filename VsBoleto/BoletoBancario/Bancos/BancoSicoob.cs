using BoletoBancario.Conta;
using BoletoBancario.Utilitarios;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BoletoBancario.Bancos
{
    public class BancoSicoob : Banco
    {
        public const string Cnab400 = "Cnab400";
        public const string Cnab240 = "Cnab240";
        public const string BoletoPadrao = "Padrao";
        public const string BoletoPadraoInvertido = "Padrao Invertido";
        private static string pathConfigBanco = AppDomain.CurrentDomain.BaseDirectory + "\\ConfigBoletosBancos.ini";


        internal BancoSicoob()
        {
            nomeBanco = "Sicoob";
            numeroBanco = 756;
            dvNumeroBanco = 0;
            identificacao = "756BANCOOBCED";

            TituloOutros1 = "Modalidade";
            TituloOutros2 = "Carteira";

            MascaraCodigoCedente = "0000000000";
            MascaraContaCorrente = "000000000-0";
            MascaraNossoNumero = "0000000";
            MascaraOutros1 = "00";
            MascaraOutros2 = "0";

            LayoutsArquivoRemessa.Add(Cnab400);
            LayoutsArquivoRemessa.Add(Cnab240);
            LayoutsArquivoRetorno.Add(Cnab400);
            LayoutsArquivoRetorno.Add(Cnab240);
            LayoutsBoleto.Add(BoletoPadrao);
            LayoutsBoleto.Add(BoletoPadraoInvertido);

            EspecieTitulo.Add("01 - Duplicata Mercantil");
            EspecieTitulo.Add("02 - Nota promissória");
            EspecieTitulo.Add("03 - Nota de Seguro");
            EspecieTitulo.Add("05 - Recibo");
            EspecieTitulo.Add("06 - Duplicata Rural");
            EspecieTitulo.Add("08 - Letra de Câmbio");
            EspecieTitulo.Add("09 - Warrant");
            EspecieTitulo.Add("10 - Cheque");
            EspecieTitulo.Add("12 - Duplicata de Serviço");
            EspecieTitulo.Add("13 - Nota de Débito");
            EspecieTitulo.Add("14 - Triplicata Mercantil");
            EspecieTitulo.Add("15 - Triplicata de Serviço");
            EspecieTitulo.Add("18 - Fatura");
            EspecieTitulo.Add("20 - Apólice de Seguro");
            EspecieTitulo.Add("21 - Mensalidade Escolar");
            EspecieTitulo.Add("22 - Parcela de Consórcio");
            EspecieTitulo.Add("99 - Outros");

        }

        internal override void CalcularDados(Boleto b)
        {
            b.DigVerNossoNumero = DigitoVerificadorNossoNumero(b);
            b.NossoNumeroComDV = b.NossoNumero + b.DigVerNossoNumero;

            string ld = InserirDigitosVerificadores(b, MontarLinhaDigitavel(b));

            if (!ValidarLinhaDigitavel(ld))
            {
                throw new Exception("Falha ao calcular Linha Digitável. Tente novamente."
            + "Se o problema persistir, contate o suporte.");
            }

            b.LinhaDigitavel = ld;

            if (!ValidarCodigoBarras(b.CodigoBarras))
            {
                throw new Exception("Falha ao calcular o Código de Barras. Tente novamente."
                    + "Se o problema persistir, contate o suporte.");
            }
        }

        internal override bool MontarArquivoRemessa(string tipoRemessa, ContaCorrente c, string path, ref string nome)
        {
            try
            {
                string remessa = "";

                if (tipoRemessa == Cnab240)
                {
                    remessa = MontarCnab240(c);
                }
                else if (tipoRemessa == Cnab400)
                {
                    remessa = MontarCnab400(c);
                }


                if (!remessa.IsNotNullOrEmpty())
                {
                    return false;
                }

                using (StreamWriter sw = new StreamWriter(path + "\\" + nome, true, Encoding.GetEncoding(1252)))
                {
                    sw.Write(remessa);
                }

                if (c.GeraRelatorioItens == true)
                {
                    // a desenvolver
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
            string[] linhas = texto.Split(new string[1] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in linhas)
            {
                if (s.Length != 400)
                    throw new ArgumentException("Uma ou mais linhas do arquivo está fora do layout.");

                object r;

                if (s.Substring(0, 1) == "0")
                {
                    r = s.Substring(26, 4); // prefixo cooperativa
                    r = s.Substring(30, 1); // dv
                    r = s.Substring(31, 8); // codigo do cliente
                    r = s.Substring(39, 1); // dv
                    r = s.Substring(46, 30); // nome cedente
                    r = s.Substring(94, 6); // data
                    r = s.Substring(101, 7); // sequencial retorno
                    r = s.Substring(394, 6); // sequenciar registro
                }
                else if (s.Substring(0, 1) == "1")
                {
                    r = s.Substring(3, 14); // cpf;cnpj
                    r = s.Substring(17, 4); //prefixo cooperativa
                    r = s.Substring(30, 1); // dv
                    r = s.Substring(31, 8); // codigo do cliente
                    r = s.Substring(39, 1); // dv
                    r = s.Substring(37, 25); // numero controle
                    r = s.Substring(63, 11); // codigo do cliente
                    r = s.Substring(73, 1); // dv
                    r = s.Substring(74, 2); // numero parcela
                    r = s.Substring(80, 2); // codigo baixa/recusa
                    r = s.Substring(85, 3); // variacao carteira
                    r = s.Substring(88, 1); // conta caução
                    r = s.Substring(89, 5); // codigo de responsabilidade
                    r = s.Substring(95, 5); // taxa de desconto
                    r = s.Substring(100, 5); // taxa de IOF
                    r = s.Substring(106, 2); // carteira/modalidade
                    r = s.Substring(108, 2); // comando/movimento
                    r = s.Substring(110, 6); // data entrada
                    r = s.Substring(115, 10); // seu numero
                    r = s.Substring(146, 6); // data vencimento
                    r = s.Substring(152, 13); // valor titulo
                    r = s.Substring(165, 3); // codigo banco receberdor
                    r = s.Substring(168, 4); // prefixo agencia recebedora
                    r = s.Substring(172, 1); // DV prefixo agencia recebedora
                    r = s.Substring(173, 2); // especie do titulo
                    r = s.Substring(175, 6); // data do credito
                    r = s.Substring(181, 7); // valor da tarifa
                    r = s.Substring(188, 13); // outras despesas
                    r = s.Substring(201, 13); // juros do desconto
                    r = s.Substring(214, 13); // IOF do desconto
                    r = s.Substring(227, 13); // valor do abatimento
                    r = s.Substring(240, 13); // desconto concedido (valor titulo - valor recebido)
                    r = s.Substring(253, 13); // valor recebido (parcial)
                    r = s.Substring(266, 13); // juros de mora
                    r = s.Substring(279, 13); // outros recebimentos
                    r = s.Substring(292, 13); // abatimento nao aproveitado pelo sacado
                    r = s.Substring(305, 13); // valor do lançamento
                    r = s.Substring(318, 13); // indicativo debito/credito
                    r = s.Substring(319, 13); // indicativo valor
                    r = s.Substring(320, 12); // valor do ajuste
                    r = s.Substring(342, 14); // cpf/cnpj sacado
                    r = s.Substring(394, 6); // complemento de registro sequencial
                }
                else if (s.Substring(0, 1) == "9")
                {
                    r = s.Substring(1, 2); // tipo de serviço
                    r = s.Substring(3, 3); // numero banco
                    r = s.Substring(6, 4); // codigo cooperativa remetente
                    r = s.Substring(10, 25); // sigla cooperativa
                    r = s.Substring(35, 50); // endereco cooperativa
                    r = s.Substring(85, 30); // bairro
                    r = s.Substring(115, 8); // cep
                    r = s.Substring(123, 30); // cidade
                    r = s.Substring(153, 2); // UF
                    r = s.Substring(155, 8); // data (aaaammdd | yyyyMMdd)
                    r = s.Substring(163, 8); // qtd registros detalhe
                    r = s.Substring(171, 11); // ultimo nosso numero cedente
                    r = s.Substring(394, 6); // complemento de registro sequencial
                }


            }
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
            linha += Utils.InserirString(b.ContaCorrente.Outros2, 1);//carteira
            linha += Utils.InserirString(b.ContaCorrente.Agencia, 4);
            //Campo 2
            linha += Utils.InserirString(b.ContaCorrente.Outros1, 2);//modalidade
            linha += Utils.InserirString(Utils.FormatNumber(b.ContaCorrente.CodigoCedente, 7), 7);
            linha += Utils.InserirString(Utils.FormatNumber(b.NossoNumeroComDV, 8), 1);
            //Campo 3
            linha += Utils.InserirString(Utils.FormatNumber(b.NossoNumeroComDV, 8).Substring(1), 7);
            int auxParcela = 1;
            if (b.NumeroParcelaAtual == 0)
            {
                auxParcela = b.NumeroParcelaAtual + 1;
            }
            linha += Utils.InserirString(Utils.FormatNumber(auxParcela, 3), 3);

            //Campo 4 == somente 1 em branco para DV do código de barras

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
                    soma += mult.ToString()[0].ToString().ToInt() + mult.ToString()[1].ToString().ToInt();
                else
                    soma += mult;
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
            linha = linha.Insert(32, "0");
            //Montar codigo de barras e verificar ultimo digito

            string codigoBarras = MontarCodigoBarras(linha);

            codigoBarras = InserirDVCodigoBarras(codigoBarras);
            b.CodigoBarras = codigoBarras;

            linha = linha.Remove(32, 1);
            linha = linha.Insert(32, codigoBarras[4].ToString());
            b.LinhaDigitavel = linha;
            return linha;
        }

        private static string MontarCodigoBarras(string linha)
        {
            string codigoBarras =
                linha.Substring(0, 3) + // Código Banco
                linha.Substring(3, 1) + // Moeda
                linha.Substring(32, 1) + // DV
                linha.Substring(33, 4) + // fator vencimento
                linha.Substring(37, 10) + // valor documento
                linha.Substring(4, 1) + // carteira
                linha.Substring(5, 4) + // agencia
                linha.Substring(10, 2) + // modalidade cobrança
                linha.Substring(12, 7) + // codigo cliente
                linha.Substring(19, 1) + linha.Substring(21, 7) + // nosso numero
                linha.Substring(28, 3);// parcela
            return codigoBarras;
        }

        private static bool ValidarCodigoBarras(string codigoBarras)
        {
            int soma = 0;
            for (int cont = 0; cont < 44; cont++)
                if (cont != 4)
                    soma += codigoBarras[cont].ToString().ToInt() * indiceCodBarra[cont].ToString().ToInt();
            int digito = 11 - (soma % 11);
            if (digito <= 1 || digito > 9)
                digito = 1;

            return codigoBarras[4].ToString().ToInt() == digito;
        }

        private static bool ValidarLinhaDigitavel(string linha)
        {
            //Primeiro Digito
            int soma = 0;
            for (int cont = 0; cont < 9; cont++)
            {
                int mult = linha[cont].ToString().ToInt() * indiceLinhaDigitavel[cont].ToString().ToInt();
                if (mult >= 10)
                    soma += mult.ToString()[0].ToString().ToInt() + mult.ToString()[1].ToString().ToInt();
                else
                    soma += mult;
            }
            int digito = Utils.ProximoMultiplo10(soma) - soma;
            if (linha[9].ToString() != digito.ToString())
                return false;

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
            if (linha[20].ToString() != digito.ToString())
                return false;

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
            if (linha[31].ToString() != digito.ToString())
                return false;
            //Montar codigo de barras e verificar ultimo digito

            string codigoBarras = MontarCodigoBarras(linha);

            return ValidarCodigoBarras(codigoBarras);
        }

        private static string DigitoVerificadorNossoNumero(Boleto b)
        {
            string seq = Utils.FormatNumber(b.ContaCorrente.Agencia.ToNoFormated(), 4) +
                Utils.FormatNumber(b.ContaCorrente.CodigoCedente.ToNoFormated(), 10) +
                Utils.FormatNumber(b.NossoNumero.ToNoFormated(), 7);

            int cont = 0;
            int soma = 0;
            foreach (char c in seq)
            {
                soma += c.ToString().ToInt() * indiceNossoNumero[cont].ToString().ToInt();
                cont++;
            }
            int digito = 11 - (soma % 11);
            if (digito < 1 || digito > 9)
                digito = 0;

            return digito.ToString();
        }

        private static int GetFatorVencimentoDaData(DateTime dataVencimento)
        {
            return dataVencimento.Subtract(new DateTime(2000, 07, 03)).Days + 1000;
        }

        private static DateTime GetDataDoFatorVencimento(int fatorVencimento)
        {
            return new DateTime(2000, 07, 03).AddDays(fatorVencimento - 1000);
        }

        private static string GetDigitoAgencia(string agencia)
        {
            switch (agencia)
            {
                case "0001": return "9";
                case "1001": return "4";
                case "1002": return "2";
                case "1003": return "0";
                case "1004": return "9";
                case "1005": return "7";
                case "1006": return "5";
                case "1007": return "3";
                case "1008": return "1";
                case "2001": return "0";
                case "2002": return "8";
                case "2003": return "6";
                case "2004": return "4";
                case "2005": return "2";
                case "2006": return "0";
                case "2007": return "9";
                case "2008": return "7";
                case "2009": return "5";
                case "2010": return "9";
                case "2011": return "7";
                case "2012": return "5";
                case "2013": return "3";
                case "2014": return "1";
                case "2015": return "0";
                case "2016": return "8";
                case "2017": return "6";
                case "2018": return "4";
                case "2019": return "2";
                case "2020": return "6";
                case "2021": return "4";
                case "2022": return "2";
                case "3001": return "5";
                case "3003": return "1";
                case "3007": return "4";
                case "3008": return "2";
                case "3009": return "0";
                case "3010": return "4";
                case "3017": return "1";
                case "3019": return "8";
                case "3020": return "1";
                case "3021": return "0";
                case "3023": return "6";
                case "3025": return "2";
                case "3027": return "9";
                case "3031": return "7";
                case "3032": return "5";
                case "3033": return "3";
                case "3034": return "1";
                case "3035": return "0";
                case "3036": return "8";
                case "3037": return "6";
                case "3038": return "4";
                case "3039": return "2";
                case "3041": return "4";
                case "3042": return "2";
                case "3043": return "0";
                case "3045": return "7";
                case "3046": return "5";
                case "3047": return "3";
                case "3049": return "0";
                case "3050": return "3";
                case "3053": return "8";
                case "3054": return "6";
                case "3055": return "4";
                case "3056": return "2";
                case "3058": return "9";
                case "3059": return "7";
                case "3060": return "0";
                case "3061": return "9";
                case "3062": return "7";
                case "3064": return "3";
                case "3066": return "0";
                case "3067": return "8";
                case "3068": return "6";
                case "3069": return "4";
                case "3070": return "8";
                case "3071": return "6";
                case "3072": return "4";
                case "3074": return "0";
                case "3075": return "9";
                case "3076": return "7";
                case "3078": return "3";
                case "3080": return "5";
                case "3081": return "3";
                case "3084": return "8";
                case "3087": return "2";
                case "3088": return "0";
                case "3089": return "9";
                case "3091": return "0";
                case "3092": return "9";
                case "3093": return "7";
                case "3094": return "5";
                case "3095": return "3";
                case "3096": return "1";
                case "3098": return "8";
                case "3099": return "6";
                case "3100": return "3";
                case "3101": return "1";
                case "3102": return "0";
                case "3103": return "8";
                case "3104": return "6";
                case "3105": return "4";
                case "3106": return "2";
                case "3107": return "0";
                case "3108": return "9";
                case "3109": return "7";
                case "3112": return "7";
                case "3113": return "5";
                case "3114": return "3";
                case "3116": return "0";
                case "3117": return "8";
                case "3118": return "6";
                case "3119": return "4";
                case "3120": return "8";
                case "3121": return "6";
                case "3122": return "4";
                case "3123": return "2";
                case "3125": return "9";
                case "3127": return "5";
                case "3129": return "1";
                case "3131": return "3";
                case "3132": return "1";
                case "3133": return "0";
                case "3134": return "8";
                case "3135": return "6";
                case "3136": return "4";
                case "3137": return "2";
                case "3138": return "0";
                case "3140": return "2";
                case "3141": return "0";
                case "3143": return "7";
                case "3144": return "5";
                case "3145": return "3";
                case "3150": return "0";
                case "3152": return "6";
                case "3154": return "2";
                case "3155": return "0";
                case "3157": return "7";
                default: return "0";
            }
        }

        private static string MontarCnab400(ContaCorrente c)
        {
            try
            {
                int sequencial = 1;

                string aux = "";
                string ret = "";

                ret += Utils.Insert("0", 1);// 1-1
                ret += Utils.Insert("1", 1);// 2-2
                ret += Utils.Insert("REMESSA", 7);// 3-9
                ret += Utils.Insert("01", 2);// 10-11
                ret += Utils.Insert("COBRANÇA", 8);// 12-19
                ret += Utils.Insert(Utils.ReplicarChar(" ", 7), 7);// 20-26
                ret += Utils.Insert(c.Banco.Carteira, 4);// 27-30
                ret += Utils.Insert(c.Banco.Carteira.ToString().Substring(4), 1); // 31-31
                ret += Utils.Insert(Utils.FormatNumber(c.CodigoCedente.Remove(c.CodigoCedente.Length - 1), 8), 8);// 32-39
                ret += Utils.Insert(c.CodigoCedente.Substring(c.CodigoCedente.Length - 1), 1);// 40-40
                ret += Utils.Insert(Utils.ReplicarChar(" ", 6), 6);// 41-46
                ret += Utils.Insert(c.Cedente.NomeCedente, 30);// 47-76
                ret += Utils.Insert(c.Banco.Identificacao, 18);// 77-94
                ret += Utils.Insert(DateTime.Now.ToString("ddMMyy"), 6);// 95-100
                ret += Utils.Insert("0000001", 7); // 101-107
                ret += Utils.Insert(Utils.ReplicarChar(" ", 287), 287);// 108-394
                ret += Utils.Insert(sequencial++.ToString("000000"), 6);// 395-400

                aux += ret + Environment.NewLine;

                decimal juros = 0;
                decimal multa = 0;

                foreach (Boleto b in c.Boletos)
                {
                    b.CalcularDadosBoleto();

                    int tamanho = ret.Length;

                    ret = "1"; // 1-1
                    ret += Utils.Insert(c.Cedente.CpfCnpj.ToNoFormated().IsCpf() ? "01" : "02", 2); //2-3
                    ret += Utils.Insert(c.Cedente.CpfCnpj.ToNoFormated(), 14, "0", true); //4-17
                    ret += Utils.Insert(c.Banco.Carteira.ToNoFormated(), 4); // 18-21
                    ret += Utils.Insert(c.Banco.Carteira.ToNoFormated().Substring(4), 1); // 22-22
                    string cC = c.NumeroConta.ToNoFormated();
                    ret += Utils.Insert(cC.Remove(cC.Length - 1), 8, "0", true); //23-30
                    ret += Utils.Insert(cC.Substring(cC.Length - 1), 1); // 31-31
                    ret += Utils.Insert("", 6, "0"); // 32-37
                    ret += Utils.Insert(" ", 25); // 38-62
                    ret += Utils.Insert(b.NossoNumeroComDV, 12, "0", true); // 63-74
                    ret += b.NumeroParcelaAtual.ToString("00"); // 75-76
                    ret += "00"; // 77-78
                    ret += Utils.Insert(" ", 3); //79-81
                    ret += " "; //82-82
                    ret += Utils.Insert(" ", 3); //83-85
                    ret += "000"; //86-88
                    ret += "0"; //89-89
                    ret += Utils.Insert("", 5, "0"); //90-94
                    ret += "0";//95-95
                    ret += Utils.Insert("", 6, "0"); //96-101
                    ret += Utils.Insert(" ", 4);//102-105     
                    ret += "2";//106-106                
                    ret += c.Outros1;//107-108
                    ret += "01";//109-110
                    ret += Utils.Insert(b.NumeroDocumento.ToNoFormated(), 10, "0", true);//111-120
                    ret += b.DataVencimento.ToString("ddMMyy");//121-126
                    ret += Utils.FormatNumber(b.ValorDocumento, 13, 2);//127-139
                    ret += Utils.Insert(c.Banco.NumeroBanco.ToString(), 3);//140-142
                    ret += Utils.Insert(c.Banco.Carteira, 4);//143-146
                    ret += Utils.Insert(c.Banco.Carteira.ToString().Substring(4), 1); //147-147
                    ret += c.EspecieTitulo;//148-149
                    ret += b.Aceite.ToUpper() == "S" ? "1" : "0";//150-150
                    ret += b.DataDocumento.ToString("ddMMyy");//151-156
                    ret += Utils.Insert(b.Instrucao1, 2);//157-158
                    ret += Utils.Insert(b.Instrucao2, 2);//159-160
                    juros = b.PercentualJurosMesAtraso;
                    ret += Utils.FormatNumber(b.PercentualJurosMesAtraso, 6, 4);//161-166
                    multa = b.PercentualMultaAtraso;
                    ret += Utils.FormatNumber(b.PercentualMultaAtraso, 6, 4);//167-172
                    ret += "2";//173-173
                    if (b.ValorDesconto > 0)
                    {
                        ret += b.DataVencimento.ToString("ddMMyy");//174-179
                    }
                    else
                    {
                        ret += "000000";
                        //ret += b.DataLimiteDesconto <= b.DataVencimento ? "000000" : b.DataLimiteDesconto.ToString("ddMMyy");//174-179
                    }
                    ret += Utils.FormatNumber(b.ValorDesconto, 13, 2);//180-192
                    ret += EnumHelper.GetNumMoeda(b.Moeda) + "000000000000";//193-205
                    ret += Utils.FormatNumber(b.ValorAbatimento, 13, 2);//206-218
                    ret += Utils.Insert(b.Sacado.CpfCnpj.ToNoFormated().IsCpf() ? "01" : "02", 2);//219-220
                    ret += Utils.Insert(b.Sacado.CpfCnpj.ToNoFormated(), 14, "0", true);//221-234
                    ret += Utils.Insert(b.Sacado.Razao.Trim().Length > 3 ? b.Sacado.Razao : b.Sacado.Nome, 40);//235-274
                    ret += Utils.Insert(b.Sacado.Endereco, 37);//275-311
                    ret += Utils.Insert(b.Sacado.Bairro, 15);//312-326
                    ret += Utils.Insert(b.Sacado.Cep.ToNoFormated(), 8);//327-334
                    ret += Utils.Insert(b.Sacado.Cidade, 15);//335-349
                    ret += Utils.Insert(b.Sacado.Estado, 2);//350-351
                    ret += Utils.Insert(" ", 40);//352-391
                    ret += b.DiasProtesto;//392-393
                    ret += " ";//394-394
                    ret += Utils.Insert(sequencial++.ToString("000000"), 6);//395-400

                    aux += ret + Environment.NewLine;
                    int cont = ret.Length;
                }

                ret = "9";
                ret += Utils.Insert(" ", 193);
                ret += Utils.Insert(juros > 0 ? "Cobrar juros de " + juros + "% ao mês" : " ", 40);
                ret += Utils.Insert(multa > 0 ? "Cobrar multa de " + multa + "%" : " ", 40);
                ret += Utils.Insert(" ", 40);
                ret += Utils.Insert(" ", 40);
                ret += Utils.Insert(" ", 40);
                ret += Utils.Insert(sequencial++.ToString("000000"), 6);

                aux += ret + Environment.NewLine;

                return aux;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao gera a remessa: " + ex.Message);
                return null;
            }

        }

        private static string MontarCnab240(ContaCorrente c)
        {
            try
            {               
                int sequenciaRegistro = 1;

                string arquivo = "";
                string linha = "";

                //Header do Arquivo

                linha = "756"; // 01-03 => Código do Banco na Compensação 
                linha += "0000"; // 04-07 => Lote do Serviço
                linha += "0"; // 08-08 => Tipo de Registro
                linha += Utils.Insert(" ", 9); // 09-17 => Uso Exclusivo Febraban
                linha += Utils.Insert(c.Cedente.CpfCnpj.ToNoFormated().IsCpf() ? "1" : "2", 1); // 18-18 => Tipo de Inscrição da Empresa (1 = CPF, 2 = CNPJ)
                linha += Utils.Insert(c.Cedente.CpfCnpj.ToNoFormated(), 14, "0", true); // 19-32 => Número de Inscrição da Empresa
                linha += Utils.Insert(" ", 20); // 33-52 => Código do Convenio no Sicoob
                linha += Utils.Insert(c.Agencia, 5, "0", true); //53-57 => Agência mantenedora
                linha += GetDigitoAgencia(c.Agencia); // 58-58 => Dígito verificador da Agencia
                linha += Utils.Insert(c.NumeroConta, 13, "0", true); // 59-71 => Conta Corrente
                linha += "0"; // 72-72 => Digito Verificador Agencia/Conta
                linha += Utils.Insert(c.Cedente.NomeCedente, 30); // 73-102 => Nome da Empresa
                linha += Utils.Insert("SICOOB", 30); // 103-132 => Nome do banco
                linha += Utils.Insert(" ", 10); // 133-142 => Uso exclusivo Febraban
                linha += "1"; // 143-143 => Código Remessa
                linha += Utils.Insert(DateTime.Now.ToString("ddMMyyyy"), 8); // 144-151 => data de Geração do Arquivo
                linha += Utils.Insert(DateTime.Now.ToString("HHmmss"), 6); // 152-157 => Hora de Geração do Arquivo
                linha += "000000"; // 158-163 => Número Sequencial do Arquivo
                linha += "081"; // 164-166 => número Versão do Layout do Arquivo
                linha += "00000"; // 167-171 => Densidade de Gravação do Arquivo
                linha += Utils.Insert(" ", 20); // 172-191 => Reservado do banco
                linha += Utils.Insert(" ", 20); // 192-211 => Reservado da empresa
                linha += Utils.Insert(" ", 29); // 212-240 => Uso Exclusivo Febraban

                arquivo += linha + Environment.NewLine;
                //Fim Header do Arquivo

                //Header do Lote

                linha = "756"; // 01-03 => Código do Banco
                linha += "0001"; // 04-07 => Lote de Serviço
                linha += "1"; // 08-08 => Tipo de Registro
                linha += "R"; // 09-09 => Tipo de Operação
                linha += "01"; // 10-11 => Tipo de Serviço
                linha += "  "; // 12-13 => Uso Exclusivo Febraban
                linha += "040"; // 14-16 => Número da Versão do Layout
                linha += " "; // 17-17 => Uso Exclusivo febraban
                linha += Utils.Insert(c.Cedente.CpfCnpj.ToNoFormated().IsCpf() ? "1" : "2", 1); // 18-18 => Tipo de Inscrição da Empresa (1 = CPF, 2 = CNPJ)
                linha += Utils.Insert(c.Cedente.CpfCnpj.ToNoFormated(), 15, "0", true); // 19-33 => Número de Inscrição da Empresa
                linha += Utils.Insert(" ", 20); // 34-53 => Código do Convenio do Banco
                linha += Utils.Insert(c.Agencia, 5, "0", true); //54-58 => Agência mantenedora
                linha += GetDigitoAgencia(c.Agencia); // 59-59 => Dígito verificador da Agencia
                linha += Utils.Insert(c.NumeroConta, 13, "0", true); // 60-72 => Conta Corrente
                linha += " "; // 73-73 => Digito Verificador 
                linha += Utils.Insert(c.Cedente.NomeCedente, 30); // 74-103 => Nome da Empresa
                linha += Utils.Insert(" ", 40); // 104-143 => Informação 1
                linha += Utils.Insert(" ", 40); // 144-183 => Informação 2

                int seqRemessa = ArquivoINI.LeString(pathConfigBanco, "REMESSA", "SEQ_REMESSA").ToInt32();
                linha += Utils.Insert(seqRemessa.ToString(), 8, "0", true); // 184-191 => Número Remessa        
                ArquivoINI.EscreveString(pathConfigBanco, "REMESSA", "SEQ_REMESSA", Convert.ToString(seqRemessa++));

                linha += Utils.Insert(DateTime.Now.ToString("ddMMyyyy"), 8); // 192-199 => Data de Gravação
                linha += Utils.Insert("0", 8); // 200-207 => Data do Credito
                linha += Utils.Insert(" ", 33); // 208-240 => uso Exclusivo Febraban

                arquivo += linha + Environment.NewLine;
                //Fim Header do Lote

                foreach (Boleto b in c.Boletos)
                {
                    b.CalcularDadosBoleto();

                    //Segmento P
                    linha = "756"; // 01-03 => Código do Banco
                    linha += "0001"; // 04-07 => Lote do Serviço
                    linha += "3"; // 08-08 => Tipo de Registro
                    linha += Utils.Insert(sequenciaRegistro.ToString(), 5, "0", true); // 09-13 => Número Sequencial do registro
                    linha += "P"; // 14-14 => Código do Segmento do registro
                    linha += " "; // 15-15 => uso Exclusivo Febraban
                    linha += "01"; // 16-17 => Código Movimento da Remessa
                    linha += Utils.Insert(c.Agencia, 5, "0", true); // 18-22 => Agência mantenedora
                    linha += GetDigitoAgencia(c.Agencia); // 23-23 => Dígito verificador da Agencia
                    linha += Utils.Insert(c.NumeroConta, 13, "0", true); // 24-36 => Conta Corrente
                    linha += " "; // 37-37 => Digito Verificador 
                    linha += Utils.Insert(b.NossoNumeroComDV, 10, "0", true); // 38-47 => Nosso Número
                    linha += Utils.Insert(b.NumeroParcelaAtual.ToString(), 2, "0", true); // 48-49 => Número da Parcela
                    linha += Utils.Insert(c.Outros1, 2, "0", true); // 50-51 => Modalidade
                    linha += "4"; // 52-52 => Tipo de Formulário
                    linha += Utils.Insert(" ", 5); // 53-57 => Brancos
                    linha += c.Outros2; // 58-58 => Carteira
                    linha += "0"; // 59-59 => Forma de Cadastramento do Título no Banco
                    linha += " "; // 60-60 => Tipo de Documento
                    linha += "2"; // 61-61 => Identificação da Emissão do Boleto (1 = Sicoob, 2 = Beneficiário)
                    linha += "2"; // 62-62 => Identificação da Distribuição do Boleto (1 = Sicoob, 2 = Beneficiário)
                    linha += Utils.Insert(b.NumeroDocumento, 15, "0"); // 63-77 => Número do Documento
                    linha += b.DataVencimento.ToString("ddMMyyyy"); // 78-85 => Data de vencimento do Título
                    linha += Utils.FormatNumber(b.ValorDocumento, 15, 2); // 86-100 => Valor do Título
                    linha += "00000"; // 101-105 => Agencia Encarregada de Cobrança
                    linha += " "; // 106-106 => Dígito Verificador da Agencia
                    linha += c.EspecieTitulo; // 107-108 => Espécie de Título
                    linha += b.Aceite.ToUpper() == "S" ? "A" : "N"; // 109-109 => Aceite do Título
                    linha += b.DataDocumento.ToString("ddMMyyyy"); // 110-117 => Data de Emissão do Título
                    linha += "1"; // 118-118 => Código do juros de Mora
                    linha += b.DataVencimento.ToString("ddMMyyyy"); // 119-126 => Data de Juros de Mora
                    linha += Utils.FormatNumber(b.ValorJurosDiaAtraso, 15, 2); // 127-141 => Juros de Mora por Dia

                    if (b.ValorDesconto > 0)
                    {
                        linha += "1"; // 142-142 => Código Desconto 1
                        linha += b.DataVencimento.ToString("ddMMyyyy"); // 143-150 => Data do Desconto
                        linha += Utils.FormatNumber(b.ValorDesconto, 15); // 151-165 => Valor do Desconto
                    }
                    else
                    {
                        linha += "0"; // 142-142 => Código Desconto 1
                        linha += "00000000"; // 143-150 => Data do desconto
                        linha += Utils.Insert("0", 15, "0"); // 151-165 => Valor do Desconto
                    }

                    linha += Utils.Insert("0", 15, "0"); // 166-180 => Valor do IOF
                    linha += Utils.Insert("0", 15, "0"); // 181-195 => Valor do Abatimento
                    linha += Utils.Insert(b.NumeroDocumento, 25, "0", true); // 196-220 => Identificação do Título na Empresa

                    if (b.DiasProtesto.ToInt() > 0)
                    {
                        linha += "1"; // 221-221 => Código para Protesto
                        linha += Utils.Insert(b.DiasProtesto, 2, "0", true); // 222-223 => Prazo para Protesto
                    }
                    else
                    {
                        linha += "3"; // 221-221 => Código para protesto
                        linha += "00"; // 222-223 => Prazo para Protesto
                    }

                    linha += "0"; // 224-224 => Código para Baixa/Devolução
                    linha += Utils.Insert(" ", 3); // 225-227 => Prazo Para Baixa/Devolução
                    linha += "09"; // 228-229 => Código da Moeda
                    linha += Utils.Insert("0", 10, "0"); // 230-239 => Número do Contrato 
                    linha += " "; // 240-240 => Uso Exclusivo Febraban

                    arquivo += linha + Environment.NewLine;
                    sequenciaRegistro++;
                    //Fim Segmento P

                    //Segmento  Q
                    linha = "756"; // 01-03 => Código do Banco
                    linha += "0001"; // 04-07 => Lote de Serviço
                    linha += "3"; // 08-08 => Tipo de Registro
                    linha += Utils.Insert(sequenciaRegistro.ToString(), 5, "0", true); // 09-13 => Número Sequencial do registro
                    linha += "Q"; // 14-14 => Código Segmento do registro
                    linha += " "; // 15-15 => Uso Exclusivo Febraban
                    linha += "01"; // 16-17 => Código de Movimento Remessa
                    linha += b.Sacado.CpfCnpj.ToNoFormated().IsCpf() ? "1" : "2"; // 18-18 => Tipo de Inscrição do Pagador
                    linha += Utils.Insert(b.Sacado.CpfCnpj.ToNoFormated(), 15, "0", true); // 19-33 => Número de Inscrição do Pagador
                    linha += Utils.Insert(b.Sacado.Nome, 40, " "); // 34-73 => Nome do Pagador
                    linha += Utils.Insert(b.Sacado.Endereco + " - " + b.Sacado.NumEndereco, 40, " "); // 74-113 => Endereço do Pagador
                    linha += Utils.Insert(b.Sacado.Bairro, 15, " "); // 114-128 => Bairro do Pagador
                    linha += Utils.Insert(b.Sacado.Cep.Replace(".", "").Replace("-", ""), 8, " "); // 129-136 => CEP do Pagador
                    linha += Utils.Insert(b.Sacado.Cidade, 15, " "); // 137-151 => Cidade do Pagador
                    linha += Utils.Insert(b.Sacado.Estado, 2, " "); // 152-153 => UF do Pagador
                    linha += b.Sacado.CpfCnpj.ToNoFormated().IsCpf() ? "1" : "2"; ; // 154-154 => tipo de Inscrição do Sacador Avalista
                    linha += Utils.Insert(b.Sacado.CpfCnpj.ToNoFormated(), 15, "0", true); // 155-169 => Número de Inscrição do Sacador Avalista
                    linha += Utils.Insert(b.Sacado.Nome, 40, " "); // 170-209 => Nome do Sacador Avalista
                    linha += "000"; // 210-212 => Código Banco Correspondente na Compensação
                    linha += Utils.Insert(" ", 20); // 213-232 => Nosso Número banco Correspondente
                    linha += Utils.Insert(" ", 8); // 233-240 => Uso Exclusivo Febraban

                    arquivo += linha + Environment.NewLine;
                    sequenciaRegistro++;
                    //Fim Segmento Q

                    //Segmento R
                    linha = "756"; // 01-03 => Código do Banco
                    linha += "0001"; // 04-07 => Lote de Serviço
                    linha += "3"; // 08-08 => Tipo de registro
                    linha += Utils.Insert(sequenciaRegistro.ToString(), 5, "0", true); // 09-13 => Número Sequencial do registro
                    linha += "R"; // 14-14 => Código Segmento do Registro
                    linha += " "; // 15-15 => Uso Exclusivo Febraban
                    linha += "01"; // 16-17 => Código Movimento da Remessa
                    linha += "0"; // 18-18 => Código Desconto 2
                    linha += Utils.Insert("0", 8); // 19-26 => Data Desconto 2
                    linha += Utils.Insert("0", 15); // 27-41 => Valor Desconto 2
                    linha += "0"; // 42-42 => Código Desconto 3
                    linha += Utils.Insert("0", 8); // 43-50 => Data Desconto 3
                    linha += Utils.Insert("0", 15); // 51-65 => Valor Desconto 3
                    linha += "1"; // 66-66 => Código da Multa
                    linha += b.DataVencimento.ToString("ddMMyyyy"); // 67-74 => Data da Multa
                    linha += Utils.FormatNumber(b.ValorMultaAtraso, 15, 2); // 75-89 => Valor da Multa
                    linha += Utils.Insert(" ", 10); // 90-99 => Informações do Pagador
                    linha += Utils.Insert(" ", 40); // 100-139 => Informaçõe 3
                    linha += Utils.Insert(" ", 40); // 140-179 => Informação 4
                    linha += Utils.Insert(" ", 20); // 180-199 => Uso Exclusivo Febraban
                    linha += Utils.Insert("0", 8); // 200-207 => Código Ocorrencia do Pagador
                    linha += "000"; // 208-210 => Código Banco Conta de Debito
                    linha += Utils.Insert("0", 5,"0"); // 211-215 => Código da Agencia de Débito
                    linha += " "; // 216-216 => Dígito Verificador da Agencia
                    linha += Utils.Insert("0", 12, "0"); // 217-228 => Conta Corrente para Débito
                    linha += " "; // 229-229 => Dígito Verificador Conta Corrente
                    linha += " "; // 230-230 => Dígito verificador Agencia/Conta
                    linha += "0"; // 231/231 => Aviso para Débito Automatico
                    linha += Utils.Insert(" ", 9); // 232-240 => Uso Exclusivo Febraban

                    arquivo += linha + Environment.NewLine;
                    sequenciaRegistro++;
                    //Fim Segmento R

                    //Segmento S
                    linha = "756"; // 01-03 => Código do Banco
                    linha += "0001"; // 04-07 => Lote de Serviço
                    linha += "3"; // 08-08 => Tipo de registro
                    linha += Utils.Insert(sequenciaRegistro.ToString(), 5, "0", true); // 09-13 => Número Sequencial do registro
                    linha += "S"; // 14-14 => Código Segmento do Registro
                    linha += " "; // 15-15 => Uso Exclusivo Febraban
                    linha += "01"; // 16-17 => Código de Movimento da Remessa
                    linha += "3"; // 18-18 => Identificação da Impressão
                    linha += Utils.Insert(" ", 40); // 19-58 => Mensagem 5
                    linha += Utils.Insert(" ", 40); // 59-98 => Mensagem 6
                    linha += Utils.Insert(" ", 40); // 99-138 => Mensagem 7
                    linha += Utils.Insert(" ", 40); // 139-178 => Mensagem 8
                    linha += Utils.Insert(" ", 40); // 179-218 => Mensagem 9
                    linha += Utils.Insert(" ", 22); // 219-240 => Uso Exclusivo Febraban

                    arquivo += linha + Environment.NewLine;
                    sequenciaRegistro++;
                    //Fim Segmento S
                }

                //Trailler do Lote
                sequenciaRegistro++; // Incremento para Totalizar Registros do Lote

                linha = "756"; // 01-03 => Código do Banco
                linha += "0001"; // 04-07 => Lote de Serviço
                linha += "5"; // 08-08 => Tipo de Registro
                linha += Utils.Insert(" ", 9); // 09-17 => Uso Exclusivo Febraban
                linha += Utils.Insert(sequenciaRegistro.ToString(), 6, "0", true); // 18-23 => Quantidade registros Lote
                linha += Utils.Insert("0", 6, "0"); // 24-29 => Quantidade Titulos em Cobrança Simples
                linha += Utils.Insert("0", 17, "0"); // 30-46 => Valor Total dos Titulos em Cobrança Simples
                linha += Utils.Insert("0", 6, "0"); // 47-52 => Quantidade de Titulos em Cobrança Vinculada
                linha += Utils.Insert("0", 17, "0"); // 53-69 => Valor Total dos Titulos em Cobrança Vinculada
                linha += Utils.Insert("0", 6, "0"); // 70-75 => Quantidade Total dos Titulos em Cobrança Caucionada
                linha += Utils.Insert("0", 17, "0"); // 76-92 => Valor Total dos Titulos em Cobrança Caucionada
                linha += Utils.Insert("0", 6, "0"); // 93-98 => Quantidade dos Titulos em Cobrança Descontada
                linha += Utils.Insert("0", 17, "0"); // 99-115 => Valor Total dos Titulos em Cobrança Descontada
                linha += Utils.Insert(" ", 8, "0"); // 116-123 => Número do Aviso de Lançamento
                linha += Utils.Insert(" ", 117, "0"); // 124-240 => Uso Febraban

                arquivo += linha + Environment.NewLine;
                sequenciaRegistro = sequenciaRegistro + 2;
                //Fim Trailler do Lote

                //Trailler do Arquivo
                linha = "756"; // 01-03 => Código do Banco
                linha += "9999"; // 04-07 => lote do Serviço
                linha += "9"; // 08-08 => Tipo de Registro
                linha += Utils.Insert(" ", 9); // 09-17 => Uso Exclusivo do Febraban
                linha += "000001"; // 18-23 => Quantidade de Lotes do Arquivo
                linha += Utils.Insert(sequenciaRegistro.ToString(), 6, "0", true); // 24-29 => Quantidade de Registros do Arquivo
                linha += Utils.Insert("0", 6, "0"); // 30-35 => Quantidade de Contas
                linha += Utils.Insert(" ", 205); // 36-240 => Uso Exclusivo do Febraban


                arquivo += linha + Environment.NewLine;
                //Fim Trailler do Arquivo

                return arquivo;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao gera a remessa: " + ex.Message);
                return null;
            }

        }

        private const string indiceNossoNumero = "31973197319731973197319731973197";
        private const string indiceCodBarra = "43290876543298765432987654329876543298765432";
        private const string indiceLinhaDigitavel = "2121212120121212121201212121212";
    }
}
