using io.proleap.vb6.asg.exception;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VB6ToCSharpCompiler
{
    public partial class frmPatterns : Form
    {
        private List<string> fileNames;
        private Dictionary<string, CompileResult> compileResults = new Dictionary<string, CompileResult>();

        public frmPatterns(List<string> fileNames)
        {
            this.fileNames = fileNames;
            InitializeComponent();
        }

        private void frmPatterns_Load(object sender, EventArgs e)
        {
            CompileAll();
            PopulateGrid();
        }

        void PopulateGrid() {
            DataTable grdPatternsDataSource = new DataTable("Patterns");
            DataColumn c0 = new DataColumn("Node Type");
            DataColumn c1 = new DataColumn("Context");
            DataColumn c2 = new DataColumn("VB6 Pattern");
            DataColumn c3 = new DataColumn("C# Replacement");
            DataColumn c4 = new DataColumn("Matches#");
            grdPatternsDataSource.Columns.Add(c0);
            grdPatternsDataSource.Columns.Add(c1);
            grdPatternsDataSource.Columns.Add(c2);
            grdPatternsDataSource.Columns.Add(c3);
            grdPatternsDataSource.Columns.Add(c4);

            foreach (var patternText in TranslatorForPattern.TranslatorPatterns)
            {
                DataRow row = grdPatternsDataSource.NewRow();

                string nodeTypeName = "COMPILATION_ERROR";
                int matchCount = 0;
                try
                {
                    var pattern = patternText.Compile();
                    nodeTypeName = pattern.VbTreeNodeType;
                    matchCount = GetMatches(pattern);
                }
                catch (VbParserException e)
                {
                    DebugClass.LogError("Pattern Compile Failed: " + patternText.LogValue() + ": " + e.toString());
                    throw;
                }

                row["Node Type"] = nodeTypeName;
                row["Context"] = patternText.VbWrapperCode;
                row["VB6 Pattern"] = patternText.VbCode;
                row["C# Replacement"] = patternText.VbCode;
                row["Matches#"] = matchCount;

                grdPatternsDataSource.Rows.Add(row);
            }

            grdPatterns.DataSource = grdPatternsDataSource;
        }

        public void CompileAll()
        {
            foreach (var fileName in fileNames)
            {
                if (fileName.EndsWith(".frx", StringComparison.InvariantCulture))
                {
                    continue;
                }
                compileResults[fileName] = VB6Compiler.Compile(fileName, null, false);
            }
        }

        int GetMatches(VbToCsharpPattern pattern)
        {
            int matches = 0;
            foreach (var fileName in fileNames) {
                if (fileName.EndsWith(".frx", StringComparison.InvariantCulture))
                {
                    continue;
                }

                var compileResult = compileResults[fileName];
                var nodeTree = new VB6NodeTree(compileResult);

                foreach (var node in nodeTree.GetAllNodes())
                {
                    if (pattern.CanTranslate(nodeTree.GetChildren, node))
                    {
                        matches++;
                    }
                }
            }

            return matches;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
