
using org.antlr.v4.runtime.tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VB6ToCSharpCompiler.VB6NodeTranslatorLogging
{
public class TypeStmtContext : VB6NodeTranslator
{
    public TypeStmtContext(VB6NodeTree nodeTree, Dictionary<ContextNodeType, VB6NodeTranslator> translatorDict) : base(nodeTree, translatorDict)
    {
    }

    public override ContextNodeType GetNodeContextType()
    {
        return ContextNodeType.TypeStmtContext;
    }

    public override IEnumerable<OutToken> PreTranslate(List<ParseTree> parseTrees)
    {
            if (parseTrees == null)
            {
                throw new ArgumentNullException(nameof(parseTrees));
            }
            var typeName = "";
            var body = "Dim s as String\r\ns = \"\"\r\n";
            foreach (var child in parseTrees)
            {
                if (VB6NodeTranslator.GetNodeTypeName(child).Contains("Identifier"))
                {
                    typeName =  child.getText();
                }
                
                if (VB6NodeTranslator.GetNodeTypeName(child) == "TypeStmt_ElementContext")
                {
                    var childTypeName = "";
                    var childName = "";
                    var fromto = new List<int>();
                    foreach (var child2 in nodeTree.GetChildren(child))
                    {
                        if (VB6NodeTranslator.GetNodeTypeName(child2).Contains("Identifier"))
                        {
                            childName = child2.getText();
                        }
                        if (VB6NodeTranslator.GetNodeTypeName(child2) == "SubscriptsContext")
                        {
                            foreach (var child3 in nodeTree.GetChildren(child2))
                            {
                                if (VB6NodeTranslator.GetNodeTypeName(child3) == "SubscriptContext")
                                {
                                    foreach (var child4 in nodeTree.GetChildren(child3))
                                    {
                                        if (VB6NodeTranslator.GetNodeTypeName(child4) == "VsLiteralContext")
                                        {
                                            fromto.Add(int.Parse(child4.getText(), System.Globalization.NumberFormatInfo.InvariantInfo));
                                        }
                                    }
                                }
                            }
                        }
                        if (VB6NodeTranslator.GetNodeTypeName(child2) == "AsTypeClauseContext")
                        {
                            foreach (var child3 in nodeTree.GetChildren(child2)) {
                                if (VB6NodeTranslator.GetNodeTypeName(child3) == "TypeContext") {
                                    childTypeName = child3.getText();
                                }
                            }
                        }
                    }
                    if (fromto.Count == 2)
                    {
                        body += "For x = " + fromto[0] + " to " + fromto[1] + "\r\n";
                        body += "  s = s & Serialize" + childTypeName + "(arg." + childName + "(x))\r\n";
                        body += "Next x\r\n";
                    }
                    else
                    {
                        body += "s = s & Serialize" + childTypeName + "(arg." + childName + ")\r\n";
                    }
                    
                }
            }
            var serializeFunctionName = "Serialize" + typeName;
            body += serializeFunctionName + " = s\r\n";
            nodeTree.AppendExtra(serializeFunctionName, @"
Public Function $FUNCTION(ByRef arg as $ARGTYPE) as String
    $BODY    
End Function
".Replace("$FUNCTION", serializeFunctionName).Replace("$ARGTYPE", typeName).Replace("$BODY", body));

            return new List<OutToken>();
    }
        
    public override IEnumerable<OutToken> PostTranslate(List<ParseTree> parseTrees)
    {
        return new List<OutToken>();
    }
}
}
