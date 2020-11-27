using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using org.antlr.v4.runtime.tree;

// Create a new form
// Add list of items which are matched
// Provide statistics for items which are matched
// Add list of items which are not mached
// Make input box to edit pattern for item which is not matched
// Provide statistics for matching of input box

// Display conflicting patterns, patterns that match the same code
// Display how many conflicts there are
// Advanced: Suggest resolution of conflict?

// Edit control with syntax highlighting

namespace VB6ToCSharpCompiler
{
    public class ASTSequenceItem
    {
        public int setpos { get; set; }
        public int depth { get; set; }
        public int childIndex { get; set; }
        public string typeName { get; set; }
        public string token { get; set; }

        public static List<ASTSequenceItem> Create(ParseTree node)
        {
            //var VbCode = "A & B";
            //var CsharpCode = "A + B";
            //var pattern = new PatternText(PatternText.vbExpressionWrapper, VbCode, CsharpCode);
            //var realPattern = pattern.Compile();

            //var thosePaths = realPattern.GetVBPaths();
            var thosePaths = VbToCsharpPattern.GetExtendedPathList(node);

            List<ASTSequenceItem> returnedList = new List<ASTSequenceItem> { };
            foreach (ImmutableList<IndexedPath> paths in thosePaths)
            {
                int depth = 0;
                foreach (IndexedPath path in paths)
                {
                    depth++;
                    var asi = new ASTSequenceItem { setpos = returnedList.Count, depth = depth, childIndex = path.ChildIndex, token = paths.Last().Token, typeName = path.NodeTypeName };
                    returnedList.Add(asi);
                }
            }
            return returnedList;
        }

        public override string ToString() {
            return "(" + setpos + ", " + depth + ", " + childIndex + ", " + typeName + ", " + token + ")";
        }

        public static string ToString(List<ASTSequenceItem> list)
        {
            string s = "";
            foreach (var asi in list.Take(1000))
            {
                s += asi.ToString() + "\r\n";
            }
            return s;
        }
    }
}
