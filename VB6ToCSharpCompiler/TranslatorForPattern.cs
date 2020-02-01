using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using com.sun.xml.@internal.ws.message;
using io.proleap.vb6.asg.exception;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using org.antlr.v4.runtime.tree;

namespace VB6ToCSharpCompiler
{
    public static class TranslatorForPattern
    {

        private static Dictionary<string, List<Pattern>> compiledPatterns;
        //private static List<Pattern> compiledPatternsList;

        // Statement pattern
        static PatternText S(string VbCode, string CsharpCode)
        {
            return new PatternText(PatternText.vbStatementWrapper, VbCode, CsharpCode);
        }

        static PatternText S2(string VbCode, string CsharpCode)
        {
            return new PatternText(PatternText.vbDeclaredStatementWrapper, VbCode, CsharpCode);
        }

        static PatternText S3(string VbCode, string CsharpCode)
        {
            return new PatternText(PatternText.vbFunctionStatementWrapper, VbCode, CsharpCode);
        }

        // Expression pattern
        static PatternText E(string VbCode, string CsharpCode)
        {
            return new PatternText(PatternText.vbExpressionWrapper, VbCode, CsharpCode);
        }

        // Conditional pattern
        static PatternText C(string VbCode, string CsharpCode)
        {
            return new PatternText(PatternText.vbConditionalWrapper, VbCode, CsharpCode);
        }

        public static void IntializeTranslatorForPattern()
        {
            var patterns = new List<PatternText>()
            {
                // Note: Pattern letters which are used on left hand side but do not appear on right hand side simply disappear
                E("A & B", "A + B"),
                E("A * B", "A * B"),
                E("A + B", "A + B"),
                E("A - B", "A - B"),
                E("A / B", "A / B"),
                E("A \\ B", "((int) (Math.Round(A)) / ((int) Math.Round(B)"),
                E("A ^ B", "Math.Pow(A, B)"),
                E("AddressOf A","VB6Compat.AddressOf(A);"),
                // Eqv -> Not Xor?
                E("A Eqv B","VB6Compat.Eqv(A, B)"),
                // Imp -> Google which logical operators to use
                E("A Imp B","VB6Compat.VB6Imp(A, B)"),
                E("A Is B", "A == B;"),
                E("(A)", "(A)"),
                E("A.B", "A.B"),
                E("A(B)", "A[B]"),
                S("A.B = C", "A.B = C;"),
                // A is declared as array in statement wrapper
                S2("A(B) = C", "A[B] = C;"),
                S("A = B", "A = B;"),
                S("A:", "A:"),
                S("Open A For Input Shared As B", "VB6Compat.OpenFileShared(A, B);"),
                S("Open A For Input As B", "VB6Compat.OpenFileForInput(A, B);"),
                S("Open A For Output As B", "VB6Compat.OpenFileForOutput(A, B);"),
                S("Close A", "VB6Compat.CloseFile(A);"),
                S("Erase A", "VB6Compat.EraseArray(A);"),
                S("Exit Sub", "return;"),
                S3("Exit Function", "return " + Translator.FunctionReturnValueName + ";"),
                S("If A Then\n B\n Else\n C\n End If\n", "if (A) { B; } else { C; }"),
                S("If A Then\n B\n End If\n", "if (A) { B; }"),
                S("For Each A In B\n C\n Next", CsharpCode: "foreach (var A in B) { C; }"),
                //S("Select Case A\n Case B\n C\n Case D\n E\n End Select",
                //  "switch (A) { case B: C; break; case D: E; break; };"),
                C("A = B", @"A == B"),
                C("A < B", @"A < B"),
                C("A <= B", @"A <= B"),
                C("A > B", @"A > B"),
                C("A >= B", @"A >= B"),
                C("A <> B", @"A != B"),
                C("A Or B", "A || B"),
                C("A And B", "A && B"),
                C("Not A", "!A"),
                /*
                new PatternText(
                    PatternText.vbFunctionStatementWrapper,
                    "B = A",
                    Translator.FunctionReturnValueName + " = A")
                    */
            };

            compiledPatterns = new Dictionary<string, List<Pattern>>();
            foreach (var patternText in patterns)
            {
                Pattern pattern = null;
                try
                {
                    pattern = patternText.Compile();
                } catch (VbParserException e)
                {
                    Console.Error.WriteLine("Pattern Compile Failed: " + patternText.LogValue());
                    throw;
                }

                Console.Error.WriteLine(
                    "Pattern: " + patternText.LogValue() + ", " +
                    nameof(pattern.VbTreeNodeType) + ": " + pattern.VbTreeNodeType);

                if (!compiledPatterns.ContainsKey(pattern.VbTreeNodeType))
                {
                    compiledPatterns[pattern.VbTreeNodeType] = new List<Pattern>();
                }
                compiledPatterns[pattern.VbTreeNodeType].Add(pattern);
            }

            foreach (var pat in compiledPatterns)
            {
                foreach (var pat2 in pat.Value)
                {
                    Console.Error.WriteLine("PATTERNSTATUS: " + pat2.VbCode + ": " + pat2.VbTreeNodeType + ": " + pat2.GetLogPath());
                    foreach (var tki in pat2.PatternTokens)
                    {
                        Console.Error.WriteLine("PATTERNTOKENS: " + Pattern.PrintPath(tki.Path));
                        Console.Error.WriteLine("PATTERNTOKENS: " +
                                                string.Join("@", tki.Tokens));
                    }
                }
            }

            ;
        }

        public static bool CanTranslate(Translator translator, ParseTree tree)
        {
            if (tree == null)
            {
                throw new ArgumentNullException(nameof(tree));
            }
            var name = tree.GetType().Name;
            var canTranslate = compiledPatterns.ContainsKey(name);
            if (canTranslate)
            {
                canTranslate = false;
                foreach (var pattern in compiledPatterns[name])
                {
                    if (pattern.CanTranslate(translator, tree))
                    {
                        canTranslate = true;
                        break;
                    };
                }

                if (!canTranslate)
                {
                    Console.Error.WriteLine("PATTERN NODE: " + name + ", CASE: " + tree.getText());
                    foreach (var pattern in compiledPatterns[name])
                    {
                        Console.Error.WriteLine("TOKENS: " + string.Join("@", pattern.PatternTokens));
                    }

                    throw new InvalidOperationException("Valid patterns for case, but none of them worked.");
                }
            }
            return canTranslate;
        }

        public static SyntaxNode Translate(Translator translator, ParseTree tree)
        {
            if (translator == null)
            {
                throw new ArgumentNullException(nameof(translator));
            }
            if (tree == null)
            {
                throw new ArgumentNullException(nameof(tree));
            }
            var name = tree.GetType().Name;
            var patterns = compiledPatterns[name];
            foreach (var pattern in patterns)
            {
                if (pattern.CanTranslate(translator, tree))
                {
                    return pattern.Translate(translator, tree);
                }
            }
            throw new InvalidOperationException("Should not happen if we call CanTranslate before Translate");
        }

        public static ExpressionSyntax TranslateExpression(Translator translator, ParseTree tree)
        {
            var result = Translate(translator, tree);
            Console.Error.WriteLine("Result: " + result);
            if (result is ExpressionSyntax es)
            {
                return es;
            }
            else if (result is ExpressionStatementSyntax ess)
            {
                return ess.Expression;
            }
            else
            {
                throw new InvalidOperationException("This shouldn't happen.");
            }
        }

        public static string DocPatterns()
        {
            return string.Join(", ", compiledPatterns.Keys);
        }

    }
}
