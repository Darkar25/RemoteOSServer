using System.Globalization;
using System.Text;

namespace EasyJSON
{
    public class JSONNumber : JSONNode
    {
        public override JSONNodeType Tag
        {
            get
            {
                return JSONNodeType.Number;
            }
        }
        public override bool IsNumber
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
                return this.m_Data.ToString();
            }
            set
            {
                double data;
                if (double.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out data))
                {
                    this.m_Data = data;
                }
            }
        }
        public override double AsDouble
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
        public JSONNumber(double aData)
        {
            this.m_Data = aData;
        }
        public JSONNumber(string aData)
        {
            this.Value = aData;
        }
        internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
        {
            aSB.Append(this.m_Data);
        }
        public static bool IsNumeric(object value)
        {
            return value is int || value is uint || value is float || value is double || value is decimal || value is long || value is ulong || value is short || value is ushort || value is sbyte || value is byte;
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (base.Equals(obj))
            {
                return true;
            }
            JSONNumber jsonnumber = obj as JSONNumber;
            if (jsonnumber != null)
            {
                return this.m_Data == jsonnumber.m_Data;
            }
            return JSONNumber.IsNumeric(obj) && Convert.ToDouble(obj) == this.m_Data;
        }
        public override int GetHashCode()
        {
            return this.m_Data.GetHashCode();
        }
        public double m_Data;
    }
}
