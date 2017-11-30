using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwoWayPicker.Core.PropertyValueConverters.Utilities
{
    public static class ConverterHelper
    {
        internal static IEnumerable<T> Yield<T>(this IEnumerable<T> source)
        {
            foreach (var element in source)
            {
                yield return element;
            }
        }
    }
}
