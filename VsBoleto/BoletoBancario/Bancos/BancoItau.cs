using BoletoBancario.Conta;
using BoletoBancario.Utilitarios;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BoletoBancario.Bancos
{
    public class BancoItau : Banco
    {
        /// <summary>
        /// constantes com Padroes de boleto e arquivos remessa/retorno.
        /// </summary>
        public const string Cnab400 = "Cnab400";
        public const string BoletoPadrao = "Padrao";
        public const string BoletoPadraoInvertido = "Padrao Invertido";

        internal BancoItau()
        {
            this.nomeBanco = "Banco Itaú S.A.";
            this.numeroBanco = 341;
            this.dvNumeroBanco = 7;
            this.identificacao = "BANCO ITAU SA";

            this.TituloOutros1 = "Código Flash";
            this.MascaraOutros1 = "AAA";
            this.HabilitarOutros2 = false;

            this.MascaraCodigoCedente = "00000";
            this.MascaraContaCorrente = "00000-0";
            this.MascaraNossoNumero = "00000000";
            this.MascaraAgencia = "0000";

            this.LayoutsArquivoRemessa.Add(Cnab400);
            this.LayoutsArquivoRetorno.Add(Cnab400);
            this.LayoutsBoleto.Add(BoletoPadrao);
            this.LayoutsBoleto.Add(BoletoPadraoInvertido);

            EspecieTitulo.Add("01 - Duplicata Mercantil");
            EspecieTitulo.Add("02 - Nota Promissória");
            EspecieTitulo.Add("03 - Nota de Seguro");
            EspecieTitulo.Add("04 - Mensalidade Escolar");
            EspecieTitulo.Add("05 - Recibo");
            EspecieTitulo.Add("06 - Contrato");
            EspecieTitulo.Add("07 - Cosseguros");
            EspecieTitulo.Add("08 - Duplicata de Serviço");
            EspecieTitulo.Add("09 - Letra de Câmbio");
            EspecieTitulo.Add("13 - Nota de Débitos");
            EspecieTitulo.Add("15 - Documento em Dívida");
            EspecieTitulo.Add("16 - Encargos Condominais");
            EspecieTitulo.Add("17 - Conta de Prestação de Serviços");
            EspecieTitulo.Add("18 - Boleto de Proposta");
            EspecieTitulo.Add("99 - Diversos");
        }

        #region herdados de BANCO

        internal override void CalcularDados(Boleto b)
        {
            b.DigVerNossoNumero = DigitoVerificadorNossoNumero(b); // pronto itau
            b.NossoNumeroComDV = b.NossoNumero + b.DigVerNossoNumero; // pronto itau


            b.CodigoBarras = MontarCodigoBarras(b);
            b.LinhaDigitavel = InserirDigitosVerificadores(MontarLinhaDigitavel(b));
        }

        internal override bool MontarArquivoRemessa(string tipoRemessa, ContaCorrente c, string path, ref string nome)
        {
            try
            {
                string remessa = "";
                switch (tipoRemessa)
                {
                    case Cnab400:
                        remessa = MontarCnab400(c);
                        break;
                    default: return false;
                }

                if (!remessa.IsNotNullOrEmpty())
                    return false;

                //Enconding 1252 = ANSI
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

        #endregion

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
            if (indiceCodBarra.Length != codBarra.Length)
                throw new ArgumentException("Falha ao calcular o Código de Barras. Tente novamente."
                    + "Se o problema persistir, contate o suporte.");
            int soma = 0;
            for (int cont = 0; cont < indiceCodBarra.Length; cont++)
                soma += codBarra[cont].ToString().ToInt() * indiceCodBarra[cont].ToString().ToInt();
            int digito = 11 - (soma % 11);
            if (digito <= 1 || digito > 9)
                digito = 1;

            codBarra = codBarra.Insert(4, digito.ToString()[0].ToString());
            return codBarra;
        }

        private static string MontarLinhaDigitavel(Boleto b)
        {
            string linha = "";

            //Campo 1
            linha += Utils.InserirString(b.ContaCorrente.Banco.NumeroBanco, 3);
            linha += Utils.InserirString(EnumHelper.GetNumMoeda(b.Moeda), 1);
            linha += Utils.InserirString(b.ContaCorrente.Banco.Carteira, 3);
            linha += Utils.InserirString(Utils.Insert(b.NossoNumero, 8, "0", true).Substring(0, 2), 2);

            //Campo 2
            linha += Utils.InserirString(Utils.Insert(b.NossoNumero, 8, "0", true).Substring(2, 6), 6);
            linha += Utils.InserirString(b.DigVerNossoNumero, 1);
            linha += Utils.InserirString(b.ContaCorrente.Agencia.Substring(0, 3), 3);
            //Campo 3
            linha += Utils.InserirString(b.ContaCorrente.Agencia.Substring(2, 1), 1);
            linha += Utils.Insert(b.ContaCorrente.NumeroConta.ToNoFormated(), 6, "0", true);
            linha += "000"; // inutilizado

            //Campo 4 == somente 1 em branco para DV do código de barras
            linha += Utils.InserirString(b.CodigoBarras.Substring(4, 1), 1);

            //Campo 5
            linha += Utils.InserirString(GetFatorVencimentoDaData(b.DataVencimento).ToString(), 4);
            linha += Utils.InserirString(Utils.FormatNumber(b.ValorDocumento, 10, 2), 10);

            return linha;
        }

        private static string InserirDigitosVerificadores(string linha)
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
            int digito = 10 - (soma % 10);
            digito = digito >= 10 ? 0 : digito;
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
            digito = 10 - (soma % 10);
            digito = digito >= 10 ? 0 : digito;
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
            digito = 10 - (soma % 10);
            digito = digito >= 10 ? 0 : digito;
            linha = linha.Insert(31, digito.ToString()[0].ToString());
            return linha;
        }

        private static string MontarCodigoBarras(Boleto b)
        {

            string codigoBarras = "";

            //Campo 1
            codigoBarras += Utils.Insert(b.ContaCorrente.Banco.NumeroBanco.ToString(), 3, "0", true);
            codigoBarras += Utils.Insert(EnumHelper.GetNumMoeda(b.Moeda).ToString(), 1);
            //codigoBarras += "0"; // Onde será inserido o DV do cód de barras; (1)
            codigoBarras += Utils.Insert(GetFatorVencimentoDaData(b.DataVencimento).ToString(), 4, "0", true);
            codigoBarras += Utils.Insert(Utils.FormatNumber(b.ValorDocumento, 10, 2), 10, "0", true);
            codigoBarras += Utils.Insert(Utils.FormatNumber(b.ContaCorrente.Banco.Carteira, 3), 3, "0", true);
            codigoBarras += Utils.Insert(Utils.FormatNumber(b.NossoNumeroComDV, 9), 9, "0", true);
            codigoBarras += Utils.Insert(Utils.FormatNumber(b.ContaCorrente.Agencia, 4), 4, "0", true);
            codigoBarras += Utils.Insert(Utils.FormatNumber(b.ContaCorrente.NumeroConta.ToNoFormated(), 6), 6, "0", true);
            codigoBarras += "000";

            codigoBarras = InserirDVCodigoBarras(codigoBarras);

            return codigoBarras;
        }

        private static string DigitoVerificadorNossoNumero(Boleto b)
        {
            string con = b.ContaCorrente.NumeroConta.ToNoFormated();
            string seq = Utils.FormatNumber(b.ContaCorrente.Agencia.ToNoFormated(), 4) +
                            Utils.FormatNumber(con.Remove(con.Length - 1), 5) +
                            Utils.FormatNumber(b.ContaCorrente.Banco.Carteira, 3) +
                            Utils.FormatNumber(b.NossoNumero.ToNoFormated(), 8);

            int cont = 0;
            int soma = 0;
            foreach (char c in seq)
            {
                int aux = c.ToString().ToInt() * indiceNossoNumero[cont].ToString().ToInt();
                if (aux >= 10)
                    soma += aux.ToString()[0].ToString().ToInt() + aux.ToString()[1].ToString().ToInt();
                else
                    soma += aux;
                cont++;
            }
            int digito = 10 - (soma % 10);
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
            return new DateTime(2000, 07, 03)
                .AddDays(fatorVencimento - 1000);
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

            // *** DZB Adicionado Agencia
            ret += Utils.Insert(c.Agencia.ToNoFormated(), 4);
            //ret += Utils.Insert(Utils.ReplicarChar(" ", 4), 4);

            ret += Utils.Insert("00", 2);
            ret += Utils.Insert(c.NumeroConta.ToNoFormated(), 6, "0", true);
            ret += Utils.Insert(Utils.ReplicarChar(" ", 8), 8);
            ret += Utils.Insert(c.Cedente.NomeCedente, 30);
            ret += Utils.Insert(c.Banco.NumeroBanco.ToString(), 3);
            ret += Utils.Insert(c.Banco.Identificacao, 15);
            ret += Utils.Insert(DateTime.Now.ToString("ddMMyy"), 6);
            ret += Utils.Insert(Utils.ReplicarChar(" ", 294), 294);
            ret += Utils.Insert(sequencial++.ToString("000000"), 6);

            aux += ret + Environment.NewLine;

            foreach (Boleto b in c.Boletos)
            {
                b.CalcularDadosBoleto();

                ret = "1";
                ret += Utils.Insert(c.Cedente.CpfCnpj.ToNoFormated().IsCpf() ? "01" : "02", 2);
                ret += Utils.Insert(c.Cedente.CpfCnpj.ToNoFormated(), 14);
                ret += Utils.Insert(c.Agencia.ToNoFormated(), 4);
                ret += Utils.Insert("00", 2);
                ret += Utils.Insert(c.NumeroConta.ToNoFormated(), 6, "0", true);
                ret += Utils.Insert(" ", 4);
                ret += Utils.Insert(" ", 4); // NOTA 27
                ret += Utils.Insert(b.NumeroDocumento.ToNoFormated(), 25, "0", true); // NOTA 2 ???
                ret += Utils.Insert(b.NossoNumero, 8, "0", true); // NOTA 3
                ret += Utils.Insert("", 13, "0", true); // NOTA 4
                ret += Utils.Insert(c.Banco.Carteira.ToNoFormated(), 3, "0", true); // NOTA 5
                ret += Utils.Insert("", 21, " ", true); // IDENTIFICAÇÃO DA OPERAÇÃO NO BANCO - uso do banco
                string codCarteira = b.ContaCorrente.Banco.Carteira;
                codCarteira = codCarteira.Equals("150") ? "U" : codCarteira.Equals("191") ? "1" : codCarteira.Equals("147") ? "E" : "I";
                ret += Utils.Insert(codCarteira, 1); // NOTA 5
                ret += Utils.Insert("01", 2); // NOTA 6 - OCORRENCIA
                ret += Utils.Insert(b.NumeroDocumento.ToNoFormated(), 10, "0", true); // NOTA 18 ???
                ret += b.DataVencimento.ToString("ddMMyy"); // NOTA 7
                ret += Utils.FormatNumber(b.ValorDocumento, 13, 2); // NOTA 8
                ret += Utils.Insert(c.Banco.NumeroBanco.ToString(), 3);
                ret += Utils.Insert("00000", 5); // NOTA 9 ??
                ret += c.EspecieTitulo; // NOTA 10 - ESPECIE
                ret += b.Aceite.ToUpper() == "S" ? "A" : "N";
                ret += b.DataDocumento.ToString("ddMMyy");
                ret += Utils.Insert(b.Instrucao1, 2);//1 instrução - verificar // alterado 19/03 (cliente configura)
                ret += Utils.Insert(b.Instrucao2, 2);//2 instrução - verificar // alterado 19/03 (cliente configura)
                ret += Utils.FormatNumber(b.ValorJurosDiaAtraso, 13, 2); // NOTA 12
                ret += b.DataLimiteDesconto <= b.DataVencimento ? "000000" : b.DataLimiteDesconto.ToString("ddMMyy");
                ret += Utils.FormatNumber(b.ValorDesconto, 13, 2); // NOTA 13
                ret += Utils.FormatNumber(b.ValorJurosDiaAtraso, 13, 2); // NOTA 14 ??? IOF
                ret += Utils.FormatNumber(b.ValorAbatimento, 13, 2); // NOTA 13
                ret += Utils.Insert(b.Sacado.CpfCnpj.ToNoFormated().IsCpf() ? "01" : "02", 2);
                ret += Utils.Insert(b.Sacado.CpfCnpj.ToNoFormated(), 14, "0", true);
                ret += Utils.Insert(b.Sacado.Razao.Trim().Length > 3 ? b.Sacado.Razao : b.Sacado.Nome, 30); // NOTA 15
                ret += Utils.Insert(" ", 10); //NOTA 15
                ret += Utils.Insert(b.Sacado.Endereco, 40);
                ret += Utils.Insert(b.Sacado.Bairro, 12);
                ret += Utils.Insert(b.Sacado.Cep.ToNoFormated(), 8);
                ret += Utils.Insert(b.Sacado.Cidade, 15);
                ret += Utils.Insert(b.Sacado.Estado, 2);
                ret += Utils.Insert(b.Sacado.Razao.Trim().Length > 3 ? b.Sacado.Razao : b.Sacado.Nome, 30); // NOTA 16 -- SACADOR/AVALISTA
                ret += Utils.Insert(" ", 4);
                ret += b.DataLimiteDesconto.ToString("ddMMyy"); // NOTA 11 A
                ret += "00";//NOTA 11 A
                ret += " ";
                ret += Utils.Insert(sequencial++.ToString("000000"), 6);


                aux += ret + Environment.NewLine;
            }

            ret = "9";
            ret += Utils.Insert(" ", 393);
            ret += Utils.Insert(sequencial++.ToString("000000"), 6);

            aux += ret + Environment.NewLine;

            return aux;
        }

        private const string indiceNossoNumero = "12121212121212121212";
        private const string indiceCodBarra = "4329876543298765432987654329876543298765432";
        private const string indiceLinhaDigitavel = "2121212120121212121201212121212";
    }
}
