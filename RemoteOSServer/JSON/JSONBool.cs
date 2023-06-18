using System.Text;

namespace EasyJSON
{
    public class JSONBool : JSONNode
    {
        public override JSONNodeType Tag => JSONNodeType.Boolean;
        public override bool IsBoolean => true;
        public override JSONNode.Enumerator GetEnumerator() => default;
        public override string Value
        {
            get => m_Data.ToString();
            set
            {
                if (bool.TryParse(value, out bool data)) m_Data = data;
            }
        }
        public override bool AsBool
        {
            get => m_Data;
            set => m_Data = value;
        }
        public JSONBool(bool aData) => m_Data = aData;
        public JSONBool(string aData) => Value = aData;
        internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
        {
            aSB.Append(m_Data ? "true" : "false");
        }
        public override bool Equals(object obj) => obj is bool v && m_Data == v;
        public override int GetHashCode() => m_Data.GetHashCode();
        public bool m_Data;
    }
}
