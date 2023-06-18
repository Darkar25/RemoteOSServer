using System.Text;

namespace EasyJSON
{
    public class JSONString : JSONNode
    {
		public override JSONNodeType Tag => JSONNodeType.String;
		public override bool IsString => true;
		public override Enumerator GetEnumerator() => default;
		public override string Value
		{
			get => m_Data;
			set => m_Data = value;
		}
		public JSONString(string aData) => m_Data = aData;
		internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
        {
            aSB.Append('"').Append(Escape(m_Data)).Append('"');
        }
        public override bool Equals(object obj)
        {
            if (base.Equals(obj)) return true;
            string? text = obj as string;
            if (text is not null) return m_Data == text;
            JSONString? jsonstring = obj as JSONString;
            return jsonstring is not null && m_Data == jsonstring.m_Data;
        }
		public override int GetHashCode() => m_Data.GetHashCode();
		public string m_Data;
    }
}
