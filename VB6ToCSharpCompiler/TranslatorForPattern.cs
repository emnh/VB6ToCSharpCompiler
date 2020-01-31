using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using org.antlr.v4.runtime.tree;

namespace VB6ToCSharpCompiler
{
    public static class TranslatorForPattern
    {

        private static Dictionary<string, List<Pattern>> compiledPatterns;
        //private static List<Pattern> compiledPatternsList;

        static TranslatorForPattern()
        {
            var patterns = new List<Tuple<string, string>>()
            {
                // Note: Pattern letters which are used on left hand side but do not appear on right hand side simply disappear
                // Note: Z = is a special pattern to denote an expression.
                Tuple.Create("Z = A + B", "A + B"),
                Tuple.Create("Z = A - B", "A - B"),
                Tuple.Create("Z = A / B", "A / B"),
                Tuple.Create("Z = A * B", "A * B"),
                /* Not supported in VB6
                {"A += B", "A += B"},
                {"A -= B", "A -= B"},
                {"A /= B", "A /= B"},
                {"A *= B", "A *= B"},
                */
                Tuple.Create("Z = A & B", "A + B"),
                Tuple.Create("Z = (A)", "(A)"),
                Tuple.Create("A = B", "A = B;"),
                Tuple.Create(@"
If A Then
    B
Else
    C
End If
",
                    @"
if (A) {
    B;
} else {
    C;
}
"
),
                Tuple.Create(@"
If A Then
    B
End If
",
                    @"
if (A) {
    B;
}
"
                )
            };

            compiledPatterns = new Dictionary<string, List<Pattern>>();
            foreach (var item in patterns)
            {
                var pattern = new Pattern(item.Item1, item.Item2);
                if (!compiledPatterns.ContainsKey(pattern.vbASGType))
                {
                    compiledPatterns[pattern.vbASGType] = new List<Pattern>();
                }
                compiledPatterns[pattern.vbASGType].Add(pattern);
            }
        }

        public static bool CanTranslate(Translator translator, ParseTree tree)
        {
            if (tree == null)
            {
                throw new ArgumentNullException(nameof(tree));
            }
            var name = tree.GetType().Name;
            var allGood = compiledPatterns.ContainsKey(name);
            if (allGood)
            {
                allGood = false;
                foreach (var pattern in compiledPatterns[name])
                {
                    if (pattern.CanTranslate(translator, tree))
                    {
                        allGood = true;
                        break;
                    };
                }
            }
            return allGood;
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
            return (ExpressionSyntax) Translate(translator, tree);
        }

        public static string DocPatterns()
        {
            return string.Join(", ", compiledPatterns.Keys);
        }

    }
}
