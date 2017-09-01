using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

/// <summary>
/// Classe de extensão para efetuar operações recorrentes em strings
/// </summary>
public static class StringExtension
{
    /// <summary>
    /// Método para transformar uma string no formato padrão de um CPF: 000.000.000-00
    /// </summary>
    /// <param name="str">A string que representa o CPF</param>
    /// <returns>Uma string que representa o CPF informado formatado</returns>
    /// <exception cref="string">Caso a string passada não esteja em um formato válido é retornado o valor: "___.___.___-__"</exception>
    public static string ToFormatedCPF(this string str)
    {
        //Limpar a string
        str = str.Replace(".", "").Replace("-", "").Replace("/", "").Replace("\\", "");
        if (str.Length != 11)
            return "___.___.___-__";

        StringBuilder retorno = new StringBuilder();
        retorno.Append(str.Length > 2 ? str.Substring(0, 3) + "." : "");
        retorno.Append(str.Length > 5 ? str.Substring(3, 3) + "." : "");
        retorno.Append(str.Length > 8 ? str.Substring(6, 3) + "-" : "");
        retorno.Append(str.Length > 10 ? str.Substring(9, 2) : "");
        return retorno.ToString();
    }

    /// <summary>
    /// Método para transformar uma string no formato padrão de um CNPJ: 00.000.000/0000-00
    /// </summary>
    /// <param name="str">A string que representa o CNPJ</param>
    /// <returns>Uma string que representa o CNPJ informado formatado</returns>
    public static string ToFormatedCNPJ(this string str)
    {
        //Limpar a string
        str = str.Replace(".", "").Replace("-", "").Replace("/", "").Replace("\\", "");
        if (str.Length != 14)
            return "__.___.___/____-__";

        StringBuilder retorno = new StringBuilder();
        retorno.Append(str.Length > 1 ? str.Substring(0, 2) + "." : "");
        retorno.Append(str.Length > 4 ? str.Substring(2, 3) + "." : "");
        retorno.Append(str.Length > 7 ? str.Substring(5, 3) + "/" : "");
        retorno.Append(str.Length > 11 ? str.Substring(8, 4) + "-" : "");
        retorno.Append(str.Length > 13 ? str.Substring(12, 2) : "");
        return retorno.ToString();
    }

    public static decimal ToDecimal(this string str)
    {
        if (String.IsNullOrEmpty(str))
            return new decimal(0);
        return Convert.ToDecimal(str, new CultureInfo("pt-BR"));
    }

    public static string ToCurrency(this string str)
    {
        try
        {
            if (String.IsNullOrEmpty(str))
                return "R$ 0,00";
            Decimal d = Convert.ToDecimal(str);
            return String.Format("{0:C}", d);
        }
        catch (Exception)
        {
            return "R$ 0,00";
        }
    }

    public static long ToLong(this string str)
    {
        try
        {
            long l = long.Parse(str);
            return l;
        }
        catch (Exception)
        {
            return new long();
        }
    }

    public static int ToInt32(this string str)
    {
        try
        {
            if (String.IsNullOrEmpty(str))
                return new Int32();
            return Convert.ToInt32(str);
        }
        catch (Exception)
        {
            return new Int32();
        }
    }

    public static float ToFloat(this string str)
    {
        if (String.IsNullOrEmpty(str))
            return new float();
        return float.Parse(str);
    }

    public static DateTime ToDateTime(this string str)
    {
        if (String.IsNullOrEmpty(str))
            return new DateTime(1754, 01, 01);
        if (Convert.ToDateTime(str) <= new DateTime(1754, 1, 1))
            return new DateTime(1754, 01, 01);
        return Convert.ToDateTime(str);
    }

    public static DateTime ToDateTimeSQL(this string str)
    {
        if (String.IsNullOrEmpty(str))
            return new DateTime(1754, 01, 01);
        if (Convert.ToDateTime(str) <= new DateTime(1754, 1, 1))
            return new DateTime(1754, 01, 01);
        return Convert.ToDateTime(str);
    }

    public static double ToDouble(this string str)
    {
        if (String.IsNullOrEmpty(str))
            return new double();
        return Convert.ToDouble(str);
    }

    public static bool IsNotNullOrEmpty(this string str)
    {
        return !String.IsNullOrEmpty(str);
    }

    public static bool IsStrongPassword(this string str)
    {
        bool isStrong = Regex.IsMatch(str, @"[\d]");
        if (isStrong) isStrong = Regex.IsMatch(str, @"[a-z]");
        if (isStrong) isStrong = Regex.IsMatch(str, @"[A-Z]");
        if (isStrong) isStrong = Regex.IsMatch(str, @"[\s~!@#\$%\^&\*\(\)\{\}\|\[\]\\:;'?,.`+=<>\/]");
        if (isStrong) isStrong = str.Length > 7;
        return isStrong;
    }

    public static bool ContainsAny(this string str, params string[] values)
    {
        if (!string.IsNullOrEmpty(str) || values.Length == 0)
        {
            foreach (string value in values)
            {
                if (str.Contains(value))
                    return true;
            }
        }

        return false;
    }

