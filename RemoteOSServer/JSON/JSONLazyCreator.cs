using System.Text;

namespace EasyJSON
{
    internal class JSONLazyCreator : JSONNode
    {
        public override JSONNodeType Tag
        {
            get
            {
                return JSONNodeType.None;
            }
        }
        public override JSONNode.Enumerator GetEnumerator()
        {
            return default(JSONNode.Enumerator);
        }
        public JSONLazyCreator(JSONNode aNode)
        {
            this.m_Node = aNode;
            this.m_Key = null;
        }
        public JSONLazyCreator(JSONNode aNode, string aKey)
        {
            this.m_Node = aNode;
            this.m_Key = aKey;
        }
        public void Set(JSONNode aVal)
        {
            if (this.m_Key == null)
            {
                this.m_Node.Add(aVal);
            }
            else
            {
                this.m_Node.Add(this.m_Key, aVal);
            }
            this.m_Node = null;
        }
        public override JSONNode this[int aIndex]
        {
            get
            {
                return new JSONLazyCreator(this);
            }
            set
            {
                JSONArray jsonarray = new JSONArray();
                jsonarray.Add(value);
                this.Set(jsonarray);
            }
        }
        public override JSONNode this[string aKey]
        {
            get
            {
                return new JSONLazyCreator(this, aKey);
            }
            set
            {
                JSONObject jsonobject = new JSONObject();
                jsonobject.Add(aKey, value);
                this.Set(jsonobject);
            }
        }
        public override void Add(JSONNode aItem)
        {
            JSONArray jsonarray = new JSONArray();
            jsonarray.Add(aItem);
            this.Set(jsonarray);
        }
        public override void Add(string aKey, JSONNode aItem)
        {
            JSONObject jsonobject = new JSONObject();
            jsonobject.Add(aKey, aItem);
            this.Set(jsonobject);
        }
        public static bool operator ==(JSONLazyCreator a, object b)
        {
            return b == null || a == b;
        }
        public static bool operator !=(JSONLazyCreator a, object b)
        {
            return !(a == b);
        }
        public override bool Equals(object obj)
        {
            return obj == null || this == obj;
        }
        public override int GetHashCode()
        {
            return 0;
        }
        public override int AsInt
        {
            get
            {
                JSONNumber aVal = new JSONNumber(0.0);
                this.Set(aVal);
                return 0;
            }
            set
            {
                JSONNumber aVal = new JSONNumber((double)value);
                this.Set(aVal);
            }
        }
        public override float AsFloat
        {
            get
            {
                JSONNumber aVal = new JSONNumber(0.0);
                this.Set(aVal);
                return 0f;
            }
            set
            {
                JSONNumber aVal = new JSONNumber((double)value);
                this.Set(aVal);
            }
        }
        public override double AsDouble
        {
            get
            {
                JSONNumber aVal = new JSONNumber(0.0);
                this.Set(aVal);
                return 0.0;
            }
            set
            {
                JSONNumber aVal = new JSONNumber(value);
                this.Set(aVal);
            }
        }
        public override bool AsBool
        {
            get
            {
                JSONBool aVal = new JSONBool(false);
                this.Set(aVal);
                return false;
            }
            set
            {
                JSONBool aVal = new JSONBool(value);
                this.Set(aVal);
            }
        }
        public override JSONArray AsArray
        {
            get
            {
                JSONArray jsonarray = new JSONArray();
                this.Set(jsonarray);
                return jsonarray;
            }
        }
        public override JSONObject AsObject
        {
            get
            {
                JSONObject jsonobject = new JSONObject();
                this.Set(jsonobject);
                return jsonobject;
            }
        }
        internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
        {
            aSB.Append("null");
        }
        public JSONNode m_Node;
        public string m_Key;
    }
}
