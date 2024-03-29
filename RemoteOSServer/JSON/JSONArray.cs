﻿using System.Text;

namespace EasyJSON
{
    public class JSONArray : JSONNode
    {
        public JSONArray(IEnumerable<JSONNode> values) : base()
        {
            foreach (var a in values) Add(a);
        }
        public JSONArray() : base() { }
        public override bool Inline
        {
            get => inline;
            set => inline = value;
        }
        public override JSONNodeType Tag => JSONNodeType.Array;
        public override bool IsArray => true;
        public override JSONNode.Enumerator GetEnumerator() => new(m_List.GetEnumerator());
        public override JSONNode this[int aIndex]
        {
            get
            {
                if (aIndex < 0 || aIndex >= m_List.Count)
                    return new JSONLazyCreator(this);
                return m_List[aIndex];
            }
            set
            {
                value ??= JSONNull.CreateOrGet();
                if (aIndex < 0 || aIndex >= m_List.Count)
                {
                    m_List.Add(value);
                    return;
                }
                m_List[aIndex] = value;
            }
        }
        public override JSONNode this[string aKey]
        {
            get => new JSONLazyCreator(this);
            set => m_List.Add(value ?? JSONNull.CreateOrGet());
        }
        public override int Count => m_List.Count;
        public override void Add(string aKey, JSONNode aItem) => m_List.Add(aItem ?? JSONNull.CreateOrGet());
        public override JSONNode Remove(int aIndex)
        {
            if (aIndex < 0 || aIndex >= m_List.Count) return null;
            JSONNode result = m_List[aIndex];
            m_List.RemoveAt(aIndex);
            return result;
        }
        public override JSONNode Remove(JSONNode aNode)
        {
            m_List.Remove(aNode);
            return aNode;
        }
        public override IEnumerable<JSONNode> Children
        {
            get
            {
                foreach (JSONNode jsonnode in m_List)
                    yield return jsonnode;
                yield break;
            }
        }
        internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
        {
            aSB.Append('[');
            int count = m_List.Count;
            if (inline) aMode = JSONTextMode.Compact;
            for (int i = 0; i < count; i++)
            {
                if (i > 0) aSB.Append(',');
                if (aMode == JSONTextMode.Indent)
                {
                    aSB.AppendLine();
                    aSB.Append(' ', aIndent + aIndentInc);
                }
                m_List[i].WriteToStringBuilder(aSB, aIndent + aIndentInc, aIndentInc, aMode);
            }
            if (aMode == JSONTextMode.Indent) aSB.AppendLine().Append(' ', aIndent);
            aSB.Append(']');
        }
        public List<JSONNode> m_List = new();
        public bool inline;
    }
}
