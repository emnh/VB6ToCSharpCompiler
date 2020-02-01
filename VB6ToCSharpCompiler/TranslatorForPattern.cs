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
using io.proleap.vb6;
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

        static PatternText S4(string VbCode, string CsharpCode)
        {
            return new PatternText(PatternText.vbStatementFunctionWrapper, VbCode, CsharpCode);
        }

        static PatternText S5(string VbCode, string CsharpCode)
        {
            return new PatternText(PatternText.vbStatementSubWrapper, VbCode, CsharpCode);
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

        //public static PatternText[] MultiplyPattern()
        //{

        //}

        public static void IntializeTranslatorForPattern()
        {
            var patterns = new List<PatternText>()
            {
                // More special cases should go higher up (for now)

                // Special cases
                // 5 clauses, special case
                S("Select Case A\n Case B,J\n C\n Case D\n E\n Case F\n G\n Case Is >= K\n I\n Case Else\n L\n End Select",
                    "switch (A) { case B: case J: C; break; case D: E; case F: G; case int i when i >= K: I; break; default: L; };"),
                // 2 clauses, double cases
                S("Select Case A\n Case B,F\n C\n Case D,G\n E\n End Select",
                    "switch (A) { case B: case F: C; break; case D: case G: E; break; };"),

                // General cases
                S("Mid(A, B, C) = D", "VB6Compat.SetSlice(A, B, C, D);"),
                // Note: Pattern letters which are used on left hand side but do not appear on right hand side simply disappear
                E("A & B", "A + B"),
                E("A Mod B", "A % B"),
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
                C("TypeOf A Is B", "A.GetType() == typeof(B)"),
                //E("TypeOf A is B", "A.GetType() == typeof(B)"),
                //E("TypeOf A", "A.GetType()"),
                // TODO: This assumes that all usage of Is operator is a type check.
                //E("A Is B", "A == typeof(B)"),
                E("A Is B", "A == B"),
                E("(A)", "(A)"),
                E("A.B", "A.B"),
                // Function calls up to 8 arguments: TODO: Give exception when too many arguments.
                E("A$(B,C,D,E,F,G,H)", "(string) A(B,C,D,E,F,G,H)"),
                E("A$(B,C,D,E,F,G)", "(string) A(B,C,D,E,F,G)"),
                E("A$(B,C,D,E,F)", "(string) A(B,C,D,E,F)"),
                E("A$(B,C,D,E)", "(string) A(B,C,D,E)"),
                E("A$(B,C,D)", "(string) A(B,C,D)"),
                E("A$(B,C)", "(string) A(B,C)"),
                E("A$(B)", "(string) A(B)"),
                E("A$()", "(string) A()"),
                // Function calls up to 8 arguments: TODO: Give exception when too many arguments.
                E("A(B,C,D,E,F,G,H)", "A(B,C,D,E,F,G,H)"),
                E("A(B,C,D,E,F,G)", "A(B,C,D,E,F,G)"),
                E("A(B,C,D,E,F)", "A(B,C,D,E,F)"),
                E("A(B,C,D,E)", "A(B,C,D,E)"),
                E("A(B,C,D)", "A(B,C,D)"),
                E("A(B,C)", "A(B,C)"),
                E("A(B)", "A(B)"),
                E("A()", "A()"),
                //S("A", "A"),
                //E("A()", "A()"),
                S("MsgBox A, B, C", "VB6Compat.MsgBox(A, B, C);"),
                //E("A(B)", "A[B]"),
                // Arne: Check this one
                S("Beep", "Console.Beep();"),
                S("A.B = C", "A.B = C;"),
                // A is declared as array in statement wrapper
                S2("A(B) = C", "A[B] = C;"),
                S("A = B", "A = B;"),
                S("A:", "A:"),
                S("Open A For Input Shared As B", "VB6Compat.OpenFileForInputShared(A, B);"),
                S("Open A For Input As B", "VB6Compat.OpenFileForInput(A, B);"),
                S("Open A For Binary Shared As B", "VB6Compat.OpenFileForBinaryShared(A, B);"),
                S("Open A For Binary As B", "VB6Compat.OpenFileForBinary(A, B);"),
                S("Open A For Output As B", "VB6Compat.OpenFileForOutput(A, B);"),
                S("Line Input A, B", "VB6Compat.LineInput(A, B);"),
                S("Get A, B, C", "VB6Compat.GetRecord(A, B, C);"),
                S("Get A, , C", "VB6Compat.GetRecord(A, null, C);"),
                S("Put A, B, C", "VB6Compat.PutRecord(A, B, C);"),
                S("Put A, , C", "VB6Compat.PutRecord(A, null, C);"),
                S("Print A, B", "VB6Compat.LineOutputPrint(A, B);"),
                S("Name A As B", "System.IO.File.Move(A, B);"),
                S("FileCopy A, B", "System.IO.File.Copy(A, B);"),
                S("MkDir A", "System.IO.DirectoryInfo.CreateDirectory(A);"),
                S("Close A", "VB6Compat.CloseFile(A);"),
                S("Erase A", "VB6Compat.EraseArray(A);"),
                S("Kill A", "File.Delete(A);"),
                S("Exit Sub", "return;"),
                S("Exit For", "throw new NotImplementedException(\"Check if break corresponds to for loop!\"); break;"),
                S("Exit Do", "throw new NotImplementedException(\"Check if break corresponds to while loop!\"); break;"),
                S3("Exit Function", "return " + Translator.FunctionReturnValueName + ";"),
                S("If A Then\n B\n Else\n C\n End If\n", "if (A) { B; } else { C; }"),
                S("If A Then\n B\n End If\n", "if (A) { B; }"),
                // Handle empty if clause
                S("If A Then\n \n End If\n", "if (A) { ; }"),
                S("If A Then B", "if (A) { B; }"),
                S("If A Then B Else C", "if (A) { B; } else { C; }"),
                S("For Each A In B\n C\n Next", CsharpCode: "foreach (var A in B) { C; }"),
                S("For A = B To C Step D\n E\n Next", CsharpCode: "foreach (var A in VB6Compat.ForLoopRange(B, C, D))) { E; }"),
                S("For A = B To C \n D\n Next", CsharpCode: "foreach (var A in VB6Compat.ForLoopRange(B, C, 1))) { D; }"),
                S("Do While A\nB\nLoop", "while (A) { B; }"),
                S("Do\nA\nLoop", "while (true) { A; }"),
                S("Do\nA\nLoop Until B", "while (true) { A; if (B) break; }"),
                S("With A\nB\nEnd With", "{ var withVar = A; B; }"),
                S("Const A As B = C", "const B A = C;"),
                E(".B", "withVar.B"),
                S(".B = C", "withVar.B = C;"),
                // Arne: What's this End doing at the end of the case?
                S("End", "Application.Exit();"),
                // 5 clauses
                S("Select Case A\n Case B\n C\n Case D\n E\n Case F\n G\n Case H\n I\n Case J\n K\n End Select",
                    "switch (A) { case B: C; break; case D: E; case F: G; case H: I; case J: K; break; };"),
                // 4 clauses
                S("Select Case A\n Case B\n C\n Case D\n E\n Case F\n G\n Case H\n I\n End Select",
                    "switch (A) { case B: C; break; case D: E; case F: G; case H: I; break; };"),
                // 2 clauses + default
                S("Select Case A\n Case B\n C\n Case D\n E\n Case Else\n F\n End Select",
                    "switch (A) { case B: C; break; case D: E; break; default: F; };"),
                // 3 clauses
                S("Select Case A\n Case B\n C\n Case D\n E\n Case F\n G\n End Select",
                    "switch (A) { case B: C; break; case D: E; case F: G; break; };"),
                // 1 clause + default
                S("Select Case A\n Case B\n C\n Case Else\n D\n End Select",
                    "switch (A) { case B: C; break; default: D; };"),
                // 2 clauses
                S("Select Case A\n Case B\n C\n Case D\n E\n End Select",
                    "switch (A) { case B: C; break; case D: E; break; };"),
                S("GoTo A", "throw new NotImplementedException(\"GoTo A\");"),
                S("On Error GoTo A", "throw new NotImplementedException(\"On Error GoTo A\");"),
                S("On Error Resume Next", "throw new NotImplementedException(\"On Error Resume Next\");"),
                S("Resume Next", "throw new NotImplementedException(\"Resume Next\");"),
                S("Resume A", "throw new NotImplementedException(\"Resume A\");"),
                // Function calls up to 8 arguments: TODO: Give exception when too many arguments.
                S4("A B,C,D,E,F,G,H", "A(B,C,D,E,F,G,H);"),
                S4("A B,C,D,E,F,G", "A(B,C,D,E,F,G);"),
                S4("A B,C,D,E,F", "A(B,C,D,E,F);"),
                S4("A B,C,D,E", "A(B,C,D,E);"),
                S4("A B,C,D", "A(B,C,D);"),
                S4("A B,C", "A(B,C);"),
                S4("A B", "A(B);"),
                // Sub calls up to 8 arguments: TODO: Give exception when too many arguments.
                S5("A B,C,D,E,F,G,H", "A(B,C,D,E,F,G,H);"),
                S5("A B,C,D,E,F,G", "A(B,C,D,E,F,G);"),
                S5("A B,C,D,E,F", "A(B,C,D,E,F);"),
                S5("A B,C,D,E", "A(B,C,D,E);"),
                S5("A B,C,D", "A(B,C,D);"),
                S5("A B,C", "A(B,C);"),
                S5("A B", "A(B);"),
                // Call calls up to 8 arguments
                S("Call A(B,C,D,E,F,G,H)", "A(B,C,D,E,F,G,H);"),
                S("Call A(B,C,D,E,F,G)", "A(B,C,D,E,F,G);"),
                S("Call A(B,C,D,E,F)", "A(B,C,D,E,F);"),
                S("Call A(B,C,D,E)", "A(B,C,D,E);"),
                S("Call A(B,C,D)", "A(B,C,D);"),
                S("Call A(B,C)", "A(B,C);"),
                S("Call A(B)", "A(B);"),
                //S("Call A()", "A();"),
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
            var name = Pattern.LookupNodeType(tree);
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
                        foreach (var tokens in pattern.PatternTokens)
                        {
                            Console.Error.WriteLine("TOKENS: " + string.Join("@", tokens.Tokens));
                        }
                    }

                    if (name != "ImplicitCallStmt_InBlockContext")
                    {
                        throw new InvalidOperationException("Valid patterns for case, but none of them worked.");
                    }
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
            var name = Pattern.LookupNodeType(tree);
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
