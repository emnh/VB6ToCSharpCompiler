using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VB6ToCSharpCompiler
{
    public class IndexedPath
    {
        public string NodeTypeName { get; set; }
        public int ChildIndex { get; set; }

        public IndexedPath(string nodeTypeName, int childIndex)
        {
            NodeTypeName = nodeTypeName;
            ChildIndex = childIndex;
        }

        public override bool Equals(Object obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                IndexedPath p = (IndexedPath) obj;
                return (NodeTypeName == p.NodeTypeName) && (ChildIndex == p.ChildIndex);
            }
        }
    }
}
