using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using org.antlr.v4.runtime.tree;

namespace VB6ToCSharpCompiler
{
    public class ASTSequenceItem
    {
        public int setpos { get; set; }
        public int depth { get; set; }
        public int childIndex { get; set; }
        public string token { get; set; }

        public static List<ASTSequenceItem> Create(ParseTree node)
        {
            //var VbCode = "A & B";
            //var CsharpCode = "A + B";
            //var pattern = new PatternText(PatternText.vbExpressionWrapper, VbCode, CsharpCode);
            //var realPattern = pattern.Compile();

            //var thosePaths = realPattern.GetVBPaths();
            var thosePaths = VbToCsharpPattern.GetExtendedPathList(node);

            List<ASTSequenceItem> returlisto = new List<ASTSequenceItem> { };
            foreach (List<IndexedPath> paths in thosePaths)
            {
                int depth = 0;
                foreach (IndexedPath path in paths)
                {
                    depth++;
                    var asi = new ASTSequenceItem { setpos = returlisto.Count, depth = depth, childIndex = path.ChildIndex, token = paths.Last().NodeTypeName };
                    returlisto.Add(asi);
                }
            }
            return returlisto;
        }

        public string ToString() {
            return "(" + setpos + ", " + depth + ", " + childIndex + ", " + token + ")";
        }

        public static string ToString(List<ASTSequenceItem> list)
        {
            string s = "";
            foreach (var asi in list)
            {
                s += asi.ToString() + "\r\n";
            }
            return s;
        }
    }
}
