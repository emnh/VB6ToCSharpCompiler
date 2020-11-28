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

// Add new form for pattern editing.
// Display list of patterns.
// Button to save patterns to file .ini syntax or JSON? JSON.

// Given a directory, show how many matches for each pattern.
// Browse list of matched patterns. They should already be under same nodetype, i.e. one pattern matches only one nodetype. More or less at least.
// Browse list of unmatched patterns. Hash by nodetype. With counts.

namespace VB6ToCSharpCompiler
{
    public class ASTSequenceItem
    {
        public int setpos { get; set; }
        public int depth { get; set; }
        public string childPath { get; set; }
        public string typeName { get; set; }
        public string token { get; set; }
        public string pattern { get; set; }

        public static List<ASTSequenceItem> Create(ASTPatternGenerator apg, ParseTree rootNode)
        {
            if (apg == null)
            {
                throw new ArgumentNullException(nameof(apg));
            }

            VB6NodeTree nodeTree = new VB6NodeTree(rootNode);
            List<ASTSequenceItem> returnedList = new List<ASTSequenceItem> { };

            foreach (var node in nodeTree.GetAllNodes())
            {
                var subtree = new VB6SubTree(nodeTree, node);
                var pattern = apg.Lookup(subtree);

                int depth = 0;
                string childIndices = "";
                string typePath = "";

                var paths = nodeTree.GetPath(node).ToList();
                if (paths.Count == 0)
                {
                    continue;
                }

                foreach (IndexedPath path in paths)
                {
                    childIndices += path.ChildIndex + "/";
                    typePath += path.NodeTypeName + "/";
                    depth++;
                }

                var asi = new ASTSequenceItem {
                    setpos = returnedList.Count,
                    depth = depth,
                    childPath = childIndices,
                    token = paths.Last().Token,
                    typeName = paths.First().NodeTypeName,
                    pattern = pattern
                };
                returnedList.Add(asi);
            }
            return returnedList;
        }

        public override string ToString() {
            return "(" + setpos + ", " + depth + ", " + childPath + ", " + typeName + ", " + token + ")";
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

        public static string HashNodes(List<ASTSequenceItem> list)
        {
            string s = "";
            foreach (var asi in list.Take(1000))
            {
                s += asi.pattern + " ";
            }
            return s;
        }
    }
}
