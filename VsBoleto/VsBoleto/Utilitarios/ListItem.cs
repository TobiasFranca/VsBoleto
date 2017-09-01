using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VsBoleto.Utilitarios
{
    class ListItem
    {
        private string nome;

        public string Nome
        {
            get { return nome; }
            set { nome = value; }
        }
        private string value;

        public string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        public ListItem(string nome, string value)
        {
            this.nome = nome;
            this.value = value;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(ListItem))
                return ((ListItem)obj).Value == this.Value;
            else
                return false;
        }

        public override string ToString()
        {
            return nome;
        }
    }
}
