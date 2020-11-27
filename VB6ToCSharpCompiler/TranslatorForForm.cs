using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ikvm.extensions;
using io.proleap.vb6;
using io.proleap.vb6.asg.metamodel.impl;
using javax.swing.plaf;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using org.antlr.v4.runtime.tree;
using sun.management.jmxremote;
using sun.reflect.generics.reflectiveObjects;

namespace VB6ToCSharpCompiler
{
    public class TranslatorForForm
    {
        private CompileResult _compileResult;
        private VB6NodeTree vb6NodeTree;
        //private readonly Dictionary<string, C> children;

        private string IgnoreString = "IGNORE";

        private Dictionary<string, string> vbtoCSharpControlTypeMapping;

        private Dictionary<string, string> vbToCSharpPropertyMapping;

        private Dictionary<string, bool> seenProperties = new Dictionary<string, bool>();

        public TranslatorForForm(CompileResult compileResult)
        {
            this._compileResult = compileResult;
            vb6NodeTree = new VB6NodeTree(compileResult);

            vbtoCSharpControlTypeMapping = new Dictionary<string, string>()
            {
                { "MSFlexGridLib.MSFlexGrid", "DataGridView"},
                { "TabDlg.SSTab", "TabControl" },
                { "VB.CheckBox", "CheckBox"},
                { "VB.ComboBox", "ComboBox" },
                { "VB.CommandButton", "Button" },
                { "VB.Form", "Form" },
                { "VB.Frame", "Panel" },
                { "VB.Label", "Label" },
                { "VB.Line", IgnoreString },
                { "VB.ListBox", "ListBox"},
                { "VB.TextBox", "TextBox" }
            };

            vbToCSharpPropertyMapping = new Dictionary<string, string>()
            {
                // Control: VB.Form
                {"BorderStyle", "BorderStyle"},
                // Control: VB.Form
                {"Caption", "Caption"},
                // Control: VB.Form
                {"ClientHeight", "ClientHeight"},
                // Control: VB.Form
                {"ClientLeft", "ClientLeft"},
                // Control: VB.Form
                {"ClientTop", "ClientTop"},
                // Control: VB.Form
                {"ClientWidth", "ClientWidth"},
                // Control: VB.Form
                {"Font", "Font"},
                // Control: Font
                {"Name", "Name"},
                // Control: Font
                {"Size", "Size"},
                // Control: Font
                {"Charset", "Charset"},
                // Control: Font
                {"Weight", "Weight"},
                // Control: Font
                {"Underline", "Underline"},
                // Control: Font
                {"Italic", "Italic"},
                // Control: Font
                {"Strikethrough", "Strikethrough"},
                // Control: VB.Form
                {"LinkTopic", "LinkTopic"},
                // Control: VB.Form
                {"MaxButton", "MaxButton"},
                // Control: VB.Form
                {"MinButton", "MinButton"},
                // Control: VB.Form
                {"ScaleHeight", "ScaleHeight"},
                // Control: VB.Form
                {"ScaleWidth", "ScaleWidth"},
                // Control: VB.Form
                {"StartUpPosition", "StartUpPosition"},
                // Control: TabDlg.SSTab
                {"Height", "Height"},
                // Control: TabDlg.SSTab
                {"Left", "Left"},
                // Control: TabDlg.SSTab
                {"TabIndex", "TabIndex"},
                // Control: TabDlg.SSTab
                {"Top", "Top"},
                // Control: TabDlg.SSTab
                {"Width", "Width"},
                // Control: TabDlg.SSTab
                {"_ExtentX", "_ExtentX"},
                // Control: TabDlg.SSTab
                {"_ExtentY", "_ExtentY"},
                // Control: TabDlg.SSTab
                {"_Version", "_Version"},
                // Control: TabDlg.SSTab
                {"Style", "Style"},
                // Control: TabDlg.SSTab
                {"Tab", "Tab"},
                // Control: TabDlg.SSTab
                {"TabHeight", "TabHeight"},
                // Control: TabDlg.SSTab
                {"TabCaption(0)", "TabCaption(0)"},
                // Control: TabDlg.SSTab
                {"TabPicture(0)", "TabPicture(0)"},
                // Control: TabDlg.SSTab
                {"Tab(0).ControlEnabled", "Tab(0).ControlEnabled"},
                // Control: TabDlg.SSTab
                {"Tab(0).Control(0)", "Tab(0).Control(0)"},
                // Control: TabDlg.SSTab
                {"Tab(0).Control(0).Enabled", "Tab(0).Control(0).Enabled"},
                // Control: TabDlg.SSTab
                {"Tab(0).Control(1)", "Tab(0).Control(1)"},
                // Control: TabDlg.SSTab
                {"Tab(0).Control(1).Enabled", "Tab(0).Control(1).Enabled"},
                // Control: TabDlg.SSTab
                {"Tab(0).ControlCount", "Tab(0).ControlCount"},
                // Control: TabDlg.SSTab
                {"TabCaption(1)", "TabCaption(1)"},
                // Control: TabDlg.SSTab
                {"TabPicture(1)", "TabPicture(1)"},
                // Control: TabDlg.SSTab
                {"Tab(1).ControlEnabled", "Tab(1).ControlEnabled"},
                // Control: TabDlg.SSTab
                {"Tab(1).Control(0)", "Tab(1).Control(0)"},
                // Control: TabDlg.SSTab
                {"Tab(1).Control(0).Enabled", "Tab(1).Control(0).Enabled"},
                // Control: TabDlg.SSTab
                {"Tab(1).Control(1)", "Tab(1).Control(1)"},
                // Control: TabDlg.SSTab
                {"Tab(1).Control(1).Enabled", "Tab(1).Control(1).Enabled"},
                // Control: TabDlg.SSTab
                {"Tab(1).ControlCount", "Tab(1).ControlCount"},
                // Control: TabDlg.SSTab
                {"TabCaption(2)", "TabCaption(2)"},
                // Control: TabDlg.SSTab
                {"TabPicture(2)", "TabPicture(2)"},
                // Control: TabDlg.SSTab
                {"Tab(2).ControlEnabled", "Tab(2).ControlEnabled"},
                // Control: TabDlg.SSTab
                {"Tab(2).Control(0)", "Tab(2).Control(0)"},
                // Control: TabDlg.SSTab
                {"Tab(2).Control(0).Enabled", "Tab(2).Control(0).Enabled"},
                // Control: TabDlg.SSTab
                {"Tab(2).ControlCount", "Tab(2).ControlCount"},
                // Control: VB.Frame
                {"Visible", "Visible"},
                // Control: MSFlexGridLib.MSFlexGrid
                {"FixedCols", "FixedCols"},
                // Control: MSFlexGridLib.MSFlexGrid
                {"RowHeightMin", "RowHeightMin"},
                // Control: MSFlexGridLib.MSFlexGrid
                {"BackColor", "BackColor"},
                // Control: MSFlexGridLib.MSFlexGrid
                {"ScrollTrack", "ScrollTrack"},
                // Control: MSFlexGridLib.MSFlexGrid
                {"FocusRect", "FocusRect"},
                // Control: MSFlexGridLib.MSFlexGrid
                {"ScrollBars", "ScrollBars"},
                // Control: MSFlexGridLib.MSFlexGrid
                {"SelectionMode", "SelectionMode"},
                // Control: MSFlexGridLib.MSFlexGrid
                {"FormatString", "FormatString"},
                // Control: VB.Label
                {"ForeColor", "ForeColor"},
                // Control: VB.TextBox
                {"Text", "Text"},
                // Control: VB.TextBox
                {"MaxLength", "MaxLength"},
                // Control: VB.CommandButton
                {"Enabled", "Enabled"},
                // Control: MSFlexGridLib.MSFlexGrid
                {"Cols", "Cols"},
                // Control: TabDlg.SSTab
                {"Tabs", "Tabs"},
                // Control: TabDlg.SSTab
                {"TabsPerRow", "TabsPerRow"},
                // Control: TabDlg.SSTab
                {"Tab(0).Control(2)", "Tab(0).Control(2)"},
                // Control: TabDlg.SSTab
                {"Tab(0).Control(2).Enabled", "Tab(0).Control(2).Enabled"},
                // Control: MSFlexGridLib.MSFlexGrid
                {"AllowBigSelection", "AllowBigSelection"},
                // Control: MSFlexGridLib.MSFlexGrid
                {"TabStop", "TabStop"},
                // Control: MSFlexGridLib.MSFlexGrid
                {"Rows", "Rows"},
                // Control: MSFlexGridLib.MSFlexGrid
                {"BackColorFixed", "BackColorFixed"},
                // Control: VB.Label
                {"Alignment", "Alignment"},
            };
        }

