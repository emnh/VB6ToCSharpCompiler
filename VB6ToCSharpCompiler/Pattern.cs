using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.ibm.icu.text;
using com.sun.org.apache.xpath.@internal.compiler;
using ikvm.extensions;
using io.proleap.vb6.asg.metamodel.statement.print;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using org.antlr.v4.runtime.tree;
using SyntaxTree = org.antlr.v4.runtime.tree.SyntaxTree;

namespace VB6ToCSharpCompiler
{
    public class Pattern
    {
        // TODO: implement unused PatternIdentifiers, null paths etc
        private string[] PatternIdentifiers;
        /*= new string[] {
            "A", "B"
        };*/
        private const string Content = "$CONTENT";

        public string vbASGType { get; set; }
        private string vbString;
        private string csharpString;

        private List<IndexedPath>[] VbPaths;
        private List<IndexedPath>[] CSharpPaths;

        private CompileResult VbCompiledPattern;
        private SyntaxTree CsharpParsedPattern;

        // TODO: set cutDepth by parsing wrapper code
        private int cutDepthofDollarContent = 8;
        private string vbWrapper = @"
public Sub MySub()
$CONTENT
End Sub
";

        public Pattern(string vbString, string csharpString)
        {
            this.vbString = vbString;
            this.csharpString = csharpString;

            VbCompiledPattern = VbParsePattern(vbString);
            CsharpParsedPattern = CSharpParsePattern(csharpString);
            Console.Error.WriteLine("ASGType: " + vbASGType);
            foreach (var path in VbPaths)
            {
                Console.Error.WriteLine("Path: " + PrintPath(path));
            }
            foreach (var path in CSharpPaths)
            {
                Console.Error.WriteLine("Path: " + PrintPath(path));
            }
        }

        public string[] GetIdentifiers(CompileResult compileResult)
        {
            var identifiers = new List<string>();

            var visitorCallback = new VisitorCallback()
            {
                Callback = (node, parent) =>
                {
                    if (node.GetType().Name == "AmbiguousIdentifierContext")
                    {
                        var identifier = node.getText().Trim('"').Trim();
                        if (identifier != "MySub" && identifier != "Z")
                        {
                            identifiers.Add(identifier);
                        }
                    }
                }
            };
            VB6Compiler.Visit(compileResult, visitorCallback);
            return identifiers.ToArray();
        }

        public string PrintPath(List<IndexedPath> path)
        {
            var list = new List<string>();
            foreach (var item in path)
            {
                list.Add(item.NodeTypeName + ":" + item.ChildIndex);
            }
            return string.Join("/", list);
        }

        ParseTree LookupNodeFromPath(ParseTree root, List<IndexedPath> path)
        {
            ParseTree node = root;
            foreach (var indexedPath in path)
            {
                var child = node.getChild(indexedPath.ChildIndex);
                var last = indexedPath == path[path.Count - 1];
                if (!last && child.GetType().Name != indexedPath.NodeTypeName)
                {
                    throw new InvalidOperationException("child type doesn't match");
                }
                node = child;
            }
            
            return node;
        }

        public CompileResult VbParsePattern(string pattern)
        {
            string code = vbWrapper.Replace(Content, pattern);
            var compileResult = VB6Compiler.Compile("Test.bas", code, false);

            PatternIdentifiers = GetIdentifiers(compileResult);

            var paths = new List<IndexedPath>[PatternIdentifiers.Length];
            ParseTree root = null;

            var visitorCallback = new VisitorCallback()
            {
                Callback = (node, parent) =>
                {
                    if (root == null)
                    {
                        root = node;
                    }
                    var i = 0;
                    foreach (var identifier in PatternIdentifiers)
                    {
                        if (node.getText().Trim('"') == identifier)
                        {
                            var path = Translator.GetExtendedPathList(node);
                            if (paths[i] == null)
                            {
                                paths[i] = path;
                            }
                        }

                        i++;
                    }
                }
            };
            VB6Compiler.Visit(compileResult, visitorCallback);

            var minDepth = 1000;
            foreach (var path in paths)
            {
                if (path == null)
                {
                    throw new InvalidOperationException(nameof(path) + " is null");
                }
                minDepth = Math.Min(minDepth, path.Count);
            }
            
            /*
            var lowestCommonDepth = 0;
            for (int i = 0; i < minDepth; i++)
            {
                var allEqual = true;
                foreach (var path in paths)
                {
                    if (!path[i].Equals(paths[0][i]))
                    {
                        allEqual = false;
                        break;
                    }
                }
                if (allEqual)
                {
                    lowestCommonDepth = i;
                }
                else
                {
                    break;
                }
            }*/

            var lowestCommonDepth = cutDepthofDollarContent;

            vbASGType = paths[0][lowestCommonDepth].NodeTypeName;

            var cutPaths = new List<IndexedPath>[PatternIdentifiers.Length];
            var k = 0;
            foreach (var path in paths)
            {
                var cutPath = new List<IndexedPath>();
                for (int i = lowestCommonDepth + 1; i < path.Count; i++)
                {
                    cutPath.Add(path[i]);
                }
                cutPaths[k] = cutPath;
                k++;
            }

            VbPaths = cutPaths;

            return compileResult;
        }

