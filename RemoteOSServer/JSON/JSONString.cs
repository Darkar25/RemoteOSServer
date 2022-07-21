using System.Text;

namespace EasyJSON
{
    public class JSONString : JSONNode
    {
        public override JSONNodeType Tag
        {
            get
            {
                return JSONNodeType.String;
            }
        }
        public override bool IsString
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
                return this.m_Data;
            }
            set
            {
                this.m_Data = value;
            }
        }
        public JSONString(string aData)
        {
            this.m_Data = aData;
        }
        internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
        {
            aSB.Append('"').Append(JSONNode.Escape(this.m_Data)).Append('"');
        }
        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                return true;
            }
            string text = obj as string;
            if (text != null)
            {
                return this.m_Data == text;
            }
            JSONString jsonstring = obj as JSONString;
            return jsonstring != null && this.m_Data == jsonstring.m_Data;
        }
        public override int GetHashCode()
        {
            return this.m_Data.GetHashCode();
        }
        public string m_Data;
    }
}
