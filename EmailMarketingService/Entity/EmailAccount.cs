namespace EmailMarketing.SalesLogix.Entities
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// Contains details of the email marketing integration accounts.
    /// </summary>
    [SchemaName(EntityName)]
    public class EmailAccount : Entity
    {
        /// <summary>The name of the entity within SalesLogix</summary>
        public const string EntityName = "EMEmailAccount";

        /// <summary>The name of the SalesLogix sdata feed</summary>
        public const string ResourceKind = "emailaccounts";

        /// <summary>Gets or sets the text identifier of the email marketing integration account.</summary>
        [SchemaName("AccountName")]
        public string AccountName { get; set; }

        /// <summary>Gets or sets the Textual description of the email marketing integration account.</summary>
        [SchemaName("Description")]
        public string Description { get; set; }

        /// <summary>Gets or sets the Date and time that the email marketing integration account was last synchronised.</summary>
        [SchemaName("LastSynchronised")]
        public DateTime? LastSynchronised { get; set; }

        /// <summary>Gets or sets the API ID for used by this email marketing integration account.</summary>
        [SchemaName("EmailApiID")]
        public string EmailApiId { get; set; }

        /// <summary>Gets or sets the length of time in minutes between synchronisation runs for this email marketing integration account.</summary>
        [SchemaName("SyncIntervalMinutes")]
        public int SyncIntervalMinutes { get; set; }

        /// <summary>Gets or sets the password to use for connecting the the email marketing service.</summary>
        /// <remarks>When stored in the DB, the password is encrypted, you will need to decrypt it to use it.</remarks>
        [SchemaName("APIPassword")]
        public string ApiPasswordEncrypted { get; set; }

        /// <summary>Gets or sets the API Key (Username) to be used to connect to the email marketing service.</summary>
        [SchemaName("APIKey")]
        public string ApiKey { get; set; }

        public string GetDecryptedPassword()
        {
            string secKey = @"2@t?0e~))""x}fzke#8S!%Y<Ze@:s{_KG[!z?WS\a?zR)T.\-Hr,%Y'_f~wy*oO1";
            return AESDecryptData(ApiPasswordEncrypted, secKey);
        }

        public override string ToString()
        {
            return AccountName;
        }

        private static string AESDecryptData(string input, string secKey)
        {
            if (input == null)
            {
                return null;
            }

            if (secKey == null)
            {
                throw new ArgumentNullException("secKey");
            }

            byte[] encryptedBytes = Convert.FromBase64String(input);
            byte[] saltBytes = Encoding.UTF8.GetBytes(secKey);
            string decryptedString = string.Empty;
            using (var aes = new AesManaged())
            {
                Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(secKey, saltBytes);
                aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
                aes.KeySize = aes.LegalKeySizes[0].MaxSize;
                aes.Key = rfc.GetBytes(aes.KeySize / 8);
                aes.IV = rfc.GetBytes(aes.BlockSize / 8);

                using (ICryptoTransform decryptTransform = aes.CreateDecryptor())
                {
                    using (MemoryStream decryptedStream = new MemoryStream())
                    {
                        CryptoStream decryptor =
                            new CryptoStream(decryptedStream, decryptTransform, CryptoStreamMode.Write);
                        decryptor.Write(encryptedBytes, 0, encryptedBytes.Length);
                        decryptor.Flush();
                        decryptor.Close();

                        byte[] decryptBytes = decryptedStream.ToArray();
                        decryptedString = UTF8Encoding.UTF8.GetString(decryptBytes, 0, decryptBytes.Length);
                    }
                }
            }

            return decryptedString;
        }
    }
}