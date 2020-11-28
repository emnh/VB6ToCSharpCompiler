using org.antlr.v4.runtime.tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VB6ToCSharpCompiler
{
    public class ASTPatternGenerator
    {
        private Dictionary<string, List<VB6SubTree>> nodeHash = new Dictionary<string, List<VB6SubTree>>();
        private Dictionary<string, string> generatedPatterns = new Dictionary<string, string>();

        public ASTPatternGenerator(ParseTree node)
        {
            VB6NodeTree nodeTree = new VB6NodeTree(node);
            GetPatterns(nodeTree);
        }

        public string Lookup(VB6SubTree subTree)
        {
            var s = GetNodeTreeHashString(subTree);
            if (generatedPatterns.ContainsKey(s))
            {
                return generatedPatterns[s];
            }
            return s;
        }

        public void GetPatterns(VB6NodeTree nodeTree)
        {
            if (nodeTree == null)
            {
                throw new ArgumentNullException(nameof(nodeTree));
            }
            // Iterate over all nodes and add them to node hash based on their concatenated type strings
            foreach (var node in nodeTree.GetAllNodes())
            {
                var subtree = new VB6SubTree(nodeTree, node);

                var nodeTreeHashString = GetNodeTreeHashString(subtree);

                if (!nodeHash.ContainsKey(nodeTreeHashString))
                {
                    nodeHash[nodeTreeHashString] = new List<VB6SubTree>();
                }
                nodeHash[nodeTreeHashString].Add(subtree);
            }

            // Iterate over all nodes and replace each token/text in pattern,
            // if there are two or more variations of this token in this particular location
            foreach (var key in nodeHash.Keys)
            {
                string letters = "ABCDEFGHIJKLMNOPQRSTUVXYZ";
                int letterIndex = 0;
                Dictionary<int, string> hashTokens = new Dictionary<int, string>();
                //Dictionary<string, List<int>> textCount = new Dictionary<string, List<int>>();

                foreach (var subtree in nodeHash[key])
                {
                    var node = subtree.GetRoot();

                    var tokensSoFar = node.getSourceInterval();
                    //var a = tokensSoFar.a;
                    //var b = tokensSoFar.b;
                    //var text = node.getText();
                    var code = nodeTree.GetRoot().getText();
                    var text = code;

                    bool[] claimed = new bool[code.Length];
                    for (int i = 0; i < code.Length; i++)
                    {
                        claimed[i] = false;
                    }

                    var nodes = subtree.GetAllNodes().ToList();
                    var nodeList = new List<Tuple<int, ParseTree>>();
                    var childIndex = 0;
                    foreach (var child in nodes)
                    {
                        nodeList.Add(Tuple.Create(childIndex, child));
                        childIndex++;
                    }
                    nodeList.Sort((a, b) => nodeTree.GetDepth(b.Item2).CompareTo(nodeTree.GetDepth(a.Item2)));

                    foreach (var child in nodeList)
                    {
                        var tokensHere = child.Item2.getSourceInterval();
                        var length = text.Length;
                        var childText = ""; // text.Substring(tokensHere.a, tokensHere.b - tokensHere.a);
                        for (int i = tokensHere.a; i < tokensHere.b; i++) {
                            if (!claimed[i])
                            {
                                childText += text[i];
                            }
                            claimed[i] = true;
                        }

                        var index = child.Item1;
                        if (hashTokens.ContainsKey(index) && hashTokens[index] != childText)
                        {
                            if (letterIndex < letters.Length)
                            {
                                hashTokens[index] = letters[letterIndex++].ToString(System.Globalization.CultureInfo.InvariantCulture);
                            } else
                            {
                                hashTokens[index] = "*";
                            }
                        } else
                        {
                            hashTokens[index] = childText;
                        }
                    }
                }

                var hashTokensList = new List<Tuple<int, string>>();
                foreach (var tk in hashTokens)
                {
                    hashTokensList.Add(Tuple.Create(tk.Key, tk.Value));
                }
                hashTokensList.Sort((a, b) => a.Item1.CompareTo(b.Item1));
                generatedPatterns[key] = String.Join(" ", hashTokensList.Select(x => x.Item2));
            }
        }

        public static string GetNodeTreeHashString(VB6SubTree nodeTree)
        {
            if (nodeTree == null)
            {
                throw new ArgumentNullException(nameof(nodeTree));
            }

            var typeNames = new List<string>();
            var count = 0;
            foreach (var node in nodeTree.GetAllNodes())
            {
                typeNames.Add(VbToCsharpPattern.LookupNodeType(node));
                count++;
                if (count > 20)
                {
                    break;
                }
            }
            return String.Join("/", typeNames);
        }

    }
}
