using EasyJSON;

namespace RemoteOS.OpenComputers.Components
{
    [Obsolete("Use cryptography methods defined on the server, not on the remote machine.")]
    [Component("data")]
    public class DataComponent : Component
    {
        int? _limit;
        public DataComponent(Machine parent, Guid address) : base(parent, address)
        {
        }

        /// <returns>The maximum size of data that can be passed to other functions of the card.</returns>
        public async Task<int> GetLimit() => _limit ??= (await Invoke("getLinit"))[0];

#if ROS_PROPERTIES
        public int Limit => GetLimit().Result;
#endif

        #region Tier 1

        /// <summary>
        /// Computes CRC-32 hash of the data. Result is binary data.
        /// </summary>
        /// <param name="message">Message to compute hash on</param>
        /// <returns>Hash of the specified message</returns>
        public async Task<string> CRC32(string message) => (await Invoke("crc32", $@"""{message}"""))[0];
        /// <summary>
        /// Applies base64 encoding to the data.
        /// </summary>
        /// <param name="message">Message to encode</param>
        /// <returns>Encoded message</returns>
        public async Task<string> Encode64(string message) => (await Invoke("encode64", $@"""{message}"""))[0];
        /// <summary>
        /// Applies base64 decoding to the data.
        /// </summary>
        /// <param name="message">Encoded message</param>
        /// <returns>Decoded message</returns>
        public async Task<string> Decode64(string message) => (await Invoke("decode64", $@"""{message}"""))[0];
        /// <summary>
        /// Computes MD5 hash of the data. Result is binary data.
        /// </summary>
        /// <param name="message">The message to compute hash on</param>
        /// <returns>Hash of the specified message</returns>
        public async Task<string> MD5(string message) => (await Invoke("md5", $@"""{message}"""))[0];
        /// <summary>
        /// Computes SHA2-256 hash of the data. Result is binary data.
        /// </summary>
        /// <param name="message">The message to compute hash on</param>
        /// <returns>Hash of the specified message</returns>
        public async Task<string> SHA256(string message) => (await Invoke("sha256", $@"""{message}"""))[0];
        /// <summary>
        /// Applies deflate compression to the data.
        /// </summary>
        /// <param name="message">Message to deflate</param>
        /// <returns>Compressed message</returns>
        public async Task<string> Deflate(string message) => (await Invoke("deflate", $@"""{message}"""))[0];
        /// <summary>
        /// Applies inflate decompression to the data.
        /// </summary>
        /// <param name="message">Deflated message</param>
        /// <returns>Decoded message</returns>
        public async Task<string> Inflate(string message) => (await Invoke("inflate", $@"""{message}"""))[0];

        #endregion
        #region Tier 2

        /// <summary>
        /// Encrypt data with AES. Result is binary data.
        /// </summary>
        /// <param name="message">The message to encrypt</param>
        /// <param name="key">Key to encrypt with</param>
        /// <param name="iv">Enryption vector</param>
        /// <returns>Encrypted data</returns>
        public async Task<string> Encrypt(string message, string key, string iv) => (await Invoke("encrypt", $@"""{message}""", $@"""{key}""", $@"""{iv}"""))[0];
        /// <summary>
        /// Decrypt data with AES.
        /// </summary>
        /// <param name="message">Encrypted data</param>
        /// <param name="key">Key that the data was encrypted with</param>
        /// <param name="iv">Encryption vector</param>
        /// <returns></returns>
        public async Task<string> Decrypt(string message, string key, string iv) => (await Invoke("encrypt", $@"""{message}""", $@"""{key}""", $@"""{iv}"""))[0];
        /// <summary>
        /// Generates secure random binary data.
        /// </summary>
        /// <param name="length">Length of the random data</param>
        /// <returns>Random binary data</returns>
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
