using org.antlr.v4.runtime.tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VB6ToCSharpCompiler.VB6NodeTranslatorLogging
{
    public static class LoggingFunctionality
    {
        public static string GetFunctionName(VB6NodeTranslator translator, List<ParseTree> parseTrees)
        {
            if (translator == null)
            {
                throw new ArgumentNullException(nameof(translator));
            }
            if (parseTrees == null)
            {
                throw new ArgumentNullException(nameof(parseTrees));
            }
            foreach (var child in parseTrees)
            {
                if (VB6NodeTranslator.GetNodeTypeName(child).Contains("Identifier"))
                {
                    return child.getText();
                }
            }
            return "UNKNOWN_FUNCTION";
        }

        public static string StringArgs(string functionName, VB6NodeTranslator translator, List<ParseTree> parseTrees)
        {
            if (translator == null)
            {
                throw new ArgumentNullException(nameof(translator));
            }
            if (parseTrees == null)
            {
                throw new ArgumentNullException(nameof(parseTrees));
            }

            var sl = new List<string>();
            sl.Add(functionName);
            foreach (var child in parseTrees)
            {
                if (VB6NodeTranslator.GetNodeTypeName(child) == "ArgListContext")
                {
                    foreach (var child2 in translator.nodeTree.GetChildren(child))
                    {
                        if (VB6NodeTranslator.GetNodeTypeName(child2) == "ArgContext") {
                            var argName = "";
                            var argType = "";
                            foreach (var child3 in translator.nodeTree.GetChildren(child2))
                            {
                                if (VB6NodeTranslator.GetNodeTypeName(child3).Contains("Identifier")) {
                                    argName = child3.getText();
                                }
                                if (VB6NodeTranslator.GetNodeTypeName(child3).Contains("AsTypeClauseContext"))
                                {
                                    foreach (var child4 in translator.nodeTree.GetChildren(child3))
                                    {
                                        if (VB6NodeTranslator.GetNodeTypeName(child4).Contains("TypeContext")) {
                                            argType = child4.getText();
                                        }
                                    }
                                }
                            }
                            var serializeFunctionName = "Serialize" + argType;
                            //var body = "return arg";

                            sl.Add(serializeFunctionName + "(" + argName + ")");
//                          translator.nodeTree.AppendExtra(serializeFunctionName, @"
//Public Function $FUNCTION(ByVal arg as $ARGTYPE) as String
//    $BODY    
//End Function
//".Replace("$FUNCTION", serializeFunctionName).Replace("$ARGTYPE", argType).Replace("$BODY", body));

                        }
                    }
                }
            }
            return string.Join(", ", sl);
        }

        public static IEnumerable<OutToken> FunctionEnter(VB6NodeTranslator translator, List<ParseTree> parseTrees)
        {
            if (translator == null)
            {
                throw new ArgumentNullException(nameof(translator));
            }
            if (parseTrees == null)
            {
                throw new ArgumentNullException(nameof(parseTrees));
            }
            if (parseTrees.Count > 0)
            {
                //var index = 0.0;
                //foreach (var child in parseTrees)
                //{
                //    if (VB6NodeTranslator.GetNodeTypeName(child).Contains("BlockContext"))
                //    {
                //        index = translator.GetFirstOrLastTokenIndex(child);
                //        break;
                //    }
                //    //index = translator.GetFirstOrLastTokenIndex(child, true);
                //}
                var index = translator.GetFirstOrLastTokenIndex(parseTrees[parseTrees.Count - 1]);

                //DebugClass.LogError("INDEX: " + index);
                var functionName = GetFunctionName(translator, parseTrees);
                var args = StringArgs("\"" + functionName + "\"", translator, parseTrees);

                yield return new OutToken(index - 0.5, @"
'Dim FileNum
'FileNum = FreeFile
'Open App.Path & ""\ProgramLog.txt"" For Append As FileNum
'Print #FileNum, ""ENTER $FUNCTION""
'Close FileNum
LogEnter $FUNCTION
".Replace("$FUNCTION", args));
            }
            else
            {
                DebugClass.LogError("PRECALL FAIL");
            }
        }

        public static IEnumerable<OutToken> FunctionLeave(VB6NodeTranslator translator, List<ParseTree> parseTrees)
        {
            if (translator == null)
            {
                throw new ArgumentNullException(nameof(translator));
            }
            if (parseTrees == null)
            {
                throw new ArgumentNullException(nameof(parseTrees));
            }
            if (parseTrees.Count > 0)
            {
                var index = translator.GetFirstOrLastTokenIndex(parseTrees[parseTrees.Count - 1], true);
                //var index = 0;
                //foreach (var child in parseTrees)
                //{
                //    if (VB6NodeTranslator.GetNodeTypeName(child) == "BlockContext")
                //    {
                //        index = translator.GetFirstOrLastTokenIndex(child);
                //    }
                //}

                var functionName = GetFunctionName(translator, parseTrees);
                var args = StringArgs("\"" + functionName + "\"", translator, parseTrees);

                yield return new OutToken(index + 0.5, @"
'Dim FileNum
'FileNum = FreeFile
'Open App.Path & ""\ProgramLog.txt"" For Append As FileNum
'Print #FileNum, ""ENTER $FUNCTION""
'Close FileNum
LogLeave $FUNCTION
".Replace("$FUNCTION", args));
            }
            else
            {
                DebugClass.LogError("POSTCALL FAIL");
            }
        }
    }
}
