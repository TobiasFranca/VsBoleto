using BoletoBancario.Conta;
using BoletoBancario.Utilitarios;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BoletoBancario.Bancos
{
    public class BancoBrasil : Banco
    {
        public const string Cnab400 = "Cnab400";
        public const string Cnab240 = "Cnab240";
        private static string pathConfigBanco = AppDomain.CurrentDomain.BaseDirectory + "\\ConfigBoletosBancos.ini";

        internal BancoBrasil()
        {
            nomeBanco = "Banco do Brasil";
            numeroBanco = 001;
            dvNumeroBanco = 9;
            identificacao = "Banco do Brasil";

            TituloOutros1 = "Carteira";
            TituloOutros2 = "Variação";

            MascaraOutros1 = "00";
            MascaraOutros2 = "00-0";

            LayoutsArquivoRemessa.Add(Cnab400);
            LayoutsArquivoRemessa.Add(Cnab240);

            EspecieTitulo.Add("01 - Cheque");
            EspecieTitulo.Add("02 - Duplicata Mercantil");
            EspecieTitulo.Add("04 - Duplicata de Serviço");
            EspecieTitulo.Add("06 - Duplicata Rural");
            EspecieTitulo.Add("07 - Letra de Câmbio");
            EspecieTitulo.Add("12 - Nota Promissória");
            EspecieTitulo.Add("16 - Nota de Seguro");
            EspecieTitulo.Add("17 - Recibo");
            EspecieTitulo.Add("19 - Nota de Débito");
            EspecieTitulo.Add("20 - Apólice de Seguro");
            EspecieTitulo.Add("26 - Warrant");
            EspecieTitulo.Add("27 - Dívida Ativa de Estado");
            EspecieTitulo.Add("28 - Dívida Ativa de Município");
            EspecieTitulo.Add("29 - Dívida Ativa da União");
        }

        internal override void CalcularDados(Boleto b)
        {
            b.DigVerNossoNumero = DigitoVerificadorNossoNumero(b);
            b.NossoNumeroComDV = b.NossoNumero + b.DigVerNossoNumero;

            string codBarras = MontarCodigoBarras(b);
            string linhaAux = MontarLinhaDigitavel(b);
            string ld = InserirDigitosVerificadores(b, linhaAux);

            b.LinhaDigitavel = ld;
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

        private static string InserirDVCampo(string codBarra)
        {
            int soma = 0;
            for (int cont = 0; cont < codBarra.Length; cont++)
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
            linha += "001"; // 0-3 Código da IF(banco)
            linha += "9"; // 4-4 Código da moeda
            linha += "00000"; // 5-9 Posições 20 a 24 do código de barras

            // 10-10 Digito verificador campo 1

            //Campo 2
            linha += "0"; //Posições 20 a 24 do código de barras
            linha += Utils.Insert(b.ContaCorrente.CodigoCedente, 9, "0", false);//Posições 25 a 34 do código de barras

            //Campo 3
            int auxCarteira = b.ContaCorrente.Banco.Carteira.ToNoFormated().ToInt(); ;
            linha += Utils.Insert(b.NossoNumero, 8, "0", true);////Posições 35 a 44 do código de barras
            linha += Utils.Insert(auxCarteira.ToString(), 2);//Carteira de Cobrança

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
            string cb = Utils.InserirString("001", 3); // 01-03 Código do Banco na Câmara de Compensação = '001'
            cb += Utils.InserirString("9", 1); // 04-04 Código da Moeda = 9 (Real)
            cb += Utils.InserirString("0", 1); // 05-05 Digito Verificador (DV) do código de Barras*   Anexo VI
            cb += Utils.Insert(GetFatorVencimentoDaData(b.DataVencimento).ToString(), 4, "0", true); // 06-09 Fator de Vencimento ** Anexo IV
            cb += Utils.Insert(Utils.FormatNumber(b.ValorDocumento, 10, 2), 10); // 10-19 Valor
            cb += "000000"; // 20-25 Zeros
            cb += Utils.Insert(b.ContaCorrente.CodigoCedente, 7); // 26-32 Numero do Convenio
            cb += Utils.Insert(b.NossoNumero, 10, "0", true); // 33-42 Complemento do Nosso-Numero sem DV
            cb += Utils.Insert(b.ContaCorrente.Outros1, 2, "0", true); // 43-44 Carteira/Modalidade de Cobrança

            cb = InserirDVCodigoBarras(cb);

            return cb;
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

        private static string RetDV(string numero)
        {
            char dv;
            dv = numero[(numero.Length - 1)];
            return dv + "";
        }

        private static string RetPrefixo(string numero)
        {
            string prefixo = "";
            for (int i = 0; i < numero.Length - 1; i++)
            {
                prefixo += numero[i] + "";
            }
            return prefixo;

        }

        private static string MontarCnab240(ContaCorrente c)
        {
            int sequencial1 = 2;
            //int totalLote = 0;

            string aux = "";
            string ret = "";

            //Header do Arquivo - Tobias
            ret += "001"; // 01-03 Código do Banco
            ret += "0000"; // 04-07 Lote do Serviço
            ret += "0"; // 08-08 Tipo de registro
            ret += Utils.Insert(" ", 9); // 09-17 Uso do Febraban
            ret += Utils.Insert(c.Cedente.CpfCnpj.ToNoFormated().IsCpf() ? "1" : "2", 1); // 18-18 Tipo de Inscricao da Empresa (1 = CPF, 2 = CNPJ)
            ret += Utils.Insert(c.Cedente.CpfCnpj.ToNoFormated(), 14, "0", true); // 19-32 Numero de Inscrição da Empresa(CPF ou CNPJ)
            ret += Utils.Insert(c.CodigoCedente, 9, "0", true); // 33-41 Numero do Convênio de cobrança BB
            ret += "0014"; // 42-45 Cobrança Cedente BB (0014 = Cobrança Cedente)
            ret += c.Outros1; // 46-47 Número da Carteira de Cobrança
            ret += c.Outros2; // 48-50 Número da Variação da Carteira de Cobrança
            ret += Utils.Insert(" ", 2); // 51-52 Campo reservado Banco do Brasil
            ret += Utils.Insert(c.Agencia, 6, "0", true); // 53-58 Agência
            ret += Utils.Insert(c.NumeroConta, 13, "0", true); // 59-71 Conta
            ret += " "; // 72-72 Não utilizado pelo Banco do Brasil
            ret += Utils.Insert(c.Cedente.NomeCedente, 30); // 73-102 Nome da Empresa
            ret += Utils.Insert("BANCO DO BRASIL S.A.", 30); // 103-132 Nome do Banco
            ret += Utils.Insert(" ", 10); // 133-142 Uso do Febraban
            ret += "1"; // 143-143 Código Remessa/Retorno
            ret += Utils.Insert(DateTime.Now.ToString("ddMMyyyy"), 8); // 144-151 Data de Geração do Arquivo
            ret += Utils.Insert(DateTime.Now.ToString("HHmmss"), 6); // 152-157 Hora de Geração do Arquivo
            ret += "000000"; // 158-163 Número Sequencial do Arquivo
            ret += "083"; // 164-166 Número da Versão do Layout do Arquivo
            ret += Utils.Insert(" ", 5); // 167-171 Densidade da Gravação do Arquivo
            ret += Utils.Insert(" ", 20); // 172-191 Uso Exclusivo do Banco
            ret += Utils.Insert(" ", 20); // 192-211 Uso Reservado da Empresa
            ret += Utils.Insert(" ", 29); // 212-240 Uso do Febraban

            aux += ret + Environment.NewLine;
            //Fim Header do Arquivo

            //Header do Lote - Tobias
            ret = "001"; // 01-03 Código do Banco
            ret += "0001"; // 04-07 Lote do Serviço
            ret += "1"; // 08-08 Tipo de Registro
            ret += "R"; // 09-09 Tipo de Operação (R = Remessa, T = Retorno)
            ret += "01"; // 10-11 Tipo de Serviço
            ret += "  "; // 12-13 Uso do Febraban
            ret += "000"; // 14-16 Número da Versão do Layout do Lote (Baseado no Número da Versão do Layout do Arquivo)
            ret += " "; // 17-17 Uso do Febraban
            ret += Utils.Insert(c.Cedente.CpfCnpj.ToNoFormated().IsCpf() ? "1" : "2", 1); // 18-18 Tipo de Inscrição da Empresa
            ret += Utils.Insert(c.Cedente.CpfCnpj.ToNoFormated(), 15, "0", true); // 19-33 Número de Inscrição da Empresa
            ret += Utils.Insert(c.CodigoCedente, 9, "0", true); // 34-42 Numero do Convênio de cobrança BB
            ret += "0014"; // 43-46 Cobrança Cedente BB (0014 = Cobrança Cedente)
            ret += c.Outros1; // 47-48 Número da Carteira de Cobrança BB
            ret += c.Outros2; // 49-51 Número da Variação da Carteira de Cobrança BB
            ret += "  "; // 52-53 Campo que identifica remessa de testes (brancos = remessa normal, TS = remessa de teste)
            ret += Utils.Insert(c.Agencia, 6, "0", true); // 54-59 Agência
            ret += Utils.Insert(c.NumeroConta, 13, "0", true); // 60-72 Conta
            ret += " "; // 73-73 Não utilizado pelo banco do Brasil
            ret += Utils.Insert(c.Cedente.NomeCedente, 30); // 74-103 Nome da Empresa
            ret += Utils.Insert(" ", 40); // 104-143 Mensagem 1 (brancos)
            ret += Utils.Insert(" ", 40); // 144-183 Mensagem 2 (Não utilizado pelo banco do brasil)

            int seqRemessa = ArquivoINI.LeString(pathConfigBanco, "REMESSA", "SEQ_REMESSA").ToInt32();
            ret += Utils.Insert(seqRemessa.ToString(), 8, "0", true); // 184-191 Número Remessa (Opcional)            
            ArquivoINI.EscreveString(pathConfigBanco, "REMESSA", "SEQ_REMESSA", Convert.ToString(seqRemessa++));

            ret += Utils.Insert(DateTime.Now.ToString("ddMMyyyy"), 8); // 192-199 Data de Geração do Arquivo
            ret += Utils.Insert(" ", 8); // 200-207 Não utilizado pelo banco do Brasil
            ret += Utils.Insert(" ", 33); // 208-240 Uso exclusivo do Febraban

            //totalLote++;

            aux += ret + Environment.NewLine;
            //Fim Header do Lote

            int sequenciaRegistro = 1;

            foreach (Boleto b in c.Boletos)
            {                
                string sequenciaRegistro1 = Utils.Insert(sequencial1++.ToString("00000"), 5);

                //Segmento P - Tobias
                ret = "001"; // 01-03 Código do Banco
                ret += "0001"; // 04-07 Lote do Serviço
                ret += "3"; // 08-08 Tipo de Registro
                ret += Utils.Insert(sequenciaRegistro.ToString(), 5, "0", true); // 09-13 Número Sequencial do registro no Lote (Começa com 00001 e incrementa 1 a cada linha)
                ret += "P"; // 14-14 Código do Seguimento
                ret += " "; // 15-15 Uso do Febraban
                ret += "01"; // 16-17 Código do Movimento da Remessa
                ret += Utils.Insert(c.Agencia, 6, "0", true); // 18-23 Agência
                ret += Utils.Insert(c.NumeroConta, 13, "0", true); // 24-36 Conta
                ret += " "; // 37-37 Não utilizado pelo Banco do Brasil
                ret += Utils.Insert(Utils.FormataNossoNumeroCNAB240BB(b.NossoNumero, b.DigVerNossoNumero, c.CodigoCedente), 20, " "); // 38-57 Identificação do Título
                ret += "7"; // 58-58 Código da Carteira (Verificar alterações no manual)
                ret += " "; // 59-59 Não Tratado pelo Banco do Brasil
                ret += " "; // 60-60 Não Tratado pelo Banco do Brasil
                ret += " "; // 61-61 Não Tratado pelo Banco do Brasil
                ret += " "; // 62-62 Não Tratado pelo Banco do Brasil
                ret += Utils.Insert(b.NumeroDocumento, 15, "0", true);
                ret += b.DataVencimento.ToString("ddMMyyyy"); // 78-85 Data de Vencimento do Título
                ret += Utils.FormatNumber(b.ValorDocumento, 15, 2); // 86-100 Valor do Título
                ret += "00000"; // 101-105 Agencia Encarregada da Cobrança
                ret += " "; // 106-106 Dígito Verificador da Agência
                ret += c.EspecieTitulo; // 107-108 Espécie do Titulo
                ret += "N"; // 109-109 Aceite
                ret += b.DataDocumento.ToString("ddMMyyyy"); // 110-117 Data de Emissão do Título
                ret += "1"; // 118-118 Código do Juros de Mora
                ret += "00000000"; // 119-126 Data do Juros de Mora
                ret += Utils.FormatNumber(b.ValorJurosDiaAtraso, 15, 2); // 127-141 Juros de Mora por dia
                ret += "0"; // 142-142 Código do Desconto 1
                ret += b.DataLimiteDesconto <= b.DataVencimento ? "00000000" : b.DataLimiteDesconto.ToString("ddMMyyyy"); // 143-150 Data Desconto 1
                ret += Utils.FormatNumber(b.ValorDesconto, 15, 2); // 151-165 Valor/Percentual do Desconto
                ret += Utils.FormatNumber(0, 15, 2); // 166-180 Valor do IOF a ser recolhido
                ret += Utils.FormatNumber(b.ValorDesconto, 15, 2); // 181-195 Valor do Desconto
                ret += "                         "; // 196-220 Identificação do Titulo na Empresa

                if (b.DiasProtesto.ToInt() > 0)
                {
                    ret += "1"; // 221-221 Código para protesto
                }
                else
                {
                    ret += "3"; // 221-221 Código para protesto
                }

                ret += b.DiasProtesto; // 222-223 Número Dias para Protesto
                ret += "0"; // 224-224 Código para Baixa/Devolução (O banco considera a informação no cadastro da carteira junto a Agencia)
                ret += "000"; // 225-227 Número de dias para baixa/devolução (o banco considera a informação da carteira junto a agencia)
                ret += "09"; // 228-229 Código da Moeda
                ret += "0000000000"; // 230-239 Numero do Contrato
                ret += " "; // 240-240 Uso Exclusivo do Febraban

                //totalLote++;
                sequenciaRegistro++;

                aux += ret + Environment.NewLine;
                //Fim Segmento P

                //Segmento Q - Tobias
                ret = "001"; // 01-03 Código do Banco
                ret += "0001"; // 04-07 Lote do Serviço
                ret += "3"; // 08-08 Tipo de Registro
                ret += Utils.Insert(sequenciaRegistro.ToString(), 5, "0", true); // 09-13 Sequencial Registro no Lote
                ret += "Q"; // 14-14 Código Segmento o Registro Detalhe
                ret += " "; // 15-15 Uso Exclusivo do Febraban
                ret += "01"; // 16-17 Código de Movimento Remessa
                ret += b.Sacado.CpfCnpj.ToNoFormated().IsCpf() ? "1" : "2"; // 18-18 Tipo de Inscrição do Sacado
                ret += Utils.Insert(b.Sacado.CpfCnpj.ToNoFormated(), 15, "0", true); // 19-33 Número do CNPJ ou CPF do Sacado
                ret += Utils.Insert(b.Sacado.Nome, 40, " "); // 34-73 Nome do Sacado
                ret += Utils.Insert(b.Sacado.Endereco, 40, " "); // 74-113 Endereço do Sacado
                ret += Utils.Insert(b.Sacado.Bairro, 15, " "); // 114-128 Bairro do Sacado
                ret += Utils.Insert(b.Sacado.Cep.Replace(".", "").Replace("-", ""), 8, " "); // 129-136 CEP do Sacado
                ret += Utils.Insert(b.Sacado.Cidade, 15, " "); // 137-151 Cidade do Sacado
                ret += Utils.Insert(b.Sacado.Estado, 2, " "); // 152-153 UF do Sacado
                ret += "0"; // 154-154 Tipo de Inscrição (utilizar 0 se não houver avalista)
                ret += Utils.Insert("0", 15); // 155-169 Numero de inscrição (utilizar 0 se não houver avalista)
                ret += Utils.Insert("0", 40); // 170-209 Nome do Sacador/Avalista (utilizar brancos se não houver avalista)
                ret += "000"; // 210-212 Campo não tratado
                ret += Utils.Insert(" ", 20); // 213-232 Campo não tratado
                ret += Utils.Insert(" ", 8); // 233-240 Uso Exclusivo do Febraban

                //totalLote++;
                sequenciaRegistro++;


                aux += ret + Environment.NewLine;
                //Fim Segmento Q

                //Segmento R - Tobias
                ret = "001"; // 01-03 Código do Banco
                ret += "0001"; // 04-07 Lote do Serviço
                ret += "3"; // 08-08 Tipo de Registro
                ret += Utils.Insert(sequenciaRegistro.ToString(), 5, "0", true); // 09-13 Número Sequencial do Registro no Lote
                ret += "R"; // 14-14 Código do Segmento de Registro detalhe
                ret += " "; // 15-15 Uso Exclusivo do Febraban
                ret += "01"; // 16-17 Código de Movimento da Remessa
                ret += "0"; // 18-18 Código do desconto (Não tratado pelo banco)
                ret += Utils.Insert("0", 8); // 19-26 Data do Desconto (Não tratado pelo banco)
                ret += Utils.Insert("0", 15); // 27-41 Percentual Desconto (Não tratado pelo banco)
                ret += "0"; // 42-42 Código do desconto (Não tratado pelo banco)
                ret += Utils.Insert("0", 8); // 43-50 Data do Desconto (Não tratado pelo banco)
                ret += Utils.Insert("0", 15); // 51-65 Percentual Desconto (Não tratado pelo banco)
                ret += "1"; // 66-66 Código da Multa
                ret += b.DataVencimento.ToString("ddMMyyyy"); // 67-74 Data da Multa
                ret += Utils.FormatNumber(b.ValorMultaAtraso, 15, 2); // 75-89 Valor da Multa
                ret += Utils.Insert("0", 10); // 90-99 Informação do Sacado (Não Tratado pelo banco)
                ret += Utils.Insert(" ", 40); // 100-139 Mensagem 3
                ret += Utils.Insert("0", 40); // 140-179 Mensagem 4 (Não Tratado pelo banco)
                ret += Utils.Insert(" ", 20); // 180-199 Uso Exclusivo do Febraban
                ret += Utils.Insert("0", 8); // 200-207 Campo Não Tratado
                ret += Utils.Insert("0", 3); // 208-210 Campo Não Tratado
                ret += Utils.Insert("0", 5); // 211-215 Campo Não Tratado
                ret += "0"; // 216-216 Campo Não Tratado
                ret += Utils.Insert("0", 12); // 217-228 Campo Não Tratado
                ret += "0"; // 229-229 Campo Não Tratado
                ret += "0"; // 230-230 Campo Não Tratado
                ret += "0"; // 231-231 Campo Não Tratado
                ret += Utils.Insert(" ", 9); // Campo Não Tratado

                //totalLote++;
                sequenciaRegistro++;

                aux += ret + Environment.NewLine;
                //Fim Segmento R                
            };

            //totalLote++;
            sequenciaRegistro++;

            //Trailer do Lote - Tobias
            ret = "001"; // 01-03 Código do Banco
            ret += "0001"; // 04-07 Lote do Serviço
            ret += "5"; // 08-08 Tipo de Registro
            ret += Utils.Insert(" ", 9); // 09-17 Uso Exclusivo do Febraban
            ret += Utils.Insert(sequenciaRegistro.ToString(), 6, "0", true); // 18-23 Quantidade registros do Lote
            ret += Utils.Insert(" ", 217); // 24-240 Uso Exclusivo do Febraban

            int p = ret.Length;
            aux += ret + Environment.NewLine;

            sequenciaRegistro = sequenciaRegistro +2;
            //Fim Trailer do Lote

            //Trailer do Arquivo - Tobias
            ret = "001"; // 01-03 Código do Banco
            ret += "9999"; // 04-07 lote do Serviço
            ret += "9"; // 08-08 Tipo de Registro
            ret += Utils.Insert(" ", 9); // 09-17 Uso Exclusivo do Febraban
            ret += "000001"; // 18-23 Quantidade de Lotes do Arquivo
            ret += Utils.Insert(sequenciaRegistro.ToString(), 6, "0", true); // 24-29 Quantidade de Registros do Arquivo
            ret += Utils.Insert(" ", 6); // 30-35 Não Utilizado Pelo Banco
            ret += Utils.Insert(" ", 205); // 36-240 Uso Exclusivo do Febraban

            int g = ret.Length;
            aux += ret;
            //Fim Trailer do Arquivo

            return aux;
        }

        private static string MontarCnab400(ContaCorrente c)
        {
            int sequencial = 1;
            decimal auxValorTotal = 0;

            string aux = "";
            string ret = "";


            ret += Utils.Insert("0", 1); // Identificação do Registro Header: “0” (zero)
            ret += Utils.Insert("1", 1); // Tipo de Operação: “1” (um)
            ret += Utils.Insert("REMESSA", 7); //Identificação por Extenso do Tipo de Operação
            ret += Utils.Insert("01", 2); // Identificação do Tipo de Serviço: “01”
            ret += Utils.Insert("COBRANCA", 8); // Identificação por Extenso do Tipo de Serviço: “COBRANCA”
            ret += Utils.Insert(" ", 7); //Complemento do Registro: “Brancos”
            ret += Utils.Insert(RetPrefixo(c.Agencia), 4); // Prefixo da Agência: Número da Agência onde está cadastrado o convênio líder do cedente  "2921"

            ret += Utils.Insert(RetDV(c.Agencia), 1); // Dígito Verificador - D.V. - do Prefixo da Agência.  "1"

            ret += Utils.Insert(RetPrefixo(c.NumeroConta), 8, "0", true); // Número da Conta Corrente: Número da conta onde está cadastrado o Convênio Líder do Cedente    "7795"
            ret += Utils.Insert(RetDV(c.NumeroConta), 1); // Dígito Verificador - D.V. – do Número da Conta Corrente do Cedente   "X"
            ret += Utils.Insert("000000", 6); //Complemento do Registro: “000000”
            ret += Utils.Insert(c.Cedente.NomeCedente, 30); // Nome do Cedente
            ret += Utils.Insert("001BANCODOBRASIL", 18); //001BANCODOBRASIL
            ret += Utils.Insert(DateTime.Now.ToString("ddMMyy"), 6); // Data da Gravação: Informe no formato “DDMMAA”


            int seqRemessa = ArquivoINI.LeString(pathConfigBanco, "BANCODOBRASIL", "SEQ_REMESSA").ToInt32();
            ret += Utils.Insert(seqRemessa.ToString(), 7, "0", true);
            seqRemessa++;
            ArquivoINI.EscreveString(pathConfigBanco, "BANCODOBRASIL", "SEQ_REMESSA", seqRemessa.ToString());


            ret += Utils.Insert(" ", 22); //Complemento do Registro: “Brancos”
            ret += Utils.Insert("2289001", 7); //Número do Convênio Líder (numeração acima de 1.000.000 um milhão)" 
            ret += Utils.Insert(" ", 258); //Complemento do Registro: “Brancos”
            ret += Utils.Insert(sequencial++.ToString("000000"), 6); //Seqüencial do Registro:”000001”

            aux += ret + Environment.NewLine;

            foreach (Boleto b in c.Boletos)
            {
                auxValorTotal += b.ValorDocumento;

                int auxCarteira = c.Banco.Carteira.ToNoFormated().ToInt();

                b.CalcularDadosBoleto();

                ret = "7"; // Identificação do Registro Detalhe: 7 (sete)
                ret += Utils.Insert(c.Cedente.CpfCnpj.ToNoFormated().IsCpf() ? "01" : "02", 2); // Tipo de Inscrição do Cedente
                ret += Utils.Insert(c.Cedente.CpfCnpj.ToNoFormated(), 14, "0", true); // Número do CPF/CNPJ do Cedente
                ret += Utils.Insert(RetPrefixo(c.Agencia), 4); // Prefixo da Agência: Número da Agência onde está cadastrado o convênio líder do cedente  "2921"

                ret += Utils.Insert(RetDV(c.Agencia), 1); // Dígito Verificador - D.V. - do Prefixo da Agência.  "1"
                ret += Utils.Insert(RetPrefixo(c.NumeroConta), 8, "0", true); // Número da Conta Corrente: Número da conta onde está cadastrado o Convênio Líder do Cedente    "7795"
                ret += Utils.Insert(RetDV(c.NumeroConta), 1); // Dígito Verificador - D.V. – do Número da Conta Corrente do Cedente   "X"  "2289001"
                ret += Utils.Insert(c.Outros2, 7); // Número do Convênio de Cobrança do Cedente 
                ret += Utils.Insert(Utils.FormatNumber(c.CodigoCedente, 8), 25); //Código de Controle da Empresa  
                ret += Utils.Insert(c.Outros2 + "000", 10);//Nosso-Número ?????????
                ret += Utils.Insert(b.NossoNumero, 7, "0", true);//Nosso-Número ?????????
                ret += Utils.Insert("00", 2); //Número da Prestação: “00” (Zeros)
                ret += Utils.Insert("00", 2); // Grupo de Valor: “00” (Zeros)
                ret += Utils.Insert(" ", 3); //Complemento do Registro: “Brancos”
                ret += Utils.Insert(" ", 1); //Indicativo de Mensagem ou Sacador/Avalista
                ret += Utils.Insert(" ", 3); //Prefixo do Título: “Brancos”
                ret += Utils.Insert(c.Outros1, 3);//Variação da Carteira 
                ret += Utils.Insert("0", 1); // Conta Caução: “0” (Zero)
                ret += Utils.Insert("000000", 6);//Número do Borderô: “000000” (Zeros)
                ret += Utils.Insert(" ", 5); //Tipo de Cobrança 
                ret += Utils.Insert(auxCarteira.ToString(), 2);//Carteira de Cobrança 
                ret += Utils.Insert("01", 2); // Comando ?????????
                ret += Utils.Insert(b.NumeroDocumento.ToNoFormated(), 10, "0", true); //Seu Número/Número do Título Atribuído pelo Cedente 
                ret += b.DataVencimento.ToString("ddMMyy"); // Data de Vencimento
                ret += Utils.FormatNumber(b.ValorDocumento, 13, 2); // Valor do Título
                ret += Utils.Insert("001", 3); //Número do Banco: “001”
                ret += Utils.Insert("0000", 4);//Prefixo da Agência Cobradora: “0000”
                ret += Utils.Insert(" ", 1);//Dígito Verificador do Prefixo da Agência Cobradora: “Brancos”
                ret += c.EspecieTitulo;//Espécie de Titulo
                ret += Utils.Insert("N", 1); //Aceite do Título:
                ret += b.DataDocumento.ToString("ddMMyy");//Data de Emissão: Informe no formato “DDMMAA”
                ret += Utils.Insert(b.Instrucao1, 2); //Instrução Codificada
                ret += Utils.Insert(b.Instrucao2, 2);// Instrução Codificada
                ret += Utils.FormatNumber(b.ValorJurosDiaAtraso, 13, 2); // Juros de Mora por Dia de Atraso
                ret += b.DataLimiteDesconto <= b.DataVencimento ? "000000" : b.DataLimiteDesconto.ToString("ddMMyy"); //Data Limite para Concessão de Desconto/Data de Operação do BBVendor/Juros de Mora.
                ret += Utils.FormatNumber(b.ValorDesconto, 13, 2); //Valor do Desconto
                ret += Utils.FormatNumber(0, 13, 2);//Valor do IOF/Qtde Unidade Variável.
                ret += Utils.FormatNumber(b.ValorAbatimento, 13, 2);//Valor do Abatimento
                ret += Utils.Insert(b.Sacado.CpfCnpj.ToNoFormated().IsCpf() ? "01" : "02", 2);//Tipo de Inscrição do Sacado
                ret += Utils.Insert(b.Sacado.CpfCnpj.ToNoFormated(), 14, "0", true);//Número do CNPJ ou CPF do Sacado
                ret += Utils.Insert(b.Sacado.Razao.Trim().Length > 3 ? b.Sacado.Razao : b.Sacado.Nome, 37); //Nome do Sacado
                ret += Utils.Insert(" ", 3); //Complemento do Registro: “Brancos”
                ret += Utils.Insert(b.Sacado.Endereco, 40);//Endereço do Sacado
                ret += Utils.Insert(b.Sacado.Bairro, 12);//Bairro do Sacado
                ret += Utils.Insert(b.Sacado.Cep.ToNoFormated(), 8);//CEP do Endereço do Sacado
                ret += Utils.Insert(b.Sacado.Cidade, 15);//Cidade do Sacado
                ret += Utils.Insert(b.Sacado.Estado, 2);//UF da Cidade do Sacado
                ret += Utils.Insert(" ", 40); //Observações/Mensagem ou Sacador/Avalista
                ret += b.DiasProtesto; //Número de Dias Para Protesto
                ret += Utils.Insert(" ", 1);//Complemento do Registro: “Brancos”
                ret += Utils.Insert(sequencial++.ToString("000000"), 6);// Seqüencial de Registro

                aux += ret + Environment.NewLine;
            }

            ret = "9";
            ret += Utils.Insert(" ", 393);
            ret += Utils.Insert(sequencial++.ToString("000000"), 6);

            aux += ret + Environment.NewLine;

            return aux;
        }

        private const string indiceNossoNumero = "543298765432";
        private const string indiceCodBarra = "43290876543298765432987654329876543298765432";
        private const string indiceLinhaDigitavel = "2121212120121212121201212121212";
    }
}
