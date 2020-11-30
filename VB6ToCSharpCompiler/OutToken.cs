using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VB6ToCSharpCompiler
{
    public class OutToken
    {
        public double index { get; set; }  = 0;
        public string token { get; set; } = "";

        public OutToken(double index, string token)
        {
            this.index = index;
            this.token = token;
        }
    }
}
