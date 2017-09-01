using BoletoBancario.Bancos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoletoBancario.Utilitarios
{
    public static class EnumHelper
    {
        public static Banco GetBanco(EnumBanco banco)
        {
            switch (banco)
            {
                case EnumBanco.Sicoob: return new BancoSicoob();
                case EnumBanco.Itau: return new BancoItau();
                case EnumBanco.Bradesco: return new BancoBradesco();
                case EnumBanco.CaixaSR: return new BancoCaixaSR();
                case EnumBanco.Banestes: return new BancoBanestes(); //MVZ - teste 09/10/2014
                case EnumBanco.Santander: return new BancoSantander();
                case EnumBanco.BancoBrasil: return new BancoBrasil();
                default: return null;
            }
        }

        public static string GetEspecieMoeda(EnumTipoMoeda moeda)
        {
            switch (moeda)
            {
                case EnumTipoMoeda.Real: return "R$";
                default: return "";
            }
        }

        public static int GetNumMoeda(EnumTipoMoeda moeda)
        {
            switch (moeda)
            {
                case EnumTipoMoeda.Real: return 9;
                default: return 0;
            }
        }
    }

    public enum EnumTipoMoeda
    {
        Real
    }

    public enum EnumEspecieBoleto
    {
        DuplicataMercantil,
        NotaPromissoria,
        NotaDeSeguro,
        Recibo,
        DuplicataRural,
        LetraDeCambio,
        Warrant,
        Cheque,
        DuplicataDeServico,
        NotaDeDebito,
        TriplicataMercantil,
        TriplicataDeServico,
        Fatura,
        ApoliceDeSeguro,
        MensalidadeEscolar,
        ParcelaDeConsorcio,
        Outros
    }

    public enum EnumBanco
    {
        Sicoob,
        Itau,
        Bradesco,
        CaixaSR,
        Banestes,
        Santander,
        BancoBrasil
    }
}