        public static List<IndexedPath> GetExtendedPathList(SyntaxNode node)
        {
            var iterationNode = node;
            var s = new List<IndexedPath>();
            while (iterationNode != null)
            {
                var index = -1;
                if (iterationNode.Parent != null)
                {
                    var i = 0;
                    foreach (var child in iterationNode.Parent.ChildNodes())
                    {
                        if (child == iterationNode)
                        {
                            index = i;
                        }
                        i++;
                    }

                    if (i == -1)
                    {
                        throw new InvalidOperationException("could not find child node in parent");
                    }
                }
                s.Add(new IndexedPath(iterationNode.GetType().Name, index));
                iterationNode = iterationNode.Parent;
            }
            s.Reverse();
            return s;
        }

        public SyntaxTree CSharpParsePattern(string pattern)
        {
            var tree = SyntaxFactory.ParseExpression(pattern);

            // TODO: reduce code duplication

            var paths = new List<IndexedPath>[PatternIdentifiers.Length];
            SyntaxNode root = null;

            var callback = new CsharpVisitorCallback()
            {
                Callback = node =>
                {
                    if (root == null)
                    {
                        root = node;
                    }
                    var i = 0;
                    foreach (var identifier in PatternIdentifiers)
                    {
                        Console.Error.WriteLine("Node: " + node.Kind() + ": " + node.ToFullString());
                        if (node.ToFullString().Trim() == identifier)
                        {
                            var path = GetExtendedPathList(node);
                            if (paths[i] == null)
                            {
                                paths[i] = path;
                            }
                        }
                        i++;
                    }
                }
            };
            
            var walker = new CustomCSharpSyntaxWalker(callback);
            walker.Visit(tree);

            foreach (var path in paths)
            {
                if (path == null)
                {
                    throw new InvalidOperationException(nameof(path) + " is null");
                }
            }

            CSharpPaths = paths;

            /*
            var minDepth = maxDepths.Min();
            var lowestCommonDepth = 0;
            for (int i = 0; i < minDepth; i++)
            {
                var allEqual = true;
                foreach (var path in paths)
                {
                    if (!path[i].Equals(paths[0][i]))
                    {
                        allEqual = false;
                        break;
                    }
                }
                if (allEqual)
                {
                    lowestCommonDepth = i;
                }
                else
                {
                    break;
                }
            }
            csharpRoot = paths[0][lowestCommonDepth].NodeTypeName;
            VbPaths = paths;
            */

            return null;
        }

        public string GetUniqueIdentifier(string identifier)
        {
            return identifier + "NobodyShouldConflictWithThis";
        }

        public ExpressionSyntax Translate(Translator translator, ParseTree tree)
        {
            if (translator == null)
            {
                throw new ArgumentNullException(nameof(translator));
            }
            if (tree == null)
            {
                throw new ArgumentNullException(nameof(tree));
            }

            string replacement = csharpString;
            // Make a long string so we don't conflict
            foreach (var identifier in PatternIdentifiers)
            {
                replacement = replacement.Replace(identifier, GetUniqueIdentifier(identifier));
            }

            var translations = new SyntaxTree[PatternIdentifiers.Length];
            var i = 0;
            foreach (var path in VbPaths)
            {
                var node = LookupNodeFromPath(tree, path);
                var identifier = PatternIdentifiers[i];
                var translated = translator.TranslateNode(node);
                if (translated != null)
                {
                    replacement = replacement.replace(GetUniqueIdentifier(identifier), translated.NormalizeWhitespace().ToFullString());
                }
                else
                {
                    replacement = replacement.replace(GetUniqueIdentifier(identifier), "UNTRANSLATED(" + node.getText().Trim() + ")");
                }
                
                //translations[i] = translated;
                i++;
            }
            
            // TODO: what about SyntaxFactory.ParseStatement()? add support for it.
            var rewritten = SyntaxFactory.ParseExpression(replacement);

            return rewritten;
        }

    }
}
