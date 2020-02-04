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
using com.sun.tools.corba.se.idl;
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
        public List<TokenInfo> PatternTokens { get; set; }

        /*= new string[] {
            "A", "B"
        };*/
        private const string Content = "CONTENT";
        private const string ReservedLetterForDiscardedResults = "Z";

        //private Translator translator;

        public string VbTreeNodeType { get; set; }
        public string VbCode { get; set; }
        private string csharpString;

        private List<IndexedPath>[] VbPaths;
        private List<IndexedPath>[] CSharpPaths;

        private CompileResult VbCompiledPattern;
        private SyntaxTree CsharpParsedPattern;

        private int cutDepthOfContent = -1;
        private int finalCutDepthOfContent = -1;
        private List<IndexedPath> cutPath;
        private List<IndexedPath> tokenPath;

        private string vbWrapperCode;

        public Pattern(string vbWrapperCode, string vbCode, string csharpString)
        {
            this.vbWrapperCode = vbWrapperCode;
            this.VbCode = vbCode;
            this.csharpString = csharpString;

            VbCompiledPattern = VbParsePattern(vbCode);
            //CsharpParsedPattern = CSharpParsePattern(csharpString);
            Console.Error.WriteLine("ASGType: " + VbTreeNodeType);
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
            // TODO: return cutPath always?
            if (VbPaths.Length > 0)
            {
                return PrintPath(VbPaths[0]);
            }

            return "CUTPATH: " + PrintPath(cutPath);
        }

        // This method looks up the depth of the $CONTENT identifier.
        // We use it to determine where to cut relative paths.

        // TODO: Parse wrapper only once, not one time per pattern. Well, it hardly matters for performance I think, so leave it.
        public void SetCutDepthAndCutPath()
        {
            var wrapperCompileResult = VB6Compiler.Compile("Test.bas", vbWrapperCode, false);
            var translator = new Translator(wrapperCompileResult);
            Console.Error.WriteLine("SetCutDepthAndCutPath: " + vbWrapperCode);

            var visitorCallback = new VisitorCallback()
            {
                Callback = (node, parent) =>
                {
                    
                    var identifier = node.getText().Trim('"').Trim();
                    //if (identifier == Content)
                    var path = translator.GetExtendedPathList(node);

                    Console.Error.WriteLine("SetCutDepthAndCutPath: " + VbCode + ": " + PrintPath(path));

                    if (identifier == Content)
                    {
                        // Exact match
                        if (path[path.Count - 1].NodeTypeName == "VsICSContext")
                        {
                            cutDepthOfContent = path.Count - 1;
                            Console.Error.WriteLine("SetCutDepth: MATCH");
                        }
                        // Bounded
                        if (!PathContains(path, "VsICSContext"))
                        {
                            cutDepthOfContent = path.Count;
                            Console.Error.WriteLine("SetCutDepth: BOUND MATCH");
                        }
                        cutPath = path.Take(cutDepthOfContent).ToList();
                    }
                }
            };
            // First time is to set cutDepthOfContent
            VB6Compiler.Visit(wrapperCompileResult, visitorCallback);
            // Second time is to set cutPath
            VB6Compiler.Visit(wrapperCompileResult, visitorCallback);
            if (cutDepthOfContent == -1)
                throw new InvalidOperationException(nameof(cutDepthOfContent) + " not initialized");
        }

        public bool PathContains(List<IndexedPath> path, string typeName)
        {
            foreach (var item in path)
            {
                if (item.NodeTypeName == typeName)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsInsideSubOrFunction(List<IndexedPath> path)
        {
            foreach (var item in path.Take(path.Count - 1))
            {
                if (item.NodeTypeName == "SubStmtContext" || item.NodeTypeName == "FunctionStmtContext")
                {
                    return true;
                }
            }
            return false;
        }

        public string[] GetIdentifiersAndSetTokens(Translator translator, CompileResult compileResult)
        {
            var identifiers = new List<string>();
            var tokensPerNode = new List<TokenInfo>();
            //var tokens = new List<Tuple<int, string>>();

            var seen = new Dictionary<string, bool>();

            var visitorCallback = new VisitorCallback()
            {
                Callback = (node, parent) =>
                {
                    //if (node.GetType().Name == "AmbiguousIdentifierContext")
                    var identifier = node.getText().Trim('"').Trim();

                    

                    //Console.Error.WriteLine("GetIdentifier: " + identifier);
                    var path = translator.GetExtendedPathList(node);
                    if (IsInsideSubOrFunction(path))
                    {
                        if (identifier.Length == 1 &&
                            identifier.All(char.IsUpper))
                        {
                            if (!seen.ContainsKey(identifier))
                            {
                                seen[identifier] = true;
                                if (identifier != ReservedLetterForDiscardedResults) identifiers.Add(identifier);
                            }
                        }
                        else
                        {
                            // A general method will for each token add the parent node, and its path,
                            // then extract this path for each token, extract all tokens from path and
                            // check that the token exists in the extracted tokens.
                            // Does this account for the order? Partly, it accounts for the order
                            // within different nodes, but not multiple tokens within the same node.
                            // So to handle that we need to group tokens by node.

                            var tokens = GetTokens(node);
                            if (tokens.Count > 0)
                            {
                                tokenPath = tokenPath ?? path;
                                tokensPerNode.Add(new TokenInfo(path, tokens.Select(x => x.Item2).ToList()));
                                Console.Error.WriteLine("TOKENS" + string.Join("@", tokens));
                            }
                            
                        }
                    }
                    
                }
            };
            
            VB6Compiler.Visit(compileResult, visitorCallback);

            PatternTokens = tokensPerNode;

            if (cutDepthOfContent == -1)
                throw new InvalidOperationException(nameof(cutDepthOfContent) + " not initialized");

            return identifiers.ToArray();
        }

        public List<Tuple<int, string>> GetTokens(ParseTree node)
        {
            var tokens = new List<Tuple<int, string>>();
            for (var i = 0; i < node.getChildCount(); i++)
            {
                var tk = node.getChild(i);
                if (tk is TerminalNodeImpl tni)
                {
                    var sym = tni.symbol.getText().Trim();
                    // TODO: Check if inside CONTENT rather than ignore public.
                    if (!string.IsNullOrEmpty(sym) && sym != "public" &&
                        sym.All(c => char.IsLetter(c) || char.IsWhiteSpace(c) || c == '$'))
                    {
                        tokens.Add(Tuple.Create(tni.getSourceInterval().a, sym));
                    }
                }
            }
            tokens.Sort((x, y) => x.Item1.CompareTo(y.Item1));

            return tokens;
        }

        public static string PrintPath(List<IndexedPath> path)
        {
            var list = new List<string>();
            foreach (var item in path) list.Add(item.NodeTypeName + ":" + item.ChildIndex);

            return string.Join("/", list);
        }

        //public static string LookupNodeType(string a)
        public static string LookupNodeType(SyntaxNode node)
        {
            return node.GetType().Name;
        }

        public static string LookupNodeType(ParseTree node)
        {
            //string a2 = a == "ICS_S_VariableOrProcedureCallContext" ? "FunctionStmtContext" : a;
            //string a2 = a == "ICS_B_ProcedureCallContext" ? "SubStmtContext" : a;
            //string a2 = a == "ICS_B_ProcedureCallContext" ? "ICS_S_ProcedureOrArrayCallContext" : a;
            string returnValue = node.GetType().Name;
            //returnValue = returnValue == "ImplicitCallStmt_InBlockContext" ? "SubStmtContext" : returnValue;
            //returnValue = returnValue == "ICS_B_ProcedureCallContext" ? "SubStmtContext" : returnValue;
            returnValue = returnValue == "ICS_B_ProcedureCallContext" ? "ICS_S_ProcedureOrArrayCallContext" : returnValue;
            returnValue = returnValue == "CertainIdentifierContext" ? "AmbiguousIdentifierContext" : returnValue;
            return returnValue;
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
                if (!last && LookupNodeType(child) != indexedPath.NodeTypeName)
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
            SetCutDepthAndCutPath();

            var code = vbWrapperCode.Replace(Content, pattern);
            var compileResult = VB6Compiler.Compile("Test.bas", code, false);
            var translator = new Translator(compileResult);
            PatternIdentifiers = GetIdentifiersAndSetTokens(translator, compileResult);

            var paths = new List<IndexedPath>[PatternIdentifiers.Length];
            ParseTree root = null;

            Console.Error.WriteLine("PATTERN: " + VbCode);

            var visitorCallback = new VisitorCallback()
            {
                Callback = (node, parent) =>
                {
                    Console.Error.WriteLine("Node: " + PrintPath(translator.GetExtendedPathList(node)) + ": " + node.getText());

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

            var lowestCommonDepth = -1;
            var cutDepth = -1;

            var comparePath = paths.Length > 0 ? paths[0] : tokenPath;
            if (comparePath == null)
            {
                Console.Error.WriteLine("VB Code: " + VbCode);
                throw new InvalidOperationException(nameof(comparePath) + " is null");
            }
            int commonDepth = Math.Min(cutPath.Count, comparePath.Count);

            for (int i = 0; i < commonDepth; i++)
            {
                Console.Error.WriteLine("COMPARE PATHS: " + PrintPath(cutPath));
                foreach (var path in paths)
                {
                    Console.Error.WriteLine("COMPARE PATHS: " + PrintPath(path));
                }
                Console.Error.WriteLine("");
                if (cutPath[i].NodeTypeName != comparePath[i].NodeTypeName)
                {
                    break;
                }
                else
                {
                    lowestCommonDepth = Math.Min(comparePath.Count - 1, i + 1);
                }
            }

            if (lowestCommonDepth >= comparePath.Count)
            {
                Console.Error.WriteLine("VB Code: " + VbCode + ", Identifier: " + PatternIdentifiers[0]);
            }
            VbTreeNodeType = comparePath[lowestCommonDepth].NodeTypeName;
            
            // Skip uninteresting wrapper nodes
            while (VbTreeNodeType == "VsICSContext" ||
                   VbTreeNodeType == "ImplicitCallStmt_InStmtContext")
            {
                lowestCommonDepth++;
                VbTreeNodeType = comparePath[lowestCommonDepth].NodeTypeName;
            }
            
            // VbTreeNodeType == "ICS_B_ProcedureCallContext")
            while (VbTreeNodeType == "ArgsCallContext" ||
                   VbTreeNodeType == "AmbiguousIdentifierContext")
            {
                lowestCommonDepth--;
                VbTreeNodeType = comparePath[lowestCommonDepth].NodeTypeName;
            }

            cutDepth = lowestCommonDepth + 1;
            finalCutDepthOfContent = cutDepth;

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

                s.Add(new IndexedPath(LookupNodeType(iterationNode), index));
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

            if (!DoTokensMatch(translator, tree, true))
            {
                canTranslate = false;
            }

            return canTranslate;
        }

        public bool DoTokensMatch(Translator translator, ParseTree tree, bool justCheck = false)
        {
            if (translator == null) throw new ArgumentNullException(nameof(translator));

            if (tree == null) throw new ArgumentNullException(nameof(tree));

            // TODO: Doesn't handle recursion yet.
            /*
            var testPattern = ".*" + string.Join(".*", PatternTokens) + ".*";
            var rex = Regex.IsMatch(tree.getText(), testPattern);
            Console.Error.WriteLine("TOKENMATCHING: " + tree.getText() + " against " + testPattern + " match? " + rex);
            return rex;
            */
            bool returnValue = true;
            foreach (var tokenInfo in PatternTokens)
            {
                if (finalCutDepthOfContent > tokenInfo.Path.Count)
                {
                    // In this case the tokens were part of the template, not the pattern so we should skip them.
                    continue;
                }
                var tokenCutPath = tokenInfo.Path.Skip(finalCutDepthOfContent).ToList();
                var node = LookupNodeFromPath(translator, tree, tokenCutPath, true);
                if (node == null)
                {
                    Console.Error.WriteLine("UNMATCHED TOKENS 1: " + VbTreeNodeType + ":" + PrintPath(tokenCutPath) + ":" + PrintPath(tokenInfo.Path));
                    returnValue = false;
                    break;
                }
                var tokens = GetTokens(node);
                var tokenStrings = tokens.Select(x => x.Item2).ToList();
                if (!tokenInfo.Tokens.SequenceEqual(tokenStrings))
                {
                    Console.Error.WriteLine("UNMATCHED TOKENS 2: " + VbTreeNodeType + ":" + PrintPath(tokenCutPath) + ":" + PrintPath(tokenInfo.Path));
                    Console.Error.WriteLine("PATH LENGTH COMPARISON: " + finalCutDepthOfContent + ":" + tokenInfo.Path.Count);
                    Console.Error.WriteLine(string.Join("@", tokenInfo.Tokens));
                    Console.Error.WriteLine(string.Join("@", tokenStrings));
                    //return false;
                    returnValue = false;
                }
            }
            return returnValue;
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

            if (!DoTokensMatch(translator, tree))
            {
                throw new InvalidOperationException("Missing tokens. Fix CanTranslate.");
            }

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
                    Console.Error.WriteLine("UNTRANSLATED_ " + LookupNodeType(node)+
                                            ", ASG: " + translator.GetAsg<ASGElement>(node)?.GetType()?.Name +
                                            ", Identifier: " + identifier + ", Path: " + PrintPath(path) +
                                            "(" + node.getText().Trim() + ")");
                    replacement = replacement.replace(
                        uid,
                        "throw new NotImplementedException(\"UNTRANSLATED: " +
                        LookupNodeType(node) +
                        ":" + node.getText().Trim() + "\");");
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