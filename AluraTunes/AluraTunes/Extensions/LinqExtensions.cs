using System;
using System.Linq;
using System.Linq.Expressions;

namespace AluraTunes.Extensions
{
    public static class LinqExtensions
    {
        public static decimal Mediana<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, decimal>> selector)
        {
            var contagem = source.Count();

            var funcSelector = selector.Compile();

            var queryOrdenada = source.Select(funcSelector).OrderBy(total => total);

            var elementoCentral_1 = queryOrdenada.Skip(contagem / 2).First();

            var elementoCentral_2 = queryOrdenada.Skip((contagem - 1) / 2).First();

            var mediana = (elementoCentral_1 + elementoCentral_2) / 2;

            return mediana;
        }
    }
}
