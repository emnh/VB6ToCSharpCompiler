using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using org.antlr.v4.runtime.tree;

namespace VB6ToCSharpCompiler
{
    public class TranslatorForPattern
    {

        private Dictionary<string, string> patterns;
        private Dictionary<string, Pattern> compiledPatterns;

        public TranslatorForPattern()
        {
            patterns = new Dictionary<string, string>()
            {
                // Note: Pattern letters which are used on left hand side but do not appear on right hand side simply disappear
                {"C = A + B", "A + B"},
                {"C = A - B", "A - B"},
                {"C = A / B", "A / B"},
                {"C = A * B", "A * B"},
                /* Not supported in VB6
                {"A += B", "A += B"},
                {"A -= B", "A -= B"},
                {"A /= B", "A /= B"},
                {"A *= B", "A *= B"},
                */
                {"C = A & B", "A + B"}
            };

            compiledPatterns = new Dictionary<string, Pattern>();
            foreach (var item in patterns)
            {
                var pattern = new Pattern(item.Key, item.Value);
                compiledPatterns.Add(pattern.vbASGType, pattern);
            }
        }

        public bool CanTranslate(ParseTree tree)
        {
            var name = tree.GetType().Name;
            return compiledPatterns.ContainsKey(name);
        }

        public SyntaxTree Translate(ParseTree tree)
        {
            var name = tree.GetType().Name;
            var pattern = compiledPatterns[name];
            return pattern.Translate(tree);
        }
    }
}
