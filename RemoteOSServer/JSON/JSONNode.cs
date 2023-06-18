using System.Collections;
using System.Globalization;
using System.Text;

namespace EasyJSON
{
    public abstract class JSONNode
    {
        public abstract JSONNodeType Tag { get; }
        public virtual JSONNode this[int aIndex]
        {
            get => null;
            set { }
        }
        public virtual JSONNode this[string aKey]
        {
            get => null;
            set { }
        }
        public virtual string Value
        {
            get => "";
            set { }
        }
        public virtual int Count => 0;
        public virtual bool IsNumber => false;
        public virtual bool IsString => false;
        public virtual bool IsBoolean => false;
        public virtual bool IsNull => false;
        public virtual bool IsArray => false;
        public virtual bool IsObject => false;
        public virtual bool Inline
        {
            get => false;
            set { }
        }
        public virtual void Add(string aKey, JSONNode aItem) { }
        public virtual void Add(JSONNode aItem) => Add("", aItem);
        public virtual JSONNode Remove(string aKey) => null;
        public virtual JSONNode Remove(int aIndex) => null;
        public virtual JSONNode Remove(JSONNode aNode) => aNode;
        public virtual IEnumerable<JSONNode> Children
        {
            get
            {
                yield break;
            }
        }
        public IEnumerable<JSONNode> DeepChildren
        {
            get
            {
                foreach (JSONNode jsonnode in Children)
                    foreach (JSONNode jsonnode2 in jsonnode.DeepChildren)
                        yield return jsonnode2;
                yield break;
            }
        }
        public override string ToString()
        {
            StringBuilder stringBuilder = new();
            WriteToStringBuilder(stringBuilder, 0, 0, JSONTextMode.Compact);
            return stringBuilder.ToString();
        }
        public virtual string ToString(int aIndent)
        {
            StringBuilder stringBuilder = new();
            WriteToStringBuilder(stringBuilder, 0, aIndent, JSONTextMode.Indent);
            return stringBuilder.ToString();
        }
        internal abstract void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode);
        public abstract JSONNode.Enumerator GetEnumerator();
        public IEnumerable<KeyValuePair<string, JSONNode>> Linq => new LinqEnumerator(this);
        public JSONNode.KeyEnumerator Keys => new KeyEnumerator(GetEnumerator());
        public JSONNode.ValueEnumerator Values => new ValueEnumerator(GetEnumerator());
        public virtual double AsDouble
        {
            get
            {
                if (double.TryParse(Value, out double result)) return result;
                return 0.0;
            }
            set => Value = value.ToString();
        }
        public virtual int AsInt
        {
            get => (int)AsDouble;
            set => AsDouble = (double)value;
        }
        public virtual float AsFloat
        {
            get => (float)AsDouble;
            set => AsDouble = (double)value;
        }
        public virtual bool AsBool
        {
            get
            {
                if (bool.TryParse(Value, out bool result)) return result;
                return !string.IsNullOrEmpty(Value);
            }
            set => Value = (value ? "true" : "false");
        }
        public virtual JSONArray AsArray =>  this as JSONArray;
        public virtual JSONObject AsObject => this as JSONObject;
        public static implicit operator JSONNode(string s) => new JSONString(s);
        public static implicit operator string(JSONNode d) => d?.Value;
		public static implicit operator JSONNode(double n) => new JSONNumber(n);
		public static implicit operator double(JSONNode d) => d is not null ? d.AsDouble : 0.0;
		public static implicit operator JSONNode(float n) => new JSONNumber((double)n);
		public static implicit operator float(JSONNode d) => d is not null ? d.AsFloat : 0f;
		public static implicit operator JSONNode(int n) => new JSONNumber((double)n);
		public static implicit operator int(JSONNode d) => d is not null ? d.AsInt : 0;
		public static implicit operator JSONNode(bool b) => new JSONBool(b);
		public static implicit operator bool(JSONNode d) => d is not null && d.AsBool;
		public static implicit operator JSONNode(KeyValuePair<string, JSONNode> aKeyValue) => aKeyValue.Value;
		public static bool operator ==(JSONNode a, object b)
        {
            if ((object)a == b) return true;
            bool flag = a is JSONNull || a is null || a is JSONLazyCreator;
            bool flag2 = b is JSONNull || b is null || b is JSONLazyCreator;
            return (flag && flag2) || (!flag && a.Equals(b));
        }
		public static bool operator !=(JSONNode a, object b) => !(a == b);
		public override bool Equals(object obj) => (object)this == obj;
		public override int GetHashCode() => base.GetHashCode();
		internal static StringBuilder EscapeBuilder => m_EscapeBuilder ??= new StringBuilder();
		internal static string Escape(string aText)
        {
            StringBuilder escapeBuilder = EscapeBuilder;
            escapeBuilder.Length = 0;
            if (aText is null) return "";
            if (escapeBuilder.Capacity < aText.Length + aText.Length / 10)
                escapeBuilder.Capacity = aText.Length + aText.Length / 10;
            int i = 0;
            while (i < aText.Length)
            {
                char c = aText[i];
                switch (c)
                {
                    case '\b':
                        escapeBuilder.Append("\\b");
                        break;
                    case '\t':
                        escapeBuilder.Append("\\t");
                        break;
                    case '\n':
                        escapeBuilder.Append("\\n");
                        break;
                    case '\v':
                        goto IL_E2;
                    case '\f':
                        escapeBuilder.Append("\\f");
                        break;
                    case '\r':
                        escapeBuilder.Append("\\r");
                        break;
                    default:
                        if (c != '"')
                        {
                            if (c != '\\') goto IL_E2;
                            escapeBuilder.Append("\\\\");
                        }
                        else escapeBuilder.Append("\\\"");
                        break;
                }
            IL_121:
                i++;
                continue;
            IL_E2:
                if (c < ' ' || (forceASCII && c > '\u007f'))
                {
                    ushort num = (ushort)c;
                    escapeBuilder.Append("\\u").Append(num.ToString("X4"));
                    goto IL_121;
                }
                escapeBuilder.Append(c);
                goto IL_121;
            }
            string result = escapeBuilder.ToString();
            escapeBuilder.Length = 0;
            return result;
        }
        public static void ParseElement(JSONNode ctx, string token, string tokenName, bool quoted)
        {
            if (quoted)
            {
                ctx.Add(tokenName, token);
                return;
            }
            string a = token.ToLower();
            if (a == "false" || a == "true")
            {
                ctx.Add(tokenName, a == "true");
                return;
            }
            if (a == "null")
            {
                ctx.Add(tokenName, null);
                return;
            }
            if (double.TryParse(token, NumberStyles.Number, CultureInfo.InvariantCulture, out double n))
            {
                ctx.Add(tokenName, n);
                return;
            }
            ctx.Add(tokenName, token);
        }
        public static JSONNode Parse(string aJSON)
        {
            Stack<JSONNode> stack = new Stack<JSONNode>();
            JSONNode jsonnode = null;
            int i = 0;
            StringBuilder stringBuilder = new StringBuilder();
            string text = "";
            bool flag = false;
            bool flag2 = false;
            while (i < aJSON.Length)
            {
                char c = aJSON[i];
                if (c <= ',')
                {
                    if (c <= ' ')
                    {
                        switch (c)
                        {
                            case '\t':
                                break;
                            case '\n':
                            case '\r':
                                goto IL_33E;
                            case '\v':
                            case '\f':
                                goto IL_330;
                            default:
                                if (c != ' ') goto IL_330;
                                break;
                        }
                        if (flag) stringBuilder.Append(aJSON[i]);
                    }
                    else if (c != '"')
                    {
                        if (c != ',') goto IL_330;
                        if (flag) stringBuilder.Append(aJSON[i]);
                        else
                        {
                            if (stringBuilder.Length > 0 || flag2) ParseElement(jsonnode, stringBuilder.ToString(), text, flag2);
                            text = "";
                            stringBuilder.Length = 0;
                            flag2 = false;
                        }
                    }
                    else
                    {
                        flag = !flag;
                        flag2 = (flag2 || flag);
                    }
                }
                else
                {
                    if (c <= ']')
                    {
                        if (c != ':')
                        {
                            switch (c)
                            {
                                case '[':
                                    if (flag)
                                    {
                                        stringBuilder.Append(aJSON[i]);
                                        goto IL_33E;
                                    }
                                    stack.Push(new JSONArray());
                                    jsonnode?.Add(text, stack.Peek());
                                    text = "";
                                    stringBuilder.Length = 0;
                                    jsonnode = stack.Peek();
                                    goto IL_33E;
                                case '\\':
                                    i++;
                                    if (flag)
                                    {
                                        char c2 = aJSON[i];
                                        if (c2 <= 'f')
                                        {
                                            if (c2 == 'b')
                                            {
                                                stringBuilder.Append('\b');
                                                goto IL_33E;
                                            }
                                            if (c2 == 'f')
                                            {
                                                stringBuilder.Append('\f');
                                                goto IL_33E;
                                            }
                                        }
                                        else
                                        {
                                            if (c2 == 'n')
                                            {
                                                stringBuilder.Append('\n');
                                                goto IL_33E;
                                            }
                                            switch (c2)
                                            {
                                                case 'r':
                                                    stringBuilder.Append('\r');
                                                    goto IL_33E;
                                                case 't':
                                                    stringBuilder.Append('\t');
                                                    goto IL_33E;
                                                case 'u':
                                                    {
                                                        string s = aJSON.Substring(i + 1, 4);
                                                        stringBuilder.Append((char)int.Parse(s, NumberStyles.AllowHexSpecifier));
                                                        i += 4;
                                                        goto IL_33E;
                                                    }
                                            }
                                        }
                                        stringBuilder.Append(c2);
                                        goto IL_33E;
                                    }
                                    goto IL_33E;
                                case ']':
                                    break;
                                default:
                                    goto IL_330;
                            }
                        }
                        else
                        {
                            if (flag)
                            {
                                stringBuilder.Append(aJSON[i]);
                                goto IL_33E;
                            }
                            text = stringBuilder.ToString();
                            stringBuilder.Length = 0;
                            flag2 = false;
                            goto IL_33E;
                        }
                    }
                    else if (c != '{')
                    {
                        if (c != '}') goto IL_330;
                    }
                    else
                    {
                        if (flag)
                        {
                            stringBuilder.Append(aJSON[i]);
                            goto IL_33E;
                        }
                        stack.Push(new JSONObject());
                        jsonnode?.Add(text, stack.Peek());
                        text = "";
                        stringBuilder.Length = 0;
                        jsonnode = stack.Peek();
                        goto IL_33E;
                    }
                    if (flag) stringBuilder.Append(aJSON[i]);
                    else
                    {
                        if (stack.Count == 0) throw new Exception("JSON Parse: Too many closing brackets");
                        stack.Pop();
                        if (stringBuilder.Length > 0 || flag2)
                        {
							ParseElement(jsonnode, stringBuilder.ToString(), text, flag2);
                            flag2 = false;
                        }
                        text = "";
                        stringBuilder.Length = 0;
                        if (stack.Count > 0) jsonnode = stack.Peek();
                    }
                }
            IL_33E:
                i++;
                continue;
            IL_330:
                stringBuilder.Append(aJSON[i]);
                goto IL_33E;
            }
            if (flag) throw new Exception("JSON Parse: Quotation marks seems to be messed up.");
            return jsonnode;
        }
        public static bool forceASCII;
        [ThreadStatic]
        public static StringBuilder m_EscapeBuilder;
        public struct Enumerator
        {
            public bool IsValid => type > Type.None;
            public Enumerator(List<JSONNode>.Enumerator aArrayEnum)
            {
				type = Type.Array;
                m_Object = default;
                m_Array = aArrayEnum;
            }
            public Enumerator(Dictionary<string, JSONNode>.Enumerator aDictEnum)
            {
                type = Type.Object;
                m_Object = aDictEnum;
                m_Array = default;
            }
			public KeyValuePair<string, JSONNode> Current => type == Type.Array
						? new KeyValuePair<string, JSONNode>(string.Empty, m_Array.Current)
						: type == Type.Object ? m_Object.Current : new KeyValuePair<string, JSONNode>(string.Empty, null);
			public bool MoveNext() => type == Type.Array ? m_Array.MoveNext() : type == Type.Object && m_Object.MoveNext();
			public Type type;
            public Dictionary<string, JSONNode>.Enumerator m_Object;
            public List<JSONNode>.Enumerator m_Array;
            public enum Type
            {
                None,
                Array,
                Object
            }
        }
        public struct ValueEnumerator
        {
			public ValueEnumerator(List<JSONNode>.Enumerator aArrayEnum) => this = new ValueEnumerator(new Enumerator(aArrayEnum));
			public ValueEnumerator(Dictionary<string, JSONNode>.Enumerator aDictEnum) => this = new ValueEnumerator(new Enumerator(aDictEnum));
			public ValueEnumerator(Enumerator aEnumerator) => m_Enumerator = aEnumerator;
			public JSONNode Current => m_Enumerator.Current.Value;
			public bool MoveNext() => m_Enumerator.MoveNext();
			public ValueEnumerator GetEnumerator() => this;

