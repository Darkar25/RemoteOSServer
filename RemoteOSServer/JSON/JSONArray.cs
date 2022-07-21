using System.Text;

namespace EasyJSON
{
    public class JSONArray : JSONNode
    {
        public JSONArray(IEnumerable<JSONNode> values) : base()
        {
            foreach (var a in values)
                Add(a);
        }
        public JSONArray() : base() { }
        public override bool Inline
        {
            get
            {
                return this.inline;
            }
            set
            {
                this.inline = value;
            }
        }
        public override JSONNodeType Tag
        {
            get
            {
                return JSONNodeType.Array;
            }
        }
        public override bool IsArray
        {
            get
            {
                return true;
            }
        }
        public override JSONNode.Enumerator GetEnumerator()
        {
            return new JSONNode.Enumerator(this.m_List.GetEnumerator());
        }
        public override JSONNode this[int aIndex]
        {
            get
            {
                if (aIndex < 0 || aIndex >= this.m_List.Count)
                {
                    return new JSONLazyCreator(this);
                }
                return this.m_List[aIndex];
            }
            set
            {
                if (value == null)
                {
                    value = JSONNull.CreateOrGet();
                }
                if (aIndex < 0 || aIndex >= this.m_List.Count)
                {
                    this.m_List.Add(value);
                    return;
                }
                this.m_List[aIndex] = value;
            }
        }
        public override JSONNode this[string aKey]
        {
            get
            {
                return new JSONLazyCreator(this);
            }
            set
            {
                if (value == null)
                {
                    value = JSONNull.CreateOrGet();
                }
                this.m_List.Add(value);
            }
        }
        public override int Count
        {
            get
            {
                return this.m_List.Count;
            }
        }
        public override void Add(string aKey, JSONNode aItem)
        {
            if (aItem == null)
            {
                aItem = JSONNull.CreateOrGet();
            }
            this.m_List.Add(aItem);
        }
        public override JSONNode Remove(int aIndex)
        {
            if (aIndex < 0 || aIndex >= this.m_List.Count)
            {
                return null;
            }
            JSONNode result = this.m_List[aIndex];
            this.m_List.RemoveAt(aIndex);
            return result;
        }
        public override JSONNode Remove(JSONNode aNode)
        {
            this.m_List.Remove(aNode);
            return aNode;
        }
        public override IEnumerable<JSONNode> Children
        {
            get
            {
                foreach (JSONNode jsonnode in this.m_List)
                {
                    yield return jsonnode;
                }
#pragma warning disable CS0219 // Переменной "enumerator" присвоено значение, но оно ни разу не использовано.
                List<JSONNode>.Enumerator enumerator = default(List<JSONNode>.Enumerator);
#pragma warning restore CS0219 // Переменной "enumerator" присвоено значение, но оно ни разу не использовано.
                yield break;
            }
        }
        internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
        {
            aSB.Append('[');
            int count = this.m_List.Count;
            if (this.inline)
            {
                aMode = JSONTextMode.Compact;
            }
            for (int i = 0; i < count; i++)
            {
                if (i > 0)
                {
                    aSB.Append(',');
                }
                if (aMode == JSONTextMode.Indent)
                {
                    aSB.AppendLine();
                }
                if (aMode == JSONTextMode.Indent)
                {
                    aSB.Append(' ', aIndent + aIndentInc);
                }
                this.m_List[i].WriteToStringBuilder(aSB, aIndent + aIndentInc, aIndentInc, aMode);
            }
            if (aMode == JSONTextMode.Indent)
            {
                aSB.AppendLine().Append(' ', aIndent);
            }
            aSB.Append(']');
        }
        public List<JSONNode> m_List = new List<JSONNode>();
        public bool inline;
    }
}
