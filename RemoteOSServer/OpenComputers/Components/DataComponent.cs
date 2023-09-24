using EasyJSON;
using OneOf;
using OneOf.Types;
using RemoteOS.OpenComputers;
using RemoteOS.OpenComputers.Data;
using RemoteOS.Helpers;

namespace RemoteOS.OpenComputers.Components
{
    [Obsolete("Use cryptography methods defined on the server, not on the remote machine.")]
    [Component("data")]
    public partial class DataComponent : Component
    {
        int? _limit;

        public DataComponent(Machine parent, Guid address) : base(parent, address)
        {
        }

        public override async Task<Tier> GetTier() => (Tier)(await Parent.Execute($"({await GetHandle()}.ecdh and 2) or ({await GetHandle()}.random and 1) or 0"))[0].AsInt;

        /// <returns>The maximum size of data that can be passed to other functions of the card.</returns>
        public async Task<int> GetLimit() => _limit ??=
#if ROS_GLOBAL_CACHING
            GlobalCache.dataCardHardLimit ??=
#endif
            (await GetInvoker()())[0];

#if ROS_PROPERTIES
        public int Limit => GetLimit().Result;
#endif

        #region Tier 1

        /// <summary>
        /// Computes CRC-32 hash of the data. Result is binary data.
        /// </summary>
        /// <param name="message">Message to compute hash on</param>
        /// <returns>Hash of the specified message</returns>4
        public partial Task<string> CRC32(string message);
        /// <summary>
        /// Applies base64 encoding to the data.
        /// </summary>
        /// <param name="message">Message to encode</param>
        /// <returns>Encoded message</returns>
        public partial Task<string> Encode64(string message);
        /// <summary>
        /// Applies base64 decoding to the data.
        /// </summary>
        /// <param name="message">Encoded message</param>
        /// <returns>Decoded message</returns>
        public partial Task<string> Decode64(string message);
        /// <summary>
        /// Computes MD5 hash of the data. Result is binary data.
        /// </summary>
        /// <param name="message">The message to compute hash on</param>
        /// <returns>Hash of the specified message</returns>
        public partial Task<string> MD5(string message);
        /// <summary>
        /// Computes SHA2-256 hash of the data. Result is binary data.
        /// </summary>
        /// <param name="message">The message to compute hash on</param>
        /// <returns>Hash of the specified message</returns>
        public partial Task<string> SHA256(string message);
        /// <summary>
        /// Applies deflate compression to the data.
        /// </summary>
        /// <param name="message">Message to deflate</param>
        /// <returns>Compressed message</returns>
        public partial Task<string> Deflate(string message);
        /// <summary>
        /// Applies inflate decompression to the data.
        /// </summary>
        /// <param name="message">Deflated message</param>
        /// <returns>Decoded message</returns>
        public partial Task<string> Inflate(string message);

        #endregion
        #region Tier 2

        /// <summary>
        /// Encrypt data with AES. Result is binary data.
        /// </summary>
        /// <param name="message">The message to encrypt</param>
        /// <param name="key">Key to encrypt with</param>
        /// <param name="iv">Enryption vector</param>
        /// <returns>Encrypted data</returns>
        public partial Task<string> Encrypt(string message, string key, string iv);
        /// <summary>
        /// Decrypt data with AES.
        /// </summary>
        /// <param name="message">Encrypted data</param>
        /// <param name="key">Key that the data was encrypted with</param>
        /// <param name="iv">Encryption vector</param>
        /// <returns></returns>
        public partial Task<string> Decrypt(string message, string key, string iv);
        /// <summary>
        /// Generates secure random binary data.
        /// </summary>
        /// <param name="length">Length of the random data</param>
        /// <returns>Random binary data</returns>
        public partial Task<string> Random(int length);

        #endregion
        #region Tier 3

        public (JSONObject priv, JSONObject pub) GenerateKeyPair(int bitLen = 384)
        {
            if (bitLen != 256 && bitLen != 384) throw new ArgumentException("Invalid key length, must be 256 or 384");
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

        public OneOf<string, Error> ECDSA(string data, JSONObject key, string sig = "")
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