        [Obsolete("Old style method. I switched to parsing text instead of constructing syntax tree with calls.")]
        public StatementSyntax PropertyAssignment(string controlName, string propertyName, LiteralExpressionSyntax value)
        {

            var control = SyntaxFactory.IdentifierName(controlName);
            var property = SyntaxFactory.IdentifierName(propertyName);
            var left =
                SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                    control,
                    property);
            var right = value;

            return
                SyntaxFactory.ExpressionStatement(
                    SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, left, right));

        }

        public string TranslatePropertyName(string ctrlType, string propertyName)
        {
            // TODO: Add control specific property mappings.
            // Actually it's probably so few that needs to be per control that
            // I can just put them in this method with a switch.
            if (vbToCSharpPropertyMapping.ContainsKey(propertyName))
            {
                return vbToCSharpPropertyMapping[propertyName];
            }

            if (!seenProperties.ContainsKey(propertyName))
            {
                DebugClass.LogError("// Control: " + ctrlType);
                DebugClass.LogError(String.Format("{{\"{0}\", \"{0}\"}},", propertyName));
                seenProperties[propertyName] = true;
            }
            
            //throw new System.NotImplementedException("Did not find property name in translatable properties.");
            return propertyName;
        }

        public StatementSyntax TranslateProperty(
            string ctrlType,
            string ctrlIdentifier,
            VisualBasic6Parser.Cp_SinglePropertyContext property
            )
        {
            var propertyName =
                vb6NodeTree
                    .GetChildren(property)
                    .Find(x => x is VisualBasic6Parser.ImplicitCallStmt_InStmtContext)
                    .getText()
                    .Trim();
            var propertyValueContext =
                vb6NodeTree
                    .GetChildren(property)
                    .Find(x => x is VisualBasic6Parser.Cp_PropertyValueContext);
            var literalContext =
                vb6NodeTree
                    .GetChildren(propertyValueContext)
                    .Find(x => x is VisualBasic6Parser.LiteralContext);

            var asg = (LiteralImpl) _compileResult.Program.getASGElementRegistry().getASGElement(literalContext);

            if (asg == null)
            {
                DebugClass.LogError("ASG missing for: " + ctrlIdentifier + " for property: " + propertyName);
                throw new System.NotImplementedException("Don't know how to handle ASG null.");
            }

            var valueNode = TranslatorForExpression.GetExpression(asg);

            var valueString = valueNode.NormalizeWhitespace().ToFullString();

            var translatedPropertyName = TranslatePropertyName(ctrlType, propertyName);

            return SyntaxFactory.ParseStatement(ctrlIdentifier + "." + translatedPropertyName + " = " + valueString + ";");
        }

        public Tuple<string, string> GetTypeOfControl(VisualBasic6Parser.ControlPropertiesContext node)
        {
            var ctrlTypeContext =
                vb6NodeTree.GetChildren(node).Find(x => x is VisualBasic6Parser.Cp_ControlTypeContext);

            var vbTypeName = ctrlTypeContext.getText().Trim();
            var csharpTypeName = vbtoCSharpControlTypeMapping[vbTypeName];
            
            return Tuple.Create(vbTypeName, csharpTypeName);
        }

        public string GetNameOfControl(VisualBasic6Parser.ControlPropertiesContext node)
        {
            var ctrlIdentifierContext =
                vb6NodeTree.GetChildren(node).Find(x => x is VisualBasic6Parser.Cp_ControlIdentifierContext);
            var ctrlIdentifier = ctrlIdentifierContext.getText().Trim();
            return ctrlIdentifier;
        }

        public void ProcessProperties(
            string ctrlType,
            ParseTree node,
            List<StatementSyntax> statements,
            string ctrlIdentifier)
        {
            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            if (statements is null)
            {
                throw new ArgumentNullException(nameof(statements));
            }

            if (ctrlIdentifier is null)
            {
                throw new ArgumentNullException(nameof(ctrlIdentifier));
            }

            foreach (var child in vb6NodeTree.GetChildren(node))
            {
                if (child is VisualBasic6Parser.Cp_PropertiesContext)
                {
                    // This is one of the properties
                    var simpleProperty =
                        vb6NodeTree.GetChildren(child).Find(x => x is VisualBasic6Parser.Cp_SinglePropertyContext);

                    var nestedControlProperty =
                        vb6NodeTree.GetChildren(child).Find(x => x is VisualBasic6Parser.ControlPropertiesContext);

                    var nestedProperty =
                        vb6NodeTree.GetChildren(child).Find(x => x is VisualBasic6Parser.Cp_NestedPropertyContext);

                    if (simpleProperty != null)
                    {
                        statements.Add(
                            TranslateProperty(
                                    ctrlType,
                                    ctrlIdentifier,
                                    (VisualBasic6Parser.Cp_SinglePropertyContext)simpleProperty));
                    }
                    else if (nestedControlProperty != null)
                    {
                        statements.AddRange(TranslateControl(
                            (VisualBasic6Parser.ControlPropertiesContext)nestedControlProperty));
                        var nestedControlName =
                            GetNameOfControl((VisualBasic6Parser.ControlPropertiesContext)nestedControlProperty);
                        var statementString = ctrlIdentifier + ".Controls.Add(" + nestedControlName + ");";
                        var assignPropertyStatement =
                            SyntaxFactory.ParseStatement(statementString);
                        statements.Add(assignPropertyStatement);
                    }
                    else if (nestedProperty != null)
                    {
                        var nestedName = vb6NodeTree.GetChildren(nestedProperty)
                            .Find(x =>
                                x is VisualBasic6Parser.AmbiguousIdentifierContext ||
                                x is VisualBasic6Parser.CertainIdentifierContext).getText().trim();
                        var translatedNestedName = TranslatePropertyName(ctrlType, nestedName);
                        ProcessProperties(translatedNestedName, nestedProperty, statements, ctrlIdentifier + "." + nestedName);
                    }
                    else
                    {
                        foreach (var grandchild in vb6NodeTree.GetChildren(child))
                        {
                            DebugClass.LogError("GRANDCHILD: " + grandchild.GetType().Name + " " + grandchild.getText());
                        }
                        throw new System.NotImplementedException("Property not handled for: " + ctrlIdentifier);
                    }
                }
            }
        }

        public List<StatementSyntax> TranslateControl(VisualBasic6Parser.ControlPropertiesContext node)
        {
            var statements = new List<StatementSyntax>();

            var vbAndCsharpTypeName = GetTypeOfControl(node);

            var vbTypeName = vbAndCsharpTypeName.Item1;

            var csharpTypeName = vbAndCsharpTypeName.Item2;
            
            if (csharpTypeName == IgnoreString)
            {
                return statements;
            }

            var ctrlIdentifier = GetNameOfControl(node);

            var newControlStatement =
                SyntaxFactory.ParseStatement(csharpTypeName + " " + ctrlIdentifier + " = new " + csharpTypeName + "();");

            statements.Add(newControlStatement);

            ProcessProperties(vbTypeName, node, statements, ctrlIdentifier);

            return statements;
        }

        public CompilationUnitSyntax Translate()
        {

            var root = vb6NodeTree.GetRoot();

            var control = 
                vb6NodeTree.GetChildren(root).Find(x => x is VisualBasic6Parser.ControlPropertiesContext);

            var statements = new List<StatementSyntax>();
            if (control != null)
            {
                statements = TranslateControl((VisualBasic6Parser.ControlPropertiesContext) control);
            }
            var formTree = SyntaxFactory.Block(statements);

            var template = @"
namespace NAMESPACE {
    public class FORM {
        public void INITIALIZE() {
            METHOD
        }
    }
}
";

            string namespaceTitle = "namespace";
            string formTitle = "formTitle";
            string initialize = "Initialize";
            string methodBody = formTree.NormalizeWhitespace().ToFullString();

            var replacedTemplate = template;
            replacedTemplate = replacedTemplate.Replace("NAMESPACE", namespaceTitle);
            replacedTemplate = replacedTemplate.Replace("FORM", formTitle);
            replacedTemplate = replacedTemplate.Replace("INITIALIZE", initialize);
            replacedTemplate = replacedTemplate.Replace("METHOD", methodBody);

            var methodWrapper = SyntaxFactory.ParseCompilationUnit(replacedTemplate);

            return methodWrapper;
        }
    }
}