    public static bool IsDate(this string str)
    {
        if (!string.IsNullOrEmpty(str))
        {
            DateTime dt;
            return (DateTime.TryParse(str, out dt));
        }
        else
        {
            return false;
        }
    }

    public static string ConvertHtmlText(this string str)
    {
        return str.Replace("\n", "<br>");
    }

    public static string ToHtml(this string s)
    {
        s = s.Replace("ó", "&oacute;");
        s = s.Replace("ò", "&ograve;");
        s = s.Replace("ô", "&ocirc;");
        s = s.Replace("õ", "&otilde;");
        s = s.Replace("ö", "&ouml;");
        s = s.Replace("á", "&aacute;");
        s = s.Replace("à", "&agrave;");
        s = s.Replace("â", "&acirc;");
        s = s.Replace("ã", "&atilde;");
        s = s.Replace("ä", "&auml;");
        s = s.Replace("é", "&eacute;");
        s = s.Replace("è", "&egrave;");
        s = s.Replace("ê", "&ecirc;");
        s = s.Replace("ú", "&uacute;");
        s = s.Replace("ù", "&ugrave;");
        s = s.Replace("û", "&ucirc;");
        s = s.Replace("ü", "&uuml;");
        s = s.Replace("í", "&iacute;");
        s = s.Replace("ì", "&igrave;");
        s = s.Replace("ç", "&ccedil;");
        s = s.Replace("Ó", "&Oacute;");
        s = s.Replace("Ò", "&Ograve;");
        s = s.Replace("Ô", "&Ocirc;");
        s = s.Replace("Õ", "&Otilde;");
        s = s.Replace("Ö", "&Ouml;");
        s = s.Replace("Á", "&Aacute;");
        s = s.Replace("À", "&Agrave;");
        s = s.Replace("Â", "&Acirc;");
        s = s.Replace("Ã", "&Atilde;");
        s = s.Replace("Ä", "&Auml;");
        s = s.Replace("É", "&Eacute;");
        s = s.Replace("È", "&Egrave;");
        s = s.Replace("Ê", "&Ecirc;");
        s = s.Replace("Ú", "&Uacute;");
        s = s.Replace("Ù", "&Ugrave;");
        s = s.Replace("Û", "&Ucirc;");
        s = s.Replace("Ü", "&Uuml;");
        s = s.Replace("Í", "&Iacute;");
        s = s.Replace("Ì", "&Igrave;");
        s = s.Replace("Ç", "&Ccedil;");
        s = s.Replace("º", "&ordm;");
        s = s.Replace("ª", "&ordf;");
        return s;
    }

    /// <summary>
    /// Encrypt a string using dual encryption method. Return a encrypted cipher Text
    /// </summary>
    /// <param name="toEncrypt">string to be encrypted</param>
    /// <param name="useHashing">use hashing? send to for extra secirity</param>
    /// <returns></returns>
    public static string Encrypt(this string toEncrypt, bool useHashing)
    {
        byte[] keyArray;
        byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

        // Get the key from config file
        string key = "#@$%#$¨&¨56&*RTGD456456FSFSG56T456456Y¨&$%YT#R%¨&WEFW5fsdfsd456%¨&¨%&6465";
        //System.Windows.Forms.MessageBox.Show(key);
        if (useHashing)
        {
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            hashmd5.Clear();
        }
        else
            keyArray = UTF8Encoding.UTF8.GetBytes(key);

        TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
        tdes.Key = keyArray;
        tdes.Mode = CipherMode.ECB;
        tdes.Padding = PaddingMode.PKCS7;

        ICryptoTransform cTransform = tdes.CreateEncryptor();
        byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
        tdes.Clear();
        return Convert.ToBase64String(resultArray, 0, resultArray.Length);
    }

    /// <summary>
    /// DeCrypt a string using dual encryption method. Return a DeCrypted clear string
    /// </summary>
    /// <param name="cipherString">encrypted string</param>
    /// <param name="useHashing">Did you use hashing to encrypt this data? pass true is yes</param>
    /// <returns></returns>
    public static string Decrypt(this string cipherString, bool useHashing)
    {
        byte[] keyArray;
        byte[] toEncryptArray = Convert.FromBase64String(cipherString);

        string key = "#@$%#$¨&¨56&*RTGD456456FSFSG56T456456Y¨&$%YT#R%¨&WEFW5fsdfsd456%¨&¨%&6465";
        if (useHashing)
        {
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            hashmd5.Clear();
        }
        else
            keyArray = UTF8Encoding.UTF8.GetBytes(key);

        TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
        tdes.Key = keyArray;
        tdes.Mode = CipherMode.ECB;
        tdes.Padding = PaddingMode.PKCS7;

        ICryptoTransform cTransform = tdes.CreateDecryptor();
        byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

        tdes.Clear();
        return UTF8Encoding.UTF8.GetString(resultArray);
    }

