using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using com.ibm.icu.text;
using com.sun.org.apache.xpath.@internal.compiler;
using ikvm.extensions;
using io.proleap.vb6;
using io.proleap.vb6.asg.metamodel;
using io.proleap.vb6.asg.metamodel.statement.print;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
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
        private const string Content = "CONTENT";
        private const string ReservedLetterForDiscardedResults = "Z";

        //private Translator translator;

        public string vbTreeNodeType { get; set; }
        public string vbString { get; set; }
        private string csharpString;

        private List<IndexedPath>[] VbPaths;
        private List<IndexedPath>[] CSharpPaths;

        private CompileResult VbCompiledPattern;
        private SyntaxTree CsharpParsedPattern;

        private int cutDepthofContent = -1;

        private string vbWrapperCode;

        public Pattern(string vbWrapperCode, string vbString, string csharpString)
        {
            this.vbWrapperCode = vbWrapperCode;
            this.vbString = vbString;
            this.csharpString = csharpString;

            VbCompiledPattern = VbParsePattern(vbString);
            //CsharpParsedPattern = CSharpParsePattern(csharpString);
            Console.Error.WriteLine("ASGType: " + vbTreeNodeType);
            foreach (var path in VbPaths) Console.Error.WriteLine("Path: " + PrintPath(path));

            /*
            foreach (var path in CSharpPaths)
            {
                Console.Error.WriteLine("Path: " + PrintPath(path));
            }
            */
        }

        public string GetLogPath()
        {
            return PrintPath(VbPaths[0]);
        }

        // This method looks up the depth of the $CONTENT identifier.
        // We use it to determine where to cut relative paths.

        // TODO: Parse wrapper only once, not one time per pattern. Well, it hardly matters for performance I think, so leave it.
        public void SetCutDepth()
        {
            var wrapperCompileResult = VB6Compiler.Compile("Test.bas", vbWrapperCode, false);
            var translator = new Translator(wrapperCompileResult);
            Console.Error.WriteLine("SetCutDepth: " + vbWrapperCode);

            var visitorCallback = new VisitorCallback()
            {
                Callback = (node, parent) =>
                {
                    Console.Error.WriteLine("SetCutDepth: " + vbString + ": " + PrintPath(translator.GetExtendedPathList(node)));
                    var identifier = node.getText().Trim('"').Trim();
                    //if (identifier == Content)
                    if (cutDepthofContent == -1 && identifier == Content)
                    {
                        var path = translator.GetExtendedPathList(node);
                        // TODO: do it more elegantly
                        if (node.GetType().Name == "IfConditionStmtContext")
                            cutDepthofContent = path.Count;
                        else if (node.GetType().Name == "BlockContext")
                            cutDepthofContent = path.Count + 1;
                        else if (node.GetType().Name == "BlockStmtContext")
                            cutDepthofContent = path.Count - 1;
                        else if (node.GetType().Name == "VsICSContext")
                            cutDepthofContent = path.Count - 1;
                        else
                        {
                            Console.Error.WriteLine(node.GetType().Name);
                            Debugger.Break();
                        }

                        Console.Error.WriteLine("SetCutDepth: " + node.GetType().Name + ": " + PrintPath(path));
                    }
                }
            };
            VB6Compiler.Visit(wrapperCompileResult, visitorCallback);
            if (cutDepthofContent == -1)
                throw new InvalidOperationException(nameof(cutDepthofContent) + " not initialized");
        }

        public bool IsInsideSubOrFunction(List<IndexedPath> path)
        {
            foreach (var item in path)
            {
                if (item.NodeTypeName == "SubStmtContext" || item.NodeTypeName == "FunctionStmtContext")
                {
                    return true;
                }
            }
            return false;
        }

        public string[] GetIdentifiers(Translator translator, CompileResult compileResult)
        {
            var identifiers = new List<string>();

            var seen = new Dictionary<string, bool>();

            var visitorCallback = new VisitorCallback()
            {
                Callback = (node, parent) =>
                {
                    //if (node.GetType().Name == "AmbiguousIdentifierContext")
                    var identifier = node.getText().Trim('"').Trim();
                    //Console.Error.WriteLine("GetIdentifier: " + identifier);
                    if (identifier.Length == 1 &&
                        identifier.All(char.IsUpper) &&
                        IsInsideSubOrFunction(translator.GetExtendedPathList(node)))
                    {
                        if (!seen.ContainsKey(identifier))
                        {
                            seen[identifier] = true;
                            if (identifier != ReservedLetterForDiscardedResults) identifiers.Add(identifier);
                        }
                    }
                }
            };
            VB6Compiler.Visit(compileResult, visitorCallback);
            if (cutDepthofContent == -1)
                throw new InvalidOperationException(nameof(cutDepthofContent) + " not initialized");

            return identifiers.ToArray();
        }

        public static string PrintPath(List<IndexedPath> path)
        {
            var list = new List<string>();
            foreach (var item in path) list.Add(item.NodeTypeName + ":" + item.ChildIndex);

            return string.Join("/", list);
        }

        private static ParseTree LookupNodeFromPath(Translator translator, ParseTree root, List<IndexedPath> path,
            bool justCheck = false)
        {
            var node = root;
            foreach (var indexedPath in path)
            {
                //var child = node.getChild(indexedPath.ChildIndex);

                var children = translator.GetChildren(node);
                if (!(indexedPath.ChildIndex >= 0 && indexedPath.ChildIndex < children.Count))
                {
                    Console.Error.WriteLine("Did not find child: " + PrintPath(path) + " in " + root.getText());
                    if (justCheck) return null;
                    throw new InvalidOperationException("child outside bounds");
                }

                var child = children[indexedPath.ChildIndex];

                var last = indexedPath == path[path.Count - 1];
                if (!last && child.GetType().Name != indexedPath.NodeTypeName)
                {
                    if (justCheck) return null;

                    throw new InvalidOperationException("child type doesn't match");
                }

                node = child;
            }

            if (node == null)
            {
                if (justCheck) return null;

                throw new InvalidOperationException(nameof(node) + " was null");
            }

            return node;
        }

        public CompileResult VbParsePattern(string pattern)
        {
            SetCutDepth();

            var code = vbWrapperCode.Replace(Content, pattern);
            var compileResult = VB6Compiler.Compile("Test.bas", code, false);
            var translator = new Translator(compileResult);
            PatternIdentifiers = GetIdentifiers(translator, compileResult);

            var paths = new List<IndexedPath>[PatternIdentifiers.Length];
            ParseTree root = null;

            Console.Error.WriteLine("PATTERN: " + vbString);

            var visitorCallback = new VisitorCallback()
            {
                Callback = (node, parent) =>
                {
                    Console.Error.WriteLine("Node: " + PrintPath(translator.GetExtendedPathList(node)));

                    if (root == null) root = node;

                    var i = 0;
                    foreach (var identifier in PatternIdentifiers)
                    {
                        if (node.getText().Trim('"') == identifier &&
                            IsInsideSubOrFunction(translator.GetExtendedPathList(node)))
                        {
                            var path = translator.GetExtendedPathList(node);
                            if (paths[i] == null) paths[i] = path;
                        }

                        i++;
                    }
                }
            };
            VB6Compiler.Visit(compileResult, visitorCallback);

            var minDepth = 1000;
            foreach (var path in paths)
            {
                if (path == null) throw new InvalidOperationException(nameof(path) + " is null");

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

            var lowestCommonDepth = cutDepthofContent;
            var cutDepth = lowestCommonDepth + 1;

            //if (Translator.IsStatement(root))

            /*if (!vbString.Contains(("Z =")))
            {
                lowestCommonDepth--;
                cutDepth = lowestCommonDepth + 1;
            }*/

            // TODO: How to extract vbTreeNodeType when there are no identifiers, as in Exit Function for instance.

            vbTreeNodeType = paths[0][lowestCommonDepth].NodeTypeName;
            // We are not interested in this type, it is too general.
            // Therefore we go one step deeper.
            // It applies to member expressions, A.B.
            // TODO: Do this in more elegant way.
            if (vbTreeNodeType == "VsICSContext")
            {
                lowestCommonDepth++;
                cutDepth = lowestCommonDepth + 1;
                vbTreeNodeType = paths[0][lowestCommonDepth].NodeTypeName;
                if (vbTreeNodeType == "ImplicitCallStmt_InStmtContext")
                {
                    lowestCommonDepth++;
                    cutDepth = lowestCommonDepth + 1;
                    vbTreeNodeType = paths[0][lowestCommonDepth].NodeTypeName;
                }
            }

            var cutPaths = new List<IndexedPath>[PatternIdentifiers.Length];
            var k = 0;
            foreach (var path in paths)
            {
                var cutPath = new List<IndexedPath>();
                for (var i = cutDepth; i < path.Count; i++) cutPath.Add(path[i]);

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
                        if (child == iterationNode) index = i;

                        i++;
                    }

                    if (i == -1) throw new InvalidOperationException("could not find child node in parent");
                }

                s.Add(new IndexedPath(iterationNode.GetType().Name, index));
                iterationNode = iterationNode.Parent;
            }

            s.Reverse();
            return s;
        }

        [Obsolete("We changed to string rewriting so this method is not needed")]
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
                    if (root == null) root = node;

                    var i = 0;
                    foreach (var identifier in PatternIdentifiers)
                    {
                        Console.Error.WriteLine("Node: " + node.Kind() + ": " + node.ToFullString());
                        if (node.ToFullString().Trim() == identifier)
                        {
                            var path = GetExtendedPathList(node);
                            if (paths[i] == null) paths[i] = path;
                        }

                        i++;
                    }
                }
            };

            var walker = new CustomCSharpSyntaxWalker(callback);
            walker.Visit(tree);

            foreach (var path in paths)
                if (path == null)
                    throw new InvalidOperationException(nameof(path) + " is null");

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
            return identifier + "ZnobodyZshouldZconflictZwithZthis";
        }


        public ExpressionSyntax TranslateExpression(Translator translator, ParseTree tree)
        {
            return (ExpressionSyntax) Translate(translator, tree);
        }

        // Checks if all pattern identifier variables can be found in tree
        public bool CanTranslate(Translator translator, ParseTree tree)
        {
            var canTranslate = true;
            foreach (var path in VbPaths)
            {
                var node = LookupNodeFromPath(translator, tree, path, true);
                if (node == null)
                {
                    canTranslate = false;
                    break;
                }
            }

            return canTranslate;
        }

        private void DebugDumpCSharpSyntax(SyntaxNode tree)
        {
            if (tree == null) throw new ArgumentNullException(nameof(tree));

            var callback = new CsharpVisitorCallback()
            {
                Callback = node => { Console.Error.WriteLine("Node: " + node.Kind() + ": " + node.ToFullString()); }
            };

            var walker = new CustomCSharpSyntaxWalker(callback);
            walker.Visit(tree);
        }

        public SyntaxNode Translate(Translator translator, ParseTree tree)
        {
            if (translator == null) throw new ArgumentNullException(nameof(translator));

            if (tree == null) throw new ArgumentNullException(nameof(tree));

            var replacement = csharpString;
            // Make a long string so we don't conflict
            foreach (var identifier in PatternIdentifiers)
            {
                replacement = Regex.Replace(replacement, "\\b" + identifier + "\\b", GetUniqueIdentifier(identifier));
            }
                

            var translations = new SyntaxTree[PatternIdentifiers.Length];
            var pathIndex = 0;
            foreach (var identifier in PatternIdentifiers)
            {
                var path = VbPaths[pathIndex];

                var node = LookupNodeFromPath(translator, tree, path);

                Console.Error.WriteLine(
                    "Extracting Identifier: " + identifier + " with path " +
                    PrintPath(path) + " for code " + tree.getText());
                var translated = translator.TranslateNode(node);

                var uid = GetUniqueIdentifier(identifier);
                if (translated != null)
                {
                    DebugDumpCSharpSyntax(translated);
                    Console.Error.WriteLine("Extracted: " + translated.NormalizeWhitespace().ToFullString());
                    replacement = replacement.replace(uid, translated.NormalizeWhitespace().ToFullString());
                }
                else
                {
                    Console.Error.WriteLine("UNTRANSLATED_ " + node.GetType().Name +
                                            ", ASG: " + translator.GetAsg<ASGElement>(node)?.GetType()?.Name +
                                            ", Identifier: " + identifier + ", Path: " + PrintPath(path) +
                                            "(" + node.getText().Trim() + ")");
                    replacement = replacement.replace(
                        uid,
                        "UNTRANSLATED_ " + node.GetType().Name);
                }

                //translations[i] = translated;
                pathIndex++;
            }

            // TODO: what about SyntaxFactory.ParseStatement()? add support for it.
            SyntaxNode rewritten = null;
            if (Translator.IsStatement(tree))
                rewritten = SyntaxFactory.ParseStatement(replacement);
            else
                rewritten = SyntaxFactory.ParseExpression(replacement);

            return rewritten.NormalizeWhitespace();
        }
    }
}