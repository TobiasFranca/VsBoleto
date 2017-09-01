using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoletoBancario.Conta
{
    public class Cedente
    {
        /// <summary>
        /// Contas correntes do cedente.
        /// </summary>
        private IList<ContaCorrente> contasCorrentes = new List<ContaCorrente>();

        //public IList<ContaCorrente> ContasCorrentes
        //{
        //    get { return contasCorrentes; }
        //    set { contasCorrentes = value; }
        //}

        private string nomeCedente;
        /// <summary>
        /// Nome do cedente.
        /// </summary>
        public string NomeCedente
        {
            get { return nomeCedente; }
            set { nomeCedente = value; }
        }

        private string cpfCnpj;
        /// <summary>
        /// Cpf ou Cnpj do cedente.
        /// </summary>
        public string CpfCnpj
        {
            get { return cpfCnpj; }
            set { cpfCnpj = value; }
        }

        private string endereco;
        /// <summary>
        /// Logradouro do cedente
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
        /// Bairro do cedente
        /// </summary>
        public string Bairro
        {
            get { return bairro; }
            set { bairro = value; }
        }

        private string cidade;
        /// <summary>
        /// Cidade do cedente
        /// </summary>
        public string Cidade
        {
            get { return cidade; }
            set { cidade = value; }
        }

        private string cep;
        /// <summary>
        /// Cep da cidade
        /// </summary>
        public string Cep
        {
            get { return cep; }
            set { cep = value; }
        }

        private string estado;
        /// <summary>
        /// Estado do cedente (e.g. SP, MG, ES, RJ)
        /// </summary>
        public string Estado
        {
            get { return estado; }
            set { estado = value; }
        }

        private string email;
        /// <summary>
        /// Email do cedente.
        /// </summary>
        public string Email
        {
            get { return email; }
            set { email = value; }
        }
    }
}
