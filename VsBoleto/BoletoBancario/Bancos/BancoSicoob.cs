using BoletoBancario.Conta;
using BoletoBancario.Utilitarios;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BoletoBancario.Bancos
{
    public class BancoSicoob : Banco
    {
        public const string Cnab400 = "Cnab400";
        public const string BoletoPadrao = "Padrao";
        public const string BoletoPadraoInvertido = "Padrao Invertido";


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
            LayoutsArquivoRetorno.Add(Cnab400);
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
                switch (tipoRemessa)
                {
                    case Cnab400:
                        remessa = MontarCnab400(c);
                        break;
                    default: return false;
                }

                if (!remessa.IsNotNullOrEmpty())
                    return false;

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

        private static string MontarCnab400(ContaCorrente c)
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
                ret += b.DataLimiteDesconto <= b.DataVencimento ? "000000" : b.DataLimiteDesconto.ToString("ddMMyy");//174-179
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

        private const string indiceNossoNumero = "31973197319731973197319731973197";
        private const string indiceCodBarra = "43290876543298765432987654329876543298765432";
        private const string indiceLinhaDigitavel = "2121212120121212121201212121212";
    }
}
