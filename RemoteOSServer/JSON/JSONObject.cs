using System.Text;

namespace EasyJSON
{
    public class JSONObject : JSONNode
    {
        public override bool Inline
        {
            get => inline;
            set => inline = value;
        }
        public override JSONNodeType Tag => JSONNodeType.Object;
        public override bool IsObject => true;
        public override Enumerator GetEnumerator() => new(m_Dict.GetEnumerator());
        public override JSONNode this[string aKey]
        {
            get
            {
                if (m_Dict.ContainsKey(aKey)) return m_Dict[aKey];
                return new JSONLazyCreator(this, aKey);
            }
            set
            {
				value ??= JSONNull.CreateOrGet();
				if (m_Dict.ContainsKey(aKey))
                {
                    m_Dict[aKey] = value;
                    return;
                }
                m_Dict.Add(aKey, value);
            }
        }
        public override JSONNode this[int aIndex]
        {
            get
            {
                if (aIndex < 0 || aIndex >= m_Dict.Count) return null;
				return m_Dict.ElementAt(aIndex).Value;
            }
            set
            {
				if (aIndex < 0 || aIndex >= m_Dict.Count) return;
                m_Dict[m_Dict.ElementAt(aIndex).Key] = value ?? JSONNull.CreateOrGet();
            }
        }
        public override int Count => m_Dict.Count;
        public override void Add(string aKey, JSONNode aItem)
        {
			aItem ??= JSONNull.CreateOrGet();
			if (string.IsNullOrEmpty(aKey))
            {
                m_Dict.Add(Guid.NewGuid().ToString(), aItem);
                return;
            }
            if (m_Dict.ContainsKey(aKey))
            {
                m_Dict[aKey] = aItem;
                return;
            }
            m_Dict.Add(aKey, aItem);
        }
        public override JSONNode Remove(string aKey)
        {
            if (!m_Dict.ContainsKey(aKey)) return null;
            JSONNode result = m_Dict[aKey];
            m_Dict.Remove(aKey);
            return result;
        }
        public override JSONNode Remove(int aIndex)
        {
            if (aIndex < 0 || aIndex >= m_Dict.Count) return null;
            KeyValuePair<string, JSONNode> keyValuePair = m_Dict.ElementAt(aIndex);
            m_Dict.Remove(keyValuePair.Key);
            return keyValuePair.Value;
        }
        public override JSONNode Remove(JSONNode aNode)
        {
            JSONNode result;
            try
            {
                KeyValuePair<string, JSONNode> keyValuePair = (from k in m_Dict
                                                               where k.Value == aNode
                                                               select k).First<KeyValuePair<string, JSONNode>>();
                m_Dict.Remove(keyValuePair.Key);
                result = aNode;
            }
            catch
            {
                result = null;
            }
            return result;
        }
        public override IEnumerable<JSONNode> Children
        {
            get
            {
                foreach (KeyValuePair<string, JSONNode> keyValuePair in m_Dict)
                    yield return keyValuePair.Value;
                yield break;
            }
        }
        internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
        {
            aSB.Append('{');
            bool flag = true;
            if (inline) aMode = JSONTextMode.Compact;
			foreach (KeyValuePair<string, JSONNode> keyValuePair in m_Dict)
            {
                if (!flag) aSB.Append(',');
				flag = false;
                if (aMode == JSONTextMode.Indent)
                {
					aSB.AppendLine();
					aSB.Append(' ', aIndent + aIndentInc);
				}
                aSB.Append('"').Append(Escape(keyValuePair.Key)).Append('"');
                if (aMode == JSONTextMode.Compact)
                    aSB.Append(':');
                else
                    aSB.Append(" : ");
                keyValuePair.Value.WriteToStringBuilder(aSB, aIndent + aIndentInc, aIndentInc, aMode);
            }
            if (aMode == JSONTextMode.Indent) aSB.AppendLine().Append(' ', aIndent);
			aSB.Append('}');
        }
        public Dictionary<string, JSONNode> m_Dict = new();
        public bool inline;
    }
}
