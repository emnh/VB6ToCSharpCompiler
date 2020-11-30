using org.antlr.v4.runtime.tree;
using System;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.IO;

namespace VB6ToCSharpCompiler
{
    public class ASTPatternGenerator
    {
        private Dictionary<string, Dictionary<string, List<VB6SubTree>>> nodeHashDict = new Dictionary<string, Dictionary<string, List<VB6SubTree>>>();
        private Dictionary<ParseTree, string> generatedPatterns = new Dictionary<ParseTree, string>();

        public ASTPatternGenerator(ParseTree node)
        {
            VB6NodeTree nodeTree = new VB6NodeTree(node);
            GetPatterns(nodeTree);
        }

        public static string GetNodeHash(ParseTree node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            return node.GetType().Name;
        }

        public string Lookup(VB6SubTree subTree)
        {
            if (subTree == null)
            {
                throw new ArgumentNullException(nameof(subTree));
            }

            //var t = GetNodeHash(subTree.GetRoot());
            var t = subTree.GetRoot();
            if (generatedPatterns.ContainsKey(t))
            {
                return generatedPatterns[t];
            }
            return null;
        }

        //public static IEnumerable<org.antlr.v4.runtime.Token> GetTokens(ParseTree node)
        public static IEnumerable<string> GetTokens(ParseTree node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            for (int i = 0; i < node.getChildCount(); i++)
            {
                var child = node.getChild(i);
                var text = child.getText().Trim();
                if (text.Length > 0)
                {
                    if (child is TerminalNodeImpl)
                    {
                        yield return ((TerminalNodeImpl) child).getSymbol().getText();
                    }
                }
            }
        }

        public static string GetCls(string typeName)
        {
            string outString = @"
using org.antlr.v4.runtime.tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VB6ToCSharpCompiler.VB6NodeTranslatorLogging
{
public class $TYPE : VB6NodeTranslator
{
    public $TYPE(VB6NodeTree nodeTree, Dictionary<ContextNodeType, VB6NodeTranslator> translatorDict) : base(nodeTree, translatorDict)
    {
    }

    public override ContextNodeType GetNodeContextType()
    {
        return ContextNodeType.$TYPE;
    }

    public override IEnumerable<string> PreTranslate(List<ParseTree> parseTrees)
    {
        return new List<string>();
    }
        
    public override IEnumerable<string> PostTranslate(List<ParseTree> parseTrees)
    {
        return new List<string>();
    }
}
}
";
            
            outString = outString.Replace("$TYPE", typeName).Replace("$TYPE", typeName).Replace("$TYPE", typeName);
            //outString = outString.Replace("$BODY", cm + yr);
            return outString;
        }

