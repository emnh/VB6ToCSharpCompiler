using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.sun.org.apache.bcel.@internal.generic;

namespace VB6ToCSharpCompiler
{
    public class TokenInfo
    {
        public List<IndexedPath> Path { get; }
        public List<string> Tokens { get; }

        public TokenInfo(List<IndexedPath> path, List<string> tokens)
        {
            Path = path;
            Tokens = tokens;
        }
    }
}