    /// <summary>
    /// Remove os Caracteres Especiais de Uma String Passada
    /// <para>'=', '\\', ';', '.', ':', ',', '+', '*','-','!','#','£','%','[',']','\"','\'','´','`','_','/','|','¢','¬','&','<','>','~','^','(',')','☺'</para>
    /// </summary>
    /// <param name="x"> string </param>
    /// <returns>A string sem os caracteres</returns>
    public static string RemoverCaracteresEspeciais(this string x)
    {
        char[] trim = { '=', '\\', ';', '.', ':', ',', '+', '*', '-', '!', '#', '£', '%', '[', ']', '\"', '\'', '´', '`', '_', '/', '|', '¢', '¬', '&', '<', '>', '~', '^', '(', ')', '☺' };
        int pos;
        while ((pos = x.IndexOfAny(trim)) >= 0)
        {
            x = x.Remove(pos, 1);
        }
        return x;
    }

    /// <summary>
    /// Substitui Caracteres com assento ou ç
    /// <para> vogais e ç</para>
    /// </summary>
    /// <param name="x">string</param>
    /// <returns>A string modificada</returns>
    public static string SubstituirCaracteresPtBr(this string x)
    {

        x = x.Replace("á", "a");
        x = x.Replace("ã", "a");
        x = x.Replace("â", "a");
        x = x.Replace("à", "a");
        x = x.Replace("Á", "A");
        x = x.Replace("Ã", "A");
        x = x.Replace("Â", "A");
        x = x.Replace("À", "A");

        x = x.Replace("é", "e");
        x = x.Replace("ê", "e");
        x = x.Replace("è", "e");
        x = x.Replace("É", "E");
        x = x.Replace("Ê", "E");
        x = x.Replace("È", "E");

        x = x.Replace("í", "i");
        x = x.Replace("î", "i");
        x = x.Replace("ì", "i");
        x = x.Replace("Í", "I");
        x = x.Replace("Î", "I");
        x = x.Replace("Ì", "I");

        x = x.Replace("ó", "o");
        x = x.Replace("õ", "o");
        x = x.Replace("ô", "o");
        x = x.Replace("ò", "o");
        x = x.Replace("Ó", "O");
        x = x.Replace("Õ", "O");
        x = x.Replace("Ô", "O");
        x = x.Replace("Ò", "O");

        x = x.Replace("ú", "u");
        x = x.Replace("û", "u");
        x = x.Replace("ù", "u");
        x = x.Replace("Ú", "U");
        x = x.Replace("Û", "U");
        x = x.Replace("Ù", "U");

        x = x.Replace("ç", "c");
        x = x.Replace("Ç", "C");

        return x;
    }

    /// <summary>
    /// Remove Caracteres que podem comprometer códigos SQL
    /// <para> '' "" % + </para>
    /// </summary>
    /// <param name="x">string</param>
    /// <returns>A string modificada</returns>
    public static string TratarSQLInjection(this string x)
    {
        char[] trim = { '=', '\\', ';', ':', ',', '+', '*', '-', '!', '#', '£', '%', '[', ']', '\"', '\'', '_', '/', '|', '¢', '&', '<', '>', '(', ')' };
        int pos;
        while ((pos = x.IndexOfAny(trim)) >= 0)
        {
            x = x.Remove(pos, 1);
        }
        return x;
    }

    public static string SubstituirAcentos(this string x)
    {
        string s = x.Normalize(NormalizationForm.FormD);

        StringBuilder sb = new StringBuilder();

        for (int k = 0; k < s.Length; k++)
        {
            UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(s[k]);
            if (uc != UnicodeCategory.NonSpacingMark)
                sb.Append(s[k]);
        }
        return sb.ToString();
    }

    public static bool IsCnpj(this string cnpjCpf)
    {
        cnpjCpf = cnpjCpf.Replace(".", "").Replace("-", "").Replace("/", "").Replace("\\", "");
        if (cnpjCpf.Length == 14)
            return true;
        return false;
    }

    public static bool IsCpf(this string cnpjCpf)
    {
        cnpjCpf = cnpjCpf.Replace(".", "").Replace("-", "").Replace("/", "").Replace("\\", "");
        if (cnpjCpf.Length == 11)
            return true;
        return false;
    }

    public static string ToNoFormated(this string cnpjCpf)
    {
        if (string.IsNullOrEmpty(cnpjCpf))
            return "";
        return cnpjCpf.Replace(".", "").Replace("-", "").Replace("/", "").Replace("\\", "");
    }

    public static string ToCpfCnpj(this string cnpjCpf)
    {
        cnpjCpf = cnpjCpf.Replace(".", "").Replace("-", "").Replace("/", "").Replace("\\", "");
        if (cnpjCpf.Length == 14)
            return ToFormatedCNPJ(cnpjCpf);
        else if (cnpjCpf.Length == 11)
            return ToFormatedCPF(cnpjCpf);
        return "";
    }


    public static string Capitalizar(this string value)
    {
        return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value);
    }
}
