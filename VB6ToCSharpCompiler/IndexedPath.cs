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
        
        public string Token { get; set;  }

        public IndexedPath(string nodeTypeName, int childIndex, string token)
        {
            NodeTypeName = nodeTypeName;
            ChildIndex = childIndex;
            Token = token;
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

        public override int GetHashCode()
        {
            var hashCode = -1774268519;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(NodeTypeName);
            hashCode = hashCode * -1521134295 + ChildIndex.GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            return ChildIndex + ":" + NodeTypeName + ":" + Token;
        }
    }
}
