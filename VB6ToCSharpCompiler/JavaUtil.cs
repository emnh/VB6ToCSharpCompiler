using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using java.util;

namespace VB6ToCSharpCompiler
{
    public static class JavaUtil
    {
        public static IEnumerable<T> JavaListToCSharpList<T>(this java.util.List list)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }
            for (var i = 0; i < list.size(); i++)
            {
                yield return (T)list.get(i);
            }
        }
    }
}
