using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoletoBancario.Utilitarios
{
    public class DefaultList : IEnumerable
    {
        private List<string> lista = new List<string>();

        public int Count
        {
            get { return lista.Count; }
        }

        public string this[int index]
        {
            get { return lista[index]; }
        }

        internal void Add(string item)
        {
            lista.Add(item);
        }

        public IEnumerator GetEnumerator()
        {
            return (IEnumerator)this;
        }
    }
}