			public Enumerator m_Enumerator;
        }
        public struct KeyEnumerator
        {
			public KeyEnumerator(List<JSONNode>.Enumerator aArrayEnum) => this = new KeyEnumerator(new Enumerator(aArrayEnum));
			public KeyEnumerator(Dictionary<string, JSONNode>.Enumerator aDictEnum) => this = new KeyEnumerator(new Enumerator(aDictEnum));
			public KeyEnumerator(Enumerator aEnumerator) => m_Enumerator = aEnumerator;
			public JSONNode Current => m_Enumerator.Current.Key;
			public bool MoveNext() => m_Enumerator.MoveNext();
			public KeyEnumerator GetEnumerator() => this;
			public Enumerator m_Enumerator;
        }
        public class LinqEnumerator : IEnumerator<KeyValuePair<string, JSONNode>>, IEnumerator, IDisposable, IEnumerable<KeyValuePair<string, JSONNode>>, IEnumerable
        {
            internal LinqEnumerator(JSONNode aNode)
            {
                m_Node = aNode;
                if (m_Node is not null) m_Enumerator = m_Node.GetEnumerator();
            }
			public KeyValuePair<string, JSONNode> Current => m_Enumerator.Current;
			object IEnumerator.Current => m_Enumerator.Current;
			public bool MoveNext() => m_Enumerator.MoveNext();
			public void Dispose()
            {
                m_Node = null;
                
                m_Enumerator = default;
            }
			public IEnumerator<KeyValuePair<string, JSONNode>> GetEnumerator() => new LinqEnumerator(m_Node);
			public void Reset()
            {
                if (m_Node is not null) m_Enumerator = m_Node.GetEnumerator();
            }
			IEnumerator IEnumerable.GetEnumerator() => new LinqEnumerator(m_Node);
			public JSONNode m_Node;
            public Enumerator m_Enumerator;
        }
    }
}
