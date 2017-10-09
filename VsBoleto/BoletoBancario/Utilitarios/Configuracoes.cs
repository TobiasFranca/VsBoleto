using System;
using System.IO;

namespace BoletoBancario.Utilitarios
{
    public static class Configuracoes
    {
        public static void VerificarECriarDiretorios(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao criar diretório " + ex.Message);
            }
        }

        /// <summary>
        /// Caminho da dll para utilizar em outros paths.
        /// </summary>
        private static string pathAssembly = Path.GetDirectoryName(
            System.Reflection.Assembly.GetExecutingAssembly().Location);

        /// <summary>
        /// Impressora padrão.
        /// </summary>
        public static string ImpressoraPadrao = "";
        /// <summary>
        /// Caminho para imagens utilizadas pela dll
        /// </summary>
        public static string CaminhoImagens = pathAssembly + "/ArquivoSistema";
        /// <summary>
        /// Caminho para arquivos padrões utilizados pela dll
        /// </summary>
        public static string CaminhoArquivosPadroes = pathAssembly + "/ArquivoSistema";
        /// <summary>
        /// Caminho para arquivos que serão gerados pela dll
        /// </summary>
        public static string CaminhoArquivosGerados = pathAssembly + "/PDF";
        /// <summary>
        /// Porta do servidor de emails
        /// </summary>
        public static string Email_Porta = "";
        /// <summary>
        /// Host do servidor de emails
        /// </summary>
        public static string Email_Servidor = "";
        /// <summary>
        /// Usuário do email.
        /// </summary>
        public static string Email_Usuario = "";
        /// <summary>
        /// Senha do email.
        /// </summary>
        public static string Email_Senha = "";
        /// <summary>
        /// Nome para ser usado no envio.
        /// </summary>
        public static string Email_De = "";
        /// <summary>
        /// Asunto do email.
        /// </summary>
        public static string Email_Assunto_Padrao = "";
        /// <summary>
        /// Texto padrão para ser utilizado no envio de email.
        /// Pode-se usar a propriedade do envio 'corpoHtml'
        /// para ativar ou não as tags html no corpo do email.
        /// </summary>
        public static string Email_Corpo_Padrao = "";

        public static bool Email_Html = false;

        public static bool Email_Usar_SSL = false;

        /// <summary>
        /// Layout do boleto selecionado que será usado na hora de gerar o mesmo.
        /// </summary>
        public static string LayoutBoleto = "";
        /// <summary>
        /// Layout do Arquivo Remessa selecionado para geração do mesmo.
        /// </summary>
        public static string LayoutArquivoRemessa = "";
    }
}