        public static void GetCode(VB6NodeTree nodeTree)
        {
            const string genFolder = @"F:\emh-dev\VB6ToCSharpCompiler\VB6ToCSharpCompiler\VB6NodeTranslatorLogging";

            Dictionary<string, Dictionary<ImmutableList<string>, List<VB6SubTree>>> nodeTypeDict = new Dictionary<string, Dictionary<ImmutableList<string>, List<VB6SubTree>>>();

            if (nodeTree == null)
            {
                throw new ArgumentNullException(nameof(nodeTree));
            }
            // Iterate over all nodes and add them to node hash based on their concatenated type strings
            foreach (var node in nodeTree.GetAllNodes())
            {
                var subtree = new VB6SubTree(nodeTree, node);

                //var nodeTreeHashString = GetNodeTreeHashString(subtree);
                var nodeHash = GetNodeHash(node);

                if (!nodeTypeDict.ContainsKey(nodeHash))
                {
                    nodeTypeDict[nodeHash] = new Dictionary<ImmutableList<string>, List<VB6SubTree>>();
                }

                var children = ImmutableList.Create<string>();
                foreach (var child in nodeTree.GetChildren(node))
                {
                    var tokens = String.Join(" ", GetTokens(child));
                    //if (!string.IsNullOrEmpty(tokens))
                    //{
                    //    children = children.Add("\"" + tokens + "\"");
                    //} else
                    //{
                        
                    //}
                    children = children.Add(GetNodeHash(child));
                }
                
                if (!nodeTypeDict[nodeHash].ContainsKey(children))
                {
                    nodeTypeDict[nodeHash][children] = new List<VB6SubTree>();
                }
                nodeTypeDict[nodeHash][children].Add(subtree);
            }

            var hasContexts = new Dictionary<string, bool>();
            foreach (ContextNodeType contextNodeType in (ContextNodeType[]) Enum.GetValues(typeof(ContextNodeType)))
            {
                var typeName = contextNodeType.ToString("F");
                var fileName = typeName + ".cs";
                var outString = GetCls(typeName);
                hasContexts[typeName] = true;
                System.IO.File.WriteAllText(Path.Combine(genFolder, fileName), outString);
            }
            foreach (var key in nodeTypeDict.Keys)
            {
                var typeName = key;
                if (!hasContexts.ContainsKey(typeName))
                {
                    DebugClass.LogError(typeName + ",");
                    hasContexts[typeName] = true;
                }
                var fileName = typeName + ".cs";
                var outString = GetCls(typeName);
                System.IO.File.WriteAllText(Path.Combine(genFolder, fileName), outString);
            }
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

                //var nodeTreeHashString = GetNodeTreeHashString(subtree);
                var nodeHash = GetNodeHash(node);
                
                if (!nodeHashDict.ContainsKey(nodeHash))
                {
                    nodeHashDict[nodeHash] = new Dictionary<string, List<VB6SubTree>>();
                }
                var tokens = String.Join(" ", GetTokens(node));
                if (!nodeHashDict[nodeHash].ContainsKey(tokens))
                {
                    nodeHashDict[nodeHash][tokens] = new List<VB6SubTree>();
                }
                nodeHashDict[nodeHash][tokens].Add(subtree);
            }

            foreach (var key in nodeHashDict.Keys)
            {
                foreach (var key2 in nodeHashDict[key].Keys)
                {
                    var s = new List<string>();
                    foreach (var subtree in nodeHashDict[key][key2])
                    {
                        var node = subtree.GetRoot();
                        var tokens = String.Join(" ", GetTokens(node));
                        s.Add(tokens);
                    }
                    var s2 = String.Join(", ", s);
                    DebugClass.LogError("NodeTreeHashString: " + key + " " + key2 + ": " + nodeHashDict[key][key2].Count + ": " + s2);
                }
            }

            // TODO: Is it possible to auto-generate this dictionary?
            // No stress, it's small, but would be nice for other languages.
            var replaceable = new Dictionary<string, bool>();
            replaceable["AmbiguousIdentifierContext"] = true;
            replaceable["CertainIdentifierContext"] = true;
            replaceable["LiteralContext"] = true;
            replaceable["FieldLengthContext"] = true;

            // Iterate over all nodes and replace each token/text in pattern with pattern letter,
            // if there are two or more variations of this token under this node type name
            const string letters = "ABCDEFGHIJKLMNOPQRSTUVXYZ";
            foreach (var node in nodeTree.GetAllNodes())
            {
                var subtree = new VB6SubTree(nodeTree, node);
                int letterIndex = 0;
                var text = node.getText();
                var pattern = text;
                
                foreach (var child in subtree.GetAllNodes())
                {
                    if (replaceable.ContainsKey(VbToCsharpPattern.LookupNodeType(child)))
                    {
                        //DebugClass.LogError("REPLACING: " + VbToCsharpPattern.LookupNodeType(child));
                        var tokens = GetTokens(child);
                        foreach (var token in tokens)
                        {
                            if (letterIndex < letters.Length)
                            {
                                var oldPattern = pattern;
                                pattern = pattern.Replace(token, letters[letterIndex].ToString(System.Globalization.CultureInfo.InvariantCulture));
                                if (pattern != oldPattern)
                                {
                                    letterIndex++;
                                }
                                //DebugClass.LogError("REPLACING: " + token);
                            }
                            
                            if (letterIndex >= letters.Length)
                            {
                                break;
                            }
                        }
                        if (letterIndex >= letters.Length)
                        {
                            break;
                        }
                    }
                }
                if (pattern.Length >= 100)
                {
                    pattern = "PATTERN TOO LONG";
                }
                generatedPatterns[node] = pattern;
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
