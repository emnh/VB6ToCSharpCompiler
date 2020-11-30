using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VB6ToCSharpCompiler
{
    public class OutToken
    {
        public int index { get; set; }  = 0;
        public string token { get; set; } = "";

        public OutToken(int index, string token)
        {
            this.index = index;
            this.token = token;
        }
    }
}
