using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace VsBoleto.Utilitarios
{
    public class Arquivo
    {
        /// <summary> Lê conteúdo de um arquivo genérico. </summary>
        /// <param name="pPath">Path do arquivo completo (incluído nome do arquivo).</param>
        /// <param name="pConteudo">Saída: conteúdo lido do arquivo.</param>
        /// <returns>Operação realizada com sucesso (True ou False).</returns>
        public static bool LeConteudo(string pPath, out string pConteudo)
        {
            StreamReader arq = null;

            try
            {
                arq = new StreamReader(pPath, Encoding.Default);
                pConteudo = arq.ReadToEnd();

                return true;
            }
            catch
            {
                pConteudo = "";
                return false;
            }
            finally
            {
                if (arq != null) arq.Close();
            }
        }


        public static bool EscreveConteudo(string pPath, string pConteudo)
        {
            return EscreveConteudo(pPath, pConteudo, false);
        }

        /// <summary> Escreve conteúdo em um arquivo genérico. </summary>
        /// <param name="pPath">Path do arquivo completo (incluído nome do arquivo).</param>
        /// <param name="pConteudo">Conteúdo a ser escrito no arquivo.</param>
        /// <returns>Operação realizada com sucesso (True ou False).</returns>
        public static bool EscreveConteudo(string pPath, string pConteudo, bool append)
        {
            StreamWriter arq = null;

            try
            {
                arq = new StreamWriter(pPath, append, Encoding.Default);
                arq.Write(pConteudo + Environment.NewLine);

                return true;
            }
            catch (Exception)
            {
                MessageBox.Show("Erro na escrita do arquivo! (" + pPath + ")");
                return false;
            }
            finally
            {
                if (arq != null) arq.Close();
            }
        }

        #region --- Formatos ----------------------------------------------------------------------
        public static string FormatoX(string texto, int tamanho, char padding)
        {
            return texto.PadRight(tamanho, padding);
        }

        public static string FormatoX(string texto, int tamanho)
        {
            return FormatoX(texto, tamanho, ' ');
        }

        public static string FormatoN_Inteiro(string numero, int tamanho)
        {
            return numero.PadLeft(tamanho, '0');
        }

        public static string FormatoN_Decimal(string numero, int tamanho)
        {
            if (numero.Trim() == "")
            {
                numero = "0";
            }

            double valor = Arredonda(Convert.ToDouble(numero), 2);
            numero = valor.ToString("0.00").RemoveChars(new char[] { '.', ',' });
            return numero.PadLeft(tamanho, '0');
        }

        public static string FormatoN_Decimal(string numero, int tamanho, int casasDecimais)
        {
            if (numero.Trim() == "")
            {
                numero = "0";
            }

            string formato = "0." + "".PadLeft(casasDecimais, '0');

            double valor = Arredonda(Convert.ToDouble(numero), casasDecimais);
            numero = valor.ToString(formato).RemoveChars(new char[] { '.', ',' });
            return numero.PadLeft(tamanho, '0');
        }

        public static string FormatoD(DateTime data)
        {
            return data.FormataData_AAAAMMDD();
        }

        public static string FormatoH(DateTime hora)
        {
            return hora.FormataHora_HHMMSS();
        }

        private static double Arredonda(double valor, int decimais)
        {
            double fator = Math.Pow(10, decimais);
            double valorBase = Convert.ToDouble((valor * fator).ToString());
            double parteInt = Math.Truncate(valorBase);
            double parteDec = (valorBase - parteInt);

            if (parteDec >= 0.5)
            {
                ++parteInt;
            }
            if (parteDec <= -0.5)
            {
                --parteInt;
            }

            return (parteInt / fator);
        }
        #endregion --- Formatos (FIM) -------------------------------------------------------------
    }


    //public class ArquivoINI
    //{
    //    [DllImport("kernel32")]
    //    private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
    //    [DllImport("kernel32")]
    //    private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

    //    /// <summary> Lê um dado do arquivo INI. </summary>
    //    /// <param name="path">Path do arquivo INI.</param>
    //    /// <param name="secao">Nome da seção.</param>
    //    /// <param name="chave">Nome da chave.</param>
    //    /// <param name="criptografado">Informa se o valor do campo está criptografado.</param>
    //    /// <param name="multiplasLinhas">Informa se o valor do campo apresenta múltiplas linhas.</param>
    //    /// <param name="valorDefault">Valor default para retorno em caso de inexistência da chave.</param>
    //    public static string LeString(string path, string secao, string chave,
    //                                  bool criptografado = false, bool multiplasLinhas = false, string valorDefault = "")
    //    {
    //        StringBuilder temp = new StringBuilder(255);
    //        int i = GetPrivateProfileString(secao, chave, "", temp, 255, path);

    //        string retorno = temp.ToString().Trim() == "" ? valorDefault : temp.ToString();

    //        if (multiplasLinhas) retorno = retorno.Replace("|", Environment.NewLine);

    //        return (criptografado ? retorno.Decripta() : retorno);
    //    }

    //    /// <summary> Lê um dado do arquivo INI. </summary>
    //    /// <param name="path">Path do arquivo INI.</param>
    //    /// <param name="secao">Nome da seção.</param>
    //    /// <param name="chave">Nome da chave.</param>
    //    /// <param name="valorDefault">Valor default para retorno em caso de inexistência da chave.</param>
    //    public static bool LeBool(string path, string secao, string chave, bool valorDefault = false)
    //    {
    //        StringBuilder temp = new StringBuilder(255);
    //        int i = GetPrivateProfileString(secao, chave, "", temp, 255, path);

    //        string retorno = temp.ToString();
    //        if (retorno.Trim() == "")
    //            return valorDefault;
    //        else
    //            if (retorno == "0") return false;
    //        else return true;
    //    }

    //    /// <summary> Escreve um dado no arquivo INI. </summary>
    //    /// <param name="path">Path do arquivo INI.</param>
    //    /// <param name="secao">Nome da seção.</param>
    //    /// <param name="chave">Nome da chave.</param>
    //    /// <param name="valor">Valor do dado.</param>
    //    /// <param name="criptografar">Informa se o valor do campo deve ser criptografado.</param>
    //    /// <param name="multiplasLinhas">Informa se o valor do campo apresenta múltiplas linhas.</param>
    //    public static void EscreveString(string path, string secao, string chave, string valor,
    //                                     bool criptografar = false, bool multiplasLinhas = false)
    //    {
    //        if (criptografar) valor = valor.Encripta();
    //        if (multiplasLinhas)
    //        {
    //            valor = valor.RemoveChar('\r').Replace("\n", Environment.NewLine);
    //            valor = valor.Replace(Environment.NewLine, "|");
    //        }

    //        WritePrivateProfileString(secao, chave, valor, path);
    //    }

    //    /// <summary> Escreve um dado no arquivo INI. </summary>
    //    /// <param name="path">Path do arquivo INI.</param>
    //    /// <param name="secao">Nome da seção.</param>
    //    /// <param name="chave">Nome da chave.</param>
    //    /// <param name="valor">Valor do dado.</param>
    //    public static void EscreveBool(string path, string secao, string chave, bool valor)
    //    {
    //        string strValor;
    //        if (valor) strValor = "1";
    //        else strValor = "0";

    //        WritePrivateProfileString(secao, chave, strValor, path);
    //    }
    //}
}
