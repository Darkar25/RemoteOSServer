using System.Text;

namespace EasyJSON
{
    internal class JSONLazyCreator : JSONNode
    {
        public override JSONNodeType Tag => JSONNodeType.None;
        public override JSONNode.Enumerator GetEnumerator() => default;
        public JSONLazyCreator(JSONNode aNode)
        {
            m_Node = aNode;
            m_Key = null;
        }
        public JSONLazyCreator(JSONNode aNode, string aKey)
        {
            m_Node = aNode;
            m_Key = aKey;
        }
        public void Set(JSONNode aVal)
        {
            if (m_Key is null)
                m_Node.Add(aVal);
            else
                m_Node.Add(m_Key, aVal);
            m_Node = null;
        }
        public override JSONNode this[int aIndex]
        {
            get => new JSONLazyCreator(this);
            set
            {
                JSONArray jsonarray = new();
                jsonarray.Add(value);
                Set(jsonarray);
            }
        }
        public override JSONNode this[string aKey]
        {
            get => new JSONLazyCreator(this, aKey);
            set
            {
                JSONObject jsonobject = new();
                jsonobject.Add(aKey, value);
                Set(jsonobject);
            }
        }
        public override void Add(JSONNode aItem)
        {
            JSONArray jsonarray = new();
            jsonarray.Add(aItem);
            Set(jsonarray);
        }
        public override void Add(string aKey, JSONNode aItem)
        {
            JSONObject jsonobject = new();
            jsonobject.Add(aKey, aItem);
            Set(jsonobject);
        }
        public static bool operator ==(JSONLazyCreator a, object b) => b is null || a == b;
        public static bool operator !=(JSONLazyCreator a, object b) => !(a == b);
        public override bool Equals(object obj) => obj is null || this == obj;
        public override int GetHashCode() => 0;
        public override int AsInt
        {
            get
            {
                Set(new JSONNumber(0.0));
                return 0;
            }
            set => Set(new JSONNumber((double)value));
        }
        public override float AsFloat
        {
            get
            {
                Set(new JSONNumber(0.0));
                return 0f;
            }
            set => Set(new JSONNumber((double)value));
        }
        public override double AsDouble
        {
            get
            {
                Set(new JSONNumber(0.0));
                return 0.0;
            }
            set => Set(new JSONNumber(value));
        }
        public override bool AsBool
        {
            get
            {
                Set(new JSONBool(false));
                return false;
            }
            set => Set(new JSONBool(value));
        }
        public override JSONArray AsArray
        {
            get
            {
                JSONArray jsonarray = new();
                Set(jsonarray);
                return jsonarray;
            }
        }
        public override JSONObject AsObject
        {
            get
            {
                JSONObject jsonobject = new();
                Set(jsonobject);
                return jsonobject;
            }
        }
        internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
        {
            aSB.Append("null");
        }
        public JSONNode m_Node;
        public string? m_Key;
    }
}
