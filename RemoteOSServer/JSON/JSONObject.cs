using System.Text;

namespace EasyJSON
{
    public class JSONObject : JSONNode
    {
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
                return JSONNodeType.Object;
            }
        }
        public override bool IsObject
        {
            get
            {
                return true;
            }
        }
        public override JSONNode.Enumerator GetEnumerator()
        {
            return new JSONNode.Enumerator(this.m_Dict.GetEnumerator());
        }
        public override JSONNode this[string aKey]
        {
            get
            {
                if (this.m_Dict.ContainsKey(aKey))
                {
                    return this.m_Dict[aKey];
                }
                return new JSONLazyCreator(this, aKey);
            }
            set
            {
                if (value == null)
                {
                    value = JSONNull.CreateOrGet();
                }
                if (this.m_Dict.ContainsKey(aKey))
                {
                    this.m_Dict[aKey] = value;
                    return;
                }
                this.m_Dict.Add(aKey, value);
            }
        }
        public override JSONNode this[int aIndex]
        {
            get
            {
                if (aIndex < 0 || aIndex >= this.m_Dict.Count)
                {
                    return null;
                }
                return this.m_Dict.ElementAt(aIndex).Value;
            }
            set
            {
                if (value == null)
                {
                    value = JSONNull.CreateOrGet();
                }
                if (aIndex < 0 || aIndex >= this.m_Dict.Count)
                {
                    return;
                }
                string key = this.m_Dict.ElementAt(aIndex).Key;
                this.m_Dict[key] = value;
            }
        }
        public override int Count
        {
            get
            {
                return this.m_Dict.Count;
            }
        }
        public override void Add(string aKey, JSONNode aItem)
        {
            if (aItem == null)
            {
                aItem = JSONNull.CreateOrGet();
            }
            if (string.IsNullOrEmpty(aKey))
            {
                this.m_Dict.Add(Guid.NewGuid().ToString(), aItem);
                return;
            }
            if (this.m_Dict.ContainsKey(aKey))
            {
                this.m_Dict[aKey] = aItem;
                return;
            }
            this.m_Dict.Add(aKey, aItem);
        }
        public override JSONNode Remove(string aKey)
        {
            if (!this.m_Dict.ContainsKey(aKey))
            {
                return null;
            }
            JSONNode result = this.m_Dict[aKey];
            this.m_Dict.Remove(aKey);
            return result;
        }
        public override JSONNode Remove(int aIndex)
        {
            if (aIndex < 0 || aIndex >= this.m_Dict.Count)
            {
                return null;
            }
            KeyValuePair<string, JSONNode> keyValuePair = this.m_Dict.ElementAt(aIndex);
            this.m_Dict.Remove(keyValuePair.Key);
            return keyValuePair.Value;
        }
        public override JSONNode Remove(JSONNode aNode)
        {
            JSONNode result;
            try
            {
                KeyValuePair<string, JSONNode> keyValuePair = (from k in this.m_Dict
                                                               where k.Value == aNode
                                                               select k).First<KeyValuePair<string, JSONNode>>();
                this.m_Dict.Remove(keyValuePair.Key);
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
                foreach (KeyValuePair<string, JSONNode> keyValuePair in this.m_Dict)
                {
                    yield return keyValuePair.Value;
                }
#pragma warning disable CS0219 // Переменной "enumerator" присвоено значение, но оно ни разу не использовано.
                Dictionary<string, JSONNode>.Enumerator enumerator = default(Dictionary<string, JSONNode>.Enumerator);
#pragma warning restore CS0219 // Переменной "enumerator" присвоено значение, но оно ни разу не использовано.
                yield break;
            }
        }
        internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
        {
            aSB.Append('{');
            bool flag = true;
            if (this.inline)
            {
                aMode = JSONTextMode.Compact;
            }
            foreach (KeyValuePair<string, JSONNode> keyValuePair in this.m_Dict)
            {
                if (!flag)
                {
                    aSB.Append(',');
                }
                flag = false;
                if (aMode == JSONTextMode.Indent)
                {
                    aSB.AppendLine();
                }
                if (aMode == JSONTextMode.Indent)
                {
                    aSB.Append(' ', aIndent + aIndentInc);
                }
                aSB.Append('"').Append(JSONNode.Escape(keyValuePair.Key)).Append('"');
                if (aMode == JSONTextMode.Compact)
                {
                    aSB.Append(':');
                }
                else
                {
                    aSB.Append(" : ");
                }
                keyValuePair.Value.WriteToStringBuilder(aSB, aIndent + aIndentInc, aIndentInc, aMode);
            }
            if (aMode == JSONTextMode.Indent)
            {
                aSB.AppendLine().Append(' ', aIndent);
            }
            aSB.Append('}');
        }
        public Dictionary<string, JSONNode> m_Dict = new Dictionary<string, JSONNode>();
        public bool inline;
    }
}
