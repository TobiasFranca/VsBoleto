using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;
using System.Text;

public static class DateTimeExtension
{
    /// <summary>
    /// Transforma uma data em uma sua representação por extenso
    /// </summary>
    /// <param name="data">A data de referência</param>
    /// <returns>Uma string que representa a data por extenso</returns>
    public static string ToExtensionDate(this DateTime date)
    {
        return date.Day.ToString("00") + " de " +
            new CultureInfo("pt-BR").DateTimeFormat.GetMonthName(date.Month) +
            " de " + date.Year.ToString("0000");
    }

    //public static string EmptyToMinValueSQL(this DateTime date)
    //{
    //    if (date <= Utilitarios.WebUtil.MenorValorDataSqlServer2000 || date == DateTime.MaxValue)
    //        return "";
    //    else
    //        return date.ToShortDateString();
    //}

    public static string EmptyToMinValue(this DateTime date)
    {
        if (date == DateTime.MinValue || date == DateTime.MaxValue)
            return "";
        else
            return date.ToShortDateString();
    }

    public static DateTime ToMaxHourOfDay(this DateTime data)
    {
        return Convert.ToDateTime(data.ToShortDateString() + " 23:59:59");
    }

    public static DateTime ToMinHourOfDay(this DateTime data)
    {
        return Convert.ToDateTime(data.ToShortDateString() + " 00:00:00");
    }

    public static bool IsMinDateTime(this DateTime data)
    {
        return data.CompareTo(SqlDateTime.MinValue.Value) < 1;
    }

    public static bool IsMaxDateTime(this DateTime data)
    {
        return data.CompareTo(SqlDateTime.MaxValue.Value) > -1;
    }
}
