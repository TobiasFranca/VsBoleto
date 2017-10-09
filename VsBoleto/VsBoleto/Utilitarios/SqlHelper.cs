using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VsBoleto.Utilitarios
{
    public static class SqlHelper
    {
        public static string GetSelectCRMVINT(string filial, string ordem)
        {
            string s = @"SELECT * FROM CRMVINT WHERE FILIAL = " + filial +
                                            " AND ORDEM = " + ordem +
                                            " AND PARCELA > 0" +
                                            " ORDER BY PARCELA";
            return s;
        }

        public static string GetSelectNotasSaida(string data1, string data2)
        {
            string s = @"SELECT E0.ORDEM, E0.NFISCAL, E0.NRO_ECF, E0.VRNF, E0.OBSLF, E0.DATA,
                                            E0.RETORNONFE, E0.INDCANC,
                                            E0.PERCACRESCIMO, E0.ACRESCIMO, E0.PERCDESCONTO, E0.DESCONTO,
                                            E0.MODDOC, E0.FILIAL, E0.SERIE, E0.EMISSAO, E0.CLIENTE,
                                            E0.POSICAO, PS.ST2, CL.CLIENTE, CL.NOME, CL.RAZAO, CL.EMAIL, CL.CGCCPF, 
                                            CL.ENDERECO, CL.BAIRRO, CL.CIDADE, CL.ESTADO, CL.CEP, CL.NUMERO, CL.CGCCPF ,
                                                (select COUNT(*) from CRMVINT PARC 
                                                where PARC.FILIAL = E0.FILIAL AND PARC.ORDEM = E0.ORDEM )
                                                AS NUMPARCELAS
                                            FROM ESMVSAI0 E0
                                            JOIN CDCLIENT CL ON(E0.CLIENTE = CL.CLIENTE)
                                            JOIN CDCONDPG CP ON(E0.CONDPGTO = CP.CONDPGTO)
                                            JOIN CDPOSIC PS ON(E0.POSICAO = PS.POSICAO)
                                            JOIN CDOPERA OP ON(E0.OPERACAO = OP.OPERACAO)
                                            WHERE E0.DATA >= '" + data1 + @"' 
                                            AND DATA <= '" + data2 + " 23:59:59" + @"'
                                            AND E0.ST1 = 'F' 
                                            AND CP.CONDPG = 'P'
                                            AND PS.ST2 = 'S'
                                            AND OP.CLIENTE = 'S'
                                            AND (SUBSTRING(E0.OBSLF FROM 1 FOR 4) <> 'CANC'                                            
                                            OR E0.OBSLF IS NULL)
                                            ORDER BY E0.ORDEM";


            return s;
        }

        //        public static string GetSelectNotasSaida(string data1, string data2)
        //        {
        //            string s = @"SELECT E0.ORDEM, E0.NFISCAL, E0.NRO_ECF, E0.VRNF, E0.OBSLF, E0.DATA,
        //                                            E0.RETORNONFE, E0.INDCANC,
        //                                            E0.PERCACRESCIMO, E0.ACRESCIMO, E0.PERCDESCONTO, E0.DESCONTO,
        //                                            E0.MODDOC, E0.FILIAL, E0.SERIE, E0.EMISSAO, E0.CLIENTE,
        //                                            E0.POSICAO, PS.ST2, CL.CLIENTE, CL.NOME, CL.RAZAO, CL.EMAIL, CL.CGCCPF, 
        //                                            CL.ENDERECO, CL.BAIRRO, CL.CIDADE, CL.ESTADO, CL.CEP, CL.NUMERO, CL.CGCCPF
        //                                            FROM ESMVSAI0 E0
        //                                            JOIN CDCLIENT CL ON(E0.CLIENTE = CL.CLIENTE)
        //                                            JOIN CDCONDPG CP ON(E0.CONDPGTO = CP.CONDPGTO)
        //                                            JOIN CDPOSIC PS ON(E0.POSICAO = PS.POSICAO)
        //                                            WHERE E0.DATA >= '" + data1 + @"' 
        //                                            AND DATA <= '" + data2 + " 23:59:59" + @"'
        //                                            AND E0.ST1 = 'F' 
        //                                            AND CP.CONDPG = 'P'
        //                                            AND PS.ST2 = 'S'
        //                                            AND (SUBSTRING(E0.OBSLF FROM 1 FOR 4) <> 'CANC'
        //                                            OR E0.OBSLF IS NULL)
        //                                            ORDER BY E0.ORDEM
        //                                            ";
        //            return s;
        //        }
        internal static string GetSelectNotasSaidaPorNN(string nossoNumero)
        {
            string s = @"SELECT E0.ORDEM, E0.NFISCAL, E0.NRO_ECF, E0.VRNF, E0.OBSLF, E0.DATA,
                                            E0.RETORNONFE, E0.INDCANC,
                                            E0.PERCACRESCIMO, E0.ACRESCIMO, E0.PERCDESCONTO, E0.DESCONTO,
                                            E0.MODDOC, E0.FILIAL, E0.SERIE, E0.EMISSAO, E0.CLIENTE,
                                            E0.POSICAO, PS.ST2, CL.CLIENTE, CL.NOME, CL.RAZAO, CL.EMAIL, CL.CGCCPF, 
                                            CL.ENDERECO, CL.BAIRRO, CL.CIDADE, CL.ESTADO, CL.CEP, CL.NUMERO, CL.CGCCPF ,
                                                (select COUNT(*) from CRMVINT PARC 
                                                where PARC.FILIAL = E0.FILIAL AND PARC.ORDEM = E0.ORDEM )
                                                AS NUMPARCELAS
                                            FROM ESMVSAI0 E0
                                            JOIN CDCLIENT CL ON(E0.CLIENTE = CL.CLIENTE)
                                            JOIN CDCONDPG CP ON(E0.CONDPGTO = CP.CONDPGTO)
                                            JOIN CDPOSIC PS ON(E0.POSICAO = PS.POSICAO)
                                            JOIN CRMVINT CR ON(E0.FILIAL = CR.FILIAL AND E0.ORDEM = CR.ORDEM)
                                            WHERE CR.NROBOLETO = '" + nossoNumero + @"'
                                            AND E0.ST1 = 'F' 
                                            AND CP.CONDPG = 'P'
                                            AND PS.ST2 = 'S'
                                            AND (SUBSTRING(E0.OBSLF FROM 1 FOR 4) <> 'CANC'
                                            OR E0.OBSLF IS NULL)
                                            ORDER BY E0.ORDEM
                                            ";
            return s;
        }

        internal static string GetSelectNotasSaidaImpressaoAuto(string data1, string data2)
        {
            string s = @"SELECT DISTINCT E0.ORDEM, E0.NFISCAL, E0.NRO_ECF, E0.VRNF, E0.OBSLF, E0.DATA,
                                            E0.RETORNONFE, E0.INDCANC,
                                            E0.PERCACRESCIMO, E0.ACRESCIMO, E0.PERCDESCONTO, E0.DESCONTO,
                                            E0.MODDOC, E0.FILIAL, E0.SERIE, E0.EMISSAO, E0.CLIENTE,
                                            E0.POSICAO, PS.ST2, CL.CLIENTE, CL.NOME, CL.RAZAO, CL.EMAIL, CL.CGCCPF, 
                                            CL.ENDERECO, CL.BAIRRO, CL.CIDADE, CL.ESTADO, CL.CEP, CL.NUMERO, CL.CGCCPF ,
                                                (select COUNT(*) from CRMVINT PARC 
                                                where PARC.FILIAL = E0.FILIAL AND PARC.ORDEM = E0.ORDEM )
                                                AS NUMPARCELAS
                                            FROM ESMVSAI0 E0
                                            JOIN CDCLIENT CL ON(E0.CLIENTE = CL.CLIENTE)
                                            JOIN CDCONDPG CP ON(E0.CONDPGTO = CP.CONDPGTO)
                                            JOIN CDPOSIC PS ON(E0.POSICAO = PS.POSICAO)
                                            JOIN CRMVINT CR ON(E0.FILIAL = CR.FILIAL AND E0.ORDEM = CR.ORDEM)
                                            WHERE E0.DATA >= '" + data1 + @"' 
                                            AND DATA <= '" + data2 + " 23:59:59" + @"'
                                            AND (CR.ST3 IS null OR (CR.ST3 <> 'A' AND CR.ST3 <> 'I'))
                                            AND E0.ST1 = 'F' 
                                            AND CP.CONDPG = 'P'
                                            AND PS.ST2 = 'S'
                                            AND (SUBSTRING(E0.OBSLF FROM 1 FOR 4) <> 'CANC'
                                            OR E0.OBSLF IS NULL)
                                            ORDER BY E0.ORDEM
                                            ";
            return s;
            //AND CR.ST3 <> 'I' 
            //AND CR.ST3 <> 'A'
        }
    }
}
