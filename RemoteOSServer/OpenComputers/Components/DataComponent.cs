using EasyJSON;

namespace RemoteOS.OpenComputers.Components
{
    [Obsolete("Use cryptography methods defined on the server, not on the remote machine.")]
    [Component("data")]
    public class DataComponent : Component
    {
        public DataComponent(Machine parent, Guid address) : base(parent, address)
        {
        }

        #region Tier 1

        public async Task<string> CRC32(string message) => (await Invoke("crc32", $@"""{message}"""))[0];
        public async Task<string> Encode64(string message) => (await Invoke("encode64", $@"""{message}"""))[0];
        public async Task<string> Decode64(string message) => (await Invoke("decode64", $@"""{message}"""))[0];
        public async Task<string> MD5(string message) => (await Invoke("md5", $@"""{message}"""))[0];
        public async Task<string> SHA256(string message) => (await Invoke("sha256", $@"""{message}"""))[0];
        public async Task<string> Deflate(string message) => (await Invoke("deflate", $@"""{message}"""))[0];
        public async Task<string> Inflate(string message) => (await Invoke("inflate", $@"""{message}"""))[0];
        int? _limit;
        public int Limit => _limit ??= Invoke("getLimit").Result[0];

        #endregion
        #region Tier 2

        public async Task<string> Encrypt(string message, string key, string iv) => (await Invoke("encrypt", $@"""{message}""", $@"""{key}""", $@"""{iv}"""))[0];
        public async Task<string> Decrypt(string message, string key, string iv) => (await Invoke("encrypt", $@"""{message}""", $@"""{key}""", $@"""{iv}"""))[0];
        public async Task<string> Random(int length) => (await Invoke("random", length))[0];

        #endregion
        #region Tier 3

        public (JSONObject priv, JSONObject pub) GenerateKeyPair(int bitLen = 384)
        {
            if (bitLen != 256 || bitLen != 384) throw new ArgumentException("Invalid key length, must be 256 or 384");
            throw new NotSupportedException();
        }

        public JSONObject DeserializeKey(string data, string type)
        {
            throw new NotSupportedException();
        }

        public string ECDH(JSONObject priv, JSONObject pub)
        {
            throw new NotSupportedException();
        }

        public (bool Success, string Data) ECDSA(string data, JSONObject key, string sig = "")
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
