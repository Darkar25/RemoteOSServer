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
            get
            {
                return null;
            }
            set
            {
            }
        }
        public virtual JSONNode this[string aKey]
        {
            get
            {
                return null;
            }
            set
            {
            }
        }
        public virtual string Value
        {
            get
            {
                return "";
            }
            set
            {
            }
        }
        public virtual int Count
        {
            get
            {
                return 0;
            }
        }
        public virtual bool IsNumber
        {
            get
            {
                return false;
            }
        }
        public virtual bool IsString
        {
            get
            {
                return false;
            }
        }
        public virtual bool IsBoolean
        {
            get
            {
                return false;
            }
        }
        public virtual bool IsNull
        {
            get
            {
                return false;
            }
        }
        public virtual bool IsArray
        {
            get
            {
                return false;
            }
        }
        public virtual bool IsObject
        {
            get
            {
                return false;
            }
        }
        public virtual bool Inline
        {
            get
            {
                return false;
            }
            set
            {
            }
        }
        public virtual void Add(string aKey, JSONNode aItem)
        {
        }
        public virtual void Add(JSONNode aItem)
        {
            this.Add("", aItem);
        }
        public virtual JSONNode Remove(string aKey)
        {
            return null;
        }
        public virtual JSONNode Remove(int aIndex)
        {
            return null;
        }
        public virtual JSONNode Remove(JSONNode aNode)
        {
            return aNode;
        }
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
                foreach (JSONNode jsonnode in this.Children)
                {
                    foreach (JSONNode jsonnode2 in jsonnode.DeepChildren)
                    {
                        yield return jsonnode2;
                    }
#pragma warning disable CS0219 // Переменной "enumerator2" присвоено значение, но оно ни разу не использовано.
                    IEnumerator<JSONNode> enumerator2 = null;
#pragma warning restore CS0219 // Переменной "enumerator2" присвоено значение, но оно ни разу не использовано.
                }
#pragma warning disable CS0219 // Переменной "enumerator" присвоено значение, но оно ни разу не использовано.
                IEnumerator<JSONNode> enumerator = null;
