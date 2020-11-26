using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// Iterate over all files
// Produce parse tree for each file
// Use a tree control to list all the parse trees for browsing
// What happens when a user clicks a node in the tree?
// The text boxes on the right shows the user the following:

// Not editable: Original VB6 Code for the node, along with properties
// Editable: pattern which is applied to the node. This pattern is common to all syntax nodes of the same type.
// There should be statistics on:
// - how many times the syntax node exist.
// - how many times the pattern matched this syntax node.
// - how many times the generated C# code compiled successfully.
// - Difficult bonus: how many times the generated C# code executed successfully.
// -    Which means that it returned the same data as the corresponding VB6 code.
// -    What this means is that we need to serialize the return value for each executed line of VB6 code.
// -    How can we modify VB6 code to execute some code on the return value of each syntax node.
// -        One way is to write a VB6 interpreter.
// -        Another way is to insert generated code in the VB6 code, then recompile with VB6 and execute once, save the results to DB.
// -        Which way requires less developer time?
// - Not editable: Generated C# code for the node
// - Difficult: Added bonus: Infer pattern from the difference between VB6 code and C# code.
// - How to do that:
// If the same token (its string representation) appears on VB6 and C# side,
// assume that the pattern contains a unique id for the token.
// This unique id will match as usual. I have code for this. Don't mess with it or try to understand it. Just assume that it works. I was smarter back then.

// Each piece of code is an ordered set of tokens, but their length may be different.
// It's really a tree instead of an ordered set, but assume that the tree is serialized depth first into an ordered set of tokens with indices relative to its parent.

// Each token is represented by a tuple of its index and its string representation.
// In this way there are duplicate indices in the serialized representation of a tree,
// but that doesn't matter, because the set is ordered, so each (setpos, childIndex, token) even if (childIndex, token) is not unique.
// What do we do about depth? Assume depth is be <= 1. Then we don't need depth in the tuple.
// Otherwise, let node = (setpos, depth, childIndex, token).

// There are two patterns: VP and CP. Each has been serialized to a list of tuples (setpos, depth, childIndex, token).
// Should we iterate over the inner join / cross product of the lists?
// This gives the most general attack and is feasible, because the trees are small.
// Assume setpos <= n, then the cross product is less than n*n.
// If we want n*n <= 400, then n <= sqrt(400) = 20.
// So we have a limit to the pattern size if we do cross product, but that's fine.
// Like emh says, 20 pattern matches ought to be enough for everyone.
// If the pattern is bigger it should be broken down into multiple patterns.

// What is the point of a pattern?
// The point of a pattern is to match and replace. Think usual case of regex.
// A pattern consists of a sequence of literals and stars.

// Why are we working on small patterns corresponding to one syntax node and its children and not large "recursive" patterns corresponding to top level syntax nodes?
// That's because we want to be able to generalize to apply to many similar syntax nodes.
// A pattern can be reused if it's small, but if it is big it has many specifics that makes it harder to reuse across syntax nodes.
// 
// If index1 and index2 are equal, we have two cases depending on whether or not the string representation is equal.
// On the other hand, if index1 and index2

// On the other hand, if there are tokens on VB6 and C# which are different, assume they are translated.
// What about the position of the token?
// If the token is the same, the position has moved.
// If the token is different and the position is the same

// Do we do it top down or bottom up?
// For each syntax node apply pattern.
// A parent refers to child nodes.
// In which language is the pattern written? It is written in VB6. What does it mean?
// It means that we are matching on VB6 code. We cannot translate children before parent,
// because that would mean we have to match on C# code.
// Thus we have to do it top down.

// The point is: Whenever we have a syntax node in VB6, apply pattern from VB6 to C#.
// If during processing of another pattern we encounter a syntax node,
// then iterate over all patterns, see if it maches and apply pattern replacement if it does.

// What is the replacement in a pattern?
// It is either a literal or a match.
// A match is a syntax node with or without another pattern applied to it.
// If the pattern is applied it will be C#, if not it will be VB6.
// In which language do we want the text box to show the user code?
// There are two boxes, one for VB6 and two for C#.
// In the first box, the pattern has not been applied. This is VB6 code.
// In the second box, the current pattern has been applied. This is hybrid code.
// In the third box, all patterns have been applied recursively. This is C# code.
// Which box do we need, second or third?
// Isn't it weird to work with hybrid code, both in VB6 and C#?
// It doesn't compile, it doesn't run, it's no fun.

// The point is, it's too difficult to extract patterns from modified code, is it?
// What a baby. Try again. Harder this time.
// Ok... so a pattern is a sequence of literals and stars.
// Literals decide if the pattern matches and stars decide which nodes to replace.
// Code is already a sequence of literals.
// So it's a valid pattern with no modifications.
// But remember, we have a size limit of n = 20.
// What can we do to improve on the pattern size?
// We replace some literals with stars.
// So the question becomes, which literals should we replace with stars?
// The answer is trivial. Left as an exercise for the reader. Just joking.
// In fact the answer is difficult.

// Let's get back to VB6:(setpos, depth, childIndex, token) and the cross product with C#:(setpos, depth, childIndex, token).
// We need a true/false decision on each comparison. Either it is a literal or it is a star
// (next letter in sequence A..Z which is not equal to any earlier star or literal,
//  that's what we call a star in the case of our patterns).
// What is this damnable comparison going to be?
// Well, we need an example. When we have an example, we can see the actual data for the example and we can
// easily see manually if the answer is true or false.
// Since we know the answer, we can choose a comparison which satisfies the answer.
// How do we know that the comparison is correct, not too much and not too little?
// We need more examples. A few examples should be enough.
// Then we can test the comparison and check that it generates the correct results for all the examples.

// So now all the pieces of the puzzle combine and bimsalabim we have a recipe for a compiler.

namespace VB6ToCSharpCompiler
{
    public partial class frmCompiler : Form
    {
        private frmVB6ASTBrowser frmVb6AstBrowser;

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

        private void frmCompiler_Load(object sender, EventArgs e)
        {
            foreach (var fileName in VB6Compiler.GetFiles())
            {
                lstFileNames.Items.Add(fileName);
            }
            TranslatorForPattern.IntializeTranslatorForPattern();
        }

        private void btnBrowseVB6AST_Click(object sender, EventArgs e)
        {
            var fileName = (string)lstFileNames.SelectedItem;
            frmVb6AstBrowser = new frmVB6ASTBrowser(fileName);
            frmVb6AstBrowser.Visible = true;
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            var fileName = (string)lstFileNames.SelectedItem;
            
            //var tfp = new TranslatorForPattern();

        }

        private void lstFileNames_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
