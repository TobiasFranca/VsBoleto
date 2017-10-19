using BoletoBancario.Conta;
using BoletoBancario.Utilitarios;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BoletoBancario.Bancos
{
    public class BancoCaixaSR : Banco
    {
        /// <summary>
        /// constantes com Padroes de boleto e arquivos remessa/retorno.
        /// </summary>
        public const string Cnab400 = "Cnab400";
        public const string BoletoPadrao = "Padrao";
        public const string BoletoPadraoInvertido = "Padrao Invertido";

        //O Nosso Número no SIGCB é composto de 17 posições, sendo as 02 posições iniciais para identificar a Carteira e as 15 posições restantes são para livre utilização pelo Beneficiário.
        //14 registrada (RG) / 24 sem registro (SR)
        private static string pathConfigBanco = AppDomain.CurrentDomain.BaseDirectory + "\\ConfigBoletosBancos.ini";

        internal BancoCaixaSR()
        {
            nomeBanco = "Caixa Econômica Federal";
            numeroBanco = 104;
            dvNumeroBanco = 0;
            identificacao = "CEF";

            //this.carteira = "SR";

            //Sistema Antigo: Padrão SINCO
            HabilitarOutros1 = true;
            TituloOutros1 = "Sistema Antigo";
            MascaraOutros1 = "S";

            HabilitarOutros2 = false;
            TituloOutros2 = "";
            MascaraOutros2 = "";

            HabilitarCodigoCedente = true;

            MascaraCodigoCedente = "0000.000.00000000-0";
            MascaraContaCorrente = "000000000-0";
            MascaraNossoNumero = "00000000";

            LayoutsArquivoRemessa.Add(Cnab400);
            LayoutsArquivoRetorno.Add(Cnab400);
            LayoutsBoleto.Add(BoletoPadrao);
            LayoutsBoleto.Add(BoletoPadraoInvertido);

            EspecieTitulo.Add("01 - Cheque");
            EspecieTitulo.Add("02 - Duplicata Mercantil");
            EspecieTitulo.Add("03 - Duplicata Mercantil p/Indicação");
            EspecieTitulo.Add("04 - Duplicata de Serviço");
            EspecieTitulo.Add("05 - Duplicata de Serviço p/Indicação");
            EspecieTitulo.Add("06 - Duplicata Rural");
            EspecieTitulo.Add("07 - Letra de Câmbio");
            EspecieTitulo.Add("08 - Nota de Crédito Comercial");
            EspecieTitulo.Add("09 - Nota de Crédito à Exportação");
            EspecieTitulo.Add("10 - Nota de Crédito Industrial");
            EspecieTitulo.Add("11 - Nota de Crédito Rural");
            EspecieTitulo.Add("12 - Nota Promissória");
            EspecieTitulo.Add("13 - Nota Promissória Rural");
            EspecieTitulo.Add("14 - Triplicata Mercantil");
            EspecieTitulo.Add("15 - Triplicata de Serviço");
            EspecieTitulo.Add("16 - Nota de Seguro");
            EspecieTitulo.Add("17 - Recibo");
            EspecieTitulo.Add("18 - Fatura");
            EspecieTitulo.Add("19 - Nota de Débito");
            EspecieTitulo.Add("20 - Apólice de Seguro");
            EspecieTitulo.Add("21 - Mensalidade Escolar");
            EspecieTitulo.Add("22 - Parcela de Consórcio");
            EspecieTitulo.Add("23 - Nota Fiscal");
            EspecieTitulo.Add("24 - Documento de Dívida");
            EspecieTitulo.Add("25 - Cédula de Produto Rural");
            EspecieTitulo.Add("31 - Cartão de Crédito");
            EspecieTitulo.Add("32 - Boleto de Proposta");
            EspecieTitulo.Add("99 - Outros");
        }

        internal override void CalcularDados(Boleto b)
        {
            //usar Outros2 para DV Cod Beneficiario

            //  if (!carteira.ToUpper().Equals("RG") && !carteira.ToUpper().Equals("SR"))
            //     throw new ArgumentException("O valor do campo 'carteira' é diferente de 'RG' ou 'SR'.\nQualquer dúvida entre em contao com o suporte.");

            b.EspecieBoleto = "DM";

            b.ContaCorrente.Outros2 = DigitoVerificadorCodBeneficiario(b.ContaCorrente.CodigoCedente);

            b.NossoNumeroComDV = DigitoVerificadorNossoNumero(b);
            //b.DigVerNossoNumero = DigitoVerificadorNossoNumero(b);
            //b.NossoNumeroComDV = b.NossoNumero + b.DigVerNossoNumero;

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
                throw new ArgumentException("Falha ao calcular o DV campo livre. " + codBarra +
                    " (" + codBarra.Length + " dígitos), esperado 24." +
                    "\nTente novamente. Se o problema persistir, contate o suporte.");

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
            linha += Utils.InserirString(b.CodigoBarras.Substring(0, 3), 3);
            linha += Utils.InserirString(b.CodigoBarras.Substring(3, 1), 1);
            linha += Utils.InserirString(b.CodigoBarras.Substring(19, 5), 5);//campo livre (5)
            //Campo 2
            linha += Utils.InserirString(b.CodigoBarras.Substring(24, 10), 10);//campo livre (10)
            //Campo 3
            linha += Utils.InserirString(b.CodigoBarras.Substring(34, 10), 10);

            //Campo 4 == somente 1 em branco para DV do código de barras
            linha += Utils.InserirString(b.CodigoBarras.Substring(4, 1), 1);

            //Campo 5
            linha += Utils.InserirString(b.CodigoBarras.Substring(5, 4), 4);
            linha += Utils.InserirString(b.CodigoBarras.Substring(9, 10), 10);
            //linha += Utils.InserirString(GetFatorVencimentoDaData(b.DataVencimento).ToString(), 4); //Mesma coisa acima
            //linha += Utils.InserirString(Utils.FormatNumber(b.ValorDocumento, 10, 2), 10);          // Pegando do codbarra

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
            codigoBarras += Utils.InserirString(b.ContaCorrente.Banco.NumeroBanco.ToString(), 3);
            codigoBarras += Utils.InserirString(EnumHelper.GetNumMoeda(b.Moeda), 1);
            //codigoBarras += "0"; // Onde será inserido o DV do cód de barras; (1)
            codigoBarras += Utils.InserirString(GetFatorVencimentoDaData(b.DataVencimento).ToString(), 4);
            codigoBarras += Utils.InserirString(Utils.FormatNumber(b.ValorDocumento, 10, 2), 10);

            //Campo Livre
            string campoLivre = "";
            campoLivre += Utils.InserirString(Utils.FormatNumber(b.ContaCorrente.CodigoCedente.ToNoFormated(), 6), 6);
            campoLivre += Utils.InserirString(Utils.FormatNumber(DvCodigoBeneficiario(b), 1), 1); //dv cod cedente

            string carteira = b.ContaCorrente.Banco.Carteira.ToUpper();
            string _aux = carteira.Equals("RG") ? "14" : "24";
            string seqNN = _aux + b.NossoNumero.PadLeft(15, '0');
            string sequenciaSIGCB = seqNN.Substring(2, 3) +
                                    seqNN.Substring(0, 1) +
                                    seqNN.Substring(5, 3) +
                                    seqNN.Substring(1, 1) +
                                    seqNN.Substring(8, 9);

            campoLivre += sequenciaSIGCB;
            campoLivre += DvCampoLivre(campoLivre);

            codigoBarras += campoLivre;

            codigoBarras = InserirDVCodigoBarras(codigoBarras);

            return codigoBarras;
        }

        private string DigitoVerificadorCodBeneficiario(string codCendente)
        {
            string indice = "765432";
            string seq = codCendente.PadLeft(6, '0');

            if (indice.Length != seq.Length)
                throw new ArgumentException("Falha ao calcular o DV campo livre. " + seq +
                    " (" + seq.Length + " dígitos), esperado 6." +
                    "\nTente novamente. Se o problema persistir, contate o suporte.");

            int cont = 0;
            int soma = 0;
            foreach (char c in seq)
            {
                soma += c.ToString().ToInt() * indice[cont].ToString().ToInt();
                cont++;
            }
            int digito = 11 - (soma % 11);
            if (digito > 9)
                digito = 0;

            return digito.ToString();
        }

        private static string DvCampoLivre(string campoLivre)
        {
            string indice = "987654329876543298765432";
            string seq = campoLivre;

            if (indice.Length != seq.Length)
                throw new ArgumentException("Falha ao calcular o DV campo livre. " + seq +
                    " (" + seq.Length + " dígitos), esperado 24." +
                    "\nTente novamente. Se o problema persistir, contate o suporte.");

            int cont = 0;
            int soma = 0;
            foreach (char c in seq)
            {
                soma += c.ToString().ToInt() * indice[cont].ToString().ToInt();
                cont++;
            }
            int digito = 11 - (soma % 11);
            if (digito > 9)
                digito = 0;

            return digito.ToString();
        }

        private static string DvCodigoBeneficiario(Boleto b)
        {
            string indice = "765432";
            string seq = b.ContaCorrente.CodigoCedente.ToNoFormated();

            if (indice.Length != seq.Length)
                throw new ArgumentException("Falha ao calcular o DV Código Beneficiário. " + seq +
                    " (" + seq.Length + " dígitos), esperado 6." +
                    "\nTente novamente. Se o problema persistir, contate o suporte.");

            int cont = 0;
            int soma = 0;
            foreach (char c in seq)
            {
                soma += c.ToString().ToInt() * indice[cont].ToString().ToInt();
                cont++;
            }
            int digito = 11 - (soma % 11);
            if (digito > 9)
                digito = 0;

            return digito.ToString();
        }

        private static string DigitoVerificadorNossoNumero(Boleto b)
        {
            string carteira = b.ContaCorrente.Banco.Carteira.ToUpper();
            string _aux = carteira.Equals("RG") ? "14" : "24";
            string seq = _aux + b.NossoNumero.PadLeft(15, '0');

            if (indiceNossoNumero.Length != seq.Length)
                throw new ArgumentException("Falha ao calcular o DV Nosso Número. " + seq +
                    " (" + seq.Length + " dígitos), esperado 17." +
                    "\nTente novamente. Se o problema persistir, contate o suporte.");

            int cont = 0;
            int soma = 0;
            foreach (char c in seq)
            {
                soma += c.ToString().ToInt() * indiceNossoNumero[cont].ToString().ToInt();
                cont++;
            }
            int digito = 11 - (soma % 11);
            if (digito > 9)
                digito = 0;

            b.DigVerNossoNumero = digito.ToString();

            return seq + digito.ToString();
            //return digito.ToString();
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
            ret += Utils.Insert("REMESSA", 7); // para homologação: REM.TST //REMESSA
            ret += Utils.Insert("01", 2);
            ret += Utils.Insert("COBRANCA", 15);
            ret += Utils.Insert(c.Agencia.ToNoFormated(), 4);
            ret += Utils.Insert(c.CodigoCedente.ToNoFormated(), 6);
            ret += Utils.Insert("", 10);
            ret += Utils.Insert(c.Cedente.NomeCedente, 30);
            ret += Utils.Insert("104", 3);
            ret += Utils.Insert("C ECON FEDERAL", 15);
            ret += Utils.Insert(DateTime.Now.ToString("ddMMyy"), 6);
            ret += Utils.Insert("", 289);

            /*
             * Número Seqüencial do Arquivo Remessa / Retorno
                Número seqüencial adotado e controlado pelo responsável pela geração do arquivo para ordenar os
                arquivos encaminhados.
                Seqüencial a partir de ‘00001’
             */
            int seqRemessa = ArquivoINI.LeString(pathConfigBanco, "CAIXA", "SEQ_REMESSA").ToInt32();
            ret += Utils.Insert(seqRemessa.ToString(), 5, "0", true);
            seqRemessa++;
            ArquivoINI.EscreveString(pathConfigBanco, "CAIXA", "SEQ_REMESSA", seqRemessa.ToString());


            ret += Utils.Insert(sequencial++.ToString("000000"), 6); //seq B

            aux += ret + Environment.NewLine;


            foreach (Boleto b in c.Boletos)
            {
                b.CalcularDadosBoleto();

                ret = "1";
                ret += Utils.Insert(c.Cedente.CpfCnpj.ToNoFormated().IsCpf() ? "01" : "02", 2);
                ret += Utils.Insert(c.Cedente.CpfCnpj.ToNoFormated(), 14);
                ret += Utils.Insert(c.Agencia.ToNoFormated(), 4);
                ret += Utils.Insert(c.CodigoCedente.ToNoFormated(), 6);
                ret += Utils.Insert("2", 1); //Identificação da Emissão do Bloqueto NE027
                ret += Utils.Insert("0", 1); //Identificação da Entrega / Distribuição do Bloqueto NE028
                ret += Utils.Insert("00", 2); //Código do Tipo da Taxa de Permanência NE013
                ret += Utils.Insert(b.NumeroDocumento.ToNoFormated(), 25, "0", true);

                string carteira = b.ContaCorrente.Banco.Carteira.ToUpper();
                string _aux = carteira.Equals("RG") ? "14" : "24";
                ret += Utils.Insert(_aux, 2, "0", true);
                ret += Utils.Insert(b.NossoNumero, 15, "0", true);

                ret += Utils.Insert(" ", 3);
                ret += Utils.Insert(" ", 30); //Mensagem a ser impressa no bloqueto.
                ret += Utils.Insert(_aux[0].ToString(), 2, "0", true); //Codigo da carteira 01=Registrada / 02=Sem Registro
                ret += Utils.Insert("01", 2); //identificação tipo de ocorrencia
                ret += Utils.Insert(b.NumeroDocumento.ToNoFormated(), 10, "0", true); //seu numero ????
                ret += b.DataVencimento.ToString("ddMMyy");
                ret += Utils.FormatNumber(b.ValorDocumento, 13, 2);
                ret += Utils.Insert("104", 3);
                ret += Utils.Insert("00000", 5);
                ret += c.EspecieTitulo; //Especie do titulo 01=DuplicataMercantil
                ret += b.Aceite.ToUpper() == "S" ? "A" : "N";
                ret += b.DataDocumento.ToString("ddMMyy");
                ret += Utils.Insert(b.Instrucao1, 2);//1 instrução - verificar // alterado 19/03 (cliente configura)
                ret += Utils.Insert(b.Instrucao2, 2);//2 instrução - verificar // alterado 19/03 (cliente configura)
                ret += Utils.FormatNumber(b.ValorJurosDiaAtraso, 13, 2);
                if (b.ValorDesconto > 0)
                {
                    ret += b.DataVencimento.ToString("ddMMyy");
                }
                else
                {
                    ret += "000000";
                }                
                ret += Utils.FormatNumber(b.ValorDesconto, 13, 2);
                ret += Utils.FormatNumber("", 13, 2); //iof
                ret += Utils.FormatNumber(b.ValorAbatimento, 13, 2); //abatimento
                ret += Utils.Insert(b.Sacado.CpfCnpj.ToNoFormated().IsCpf() ? "01" : "02", 2);
                ret += Utils.Insert(b.Sacado.CpfCnpj.ToNoFormated(), 14, "0", true);
                ret += Utils.Insert(b.Sacado.Razao.Trim().Length > 3 ? b.Sacado.Razao : b.Sacado.Nome, 40);
                ret += Utils.Insert(b.Sacado.Endereco, 40);
                ret += Utils.Insert(b.Sacado.Bairro, 12);
                ret += Utils.Insert(b.Sacado.Cep.ToNoFormated(), 8);
                ret += Utils.Insert(b.Sacado.Cidade, 15);
                ret += Utils.Insert(b.Sacado.Estado, 2);
                ret += b.DataVencimento.AddDays(1).ToString("ddMMyy");
                ret += Utils.FormatNumber(b.ValorMultaAtraso, 10, 2);
                ret += Utils.Insert(b.Sacado.Razao.Trim().Length > 3 ? b.Sacado.Razao : b.Sacado.Nome, 22); //SACADOR/AVALISTA
                ret += Utils.Insert("00", 2);//3 instrução - verificar // 
                ret += Utils.Insert(b.DiasProtesto.ToString(), 2);//protesto/devolução (dias)
                ret += Utils.Insert("1", 1);
                ret += Utils.Insert(sequencial++.ToString("000000"), 6);

                //ret += Utils.Insert(c.Banco.Carteira.ToNoFormated().Substring(4), 1); //dv
                //string cC = c.NumeroConta.ToNoFormated();
                //ret += Utils.Insert(cC.Remove(cC.Length - 1), 8, "0", true);
                //ret += Utils.Insert(cC.Substring(cC.Length - 1), 1);
                //ret += Utils.Insert("", 6, "0");
                //ret += Utils.Insert(" ", 25);
                //ret += Utils.Insert(b.NossoNumeroComDV, 12, "0", true);
                //ret += b.NumeroParcelaAtual.ToString("00");
                //ret += "00";
                //ret += Utils.Insert(" ", 3);
                //ret += " ";
                //ret += Utils.Insert(" ", 3);
                //ret += "000";
                //ret += "0";
                //ret += Utils.Insert("", 5, "0");
                //ret += "0";
                //ret += Utils.Insert("", 6, "0");
                //ret += Utils.Insert(" ", 5);
                //ret += c.Outros1; // modalidade
                //ret += "01"; // comando ou movimento -- verificar
                //ret += Utils.Insert(b.NumeroDocumento.ToNoFormated(), 10, "0", true);
                //ret += b.DataVencimento.ToString("ddMMyy");
                //ret += Utils.FormatNumber(b.ValorDocumento, 13, 2);
                //ret += Utils.Insert(c.Banco.NumeroBanco.ToString(), 3);
                //ret += Utils.Insert(c.Banco.Carteira, 4);
                //ret += Utils.Insert(c.Banco.Carteira.ToString().Substring(4), 1); //dv
                //ret += "99";//modificar Seq 30
                //ret += b.Aceite.ToUpper() == "S" ? "1" : "0";
                //ret += b.DataDocumento.ToString("ddMMyy");
                //ret += Utils.Insert(b.Instrucao1, 2);//1 instrução - verificar // alterado 19/03 (cliente configura)
                //ret += Utils.Insert(b.Instrucao2, 2);//2 instrução - verificar // alterado 19/03 (cliente configura)
                //juros = b.PercentualJurosMesAtraso;
                //ret += Utils.FormatNumber(b.PercentualJurosMesAtraso, 6, 4);
                //multa = b.PercentualMultaAtraso;
                //ret += Utils.FormatNumber(b.PercentualMultaAtraso, 6, 4);
                //ret += " ";
                //ret += b.DataLimiteDesconto <= b.DataVencimento ? "000000" : b.DataLimiteDesconto.ToString("ddMMyy");
                //ret += Utils.FormatNumber(b.ValorDesconto, 13, 2);
                //ret += EnumHelper.GetNumMoeda(b.Moeda) + "000000000000";
                //ret += Utils.FormatNumber(b.ValorAbatimento, 13, 2);
                //ret += Utils.Insert(b.Sacado.CpfCnpj.ToNoFormated().IsCpf() ? "01" : "02", 2);
                //ret += Utils.Insert(b.Sacado.CpfCnpj.ToNoFormated(), 14, "0", true);
                //ret += Utils.Insert(b.Sacado.Razao.Trim().Length > 3 ? b.Sacado.Razao : b.Sacado.Nome, 40);
                //ret += Utils.Insert(b.Sacado.Endereco, 37);
                //ret += Utils.Insert(b.Sacado.Bairro, 15);
                //ret += Utils.Insert(b.Sacado.Cep.ToNoFormated(), 8);
                //ret += Utils.Insert(b.Sacado.Cidade, 15);
                //ret += Utils.Insert(b.Sacado.Estado, 2);
                //ret += Utils.Insert(" ", 40);
                //ret += b.DiasProtesto.ToString("00");
                //ret += " ";
                //ret += Utils.Insert(sequencial++.ToString("000000"), 6);

                aux += ret + Environment.NewLine;
            }

            ret = "9";
            //ret += Utils.Insert(" ", 193);
            //ret += Utils.Insert(juros > 0 ? "Cobrar juros de " + juros + "% ao mês" : " ", 40); // seq 3
            //ret += Utils.Insert(multa > 0 ? "Cobrar multa de " + multa + "%" : " ", 40);
            //ret += Utils.Insert(" ", 40);
            //ret += Utils.Insert(" ", 40);
            ret += Utils.Insert(" ", 393);
            ret += Utils.Insert(sequencial++.ToString("000000"), 6);

            aux += ret + Environment.NewLine;

            return aux;
        }

        private const string indiceNossoNumero = "29876543298765432";
        private const string indiceCodBarra = "4329876543298765432987654329876543298765432";
        private const string indiceLinhaDigitavel = "2121212120121212121201212121212";
    }
}
