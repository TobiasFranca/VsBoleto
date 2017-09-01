using BoletoBancario.Conta;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoletoBancario.Utilitarios
{
    public class ListaOcorrencias
    {
        private List<OcorrenciasCobranca> lista = new List<OcorrenciasCobranca>();

        public int Count
        {
            get { return lista.Count; }
        }

        public OcorrenciasCobranca this[int index]
        {
            get { return lista[index]; }
        }

        internal void Add(OcorrenciasCobranca item)
        {
            lista.Add(item);
        }

        public IEnumerator GetEnumerator()
        {
            return (IEnumerator)this;
        }
    }
}
