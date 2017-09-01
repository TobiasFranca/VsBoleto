using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace VsBoleto.Utilitarios
{
    /// <summary>
    /// 
    /// </summary>
    public static class ThisString
    {
        /// <summary> Remove uma lista de caracteres de um string. </summary>
        /// <param name="pTexto"> String inicial. </param>
        /// <param name="pCharsParaRemover"> Vetor de caracteres a serem removidos. </param>
        /// <returns> String final com os caracteres removidos. </returns>
        public static string RemoveChars(this string pTexto, char[] pCharsParaRemover)
        {
            if (pTexto.Length > 0)
            {
                StringBuilder novoTexto = new StringBuilder(pTexto.Length);
                CharEnumerator enumerator = pTexto.GetEnumerator();
                bool contem;

                while (enumerator.MoveNext())
                {
                    contem = false;

                    foreach (char c in pCharsParaRemover)
                    {
                        if (c == enumerator.Current)
                        {
                            contem = true;
                            break;
                        }
                    }

                    if (!contem)
                    {
                        novoTexto.Append(enumerator.Current);
                    }
                }

                return novoTexto.ToString();
            }
            else
            {
                return "";
            }
        }

        /// <summary> Remove caracter de um string. </summary>
        /// <param name="pTexto"> String inicial. </param>
        /// <param name="pCharParaRemover"> Caracter a ser removido. </param>
        /// <returns> String final com o caracter removido. </returns>
        public static string RemoveChar(this string pTexto, char pCharParaRemover)
        {
            return RemoveChars(pTexto, new char[] { pCharParaRemover });
        }

        /// <summary> Remove caracteres de pontuação de um string. </summary>
        /// <param name="pTexto"> String inicial. </param>
        /// <returns> String final com os caracteres removidos. </returns>
        public static string RemovePunctuation(this string pTexto)
        {
            return RemoveChars(pTexto, new char[] { ',', '.', '-', ';', '/', '\\', '?', '!' });
        }

        /// <summary>  </summary>
        /// <param name="pString"></param>
        /// <returns></returns>
        public static string EscreveSQL(this string pString)
        {
            return "'" + pString + "'";
        }

        public static string Encripta(this string pTexto)
        {
            byte[] vetorChave;
            byte[] vetorDecriptado;

            if (pTexto == null || pTexto.Length == 0)
                return "";

            try
            {
                vetorDecriptado = UTF8Encoding.UTF8.GetBytes(pTexto);
            }
            catch
            {
                return pTexto;
            }

            string key = "??*(1%R!<G$-=6@B**!;";

            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            vetorChave = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            hashmd5.Clear();

            TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();
            DES.Key = vetorChave;
            DES.Mode = CipherMode.ECB;
            DES.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = DES.CreateEncryptor();
            byte[] vetorEncriptado = cTransform.TransformFinalBlock(vetorDecriptado, 0, vetorDecriptado.Length);
            DES.Clear();

            return Convert.ToBase64String(vetorEncriptado, 0, vetorEncriptado.Length);
        }

        public static string Decripta(this string pTexto)
        {
            byte[] vetorChave;
            byte[] vetorEncriptado;

            if (pTexto == null || pTexto.Length == 0)
                return "";

            try
            {
                vetorEncriptado = Convert.FromBase64String(pTexto);
            }
            catch
            {
                return pTexto;
            }

            string key = "??*(1%R!<G$-=6@B**!;";

            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            vetorChave = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            hashmd5.Clear();

            TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();
            DES.Key = vetorChave;
            DES.Mode = CipherMode.ECB;
            DES.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = DES.CreateDecryptor();
            byte[] vetorDecriptado = cTransform.TransformFinalBlock(vetorEncriptado, 0, vetorEncriptado.Length);
            DES.Clear();

            return UTF8Encoding.UTF8.GetString(vetorDecriptado);
        }

        /// <summary> Calcula o Md5SUM de uma string. </summary>
        /// <param name="pTexto"> String de entrada. </param>
        /// <returns> String de 32 caracteres representando o MD5SUM da string fornecida como parâmetro. </returns>
        public static string MD5Sum(this string pTexto)
        {
            MD5 md5Hasher = MD5.Create();

            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(pTexto));
            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("X2"));
            }
            return sBuilder.ToString().ToLower();
        }

        public static string CalculateMD5Hash(this string input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class ThisDouble
    {
        /// <summary>  </summary>
        /// <param name="pDouble"></param>
        /// <returns></returns>
        public static string EscreveSQL(this double pDouble)
        {
            return pDouble.ToString().RemoveChar('.').Replace(',', '.');
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class ThisInt
    {
        /// <summary>  </summary>
        /// <param name="pInt"></param>
        /// <returns></returns>
        public static string EscreveSQL(this int pInt)
        {
            return pInt.ToString();
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public static class ThisData
    {
        #region --- Formata Data ------------------------------------------------------------------
        /// <summary> Retorna um string no formato DDMMAAAA relativo a um objeto DateTime. </summary>
        /// <param name="pData"> Objeto relativo à data. </param>
        /// <returns> String formatado. </returns>
        public static string FormataData_DDMMAAAA(this DateTime pData)
        {
            return pData.Day.ToString("00") +
                   pData.Month.ToString("00") +
                   pData.Year.ToString("0000");
        }

        /// <summary> Retorna um string no formato DDMMAA relativo a um objeto DateTime. </summary>
        /// <param name="pData"> Objeto relativo à data. </param>
        /// <returns> String formatado. </returns>
        public static string FormataData_DDMMAA(this DateTime pData)
        {
            return pData.Day.ToString("00") +
                   pData.Month.ToString("00") +
                   pData.Year.ToString("0000").Substring(2, 2);
        }

        /// <summary> Retorna um string no formato DD/MM/AAAA relativo a um objeto DateTime. </summary>
        /// <param name="pData"> Objeto relativo à data. </param>
        /// <returns> String formatado. </returns>
        public static string FormataData_DD_MM_AAAA(this DateTime pData)
        {
            return pData.Day.ToString("00") + "/" +
                   pData.Month.ToString("00") + "/" +
                   pData.Year.ToString("0000");
        }

        /// <summary> Retorna um string no formato DD/MM/AA relativo a um objeto DateTime. </summary>
        /// <param name="pData"> Objeto relativo à data. </param>
        /// <returns> String formatado. </returns>
        public static string FormataData_DD_MM_AA(this DateTime pData)
        {
            return pData.Day.ToString("00") + "/" +
                   pData.Month.ToString("00") + "/" +
                   pData.Year.ToString("0000").Substring(2, 2);
        }

        /// <summary> Retorna um string no formato AAAA/MM/DD relativo a um objeto DateTime. </summary>
        /// <param name="pData"> Objeto relativo à data. </param>
        /// <returns> String formatado. </returns>
        public static string FormataData_AAAA_MM_DD(this DateTime pData)
        {
            return pData.Year.ToString("0000") + "/" +
                   pData.Month.ToString("00") + "/" +
                   pData.Day.ToString("00");
        }

        /// <summary> Retorna um string no formato AAAAMMDD relativo a um objeto DateTime. </summary>
        /// <param name="pData"> Objeto relativo à data. </param>
        /// <returns> String formatado. </returns>
        public static string FormataData_AAAAMMDD(this DateTime pData)
        {
            return pData.Year.ToString("0000") +
                   pData.Month.ToString("00") +
                   pData.Day.ToString("00");
        }

        /// <summary> Retorna um string no formato AAAAMM relativo a um objeto DateTime. </summary>
        /// <param name="pData"> Objeto relativo à data. </param>
        /// <returns> String formatado. </returns>
        public static string FormataData_AAAAMM(this DateTime pData)
        {
            return pData.Year.ToString("0000") +
                   pData.Month.ToString("00");
        }

        /// <summary> Retorna um string no formato HHMMSS relativo a um objeto DateTime. </summary>
        /// <param name="pData"> Objeto relativo à data. </param>
        /// <returns> String formatado. </returns>
        public static string FormataHora_HHMMSS(this DateTime pHora)
        {
            return pHora.Hour.ToString("00") +
                   pHora.Minute.ToString("00") +
                   pHora.Second.ToString("00");
        }

        /// <summary> Retorna um string no formato HH:MM:SS relativo a um objeto DateTime. </summary>
        /// <param name="pData"> Objeto relativo à data. </param>
        /// <returns> String formatado. </returns>
        public static string FormataHora_HH_MM_SS(this DateTime pHora)
        {
            return pHora.Hour.ToString("00") + ":" +
                   pHora.Minute.ToString("00") + ":" +
                   pHora.Second.ToString("00");
        }
        #endregion --- Formata Data (FIM) ---------------------------------------------------------

        /// <summary>  </summary>
        /// <param name="pData">Objeto relativo à data.</param>
        /// <returns></returns>
        public static string EscreveSQLData(this DateTime pData)
        {
            return "'" + pData.Date.ToString().Replace('/', '.') + "'";
        }

        /// <summary>  </summary>
        /// <param name="pHora">Objeto relativo à hora.</param>
        /// <returns></returns>
        public static string EscreveSQLHora(this DateTime pHora)
        {
            return "'" + pHora.Hour.ToString().PadLeft(2, '0') + ":" +
                         pHora.Minute.ToString().PadLeft(2, '0') + ":" +
                         pHora.Second.ToString().PadLeft(2, '0') + "'";
        }

        /// <summary>  </summary>
        /// <param name="pDataHora">Objeto relativo à data/hora.</param>
        /// <returns></returns>
        public static string EscreveSQLDataHora(this DateTime pDataHora)
        {
            return "'" + pDataHora.ToString().Replace('/', '.') + "'";
        }
    }
}
