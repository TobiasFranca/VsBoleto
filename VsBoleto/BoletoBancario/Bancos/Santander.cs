using BoletoBancario.Conta;
using BoletoBancario.Utilitarios;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BoletoBancario.Bancos
{
    public class BancoSantander : Banco
    {
        /// <summary>
        /// constantes com Padroes de boleto e arquivos remessa/retorno.
        /// </summary>
        public const string Cnab400 = "Cnab400";

        internal BancoSantander()
        {
            this.nomeBanco = "Santander";
            this.numeroBanco = 33;
            this.dvNumeroBanco = 7;
            this.identificacao = "Santander";

            this.TituloOutros1 = "Código de transmissão"; //Código de transmissão
            this.TituloOutros2 = "Complemento";

            this.LayoutsArquivoRemessa.Add(Cnab400);

            EspecieTitulo.Add("01 - Duplicata");
            EspecieTitulo.Add("02 - Nota Promissória");
            EspecieTitulo.Add("03 - Apólice/Nota de Seguro");
            EspecieTitulo.Add("05 - Recibo");
            EspecieTitulo.Add("06 - Duplicata de Serviço");
            EspecieTitulo.Add("07 - Letra de Câmbio");
        }

        internal override void CalcularDados(Boleto b)
        {
            b.DigVerNossoNumero = DigitoVerificadorNossoNumero(b);
            b.NossoNumeroComDV = b.NossoNumero + b.DigVerNossoNumero;

            //string codBarras = MontarCodigoBarras(b);

            string ld = InserirDigitosVerificadores(b, MontarLinhaDigitavel(b));

            b.LinhaDigitavel = ld;
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

        private static void RetornoCnab400(string texto)
        {
            string[] linhas = texto.Split(new string[1] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in linhas)
            {
                if (s.Length != 400)
                {
                    throw new ArgumentException("Uma ou mais linhas do arquivo está fora do layout.");
                }

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
            {
                if (cont != 4)
                {
                    soma += codBarra[cont].ToString().ToInt() * indiceCodBarra[cont].ToString().ToInt();
                }
            }

            int digito = 11 - (soma % 11);
            if (digito <= 1 || digito > 9)
            {
                digito = 1;
            }

            codBarra = codBarra.Remove(4, 1);
            codBarra = codBarra.Insert(4, digito.ToString()[0].ToString());
            return codBarra;
        }

        private static string MontarLinhaDigitavel(Boleto b)
        {
            string linha = "";

            //Campo 1
            linha += Utils.InserirString("033", 3);
            linha += Utils.InserirString(EnumHelper.GetNumMoeda(b.Moeda), 1);
            linha += Utils.InserirString("9", 1);
            linha += Utils.InserirString(Utils.FormatNumber(b.ContaCorrente.CodigoCedente, 7), 4);
            //Campo 2
            linha += Utils.InserirString(Utils.FormatNumber(b.ContaCorrente.CodigoCedente, 7).Substring(4), 3);
            linha += Utils.InserirString(Utils.FormatNumber(b.NossoNumeroComDV, 13), 7);
            //Campo 3
            linha += Utils.InserirString(Utils.FormatNumber(b.NossoNumeroComDV, 13).Substring(7), 6);
            linha += Utils.InserirString("0", 1);
            linha += Utils.InserirString("101", 3);

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
                {
                    soma += mult.ToString()[0].ToString().ToInt() + mult.ToString()[1].ToString().ToInt();
                }
                else
                {
                    soma += mult;
                }
            }
            digito = Utils.ProximoMultiplo10(soma) - soma;
            linha = linha.Insert(20, digito.ToString()[0].ToString());

            //Terceiro Digito
            soma = 0;
            for (int cont = 21; cont < 31; cont++)
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
            digito = Utils.ProximoMultiplo10(soma) - soma;
            linha = linha.Insert(31, digito.ToString()[0].ToString());
            linha = linha.Insert(32, "0");
            //Montar codigo de barras e verificar ultimo digito

            string codigoBarras = MontarCodigoBarras(b);

            codigoBarras = InserirDVCodigoBarras(codigoBarras);
            b.CodigoBarras = codigoBarras;

            linha = linha.Remove(32, 1);
            linha = linha.Insert(32, codigoBarras[4].ToString());
            b.LinhaDigitavel = linha;
            return linha;
        }

        private static string MontarCodigoBarras(Boleto b)
        {
            string linha = "";

            linha += Utils.InserirString("033", 3);
            linha += Utils.InserirString("9", 1);
            linha += Utils.InserirString(" ", 1);
            linha += Utils.InserirString(GetFatorVencimentoDaData(b.DataVencimento).ToString(), 4);
            linha += Utils.InserirString(Utils.FormatNumber(b.ValorDocumento, 10, 2), 10);
            linha += Utils.InserirString("9", 1);
            linha += Utils.InserirString(Utils.FormatNumber(b.ContaCorrente.CodigoCedente, 7), 7);
            linha += Utils.InserirString(Utils.FormatNumber(b.NossoNumeroComDV, 13), 13);
            linha += Utils.InserirString("0", 1);
            linha += Utils.InserirString("101", 3);

            return linha;
        }

        private static bool ValidarCodigoBarras(string codigoBarras)
        {
            int soma = 0;
            for (int cont = 0; cont < 44; cont++)
            {
                if (cont != 4)
                {
                    soma += codigoBarras[cont].ToString().ToInt() * indiceCodBarra[cont].ToString().ToInt();
                }
            }

            int digito = 11 - (soma % 11);
            if (digito <= 1 || digito > 9)
            {
                digito = 1;
            }

            return codigoBarras[4].ToString().ToInt() == digito;
        }

        private static string DigitoVerificadorNossoNumero(Boleto b)
        {
            string seq = Utils.FormatNumber(b.NossoNumero.ToNoFormated(), 12);

            int cont = 0;
            int soma = 0;
            foreach (char c in seq)
            {
                soma += c.ToString().ToInt() * indiceNossoNumero[cont].ToString().ToInt();
                cont++;
            }
            int digito = 11 - (soma % 11);
            if (digito < 1 || digito > 9)
            {
                digito = 0;
            }

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
            decimal auxValorTotal = 0;

            string aux = "";
            string ret = "";

            ret += Utils.Insert("0", 1); // Código do registro = 0
            ret += Utils.Insert("1", 1); // Código da remessa = 1
            ret += Utils.Insert("REMESSA", 7); //Literal de transmissão = REMESSA
            ret += Utils.Insert("01", 2); // Codigo de serviço = 01
            ret += Utils.Insert("COBRANÇA", 15); // Literal de serviço = COBRAMÇA
            ret += Utils.Insert(c.Outros1, 20); // Código de Transmissão (nota 1)
            ret += Utils.Insert(c.Cedente.NomeCedente, 30); // Nome do cedente
            ret += Utils.Insert("033", 3); // Código do banco = 33
            ret += Utils.Insert("SANTANDER", 15); // nome do banco = SANTANDER
            ret += Utils.Insert(DateTime.Now.ToString("ddMMyy"), 6); // data da gravação
            ret += Utils.Insert(Utils.ReplicarChar("0", 16), 16); //zeros
            ret += Utils.Insert(Utils.ReplicarChar(" ", 275), 275); // brancos
            ret += Utils.Insert("000", 3); //Número da versão da remessa opcional = 000
            ret += Utils.Insert(sequencial++.ToString("000000"), 6); // numero sequencial do registro no arquivo = 000001

            aux += ret + Environment.NewLine;


            foreach (Boleto b in c.Boletos)
            {
                auxValorTotal += b.ValorDocumento;

                int auxCarteira = c.Banco.Carteira.ToNoFormated().ToInt();

                b.CalcularDadosBoleto();

                ret = "1"; // Código do registro = 1
                ret += Utils.Insert(c.Cedente.CpfCnpj.ToNoFormated().IsCpf() ? "01" : "02", 2); // tipo de inscrição do cedente: 01= CPF ou 02= CGC
                ret += Utils.Insert(c.Cedente.CpfCnpj.ToNoFormated(), 14, "0", true); // CGC ou CPF do cedente
                ret += Utils.Insert(c.Outros1, 20); // Código de Transmissão (nota 1)
                //ret += Utils.Insert(b.NumeroDocumento.ToNoFormated(), 25, "0", true);// Numero do controle do participante, para controle por parte do cedente
                ret += Utils.Insert(Utils.FormatNumber(c.CodigoCedente, 8), 25);// Numero controle participante ?????

                ret += Utils.Insert(b.NossoNumeroComDV, 8, "0", true);
                ret += Utils.Insert(" ", 6); // data segunda cobrança ??
                ret += Utils.Insert(" ", 1); //branco
                ret += Utils.Insert("4", 1); //Posição 78 a 78 - sempre igual a 4, sendo obrigatório a informação do percentual na posição 79 a 82.
                ret += Utils.FormatNumber(b.PercentualMultaAtraso, 4, 2);
                ret += Utils.Insert("00", 2); // unidade moeda corrente = 00
                ret += Utils.FormatNumber(0, 13, 2); //Valor do título em outra unidade (consultar banco)
                ret += Utils.Insert(" ", 4); //branco
                ret += Utils.Insert(" ", 6); //data cobrança multa
                ret += Utils.Insert(auxCarteira.ToString(), 1);
                ret += Utils.Insert("01", 2); // codigo ocorrencia
                ret += Utils.Insert(b.NumeroDocumento.ToNoFormated(), 10, "0", true);//seu  numero
                ret += b.DataVencimento.ToString("ddMMyy");
                ret += Utils.FormatNumber(b.ValorDocumento, 13, 2);
                ret += Utils.Insert("033", 3); // Código do banco = 33
                ret += Utils.Insert(auxCarteira == 5 ? c.Agencia.IsNotNullOrEmpty() ? c.Agencia : "00000" : "00000", 5);
                ret += c.EspecieTitulo; // especie documento
                ret += Utils.Insert("N", 1); // uaceite = N
                ret += b.DataDocumento.ToString("ddMMyy");
                ret += Utils.Insert(b.Instrucao1, 2);//1 instrução - (cliente configura)
                ret += Utils.Insert(b.Instrucao2, 2);//2 instrução - (cliente configura)
                ret += Utils.FormatNumber(b.ValorJurosDiaAtraso, 13, 2);//valor juros por dia
                if (b.ValorDesconto > 0)
                {
                    ret += b.DataVencimento.ToString("ddMMyy");
                }
                else
                {
                    ret += "000000";
                    //ret += b.DataLimiteDesconto <= b.DataVencimento ? "000000" : b.DataLimiteDesconto.ToString("ddMMyy");
                }                
                ret += Utils.FormatNumber(b.ValorDesconto, 13, 2);//valor desconto
                ret += Utils.FormatNumber(0, 13, 2); //valor iof para nota de seguro
                ret += Utils.FormatNumber(b.ValorAbatimento, 13, 2);// valor abatimento
                ret += Utils.Insert(b.Sacado.CpfCnpj.ToNoFormated().IsCpf() ? "01" : "02", 2);
                ret += Utils.Insert(b.Sacado.CpfCnpj.ToNoFormated(), 14, "0", true);
                ret += Utils.Insert(b.Sacado.Razao.Trim().Length > 3 ? b.Sacado.Razao : b.Sacado.Nome, 40);
                ret += Utils.Insert(b.Sacado.Endereco, 40);
                ret += Utils.Insert(b.Sacado.Bairro, 12);
                ret += Utils.Insert(b.Sacado.Cep.ToNoFormated(), 8);
                ret += Utils.Insert(b.Sacado.Cidade, 15);
                ret += Utils.Insert(b.Sacado.Estado, 2);
                ret += Utils.Insert(" ", 30); //nome do sacador ou coobrigado
                ret += Utils.Insert(" ", 1); //branco
                ret += Utils.Insert(c.Outros2, 3); //identificador e complemento
                ret += Utils.Insert(" ", 6); //branco
                ret += b.DiasProtesto;
                ret += Utils.Insert(" ", 1); //branco
                ret += Utils.Insert(sequencial++.ToString("000000"), 6);

                aux += ret + Environment.NewLine;
            }

            ret = "9";
            ret += Utils.Insert(sequencial.ToString("000000"), 6);
            ret += Utils.FormatNumber(auxValorTotal, 13, 2);// valor abatimento
            ret += Utils.Insert("", 374, "0");
            ret += Utils.Insert(sequencial++.ToString("000000"), 6);

            aux += ret + Environment.NewLine;

            return aux;
        }

        private const string indiceNossoNumero = "543298765432";
        private const string indiceCodBarra = "43290876543298765432987654329876543298765432";
        private const string indiceLinhaDigitavel = "2121212120121212121201212121212";
    }
}
