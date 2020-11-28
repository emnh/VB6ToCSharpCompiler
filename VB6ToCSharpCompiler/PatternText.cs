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

        public const string vbStatementFunctionWrapper = @"
public Function A()
End Function
public Sub Z()
    CONTENT
End Sub
";
        
        // Dim B,C,D,E,F,G,H as Integer
        // public Sub A(b as Integer, c as Integer, d as Integer, e as Integer, f as Integer, g as Integer, h as Integer)
        public const string vbStatementSubWrapper = @"
public Sub A()
End Sub
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

        public string VbWrapperCode { get; set; }
        public string VbCode { get; set; }
        public string CSharpCode { get; set; }

        public PatternText(string vbWrapperCode, string vbCode, string csharpCode)
        {
            VbWrapperCode = vbWrapperCode;
            VbCode = vbCode;
            CSharpCode = csharpCode;
        }

        public VbToCsharpPattern Compile()
        {
            return new VbToCsharpPattern(VbWrapperCode, VbCode, CSharpCode);
        }

        public string LogValue()
        {
            return " WRAPPER " + VbWrapperCode + " VBOCDE " + VbCode + " CSHARP " + CSharpCode;
        }
    }
}
