using BoletoBancario.Conta;
using BoletoBancario.Utilitarios;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BoletoBancario.Bancos
{
    public class BancoBanestes : Banco
    {
        /// <summary>
        /// constantes com Padroes de boleto e arquivos remessa/retorno.
        /// </summary>
        public const string Cnab400 = "Cnab400";
        public const string BoletoPadrao = "Padrao";
        public const string BoletoPadraoInvertido = "Padrao Invertido";

        internal BancoBanestes()
        {
            this.nomeBanco = "Banestes";
            this.numeroBanco = 021;
            this.dvNumeroBanco = 3;
            this.identificacao = "BANESTES";

            this.HabilitarOutros1 = false;
            this.TituloOutros1 = "";
            this.TituloOutros2 = "Carteira";

            //se precisar tipo de cobrança habilitar o outros1  17/10/2014 - ligação banestes
            this.MascaraCodigoCedente = "0000000000";
            this.MascaraContaCorrente = "000000000-0";
            this.MascaraNossoNumero = "00000000";
            this.MascaraOutros1 = "00";
            this.MascaraOutros2 = "00";

            this.LayoutsArquivoRemessa.Add(Cnab400);
            this.LayoutsArquivoRetorno.Add(Cnab400);
            this.LayoutsBoleto.Add(BoletoPadrao);
            this.LayoutsBoleto.Add(BoletoPadraoInvertido);

            EspecieTitulo.Add("01 - Duplicata");
            EspecieTitulo.Add("02 - Nota promissória");
            EspecieTitulo.Add("03 - Nota de Seguro");
            EspecieTitulo.Add("04 - Cobrança Seriada");
            EspecieTitulo.Add("05 - Recibo");
            EspecieTitulo.Add("10 - Letra de Câmbio");
            EspecieTitulo.Add("11 - Duplicata de Serviço");
            EspecieTitulo.Add("99 - Outros");
        }

        internal override void CalcularDados(Boleto b)
        {
            b.Aceite = "A";

            b.DigVerNossoNumero = DigitoVerificadorNossoNumero(b);
            b.NossoNumeroComDV = b.NossoNumero + b.DigVerNossoNumero;

            string ChaveASBACE = GeraChaveASBACE(b.ContaCorrente.Outros2, b.ContaCorrente.NumeroConta, b.NossoNumero, 4);

            string linhaDigitavel = MontarLinhaDigitavel(b, ChaveASBACE);

            string codBarras = MontarCodigoBarras(b, linhaDigitavel, ChaveASBACE);

            linhaDigitavel = linhaDigitavel.Insert(32, codBarras[4].ToString());

            b.LinhaDigitavel = linhaDigitavel;
            b.CodigoBarras = codBarras;
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
            // Não foi feito
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

        private static string MontarLinhaDigitavel(Boleto b, string ChaveASBACE)
        {
            int dv = 0;

            #region Parte 1

            var parte1 = string.Concat("0219", ChaveASBACE.Substring(0, 5));

            dv = CalculaDigitoVerificador(parte1);

            parte1 = string.Concat(parte1, dv);

            #endregion

            #region Parte 2

            var parte2 = ChaveASBACE.Substring(5, 10);

            dv = CalculaDigitoVerificador(parte2, 1);

            parte2 = string.Concat(parte2, dv);

            #endregion

            #region Parte 3

            var parte3 = ChaveASBACE.Substring(15, 10);

            dv = CalculaDigitoVerificador(parte3, 1);

            parte3 = string.Concat(parte3, dv);

            #endregion

            #region Parte 5

            var parte5 = Utils.InserirString(GetFatorVencimentoDaData(b.DataVencimento).ToString(), 4);
            parte5 += Utils.InserirString(Utils.FormatNumber(b.ValorDocumento, 10, 2), 10);

            #endregion

            string linha = parte1 + parte2 + parte3 + parte5;

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

            string codigoBarras = MontarCodigoBarras(null, null, null);

            codigoBarras = InserirDVCodigoBarras(codigoBarras);
            b.CodigoBarras = codigoBarras;

            linha = linha.Remove(32, 1);
            linha = linha.Insert(32, codigoBarras[4].ToString());
            b.LinhaDigitavel = linha;
            return linha;
        }

        private static string MontarCodigoBarras(Boleto boleto, string linha, string ChaveASBACE)
        {
            try
            {
                //0219DFFFFVVVVVVVVVVCCCCCCCCCCCCCCCCCCCCCCCCC

                var FFFF = GetFatorVencimentoDaData(boleto.DataVencimento);

                var VVVVVVVVVV = Utils.InserirString(Utils.FormatNumber(boleto.ValorDocumento, 10, 2), 10);

                string chave = string.Format("0219{0}{1}{2}", FFFF, VVVVVVVVVV, ChaveASBACE);

                int _dacBoleto = 0;

                int peso = 9;

                int S = 0;
                int P = 0;
                int N = 0;

                for (int i = 0; i < chave.Length; i++)
                {

                    N = Convert.ToInt32(chave.Substring(i, 1));

                    if (i == 0)
                    {
                        peso = 4;
                    }

                    P = N * peso--;

                    S += P;

                    if (peso == 1)
                    {
                        peso = 9;
                    }
                }

                int R = S % 11;

                if (R == 0 || R == 1 || R == 10)
                {
                    _dacBoleto = 1;
                }
                else
                {
                    _dacBoleto = 11 - R;
                }

                return string.Format("0219{0}{1}{2}{3}", _dacBoleto, FFFF, VVVVVVVVVV, ChaveASBACE);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao formatar código de barras.", ex);
            }
        }

        //private static bool ValidarCodigoBarras(string codigoBarras)
        //{
        //    int soma = 0;
        //    for (int cont = 0; cont < 44; cont++)
        //        if (cont != 4)
        //            soma += codigoBarras[cont].ToString().ToInt() * indiceCodBarra[cont].ToString().ToInt();
        //    int digito = 11 - (soma % 11);
        //    if (digito <= 1 || digito > 9)
        //        digito = 1;

        //    return codigoBarras[4].ToString().ToInt() == digito;
        //}

        //private static bool ValidarLinhaDigitavel(string linha)
        //{
        //    //Primeiro Digito
        //    int soma = 0;
        //    for (int cont = 0; cont < 9; cont++)
        //    {
        //        int mult = linha[cont].ToString().ToInt() * indiceLinhaDigitavel[cont].ToString().ToInt();
        //        if (mult >= 10)
        //            soma += mult.ToString()[0].ToString().ToInt() + mult.ToString()[1].ToString().ToInt();
        //        else
        //            soma += mult;
        //    }
        //    int digito = Utils.ProximoMultiplo10(soma) - soma;
        //    if (linha[9].ToString() != digito.ToString())
        //        return false;

        //    //Segundo Digito
        //    soma = 0;
        //    for (int cont = 10; cont < 20; cont++)
        //    {
        //        int mult = linha[cont].ToString().ToInt() * indiceLinhaDigitavel[cont].ToString().ToInt();
        //        if (mult >= 10)
        //            soma += mult.ToString()[0].ToString().ToInt() + mult.ToString()[1].ToString().ToInt();
        //        else
        //            soma += mult;
        //    }
        //    digito = Utils.ProximoMultiplo10(soma) - soma;
        //    if (linha[20].ToString() != digito.ToString())
        //        return false;

        //    //Terceiro Digito
        //    soma = 0;
        //    for (int cont = 21; cont < 31; cont++)
        //    {
        //        int mult = linha[cont].ToString().ToInt() * indiceLinhaDigitavel[cont].ToString().ToInt();
        //        if (mult >= 10)
        //            soma += mult.ToString()[0].ToString().ToInt() + mult.ToString()[1].ToString().ToInt();
        //        else
        //            soma += mult;
        //    }
        //    digito = Utils.ProximoMultiplo10(soma) - soma;
        //    if (linha[31].ToString() != digito.ToString())
        //        return false;
        //    //Montar codigo de barras e verificar ultimo digito

        //    string codigoBarras = MontarCodigoBarras(linha);

        //    return ValidarCodigoBarras(codigoBarras);
        //}

        private static string DigitoVerificadorNossoNumero(Boleto b)
        {
            string seq = Utils.FormatNumber(b.NossoNumero.ToNoFormated(), 8);

            int D1 = CalculaDVNossoNumero(seq);

            int D2 = CalculaDVNossoNumero(string.Concat(seq, D1), 10);

            return string.Concat(D1, D2);
        }

        //############################## CHAVE ASBACE
        private static int CalculaDigitoVerificador(string chave, short peso = 2)
        {
            int D1 = 0;
            int K = 0;
            int S = 0;

            for (int i = 0; i < chave.Length; i++)
            {
                int N = Convert.ToInt32(chave.Substring(i, 1));

                int P = N * peso;

                if (P > 9)
                    K = P - 9;

                if (P < 10)
                    K = P;

                S += K;

                if (peso == 2)
                    peso = 1;
                else
                    peso = 2;
            }

            int resto = S % 10;

            if (resto == 0)
                D1 = 0;
            else if (resto > 0)
                D1 = 10 - resto;

            return D1;
        }

        public static string GeraChaveASBACE(string carteira, string conta, string nossoNumero, int tipoCobranca)
        {
            try
            {
                conta = conta.Replace(".", "").Replace("-", "");

                if (conta.ToInt() < 1)
                    throw new Exception("Conta Corrente inválida");

                if (string.IsNullOrEmpty(carteira))
                    throw new Exception("Carteira não informada");

                string NNNNNNNN = Utils.Insert(nossoNumero, 8, "0", true);
                string CCCCCCCCCCC = Utils.Insert(conta, 11, "0", true);
                string R = tipoCobranca.ToString();

                string NNNNNNNNCCCCCCCCCCCR021 = string.Concat(NNNNNNNN, CCCCCCCCCCC, R, "021");

                int D1 = CalculaD1(NNNNNNNNCCCCCCCCCCCR021);
                int D2 = CalculaD2(NNNNNNNNCCCCCCCCCCCR021, D1);

                return string.Concat(NNNNNNNNCCCCCCCCCCCR021, D1, D2);
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Erro ao tentar gerar a chave ASBACE. {0}", e.Message));
            }
        }

        private static int CalculaD1(string chave)
        {
            int D1 = 0;
            short peso = 2;
            int K = 0;
            int S = 0;

            for (int i = 0; i < chave.Length; i++)
            {
                int N = Convert.ToInt32(chave.Substring(i, 1));

                int P = N * peso;

                if (P > 9)
                    K = P - 9;

                if (P < 10)
                    K = P;

                S += K;

                if (peso == 2)
                    peso = 1;
                else
                    peso = 2;
            }

            int resto = S % 10;

            if (resto == 0)
                D1 = 0;
            else if (resto > 0)
                D1 = 10 - resto;

            return D1;
        }

        private static int CalculaD2(string chave, int D1)
        {
            int D2 = 0;
            short peso = 7;
            int P = 0;
            int S = 0;

            string chaveD1 = string.Concat(chave, D1);

            for (int i = 0; i < chaveD1.Length; i++)
            {
                int N = Convert.ToInt32(chaveD1.Substring(i, 1));

                P = N * peso--;

                S += P;

                if (peso == 1)
                    peso = 7;
            }

            int resto = S % 11;

            if (resto == 0)
                D2 = 0;

            if (resto == 1)
            {
                D1++;
                if (D1 == 10)
                    D1 = 0;
                return CalculaD2(chave, D1);
            }

            if (resto > 1)
                D2 = 11 - resto;

            return D2;
        }

        //################################ FIM CHAVE ASBACE

        private static int GetFatorVencimentoDaData(DateTime dataVencimento)
        {
            return dataVencimento.Subtract(new DateTime(2000, 07, 03)).Days + 1000;
        }

        private static DateTime GetDataDoFatorVencimento(int fatorVencimento)
        {
            return new DateTime(2000, 07, 03)
                .AddDays(fatorVencimento - 1000);
        }

        private static int CalculaDVNossoNumero(string nossoNumero, short peso = 9)
        {
            int S = 0;
            int P = 0;
            int N = 0;
            int d = 0;

            for (int i = 0; i < nossoNumero.Length; i++)
            {
                N = Convert.ToInt32(nossoNumero.Substring(i, 1));

                P = N * peso--;

                S += P;
            }

            int R = S % 11;

            if (R == 0 || R == 1)
                d = 0;

            if (R > 1)
                d = 11 - R;

            return d;
        }

        private static string MontarCnab400(ContaCorrente c)
        {
            int sequencial = 1;

            string aux = "";
            string ret = "";

            ret += Utils.Insert("0", 1);
            ret += Utils.Insert("1", 1);
            ret += Utils.Insert("REMESSA", 7); //REMESSA(cobrança simples) OU CARNET(parcelada)? 
            ret += Utils.Insert("01", 2);
            ret += Utils.Insert("COBRANCA", 15);
            ret += Utils.Insert(Utils.FormatNumber(c.NumeroConta, 11), 11);
            ret += Utils.Insert(Utils.ReplicarChar(" ", 9), 9);
            ret += Utils.Insert(c.Cedente.NomeCedente, 30);
            ret += Utils.Insert(c.Banco.NumeroBanco.ToString(), 3, "0", true);
            ret += Utils.Insert(c.Banco.Identificacao, 8);
            ret += Utils.Insert(Utils.ReplicarChar(" ", 7), 7);
            ret += Utils.Insert(DateTime.Now.ToString("ddMMyy"), 6);
            ret += Utils.Insert(Utils.ReplicarChar(" ", 294), 294);
            ret += Utils.Insert(sequencial++.ToString("000000"), 6);

            aux += ret + Environment.NewLine;


            foreach (Boleto b in c.Boletos)
            {
                b.CalcularDadosBoleto();

                ret = "1";
                ret += Utils.Insert(c.Cedente.CpfCnpj.ToNoFormated().IsCpf() ? "01" : "02", 2);
                ret += Utils.Insert(c.Cedente.CpfCnpj.ToNoFormated(), 14, "0", true);
                ret += Utils.Insert(Utils.FormatNumber(c.NumeroConta, 11), 11);
                ret += Utils.Insert(Utils.ReplicarChar(" ", 9), 9);
                ret += Utils.Insert(b.NumeroDocumento.ToNoFormated(), 25); // identificação da operação na empresa???
                ret += Utils.Insert(b.NossoNumeroComDV, 10, "0", true);
                ret += "1"; //multa: valores em RS = 0 / valores em % = 1 anexo18/p26
                ret += Utils.FormatNumber(b.PercentualMultaAtraso, 9, 2);
                ret += Utils.Insert(" ", 6);//identificação carne   -
                ret += Utils.Insert(b.NumeroParcelas > 1 ? b.NumeroParcelaAtual.ToString() : "00", 2, "0", true);//"00"; // num parcela carne                   - anexo 14,15,16 pág 24
                ret += Utils.Insert(b.NumeroParcelas > 1 ? b.NumeroParcelas.ToString() : "00", 2, "0", true);//"00"; // total parcelas carne                -

                //ret += Utils.Insert(b.Sacado.CpfCnpj.ToNoFormated().IsCpf() ? "1" : "2", 1);//sacador/avalista
                //ret += Utils.Insert(b.Sacado.CpfCnpj.ToNoFormated(), 14, "0", true); //sacador/sta?avali
                //Não tem avalista, deixar zerado?
                ret += Utils.Insert(" ", 15);

                ret += "1"; // 1 = cobrança simples / 3 = cobrança caucionada anexo3/p18
                ret += "01";// identificação anexo4/p18
                ret += Utils.Insert(b.NumeroDocumento.ToNoFormated(), 10);
                ret += b.DataVencimento.ToString("ddMMyy");
                ret += "000";
                ret += Utils.FormatNumber(b.ValorDocumento, 10, 2);
                ret += Utils.Insert(c.Banco.NumeroBanco.ToString(), 3, "0", true);
                ret += "00501"; // Praça de cobrança
                ret += c.EspecieTitulo; //Espécie do titulo
                ret += b.Aceite.ToUpper() == "S" ? "A" : "N";
                ret += b.DataDocumento.ToString("ddMMyy");
                ret += Utils.Insert(b.Instrucao1, 2);//1 instrução - verificar // alterado 19/03 (cliente configura)
                ret += Utils.Insert(b.Instrucao2, 2);//2 instrução - verificar // alterado 19/03 (cliente configura)
                ret += "0"; //tipo juros(mora)- 0 = valor por dia / 1 = porcentagem por mes
                ret += Utils.FormatNumber(b.ValorJurosDiaAtraso, 12, 2);
                if (b.ValorDesconto > 0)
                {
                    ret += b.DataVencimento.ToString("ddMMyy");
                }
                else
                {
                    ret += "000000";
                }                
                ret += Utils.FormatNumber(b.ValorDesconto, 13, 2);
                ret += Utils.ReplicarChar("0", 13); // valor ioc???
                ret += Utils.FormatNumber(b.ValorAbatimento, 13, 2);
                ret += Utils.Insert(b.Sacado.CpfCnpj.ToNoFormated().IsCpf() ? "01" : "02", 2);
                ret += Utils.Insert(b.Sacado.CpfCnpj.ToNoFormated(), 14, "0", true);
                ret += Utils.Insert(b.Sacado.Razao.Trim().Length > 3 ? b.Sacado.Razao : b.Sacado.Nome, 40);
                ret += Utils.Insert(b.Sacado.Endereco, 40);
                ret += Utils.Insert(b.Sacado.Bairro, 12);
                ret += Utils.Insert(b.Sacado.Cep.ToNoFormated(), 8);
                ret += Utils.Insert(b.Sacado.Cidade, 15);
                ret += Utils.Insert(b.Sacado.Estado, 2);
                ret += Utils.ReplicarChar(" ", 40); // instrução
                ret += "00"; //brnaco
                ret += "0"; //codigo moeda
                ret += Utils.Insert(sequencial++.ToString("000000"), 6);

                aux += ret + Environment.NewLine;
            }

            ret = "9";
            ret += Utils.Insert(" ", 393);
            ret += Utils.Insert(sequencial++.ToString("000000"), 6);

            aux += ret + Environment.NewLine;

            return aux;
        }

        private const string indiceNossoNumero = "98765432";
        private const string indiceCodBarra = "43290876543298765432987654329876543298765432";
        private const string indiceLinhaDigitavel = "2121212120121212121201212121212";
    }
}
