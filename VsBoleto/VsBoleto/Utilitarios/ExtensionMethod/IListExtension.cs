using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class IListExtension
{
    /// <summary>
    /// Altera os indices do Ilist
    /// </summary>
    /// <typeparam name="T">O tipo da Lista</typeparam>
    /// <param name="target">A lista</param>
    public static void Randomize<T>(this IList<T> target)
    {
        Random RndNumberGenerator = new Random();
        SortedList<int, T> newList = new SortedList<int, T>();
        foreach (T item in target)
        {
            newList.Add(RndNumberGenerator.Next(), item);
        }
        target.Clear();
        for (int i = 0; i < newList.Count; i++)
        {
            target.Add(newList.Values[i]);
        }
    }

    /// <summary>
    /// Retorna uma lista aleatória com a quantidade informada
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="target"></param>
    /// <param name="count">quantidade de itens</param>
    public static void Randomize<T>(this IList<T> target, int count)
    {
        Random RndNumberGenerator = new Random();
        SortedList<int, T> newList = new SortedList<int, T>();
        foreach (T item in target)
        {
            newList.Add(RndNumberGenerator.Next(), item);
        }
        target.Clear();
        for (int i = 0; i < count; i++)
        {
            target.Add(newList.Values[i]);
        }
    }

    /// <summary>
    /// Sorteia randomicamente um item de um IList e o retorna
    /// </summary>
    /// <typeparam name="T">O tipo da Lista</typeparam>
    /// <param name="input">A lista a ser avaliada</param>
    public static T GetRandomItem<T>(this IList<T> input)
    {
        Random rand = new Random();
        if (input != null)
        {
            if (input.Count == 1)
                return input[0];

            int n = rand.Next(input.Count() + 1);

            return input[n];
        }
        return (T)input;
    }

    /// <summary>
    /// Adiciona na lista original os elementos da lista passada como parametro.  
    /// </summary>
    /// <param name="input">A lista a ser avaliada</param>
    /// <param name="lista">Lista a ser adicionada</param>
    public static void AddRange<T>(this IList<T> input, IList<T> lista)
    {
        foreach (T item in lista)
        {
            if (!input.Contains(item))
            {
                input.Add(item);
            }
        }
    }

    public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
    {
        return listToClone.Select(item => (T)item.Clone()).ToList();
    }
}
