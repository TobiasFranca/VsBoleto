using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace BoletoBancario.Utilitarios
{
    public static class Utils
    {
        // recorta a string e retorna cortada no tamanho passado por parametro
        public static string InserirString(string str, int tam)
        {
            if (str == null)
            {
                return "";
            }

            if (str.Length <= tam)
            {
                return str;
            }
            else
            {
                return str.Substring(0, tam);
            }
        }

        // recorta o numero e retorna uma string no tamanho passado por parametro
        public static string InserirString(int str1, int tam)
        {
            string str = str1.ToString();
            if (str.Length <= tam)
            {
                return str;
            }
            else
            {
                return str.Substring(0, tam);
            }
        }

        // replica a string passada até o tamanho passado como parametro
        public static string ReplicarChar(string str, int tam)
        {
            string s = "";
            while (s.Length <= tam)
            {
                s += str;
            }

            return InserirString(s, tam);
        }

        public static string FormatNumber(string str, int tam)
        {
            decimal d = str.ToInt();
            return FormatNumber(d, tam, 0);
        }

        public static string FormatNumber(decimal d, int tam)
        {
            return FormatNumber(d, tam, 0);
        }

        public static string FormatNumber(string str, int tam, int dec)
        {
            decimal d = str.ToInt();
            return FormatNumber(d, tam, dec);
        }

        public static string FormatNumber(decimal d, int tam, int dec)
        {
            string form = new string('0', tam - dec) + "." + new string('0', dec);
            return d.ToString(form, CultureInfo.InvariantCulture).Replace(".", "");
        }

        // insere uma string a partir de uma determinada posição do array
        public static string InserirStringNoArray(string array, string str, int pos)
        {
            if (string.IsNullOrEmpty(str))
            {
                return "";
            }

            char[] arrays = array.ToArray();
            foreach (char b in str)
            {
                arrays[pos] = b;
                pos++;
            }
            return new string(arrays);
        }

        // insere uma string a entre uma determinada posição do array 
        public static string InserirStringNoArray(string array, string str, int pos, int fim)
        {
            char[] arrays = array.ToArray();
            if (string.IsNullOrEmpty(str))
            {
                return "";
            }

            foreach (char b in str)
            {
                if (fim <= 0)
                {
                    break;
                }

                arrays[pos] = b;
                pos++;
                fim--;
            }
            return new string(arrays);
        }
        // preenche com uma string antes ou depois de acordo com o tamanho fornecido
        public static string Insert(string aux, int tam, string preencher = " ", bool inserirAntes = false)
        {
            if (aux.Length > tam)
            {
                return aux.Substring(0, tam);
            }
            else
            {
                if (inserirAntes)
                {
                    while (aux.Length < tam)
                    {
                        aux = preencher + aux;
                    }
                }
                else
                {
                    while (aux.Length < tam)
                    {
                        aux += preencher;
                    }
                }
            }
            return aux;
        }

        //descobre o proximo numero multiplo de 10
        public static int ProximoMultiplo10(int num)
        {
            while (num % 10 != 0)
            {
                num++;
            }

            return num;
        }

        public static int CalcularModulo10(int num)
        {
            int index = 0;
            int soma = 0;
            foreach (char c in num.ToString())
            {
                if (index % 2 == 0)
                {
                    soma += c.ToString().ToInt() * 2;
                }
                else
                {
                    soma += c.ToString().ToInt();
                }

                index++;
            }

            while (soma > 9)
            {
                soma = soma.ToString()[0].ToString().ToInt() + soma.ToString()[1].ToString().ToInt();
            }

            return ProximoMultiplo10(soma) - soma;
        }

        //retorna um digito baseado no modulo de 11
        public static int CalcularModulo11(int num)
        {
            int digito = 11 - (num % 11);
            if (digito <= 1 || digito > 9)
            {
                digito = 0;
            }

            return digito;
        }

        public static string FormataNossoNumeroCNAB240BB(string NossoNumero, string DigitoVerificadorNossoNumero, string Convenio)
        {
            string texto = "";

            if (Convenio.Length == 4)
            {
                texto = Convenio;
                texto += Insert(NossoNumero + DigitoVerificadorNossoNumero, 7, "0");
            }
            else if (Convenio.Length == 6)
            {
                texto = Convenio;
                texto += Insert(NossoNumero + DigitoVerificadorNossoNumero, 5, "0", true);
            }
            else if (Convenio.Length == 7)
            {
                texto = Convenio;
                texto += Insert(NossoNumero, 10, "0", true);
            }

            return texto;
        }
    }
}
