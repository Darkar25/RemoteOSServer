using System.Globalization;
using System.Text;

namespace EasyJSON
{
    public class JSONNumber : JSONNode
    {
		public override JSONNodeType Tag => JSONNodeType.Number;
		public override bool IsNumber => true;
		public override Enumerator GetEnumerator() => default;
		public override string Value
		{
			get => m_Data.ToString();
			set
			{
				if (double.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out double data))
				    m_Data = data;
			}
		}
		public override double AsDouble
		{
			get => m_Data;
			set => m_Data = value;
		}
		public JSONNumber(double aData) => m_Data = aData;
		public JSONNumber(string aData) => Value = aData;
		internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
        {
            aSB.Append(m_Data);
        }
		public static bool IsNumeric(object value) => value is int or uint or float or double or decimal or long or ulong or short or ushort or sbyte or byte;
		public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (base.Equals(obj)) return true;
            JSONNumber? jsonnumber = obj as JSONNumber;
			return jsonnumber is not null ? m_Data == jsonnumber.m_Data : IsNumeric(obj) && Convert.ToDouble(obj) == m_Data;
		}
		public override int GetHashCode() => m_Data.GetHashCode();
		public double m_Data;
    }
}
