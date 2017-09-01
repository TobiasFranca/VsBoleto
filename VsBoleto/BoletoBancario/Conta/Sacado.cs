using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoletoBancario.Conta
{
    public class Sacado
    {
        /// <summary>
        /// Construtor para ser usado pela dll
        /// </summary>
        internal Sacado() { }

        private Boleto boleto;
        /// <summary>
        /// Boleto de cobrança do sacado.
        /// </summary>
        public Boleto Boleto
        {
            get { return boleto; }
            set { boleto = value; }
        }

        private string codigoSistema;
        public string CodigoSistema
        {
            get { return codigoSistema; }
            set { codigoSistema = value; }
        }

        private string nome;
        /// <summary>
        /// Nome do sacado.
        /// </summary>
        public string Nome
        {
            get { return nome; }
            set { nome = value; }
        }

        private string razao;
        public string Razao
        {
            get { return razao; }
            set { razao = value; }
        }

        private string cpfCnpj;
        /// <summary>
        /// Cpf ou Cnpj do sacado
        /// </summary>
        public string CpfCnpj
        {
            get { return cpfCnpj; }
            set { cpfCnpj = value; }
        }

        private string endereco;
        /// <summary>
        /// Logradouro do sacado.
        /// </summary>
        public string Endereco
        {
            get { return endereco; }
            set { endereco = value; }
        }

        private string numEndereco;
        public string NumEndereco
        {
            get { return numEndereco; }
            set { numEndereco = value; }
        }

        private string bairro;
        /// <summary>
        /// Bairro do sacado.
        /// </summary>
        public string Bairro
        {
            get { return bairro; }
            set { bairro = value; }
        }

        private string cidade;
        /// <summary>
        /// Cidade do sacado.
        /// </summary>
        public string Cidade
        {
            get { return cidade; }
            set { cidade = value; }
        }

        private string cep;
        /// <summary>
        /// Cep da cidade.
        /// </summary>
        public string Cep
        {
            get { return cep; }
            set { cep = value; }
        }

        private string estado;
        /// <summary>
        /// Estado do sacado (e.g. ES, MG, RJ, SP)
        /// </summary>
        public string Estado
        {
            get { return estado; }
            set { estado = value; }
        }

        private string email;
        /// <summary>
        /// Email do sacado. Utilizado para envio de boletos.
        /// </summary>
        public string Email
        {
            get { return email; }
            set { email = value; }
        }
    }
}
