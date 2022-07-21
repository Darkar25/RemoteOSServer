using System.Text;

namespace EasyJSON
{
    public class JSONNull : JSONNode
    {
        public static JSONNull CreateOrGet()
        {
            if (JSONNull.reuseSameInstance)
            {
                return JSONNull.m_StaticInstance;
            }
            return new JSONNull();
        }
        public JSONNull()
        {
        }
        public override JSONNodeType Tag
        {
            get
            {
                return JSONNodeType.NullValue;
            }
        }
        public override bool IsNull
        {
            get
            {
                return true;
            }
        }
        public override JSONNode.Enumerator GetEnumerator()
        {
            return default(JSONNode.Enumerator);
        }
        public override string Value
        {
            get
            {
                return "null";
            }
            set
            {
            }
        }
        public override bool AsBool
        {
            get
            {
                return false;
            }
            set
            {
            }
        }
        public override bool Equals(object obj)
        {
            return this == obj || obj is JSONNull;
        }
        public override int GetHashCode()
        {
            return 0;
        }
        internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
        {
            aSB.Append("null");
        }
        public static JSONNull m_StaticInstance = new JSONNull();
        public static bool reuseSameInstance = true;
    }
}
