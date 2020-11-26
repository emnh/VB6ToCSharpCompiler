using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VB6ToCSharpCompiler;

namespace VB6ToCSharpCompilerTests
{
    [TestClass]
    public class TestCompiler
    {
        public void CompileAndTestBasContent(string content, string expected)
        {
            string fileName = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".bas";
            System.IO.File.WriteAllText(fileName, content);
            var compileResult = VB6Compiler.Compile(fileName);
            Assert.AreEqual(expected, compileResult.CSharpCode);
        }

        [TestMethod]
        public void TestSimpleProgram()
        {
            CompileAndTestBasContent(@"
Attribute VB_Name = ""modApplikasjon""
Option Explicit

public Sub Z()
End Sub
",
        @"
");
        }
    }
}
