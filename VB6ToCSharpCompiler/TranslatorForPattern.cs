using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using org.antlr.v4.runtime.tree;

namespace VB6ToCSharpCompiler
{
    public static class TranslatorForPattern
    {

        private static Dictionary<string, string> patterns;
        private static Dictionary<string, Pattern> compiledPatterns;

        static TranslatorForPattern()
        {
            patterns = new Dictionary<string, string>()
            {
                // Note: Pattern letters which are used on left hand side but do not appear on right hand side simply disappear
                {"Z = A + B", "A + B"},
                {"Z = A - B", "A - B"},
                {"Z = A / B", "A / B"},
                {"Z = A * B", "A * B"},
                /* Not supported in VB6
                {"A += B", "A += B"},
                {"A -= B", "A -= B"},
                {"A /= B", "A /= B"},
                {"A *= B", "A *= B"},
                */
                {"Z = A & B", "A + B"},
                {"Z = (A)", "(A)"}
            };

            compiledPatterns = new Dictionary<string, Pattern>();
            foreach (var item in patterns)
            {
                var pattern = new Pattern(item.Key, item.Value);
                compiledPatterns.Add(pattern.vbASGType, pattern);
            }
        }

        public static bool CanTranslate(ParseTree tree)
        {
            if (tree == null)
            {
                throw new ArgumentNullException(nameof(tree));
            }
            var name = tree.GetType().Name;
            return compiledPatterns.ContainsKey(name);
        }

        public static ExpressionSyntax Translate(Translator translator, ParseTree tree)
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
            var pattern = compiledPatterns[name];
            return pattern.Translate(translator, tree);
        }
    }
}
