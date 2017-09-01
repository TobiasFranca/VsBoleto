using BoletoBancario.Bancos;
using BoletoBancario.Utilitarios;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BoletoBancario.Conta
{
    public class ContaCorrente
    {
        /// <summary>
        /// Destrutor da classe fazendo a exclusao das 
        /// imagens de códigos de barras geradas.
        /// </summary>
        ~ContaCorrente()
        {
            //foreach (Boleto b in this.boletos)
            //    Relatorios.DeletarImagensCodBarras(b.PathImgCodBarras);
        }
        /// <summary>
        /// Iniciar uma nova conta corrente com os dados passados
        /// </summary>
        /// <param name="banco">Enum com opções dos bancos disponiveis</param>
        /// <param name="carteira">número da carteira</param>
        /// <param name="nomeCedente">Nome do cedente</param>
        /// <param name="cpfCnpj">Cpf ou Cnpj do cedente</param>
        /// <returns>Retorna uma nova instancia de ContaCorrente</returns>
        public static ContaCorrente NovaContaCorrente(EnumBanco banco, string carteira, string nomeCedente, string cpfCnpj)
        {//verificar como fazer a licença
            ContaCorrente c = new ContaCorrente(banco, carteira, nomeCedente, cpfCnpj);
            return c;
        }
        /// <summary>
        /// Construtor privado, utilizado somente pela Dll.
        /// </summary>
        /// <param name="banco"></param>
        /// <param name="carteira"></param>
        /// <param name="nomeCedente"></param>
        /// <param name="cpfCnpj"></param>
        private ContaCorrente(EnumBanco banco, string carteira, string nomeCedente, string cpfCnpj)
        {
            this.banco = EnumHelper.GetBanco(banco);
            if (this.banco == null)
            {
                throw new Exception("Banco Inválido");
            }

            this.banco.Carteira = carteira;
            cedente.NomeCedente = nomeCedente;
            cedente.CpfCnpj = cpfCnpj;
        }
        /// <summary>
        /// Adiciona um novo boleto à lista de boletos da conta corrente atual.
        /// </summary>
        /// <returns>Retorna uma nova instancia de Boleto</returns>
        public Boleto NovoBoleto()
        {
            Boleto b = new Boleto();
            b.ContaCorrente = this;
            Boletos.Add(b);
            return b;
        }

        private Cedente cedente = new Cedente();
        /// <summary>
        /// Cedente da Conta Corrente.
        /// </summary>
        public Cedente Cedente
        {
            get { return cedente; }
            //set { cedente = value; }
        }

        private Banco banco;
        /// <summary>
        /// Banco utilizado na Conta Corrente.
        /// </summary>
        public Banco Banco
        {
            get { return banco; }
            //set { banco = value; }
        }

        private IList<Boleto> boletos = new List<Boleto>();
        /// <summary>
        /// Lista de boletos da Conta Corrente.
        /// Utilize NovoBoleto() para adicionar boletos à lista.
        /// </summary>
        public IList<Boleto> Boletos
        {
            get { return boletos; }
            //set { boletos = value; }
        }

        private string agencia;
        /// <summary>
        /// Número da agencia da CC.
        /// </summary>
        public string Agencia
        {
            get { return agencia; }
            set { agencia = value; }
        }

        private string numeroConta;
        /// <summary>
        /// Número da CC.
        /// </summary>
        public string NumeroConta
        {
            get { return numeroConta; }
            set { numeroConta = value; }
        }

        private string codigoCedente;
        /// <summary>
        /// Código do cedente para esta CC.
        /// </summary>
        public string CodigoCedente
        {
            get { return codigoCedente; }
            set { codigoCedente = value; }
        }

        private long inicioNossoNumero;
        /// <summary>
        /// Valor emitido pelo banco para ser utilizado na geração do Nosso Número do boleto.
        /// </summary>
        public long InicioNossoNumero
        {
            get { return inicioNossoNumero; }
            set { inicioNossoNumero = value; }
        }

        private long fimNossoNumero;
        /// <summary>
        /// Ultimo valor que poderá ser utilizado para geração do Nosso Número do boleto.
        /// </summary>
        public long FimNossoNumero
        {
            get { return fimNossoNumero; }
            set { fimNossoNumero = value; }
        }

        private long proximoNossoNumero;
        /// <summary>
        /// Valor de Nosso Número a ser utilizado no proximo boleto.
        /// </summary>
        public long ProximoNossoNumero
        {
            get { return proximoNossoNumero; }
            set { proximoNossoNumero = value; }
        }

        private string demonstrativo;
        /// <summary>
        /// Texto a ser exibido na "Mensagem do Sacado" no boleto.
        /// </summary>
        public string Demonstrativo
        {
            get { return demonstrativo; }
            set { demonstrativo = value; }
        }

        private string instrucoes;
        /// <summary>
        /// Instruções a serem exibidas no boleto.
        /// </summary>
        public string Instrucoes
        {
            get { return instrucoes; }
            set { instrucoes = value; }
        }

        /// <summary>
        /// Percentual desconto aplicado até o vencimento.
        /// </summary>
        public decimal DescontoVencimento { get; set; }

        private string outros1;
        /// <summary>
        /// Campo adicional com informações variadas de cada banco
        /// </summary>
        public string Outros1
        {
            get { return outros1; }
            set { outros1 = value; }
        }

        private string outros2;
        /// <summary>
        /// Campo adicional com informações variadas de cada banco
        /// </summary>
        public string Outros2
        {
            get { return outros2; }
            set { outros2 = value; }
        }
        /// <summary>
        /// Retorna o título do campo Outros1 para determinado banco.
        /// </summary>
        public string TituloOutros1
        {
            get { return banco.TituloOutros1; }
        }
        /// <summary>
        /// Retorna o título do campo Outros2 para determinado banco.
        /// </summary>
        public string TituloOutros2
        {
            get { return banco.TituloOutros2; }
        }
        /// <summary>
        /// Lista com os layouts de arquivos remessa para determinado banco.
        /// </summary>
        public DefaultList LayoutsArquivoRemessa
        {
            get { return banco.LayoutsArquivoRemessa; }
        }
        /// <summary>
        /// Lista com os layouts de boletos existentes de determinado banco
        /// </summary>
        public DefaultList LayoutsBoleto
        {
            get { return banco.LayoutsBoleto; }
        }

        public ListaOcorrencias Ocorrencias { get; set; }

        private string pathArquivoRetorno;
        /// <summary>
        /// Caminho do arquivo retorno.
        /// </summary>
        public string PathArquivoRetorno
        {
            get { return pathArquivoRetorno; }
            set { pathArquivoRetorno = value; }
        }
        public string EspecieTitulo { get; set; }
        /// <summary>
        /// Carregar arquivo retorno. Utilize 'PathArquivoRetorno' para selecionar o arquivo.
        /// </summary>
        /// <param name="tipo">Tipo do arquivo retorno. (e.g. Cnab400, Cnab240)</param>
        public void CarregarArquivoRetorno(string tipo)
        {
            if (File.Exists(pathArquivoRetorno))
            {
                banco.AbrirArquivoRetorno(tipo, pathArquivoRetorno);
            }
            else
            {
                throw new ArgumentException("Caminho do arquivo retorno inválido.");
            }
        }
        /// <summary>
        /// Faz a impressao dos boletos gravados na CC.
        /// </summary>
        /// <returns>True se não ocorreram erros durante a impressão. Se ocorreram retorna False.</returns>
        public bool ImprimirBoletos(string pathLayout)
        {
            try
            {
                foreach (Boleto b in boletos)
                {
                    b.ImprimirBoleto(pathLayout, false);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }
        /// <summary>
        /// Gera o PDF dos boletos da CC.
        /// </summary>
        /// <param name="path">Local onde será gravado os PDFs gerados. 
        /// Caso o diretorio passado não exista, será gravado em 
        /// 'Configuracoes.CaminhoArquivosGerados'</param>
        /// <returns>True se não ocorreram erros durante a geração do PDF. Se ocorreram retorna False.</returns>
        public bool GerarPDFs(string path, string pathLayout)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    path = Configuracoes.CaminhoArquivosGerados;
                }

                foreach (Boleto b in boletos)
                {
                    b.GerarPDF(path + "/" + b.NumeroDocumento.ToNoFormated() + ".pdf", pathLayout, false);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }
        /// <summary>
        /// Abre a visualização dos boletos. Desta janela pode-se também imprimir ou importar para diversos arquivos.
        /// </summary>
        public void VisualizarBoleto(string pathLayout)
        {
            try
            {
                FastReport.Report r = Relatorios.PrepararRelatório(pathLayout, Relatorios.GetDataSetContaCorrente(this));
                r.Show();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Utilizado para abrir a janela do FastReport para desenhar relatórios.
        /// Obs.: Não habilite este comando para usuários finais.
        /// </summary>
        public void DesenharRelatorio(string pathLayout)
        {
            try
            {
                FastReport.Report r = Relatorios.PrepararRelatório(pathLayout, Relatorios.GetDataSetContaCorrente(this));
                r.Design();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Envia todos os boletos por email.
        /// Os dados do email de envio devem ser inseridos na classe Configuracoes.
        /// Os emails destino são inseridos em cada boleto.
        /// </summary>
        /// <returns>True se não ocorreram erros durante o envio do email. Se ocorreram retorna False.</returns>
        public bool EnviarBoletosPorEmail(string pathLayout)
        {
            try
            {
                foreach (Boleto b in boletos)
                {
                    b.EnviarBoletoPorEmail(pathLayout);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }
        /// <summary>
        /// Gera o arquivo remessa de todos os boletos da CC.
        /// </summary>
        /// <param name="tipo">Tipo de arquivo remessa de acordo com o banco selecionado</param>
        /// <param name="path">Pasta onde serão gerados os arquivos. </param>
        /// <returns>True se não ocorreram erros durante geração do arquivo. Se ocorreram retorna False.</returns>
        public bool GerarArquivoRemessa(string tipo, string path, ref string nome)
        {
            return banco.MontarArquivoRemessa(tipo, this, path, ref nome);
        }
        /// <summary>
        /// Calcula o DV do Nosso Número. 
        /// A linha digitável para impressão no boleto e o Código de Barras.
        /// </summary>
        /// <returns>True se não ocorreram erros durante o cálculo dos dados. Se ocorreram retorna False.</returns>
        public bool CalcularDadosDosBoletos()
        {
            try
            {
                foreach (Boleto b in boletos)
                {
                    b.CalcularDadosBoleto();
                }
            }
            catch
            {
                throw;
            }
            return true;
        }
    }
}
