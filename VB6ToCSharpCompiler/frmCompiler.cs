﻿using io.proleap.vb6;
using io.proleap.vb6.asg.metamodel.impl;
using java.io;
using java.nio.charset;
using org.antlr.v4.runtime;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VB6ToCSharpCompiler
{
    public partial class frmCompiler : Form
    {
        private frmVB6ASTBrowser frmVb6AstBrowser;
        private frmPatterns frmPatternsForm;
        private static string Folder = @"\VB6Code";

        public frmCompiler()
        {
            InitializeComponent();
        }

        private void btnConvert_Click(object sender, EventArgs e)
        {
            var fileName = (string) lstFileNames.SelectedItem;

            var compileResult = VB6Compiler.Compile(fileName);
            txtVBCode.Text = compileResult.VBCode;
            txtCSharpCode.Text += "// C Sharp Code:\r\n" + compileResult.CSharpCode;
            txtVBCode.ScrollBars = ScrollBars.Both;
            txtCSharpCode.ScrollBars = ScrollBars.Both;
        }

        private void ShowFiles()
        {
            lstFileNames.Items.Clear();
            foreach (var fileName in VB6Compiler.GetFiles(Folder))
            {
                lstFileNames.Items.Add(fileName);
            }
            //TranslatorForPattern.IntializeTranslatorForPattern();
        }

        private void frmCompiler_Load(object sender, EventArgs e)
        {
            ShowFiles();
        }

        private void btnBrowseVB6AST_Click(object sender, EventArgs e)
        {
            var fileName = (string)lstFileNames.SelectedItem;
            frmVb6AstBrowser = new frmVB6ASTBrowser(fileName);
            frmVb6AstBrowser.Visible = true;
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            var enabled = DebugClass.Enabled;
            DebugClass.Enabled = false;
            frmPatternsForm = new frmPatterns(new List<string>(VB6Compiler.GetFiles(Folder)));
            frmPatternsForm.Visible = true;
            //var tfp = new TranslatorForPattern();
            DebugClass.Enabled = enabled;

        }

        private void lstFileNames_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    Folder = fbd.SelectedPath;
                }
            }
            ShowFiles();
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            foreach (var fileName in VB6Compiler.GetFiles(Folder))
            {
                var compileResult = VB6Compiler.Compile(fileName, null, false);
                var tree = new VB6NodeTree(compileResult);
                ASTPatternGenerator.GetCode(tree);
            }
        }

        private IEnumerable<OutToken> GetComments(CompileResult result)
        {
            var modules = result.Program.getModules();

            for (int mi = 0; mi < modules.size(); mi++)
            {
                var module = (ModuleImpl) modules.get(mi);
                var cts = module.getTokens();
                var tokens = cts.getTokens();
                //DebugClass.LogError("TOKENS SIZE: " + tokens.size() + ": " + tokens.get(0));
                for (int i = 0; i < tokens.size(); i++)
                {
                    var token = (Token)tokens.get(i);
                    //DebugClass.LogError("CHANNEL: " + token.getChannel() + ": " + token.getText());
                    if (token.getChannel() != 0)
                    {
                        //DebugClass.LogError("CHANNEL: " + token.getChannel() + ": " + token.getStartIndex() + ": " + token.getText());
                        yield return new OutToken(token.getTokenIndex(), token.getText());
                    }
                }
            }

            //var text = System.IO.File.ReadAllText(fileName, Encoding.GetEncoding(1252));
            ////using (var inputStream = new ByteArrayInputStream(Encoding.GetEncoding(1252).GetBytes(text)))
            //DebugClass.LogError("TEXT: " + text);
            //using (var inputStream = new java.io.ByteArrayInputStream(Encoding.GetEncoding(1252).GetBytes(text)))
            //{
            //    var charset = Charset.forName("Windows-1252");
            //    var lexer = new VisualBasic6Lexer(CharStreams.fromStream(inputStream, charset));
            //    var cts = new CommonTokenStream(lexer);
            //    var tokens = cts.getTokens();
            //    DebugClass.LogError("TOKENS SIZE: " + tokens.size() + ": " + tokens.get(0));
            //    for (int i = 0; i < tokens.size(); i++)
            //    {
            //        var token = (Token) tokens.get(i);
            //        DebugClass.LogError("CHANNEL: " + token.getChannel() + ": " + token.getText());
            //        if (token.getChannel() == 0)
            //        {
            //            yield return new OutToken(token.getStartIndex(), token.getText());
            //        }
            //    }
            //}
        }

        private void btnTranslateWithLogging_Click(object sender, EventArgs e)
        {
            var outFolder = Folder + "-out";
            var extraModuleName = "VB6CompilerExtra.bas";

            var extraModule = @"
Public Sub Log(ByVal Msg As String)
    Static FileNum As Single
    If FileNum = 0 Then
        FileNum = FreeFile
        Open App.Path & ""\ProgramLog.txt"" For Output As FileNum
    End If
    Print #FileNum, Msg
End Sub

Public Sub LogEnter(ParamArray Text() As Variant)
    Dim intLoopIndex As Integer, Serialized As String
    Serialized = """"
    For intLoopIndex = 0 To UBound(Text)
      Serialized = Serialized & "", "" & Text(intLoopIndex)
    Next intLoopIndex
    Log ""ENTER "" & Serialized
End Sub

Public Sub LogLeave(ParamArray Text() As Variant)
    Dim intLoopIndex As Integer, Serialized As String
    Serialized = """"
    For intLoopIndex = 0 To UBound(Text)
      Serialized = Serialized & "", "" & Text(intLoopIndex)
    Next intLoopIndex
    Log ""LEAVE "" & Serialized
End Sub

Public Function SerializeInteger(arg as Integer) as String
    SerializeInteger = CStr(arg)
End Function

Public Function SerializeDouble(arg as Double) as String
    SerializeDouble = CStr(arg)
End Function

Public Function SerializeSingle(arg as Single) as String
    SerializeSingle = CStr(arg)
End Function

Public Function SerializeBoolean(arg as Boolean) as String
    If arg Then
        SerializeBoolean = ""True""
    Else
        SerializeBoolean = ""False""
    End If
End Function

Public Function SerializeForm(arg as Form) as String
    SerializeForm = arg.Name
End Function

Public Function SerializeString(arg As String) As String
    SerializeString = arg
End Function

Public Function SerializeDate(arg As Date) As String
    SerializeDate = CStr(arg)
End Function
";
            foreach (var fileName in VB6Compiler.GetFiles(Folder, true, true))
            {
                var bname = Path.GetFileName(fileName);
                string outFileName = Path.Combine(outFolder, bname);
                if (
                    (fileName.EndsWith(".frx", System.StringComparison.InvariantCulture)) ||
                    (fileName.EndsWith(".vbw", System.StringComparison.InvariantCulture)))
                {
                    System.IO.File.Copy(fileName, outFileName, true);
                    continue;
                }
                if (fileName.EndsWith(".vbp", System.StringComparison.InvariantCulture))
                {
                    var text = System.IO.File.ReadAllText(fileName, Encoding.GetEncoding(1252));
                    text += "Module=mod_VB6CompilerExtra; " + extraModuleName + "\r\n";
                    System.IO.File.WriteAllText(outFileName, text, Encoding.GetEncoding(1252));
                    continue;
                }

                var compileResult = VB6Compiler.Compile(fileName, null, false);
                var tree = new VB6NodeTree(compileResult);
                tree.AddExtraModule = (a, b) =>
                {
                    extraModule += b;
                };
                var se = VB6NodeTranslatorLoader.Translate(tree);
                var sl = se.ToList();
                sl.AddRange(GetComments(compileResult));
                sl.Sort((a, b) => a.index.CompareTo(b.index));
                //var s = String.Join("", sl.Select(x => x.index.ToString() + ":" + x.token));
                var s = String.Join("", sl.Select(x => x.token));
                System.IO.Directory.CreateDirectory(outFolder);
                s = Regex.Replace(s, "([^\r])\n", "$1\r\n");
                System.IO.File.WriteAllText(outFileName, s, Encoding.GetEncoding(1252));
            }

            System.IO.File.WriteAllText(Path.Combine(outFolder, extraModuleName), extraModule, Encoding.GetEncoding(1252));

            string message = "Wrote new files to: " + outFolder;
            string caption = "Compilation Successful!";
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            var result = MessageBox.Show(message, caption, buttons);
        }
    }
}
