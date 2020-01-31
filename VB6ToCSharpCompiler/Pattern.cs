using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.sun.org.apache.xpath.@internal.compiler;
using io.proleap.vb6.asg.metamodel.statement.print;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using org.antlr.v4.runtime.tree;
using SyntaxTree = org.antlr.v4.runtime.tree.SyntaxTree;

namespace VB6ToCSharpCompiler
{
    public class Pattern
    {
        private string[] PatternIdentifiers = new string[] {
            "A", "B"
        };
        private const string Content = "$CONTENT";

        public string vbASGType { get; set; }

        private List<IndexedPath>[] VbPaths;
        private List<IndexedPath>[] CSharpPaths;

        private CompileResult VbCompiledPattern;
        private SyntaxTree CsharpParsedPattern;

        private string vbWrapper = @"
public Sub MySub()
$CONTENT
End Sub
";

        public Pattern(string A, string B)
        {
            VbCompiledPattern = VbParsePattern(A);
            CsharpParsedPattern = CSharpParsePattern(B);
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

        public string PrintPath(List<IndexedPath> path)
        {
            var list = new List<string>();
            foreach (var item in path)
            {
                list.Add(item.NodeTypeName + ":" + item.ChildIndex);
            }
            return string.Join("/", list);
        }

        ParseTree LookupNodeFromPath(ParseTree root, string[] path)
        {
            ParseTree node = root;
            foreach (var nodeTypeName in path)
            {
                // TODO: make generic method to iterate children of node
                int c = node.getChildCount();
                for (int i = 0; i < c; i++)
                {
                    var child = node.getChild(i);
                    if (child.GetType().Name == nodeTypeName)
                    {

                    }
                }
            }
            
            return node;
        }

        public CompileResult VbParsePattern(string pattern)
        {
            string code = vbWrapper.Replace(Content, pattern);
            var compileResult = VB6Compiler.Compile("Test.bas", code, false);

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
            vbASGType = paths[0][lowestCommonDepth].NodeTypeName;
            VbPaths = paths;

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

        public SyntaxTree Translate(ParseTree tree)
        {
            return null;
        }

    }
}