#pragma warning restore CS0219 // Переменной "enumerator" присвоено значение, но оно ни разу не использовано.
                yield break;
            }
        }
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            this.WriteToStringBuilder(stringBuilder, 0, 0, JSONTextMode.Compact);
            return stringBuilder.ToString();
        }
        public virtual string ToString(int aIndent)
        {
            StringBuilder stringBuilder = new StringBuilder();
            this.WriteToStringBuilder(stringBuilder, 0, aIndent, JSONTextMode.Indent);
            return stringBuilder.ToString();
        }
        internal abstract void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode);
        public abstract JSONNode.Enumerator GetEnumerator();
        public IEnumerable<KeyValuePair<string, JSONNode>> Linq
        {
            get
            {
                return new JSONNode.LinqEnumerator(this);
            }
        }
        public JSONNode.KeyEnumerator Keys
        {
            get
            {
                return new JSONNode.KeyEnumerator(this.GetEnumerator());
            }
        }
        public JSONNode.ValueEnumerator Values
        {
            get
            {
                return new JSONNode.ValueEnumerator(this.GetEnumerator());
            }
        }
        public virtual double AsDouble
        {
            get
            {
                double result = 0.0;
                if (double.TryParse(this.Value, out result))
                {
                    return result;
                }
                return 0.0;
            }
            set
            {
                this.Value = value.ToString();
            }
        }
        public virtual int AsInt
        {
            get
            {
                return (int)this.AsDouble;
            }
            set
            {
                this.AsDouble = (double)value;
            }
        }
        public virtual float AsFloat
        {
            get
            {
                return (float)this.AsDouble;
            }
            set
            {
                this.AsDouble = (double)value;
            }
        }
        public virtual bool AsBool
        {
            get
            {
                bool result = false;
                if (bool.TryParse(this.Value, out result))
                {
                    return result;
                }
                return !string.IsNullOrEmpty(this.Value);
            }
            set
            {
                this.Value = (value ? "true" : "false");
            }
        }
        public virtual JSONArray AsArray
        {
            get
            {
                return this as JSONArray;
            }
        }
        public virtual JSONObject AsObject
        {
            get
            {
                return this as JSONObject;
            }
        }
        public static implicit operator JSONNode(string s)
        {
            return new JSONString(s);
        }
        public static implicit operator string(JSONNode d)
        {
            if (!(d == null))
            {
                return d.Value;
            }
            return null;
        }
        public static implicit operator JSONNode(double n)
        {
            return new JSONNumber(n);
        }
        public static implicit operator double(JSONNode d)
        {
            if (!(d == null))
            {
                return d.AsDouble;
            }
            return 0.0;
        }
        public static implicit operator JSONNode(float n)
        {
            return new JSONNumber((double)n);
        }
        public static implicit operator float(JSONNode d)
        {
            if (!(d == null))
            {
                return d.AsFloat;
            }
            return 0f;
        }
        public static implicit operator JSONNode(int n)
        {
            return new JSONNumber((double)n);
        }
        public static implicit operator int(JSONNode d)
        {
            if (!(d == null))
            {
                return d.AsInt;
            }
            return 0;
        }
        public static implicit operator JSONNode(bool b)
        {
            return new JSONBool(b);
        }
        public static implicit operator bool(JSONNode d)
        {
            return !(d == null) && d.AsBool;
        }
        public static implicit operator JSONNode(KeyValuePair<string, JSONNode> aKeyValue)
        {
            return aKeyValue.Value;
        }
        public static bool operator ==(JSONNode a, object b)
        {
            if ((object)a == b)
            {
                return true;
            }
            bool flag = a is JSONNull || (object)a == null || a is JSONLazyCreator;
            bool flag2 = b is JSONNull || b == null || b is JSONLazyCreator;
            return (flag && flag2) || (!flag && a.Equals(b));
        }
        public static bool operator !=(JSONNode a, object b)
        {
            return !(a == b);
        }
        public override bool Equals(object obj)
        {
            return (object)this == obj;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        internal static StringBuilder EscapeBuilder
        {
            get
            {
                if (JSONNode.m_EscapeBuilder == null)
                {
                    JSONNode.m_EscapeBuilder = new StringBuilder();
                }
                return JSONNode.m_EscapeBuilder;
            }
        }
        internal static string Escape(string aText)
        {
            StringBuilder escapeBuilder = JSONNode.EscapeBuilder;
            escapeBuilder.Length = 0;
            if (aText is null) return "";
            if (escapeBuilder.Capacity < aText.Length + aText.Length / 10)
            {
                escapeBuilder.Capacity = aText.Length + aText.Length / 10;
            }
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
                            if (c != '\\')
                            {
                                goto IL_E2;
                            }
                            escapeBuilder.Append("\\\\");
                        }
                        else
                        {
                            escapeBuilder.Append("\\\"");
                        }
                        break;
                }
            IL_121:
                i++;
                continue;
            IL_E2:
                if (c < ' ' || (JSONNode.forceASCII && c > '\u007f'))
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
            double n;
            if (double.TryParse(token, NumberStyles.Number, CultureInfo.InvariantCulture, out n))
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
                                if (c != ' ')
                                {
                                    goto IL_330;
                                }
                                break;
                        }
                        if (flag)
                        {
                            stringBuilder.Append(aJSON[i]);
                        }
                    }
                    else if (c != '"')
                    {
                        if (c != ',')
                        {
                            goto IL_330;
                        }
                        if (flag)
                        {
                            stringBuilder.Append(aJSON[i]);
                        }
                        else
                        {
                            if (stringBuilder.Length > 0 || flag2)
                            {
                                JSONNode.ParseElement(jsonnode, stringBuilder.ToString(), text, flag2);
                            }
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
                                    if (jsonnode != null)
                                    {
                                        jsonnode.Add(text, stack.Peek());
                                    }
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
                        if (c != '}')
                        {
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
                        stack.Push(new JSONObject());
                        if (jsonnode != null)
                        {
                            jsonnode.Add(text, stack.Peek());
                        }
                        text = "";
                        stringBuilder.Length = 0;
                        jsonnode = stack.Peek();
                        goto IL_33E;
                    }
                    if (flag)
                    {
                        stringBuilder.Append(aJSON[i]);
                    }
                    else
                    {
                        if (stack.Count == 0)
                        {
                            throw new Exception("JSON Parse: Too many closing brackets");
                        }
                        stack.Pop();
                        if (stringBuilder.Length > 0 || flag2)
                        {
                            JSONNode.ParseElement(jsonnode, stringBuilder.ToString(), text, flag2);
                            flag2 = false;
                        }
                        text = "";
                        stringBuilder.Length = 0;
                        if (stack.Count > 0)
                        {
                            jsonnode = stack.Peek();
                        }
                    }
                }
            IL_33E:
                i++;
                continue;
            IL_330:
                stringBuilder.Append(aJSON[i]);
                goto IL_33E;
            }
            if (flag)
            {
                throw new Exception("JSON Parse: Quotation marks seems to be messed up.");
            }
            return jsonnode;
        }
        public static bool forceASCII;
        [ThreadStatic]
        public static StringBuilder m_EscapeBuilder;
        public struct Enumerator
        {
            public bool IsValid
            {
                get
                {
                    return this.type > JSONNode.Enumerator.Type.None;
                }
            }
            public Enumerator(List<JSONNode>.Enumerator aArrayEnum)
            {
                this.type = JSONNode.Enumerator.Type.Array;
                this.m_Object = default(Dictionary<string, JSONNode>.Enumerator);
                this.m_Array = aArrayEnum;
            }
            public Enumerator(Dictionary<string, JSONNode>.Enumerator aDictEnum)
            {
                this.type = JSONNode.Enumerator.Type.Object;
                this.m_Object = aDictEnum;
                this.m_Array = default(List<JSONNode>.Enumerator);
            }
            public KeyValuePair<string, JSONNode> Current
            {
                get
                {
                    if (this.type == JSONNode.Enumerator.Type.Array)
                    {
                        return new KeyValuePair<string, JSONNode>(string.Empty, this.m_Array.Current);
                    }
                    if (this.type == JSONNode.Enumerator.Type.Object)
                    {
                        return this.m_Object.Current;
                    }
                    return new KeyValuePair<string, JSONNode>(string.Empty, null);
                }
            }
            public bool MoveNext()
            {
                if (this.type == JSONNode.Enumerator.Type.Array)
                {
                    return this.m_Array.MoveNext();
                }
                return this.type == JSONNode.Enumerator.Type.Object && this.m_Object.MoveNext();
            }
            public JSONNode.Enumerator.Type type;
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
            public ValueEnumerator(List<JSONNode>.Enumerator aArrayEnum)
            {
                this = new JSONNode.ValueEnumerator(new JSONNode.Enumerator(aArrayEnum));
            }
            public ValueEnumerator(Dictionary<string, JSONNode>.Enumerator aDictEnum)
            {
                this = new JSONNode.ValueEnumerator(new JSONNode.Enumerator(aDictEnum));
            }
            public ValueEnumerator(JSONNode.Enumerator aEnumerator)
            {
                this.m_Enumerator = aEnumerator;
            }
            public JSONNode Current
            {
                get
                {
                    KeyValuePair<string, JSONNode> keyValuePair = this.m_Enumerator.Current;
                    return keyValuePair.Value;
                }
            }
            public bool MoveNext()
            {
                return this.m_Enumerator.MoveNext();
            }
            public JSONNode.ValueEnumerator GetEnumerator()
            {
                return this;
            }

            public JSONNode.Enumerator m_Enumerator;
        }
        public struct KeyEnumerator
        {
            public KeyEnumerator(List<JSONNode>.Enumerator aArrayEnum)
            {
                this = new JSONNode.KeyEnumerator(new JSONNode.Enumerator(aArrayEnum));
            }
            public KeyEnumerator(Dictionary<string, JSONNode>.Enumerator aDictEnum)
            {
                this = new JSONNode.KeyEnumerator(new JSONNode.Enumerator(aDictEnum));
            }
            public KeyEnumerator(JSONNode.Enumerator aEnumerator)
            {
                this.m_Enumerator = aEnumerator;
            }
            public JSONNode Current
            {
                get
                {
                    KeyValuePair<string, JSONNode> keyValuePair = this.m_Enumerator.Current;
                    return keyValuePair.Key;
                }
            }
            public bool MoveNext()
            {
                return this.m_Enumerator.MoveNext();
            }
            public JSONNode.KeyEnumerator GetEnumerator()
            {
                return this;
            }
            public JSONNode.Enumerator m_Enumerator;
        }
        public class LinqEnumerator : IEnumerator<KeyValuePair<string, JSONNode>>, IEnumerator, IDisposable, IEnumerable<KeyValuePair<string, JSONNode>>, IEnumerable
        {
            internal LinqEnumerator(JSONNode aNode)
            {
                this.m_Node = aNode;
                if (this.m_Node != null)
                {
                    this.m_Enumerator = this.m_Node.GetEnumerator();
                }
            }
            public KeyValuePair<string, JSONNode> Current
            {
                get
                {
                    return this.m_Enumerator.Current;
                }
            }
            object IEnumerator.Current
            {
                get
                {
                    return this.m_Enumerator.Current;
                }
            }
            public bool MoveNext()
            {
                return this.m_Enumerator.MoveNext();
            }
            public void Dispose()
            {
                this.m_Node = null;
                this.m_Enumerator = default(JSONNode.Enumerator);
            }
            public IEnumerator<KeyValuePair<string, JSONNode>> GetEnumerator()
            {
                return new JSONNode.LinqEnumerator(this.m_Node);
            }
            public void Reset()
            {
                if (this.m_Node != null)
                {
                    this.m_Enumerator = this.m_Node.GetEnumerator();
                }
            }
            IEnumerator IEnumerable.GetEnumerator()
            {
                return new JSONNode.LinqEnumerator(this.m_Node);
            }
            public JSONNode m_Node;
            public JSONNode.Enumerator m_Enumerator;
        }
    }
}
