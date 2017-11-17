using BoletoBancario.Conta;
using BoletoBancario.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoletoBancario.Bancos
{
    /// <summary>
    /// Classe abstrata para criação de bancos diferentes.
    /// </summary>
    public abstract class Banco
    {
        /// <summary>
        /// Construtor interno usado pela Dll.
        /// </summary>
        internal Banco() { }

        protected string nomeBanco;
        /// <summary>
        /// Nome do Banco.
        /// </summary>
        public string NomeBanco
        {
            get { return nomeBanco; }
            //set { nomeBanco = value; }
        }
        protected int numeroBanco;
        /// <summary>
        /// Número do Banco.
        /// </summary>
        public int NumeroBanco
        {
            get { return numeroBanco; }
            //set { numeroBanco = value; }
        }

        protected int dvNumeroBanco;

        public int DvNumeroBanco
        {
            get { return dvNumeroBanco; }
            set { dvNumeroBanco = value; }
        }

        protected string carteira;
        /// <summary>
        /// Número da Carteira.
        /// </summary>
        public string Carteira
        {
            get { return carteira; }
            internal set { carteira = value; }
        }

        protected string identificacao;
        /// <summary>
        /// Identificação do banco.
        /// </summary>
        public string Identificacao
        {
            get { return identificacao; }
            internal set { identificacao = value; }
        }

        private string mascaraAgencia;
        /// <summary>
        /// Mascara para número da agencia utilizada para cada banco.
        /// </summary>
        public string MascaraAgencia
        {
            get { return mascaraAgencia; }
            internal set { mascaraAgencia = value; }
        }


        private string mascaraCodigoCedente;
        /// <summary>
        /// Mascara para número do cedente utilizado para cada banco.
        /// </summary>
        public string MascaraCodigoCedente
        {
            get { return mascaraCodigoCedente; }
            set { mascaraCodigoCedente = value; }
        }

        private bool habilitarCodigoCedente;
        public bool HabilitarCodigoCedente
        {
            get { return habilitarCodigoCedente; }
            set { habilitarCodigoCedente = value; }
        }

        private string mascaraContaCorrente;
        /// <summary>
        /// Máscara do número da conta corrente para cada banco.
        /// </summary>
        public string MascaraContaCorrente
        {
            get { return mascaraContaCorrente; }
            internal set { mascaraContaCorrente = value; }
        }

        private string mascaraNossoNumero;
        /// <summary>
        /// Máscara do Nosso Número para cada banco.
        /// </summary>
        public string MascaraNossoNumero
        {
            get { return mascaraNossoNumero; }
            internal set { mascaraNossoNumero = value; }
        }

        #region outros1
        private string tituloOutros1;
        /// <summary>
        /// Titulo utilizado na configuração de Outros1 para cada banco.
        /// </summary>
        public string TituloOutros1
        {
            get { return tituloOutros1; }
            internal set { tituloOutros1 = value; }
        }

        private bool habilitarOutros1 = true;

        public bool HabilitarOutros1
        {
            get { return habilitarOutros1; }
            set { habilitarOutros1 = value; }
        }


        private string mascaraOutros1;
        /// <summary>
        /// Máscara do campo Outros1 de acordo com cada banco.
        /// </summary>
        public string MascaraOutros1
        {
            get { return mascaraOutros1; }
            internal set { mascaraOutros1 = value; }
        }
        #endregion

        #region outros 2

        private string tituloOutros2;
        /// <summary>
        /// Titulo utilizado na configuração de Outros2 para cada banco.
        /// </summary>
        public string TituloOutros2
        {
            get { return tituloOutros2; }
            internal set { tituloOutros2 = value; }
        }

        private string mascaraOutros2;
        /// <summary>
        /// Máscara do campo Outros2 de acordo com cada banco.
        /// </summary>
        public string MascaraOutros2
        {
            get { return mascaraOutros2; }
            internal set { mascaraOutros2 = value; }
        }
        private bool habilitarOutros2 = true;

        public bool HabilitarOutros2
        {
            get { return habilitarOutros2; }
            set { habilitarOutros2 = value; }
        }

        #endregion

        #region Listas Layouts
        // Não é usado
        private DefaultList layoutsArquivoRetorno = new DefaultList();
        /// <summary>
        /// Lista de Arquivos Retorno para cada banco.
        /// </summary>
        public DefaultList LayoutsArquivoRetorno
        {
            get { return layoutsArquivoRetorno; }
            internal set { layoutsArquivoRetorno = value; }
        }

        private DefaultList layoutsArquivoRemessa = new DefaultList();
        /// <summary>
        /// Lista de arquivos Remessa para cada banco.
        /// </summary>
        public DefaultList LayoutsArquivoRemessa
        {
            get { return layoutsArquivoRemessa; }
            internal set { layoutsArquivoRemessa = value; }
        }

        private DefaultList layoutsBoleto = new DefaultList();
        /// <summary>
        /// Lista com layouts dos boletos para impressão.
        /// </summary>
        public DefaultList LayoutsBoleto
        {
            get { return layoutsBoleto; }
            internal set { layoutsBoleto = value; }
        }

        private DefaultList especieTitulo = new DefaultList();
        /// <summary>
        /// Lista com Espécie de títulos.
        /// </summary>
        public DefaultList EspecieTitulo
        {
            get { return especieTitulo; }
            internal set { especieTitulo = value; }
        }
        

        #endregion

        #region Abstracts methods

        /// <summary>
        /// Calcula os dados do boleto com os métodos criados em casa subclasse de Banco.
        /// </summary>
        /// <param name="b">Boleto a ser feito os cálculos</param>
        internal abstract void CalcularDados(Boleto b);
        /// <summary>
        /// Lê o arquivo retorno.
        /// </summary>
        /// <param name="tipoRetorno">Tipo de Arquivo retorno.</param>
        /// <param name="path">Path do arquivo</param>
        /// <returns>Se houve erros ou não</returns>
        internal abstract bool AbrirArquivoRetorno(string tipoRetorno, string path);
        /// <summary>
        /// Monta o arquivo remessa da conta corrente
        /// </summary>
        /// <param name="tipoRemessa">Tipo do Arquivo Remessa</param>
        /// <param name="c">Conta Corrente</param>
        /// <param name="path">Local para gravar o arquivo gerado.</param>
        /// <returns></returns>
        internal abstract bool MontarArquivoRemessa(string tipoRemessa, ContaCorrente c, string path, ref string nome);
        #endregion
    }
}
