using BoletoBancario.Utilitarios;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace BoletoBancario.Conta
{
    public class Boleto
    {

        internal Boleto() { }
        /// <summary>
        /// Destrutor da classe para excluir as imagens dos codigos de barras gerados.
        /// </summary>
        ~Boleto()
        {
            //Relatorios.DeletarImagensCodBarras(this.PathImgCodBarras);
        }

        private ContaCorrente contaCorrente;
        /// <summary>
        /// Conta Corrente do boleto.
        /// </summary>
        public ContaCorrente ContaCorrente
        {
            get { return contaCorrente; }
            set { contaCorrente = value; }
        }

        /// <summary>
        /// Cria um Sacado para este boleto.
        /// </summary>
        /// <returns>Nova instancia de Sacado</returns>
        public Sacado NovoSacado()
        {
            Sacado s = new Sacado();
            s.Boleto = this;
            sacado = s;
            return s;
        }

        private Sacado sacado = new Sacado();
        /// <summary>
        /// Sacado do boleto.
        /// Utilize NovoSacado() para uma nova instacia deste.
        /// </summary>
        public Sacado Sacado
        {
            get { return sacado; }
            set { sacado = value; }
        }

        private string pathImgCodBarras;
        /// <summary>
        /// Path completo da imagem do código de barras gerado.
        /// </summary>
        public string PathImgCodBarras
        {
            get { return pathImgCodBarras; }
            set { pathImgCodBarras = value; }
        }

        private string codigoBarras;
        /// <summary>
        /// Código que irá gerar o código de barras.
        /// </summary>
        public string CodigoBarras
        {
            get { return codigoBarras; }
            internal set { codigoBarras = value; }
        }

        private string linhaDigitavel;
        /// <summary>
        /// Linha digitável do boleto sem formatação.
        /// </summary>
        public string LinhaDigitavel
        {
            get { return linhaDigitavel; }
            internal set { linhaDigitavel = value; }
        }

        private DateTime dataDocumento;
        /// <summary>
        /// Data deste boleto.
        /// </summary>
        public DateTime DataDocumento
        {
            get { return dataDocumento; }
            set { dataDocumento = value; }
        }

        private DateTime dataVencimento;
        /// <summary>
        /// Vencimento do boleto.
        /// </summary>
        public DateTime DataVencimento
        {
            get { return dataVencimento; }
            set { dataVencimento = value; }
        }

        private string numeroDocumento;
        /// <summary>
        /// Número do boleto
        /// </summary>
        public string NumeroDocumento
        {
            get { return numeroDocumento; }
            set { numeroDocumento = value; }
        }

        public DateTime DataLimiteDesconto { get; set; }


        private EnumTipoMoeda moeda = EnumTipoMoeda.Real;
        /// <summary>
        /// Tipo da moeda utilizada no boleto.
        /// Padrão 'R$' 
        /// </summary>
        public EnumTipoMoeda Moeda
        {
            get { return moeda; }
            set { moeda = value; }
        }

        public string NossoNumero { get; set; }

        private string digVerNossoNumero;
        /// <summary>
        /// DV do nosso número
        /// </summary>
        public string DigVerNossoNumero
        {
            get { return digVerNossoNumero; }
            internal set { digVerNossoNumero = value; }
        }

        /// <summary>
        /// Nosso Número + DV
        /// Quando for atribuido, verifica se contem mais que duas casas, caso positivo
        /// a ultima delas é tratada como DV e gravado cada um em seus respectivos campos.
        /// </summary>
        public string NossoNumeroComDV { get; set; }

        private int numeroParcelaAtual = 1;
        /// <summary>
        /// Número da parcela do boleto
        /// </summary>
        public int NumeroParcelaAtual
        {
            get { return numeroParcelaAtual; }
            set
            {
                numeroParcelaAtual = value;
                if (numeroParcelaAtual > numeroParcelas)
                    numeroParcelas = numeroParcelaAtual;
            }
        }

        private int numeroParcelas = 1;
        /// <summary>
        /// Parcelas totais da compra a qual pertence o boleto
        /// </summary>
        public int NumeroParcelas
        {
            get { return numeroParcelas; }
            set { numeroParcelas = value; }
        }
        //Dias úteis para protesto
        public int ProtestoDiasUteis { get; set; }

        public decimal PercentualDesconto { get; set; }

        private decimal percentualJurosMesAtraso;
        /// <summary>
        /// Porcentagem de juros por mes de atraso do boleto.
        /// O Valor será dividido em dias para que haja o valor diario de multa.
        /// Obs: pode-se utilizar este campo ou o 'PercentualJurosDiaAtraso'.
        /// </summary>
        public decimal PercentualJurosMesAtraso
        {
            get { return percentualJurosMesAtraso; }
            set
            {
                percentualJurosMesAtraso = value;
                percentualJurosDiaAtraso = percentualJurosMesAtraso / 30;
            }
        }

        private decimal percentualJurosDiaAtraso;
        /// <summary>
        /// Porcentagem de juros por dia de atraso do boleto.
        /// O valor será multiplicado para obter a porcentagem de juros no mês.
        /// Obs: pode-se utilizar este campo ou o 'PercentualJurosMesAtraso'.
        /// </summary>
        public decimal PercentualJurosDiaAtraso
        {
            get { return percentualJurosDiaAtraso; }
            set
            {
                percentualJurosDiaAtraso = value;
                percentualJurosMesAtraso = percentualJurosDiaAtraso * 30;
            }
        }

        private decimal percentualMultaAtraso;

        public decimal PercentualMultaAtraso
        {
            get { return percentualMultaAtraso; }
            set { percentualMultaAtraso = value; }
        }

        private decimal percentualOutrosAcrescimos;

        public decimal PercentualOutrosAcrescimos
        {
            get { return percentualOutrosAcrescimos; }
            set { percentualOutrosAcrescimos = value; }
        }

        private decimal valorAbatimento;

        public decimal ValorAbatimento
        {
            get { return valorAbatimento; }
            set { valorAbatimento = value; }
        }

        private decimal valorDocumento;

        public decimal ValorDocumento
        {
            get { return valorDocumento; }
            set { valorDocumento = value; }
        }

        private decimal valorDesconto;

        public decimal ValorDesconto
        {
            get { return valorDesconto; }
            set { valorDesconto = value; }
        }

        private decimal valorJurosDiaAtraso;

        public decimal ValorJurosDiaAtraso
        {
            get { return valorJurosDiaAtraso; }
            set { valorJurosDiaAtraso = value; }
        }

        private decimal valorMultaAtraso;

        public decimal ValorMultaAtraso
        {
            get { return valorMultaAtraso; }
            set { valorMultaAtraso = value; }
        }

        private decimal valorOutrosAcrescimos;

        public decimal ValorOutrosAcrescimos
        {
            get { return valorOutrosAcrescimos; }
            set { valorOutrosAcrescimos = value; }
        }

        /// <summary>
        /// Gerar o proximo número utilizado no Nosso Número do boleto
        /// </summary>
        /// <returns>Número da sequencia para boleto</returns>
        public string GerarProximoNossoNumero()
        {
            contaCorrente.ProximoNossoNumero++;
            return contaCorrente.ProximoNossoNumero.ToString();
        }

        private string especieBoleto = "OU";

        public string EspecieBoleto
        {
            get { return especieBoleto; }
            set { especieBoleto = value; }
        }
        private string diasProtesto = "00";
        public string DiasProtesto
        {
            get { return diasProtesto; }
            set { diasProtesto = value; }
        }
        private string instrucao1 = "00";
        public string Instrucao1
        {
            get { return instrucao1; }
            set { instrucao1 = value; }
        }

        private string instrucao2 = "00";
        public string Instrucao2
        {
            get { return instrucao2; }
            set { instrucao2 = value; }
        }

        private string aceite = "N";

        public string Aceite
        {
            get { return aceite; }
            set { aceite = value; }
        }

        private int fatorVencimento;

        public int FatorVencimento
        {
            get { return fatorVencimento; }
            set { fatorVencimento = value; }
        }

        /// <summary>
        /// Calcula o DV do Nosso Número, a linha digitável e o código de barras.
        /// Além de gerar a imagem do codigo de barras
        /// (utilizado por falha no Fast Report tem que gerar separado)
        /// </summary>
        public void CalcularDadosBoleto()
        {
            contaCorrente.Banco.CalcularDados(this);
            GerarImgCodBarras();
        }

        /// <summary>
        /// Dll pra gerar a img do codigo de barras.
        /// Grava como .gif em 'Configuracoes.CaminhoArquivosPadroes'
        /// </summary>
        private void GerarImgCodBarras()
        {
            try
            {
                BarcodeLib.Barcode bc = new BarcodeLib.Barcode();
                bc.Alignment = BarcodeLib.AlignmentPositions.CENTER;
                bc.IncludeLabel = false;

                bc.Encode(BarcodeLib.TYPE.Interleaved2of5, codigoBarras, 665, 80);

                string path = Configuracoes.CaminhoArquivosPadroes;
                Configuracoes.VerificarECriarDiretorios(path);

                if (!Directory.Exists(path))
                {
                    throw new ArgumentException("Não foi possivel criar o diretório para salvar o código de barras.");
                }

                string nome = path + "/barcode" + this.NossoNumeroComDV + ".gif";

                pathImgCodBarras = nome;
                bc.SaveImage(nome, BarcodeLib.SaveTypes.GIF);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Emails de destino para envio dos boletos.
        /// </summary>
        private IList<string> emailsDestinatarios = new List<string>();

        /// <summary>
        /// Enviar boletos por email
        /// </summary>
        /// <param name="corpoHtml">Se utiliza tags html para o corpo</param>
        /// <returns>Se ocorreu erro ou não durante o envio.</returns>
        public IList<string> EmailsDestinatarios
        {
            get { return emailsDestinatarios; }
            set { emailsDestinatarios = value; }
        }

        /// <summary>
        /// Gera o PDF do boleto.
        /// </summary>
        /// <param name="pathArquivo">Path onde será gravado o arquivo</param>
        /// <param name="agruparBoletos">Se os boletos de uma CC será gerado em um único arquivo ou um arquivo para cada boleto</param>
        /// <returns>Se ocorreu erro ou não durante a geração dos arquivos.</returns>
        public bool EnviarBoletoPorEmail(string pathLayout, bool corpoHtml = false)
        {
            try
            {
                if (contaCorrente == null)
                {
                    return false;
                }

                if (emailsDestinatarios.Count <= 0)
                {
                    return false;
                }

                string pathArquivo = Configuracoes.CaminhoArquivosGerados + "/" +
                    numeroDocumento.ToNoFormated() + ".pdf";

                GerarPDF(pathArquivo, pathLayout);

                if (string.IsNullOrEmpty(Configuracoes.Email_Usuario))
                {
                    return false;
                }

                MailAddress from = new MailAddress(Configuracoes.Email_Usuario);

                SmtpClient smtp = new SmtpClient()
                {
                    Host = Configuracoes.Email_Servidor,
                    Port = Configuracoes.Email_Porta.ToInt(),
                    EnableSsl = Configuracoes.Email_Usar_SSL,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(Configuracoes.Email_Usuario,
                        Configuracoes.Email_Senha),
                    Timeout = 25000
                };

                MailMessage message = new MailMessage();

                foreach (string s in emailsDestinatarios)
                {
                    if (s.Contains("@"))
                        message.To.Add(s);
                }

                if (message.To.Count <= 0) return false;

                message.From = from;
                message.IsBodyHtml = Configuracoes.Email_Html;
                message.Subject = Configuracoes.Email_Assunto_Padrao.Replace("[NF]", NumeroDocumento.ToString());
                message.Body = Configuracoes.Email_Corpo_Padrao.Replace("[NF]", NumeroDocumento.ToString());
                message.Attachments.Add(new Attachment(pathArquivo));

                smtp.Send(message);

                message.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }

        /// <summary>
        /// Imprimi os boletos da CC.
        /// </summary>
        /// <param name="agruparBoletos">Se os boletos de uma CC será impresso em um único arquivo ou um arquivo para cada boleto</param>
        /// <returns>Se ocorreu erro ou não durante a impressão dos arquivos.</returns>
        public bool GerarPDF(string pathArquivo, string pathLayout, bool agruparBoletos = true)
        {
            if (contaCorrente == null)
            {
                return false;
            }

            FastReport.Report r = Relatorios.PrepararRelatório(
                    pathLayout, agruparBoletos ?
                    Relatorios.GetDataSetContaCorrente(contaCorrente) : Relatorios.GetDataSetBoleto(this));

            return Relatorios.ExportarRelatorioPDF(r, pathArquivo);
        }

        /// <summary>
        /// Monta o path onde estao gravados os arquivos dos relatorios de cada banco.
        /// Atenção!!! Esta pasta deve acompanhar a dll.
        /// </summary>
        /// <returns></returns>
        public bool ImprimirBoleto(string pathLayout, bool agruparBoletos = true)
        {
            if (contaCorrente == null)
            {
                return false;
            }

            FastReport.Report r = Relatorios.PrepararRelatório(
                pathLayout, agruparBoletos ?
                    Relatorios.GetDataSetContaCorrente(contaCorrente) : Relatorios.GetDataSetBoleto(this));

            return Relatorios.ImprimirBoleto(r, Configuracoes.ImpressoraPadrao);
        }

    }
}
