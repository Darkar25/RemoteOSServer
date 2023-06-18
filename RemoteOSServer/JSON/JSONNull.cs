using System.Text;

namespace EasyJSON
{
    public class JSONNull : JSONNode
    {
		public static JSONNull CreateOrGet() => reuseSameInstance ? m_StaticInstance : new JSONNull();
		public JSONNull()
        {
        }
		public override JSONNodeType Tag => JSONNodeType.NullValue;
		public override bool IsNull => true;
		public override JSONNode.Enumerator GetEnumerator() => default;
		public override string Value
		{
			get => "null";
			set { }
		}
		public override bool AsBool
		{
			get => false;
			set { }
		}
		public override bool Equals(object obj) => this == obj || obj is JSONNull;
		public override int GetHashCode() => 0;
		internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
        {
            aSB.Append("null");
        }
        public static JSONNull m_StaticInstance = new JSONNull();
        public static bool reuseSameInstance = true;
    }
}
