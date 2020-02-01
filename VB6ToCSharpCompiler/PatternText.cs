using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VB6ToCSharpCompiler
{
    public class PatternText
    {

        public const string vbExpressionWrapper = @"
public Sub Z()
    Z = CONTENT
End Sub
";

        public const string vbStatementWrapper = @"
public Sub Z()
    CONTENT
End Sub
";

        public const string vbConditionalWrapper = @"
public Sub Z()
    If CONTENT Then
    End If
End Sub
";
        public const string vbDeclaredStatementWrapper = @"
Dim A(0 to 5) As Integer
Dim B as Integer
Dim C as Integer
public Sub Z()
    CONTENT
End Sub
";

        public const string vbFunctionStatementWrapper = @"
public Function Z()
    CONTENT
End Function";

        private string VbWrapperCode;
        private string VbCode;
        private string CSharpCode;

        public PatternText(string vbWrapperCode, string vbCode, string csharpCode)
        {
            VbWrapperCode = vbWrapperCode;
            VbCode = vbCode;
            CSharpCode = csharpCode;
        }

        public Pattern Compile()
        {
            return new Pattern(VbWrapperCode, VbCode, CSharpCode);
        }

        public string LogValue()
        {
            return " WRAPPER " + VbWrapperCode + " VBOCDE " + VbCode + " CSHARP " + CSharpCode;
        }
    }
}
