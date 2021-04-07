using System;
using System.Security.Cryptography;

namespace ACQ.Web.ExternalServices.SecurityAudit
{
    public class TripleDES
    {

        private byte[] bPassword;

        private string sPassword;
        public TripleDES(string Password = "password")
        {
            // On Class Begin
            this.Password = Password;
        }

        public string PasswordHash
        {
            get
            {
                System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();
                return UTF8.GetString(bPassword);
            }
        }

        public string Password
        {
            get
            {
                System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();
                return sPassword;
            }
            set
            {
                System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();
                MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
                bPassword = HashProvider.ComputeHash(UTF8.GetBytes(value));
                sPassword = value;
            }
        }

        #region "Encrypt"

        // Encrypt using Password from Property Set (pre-hashed)
        public string Encrypt(string Message)
        {
            byte[] Results = null;
            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();
            using (MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider())
            {
                byte[] TDESKey = bPassword;
                using (TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider { Key = TDESKey, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
                {
                    byte[] DataToEncrypt = UTF8.GetBytes(Message);
                    try
                    {
                        ICryptoTransform Encryptor = TDESAlgorithm.CreateEncryptor();
                        Results = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);
                    }
                    finally
                    {
                        TDESAlgorithm.Clear();
                        HashProvider.Clear();
                    }
                }
            }
            return Convert.ToBase64String(Results);
        }

        // Encrypt using Password as byte array
        private string Encrypt(string Message, byte[] Password)
        {
            byte[] Results = null;
            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();
            using (MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider())
            {
                byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(UTF8.GetString(Password)));
                using (TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider { Key = TDESKey, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
                {
                    byte[] DataToEncrypt = UTF8.GetBytes(Message);
                    try
                    {
                        ICryptoTransform Encryptor = TDESAlgorithm.CreateEncryptor();
                        Results = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);
                    }
                    finally
                    {
                        TDESAlgorithm.Clear();
                        HashProvider.Clear();
                    }
                }
            }
            return Convert.ToBase64String(Results);
        }

        // Encrypt using Password as string
        public string Encrypt(string Message, string Password)
        {
            byte[] Results = null;
            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();
            // Step 1. We hash the Passphrase using MD5
            // We use the MD5 hash generator as the result is a 128 bit byte array
            // which is a valid length for the Triple DES encoder we use below
            using (MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider())
            {
                byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(Password));

                // Step 2. Create a new TripleDESCryptoServiceProvider object

                // Step 3. Setup the encoder
                using (TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider { Key = TDESKey, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
                {
                    // Step 4. Convert the input string to a byte[]

                    byte[] DataToEncrypt = UTF8.GetBytes(Message);

                    // Step 5. Attempt to encrypt the string
                    try
                    {
                        ICryptoTransform Encryptor = TDESAlgorithm.CreateEncryptor();
                        Results = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);
                    }
                    finally
                    {
                        // Clear the Triple Des and Hashprovider services of any sensitive information
                        TDESAlgorithm.Clear();
                        HashProvider.Clear();
                    }
                }
            }

            // Step 6. Return the encrypted string as a base64 encoded string
            return Convert.ToBase64String(Results);
        }
        #endregion

        #region "Decrypt"
        // Decrypt using Password from Property (pre-hashed)
        public string Decrypt(string Message)
        {
            byte[] Results = null;
            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();
            using (MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider())
            {
                byte[] TDESKey = this.bPassword;
                using (TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider { Key = TDESKey, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
                {
                    byte[] DataToDecrypt = Convert.FromBase64String(Message);
                    try
                    {
                        ICryptoTransform Decryptor = TDESAlgorithm.CreateDecryptor();
                        Results = Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length);
                    }
                    finally
                    {
                        TDESAlgorithm.Clear();
                        HashProvider.Clear();
                    }
                }
            }
            return UTF8.GetString(Results);
        }

        // Decrypt using Password as Byte array
        public string Decrypt(string Message, byte[] Password)
        {
            byte[] Results = null;
            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();
            using (MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider())
            {
                byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(UTF8.GetString(Password)));
                using (TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider { Key = TDESKey, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
                {
                    byte[] DataToDecrypt = Convert.FromBase64String(Message);
                    try
                    {
                        ICryptoTransform Decryptor = TDESAlgorithm.CreateDecryptor();
                        Results = Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length);
                    }
                    finally
                    {
                        TDESAlgorithm.Clear();
                        HashProvider.Clear();
                    }
                }
            }
            return UTF8.GetString(Results);
        }


        // Decrypt using Password as string
        public string Decrypt(string Message, string Password)
        {
            byte[] Results = null;
            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();

            // Step 1. We hash the pass phrase using MD5
            // We use the MD5 hash generator as the result is a 128-bit byte array
            // which is a valid length for the Triple DES encoder we use below
            using (MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider())
            {
                byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(Password));

                // Step 2. Create a new TripleDESCryptoServiceProvider object
                // Step 3. Setup the decoder
                using (TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider { Key = TDESKey, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
                {

                    // Step 4. Convert the input string to a byte[]
                    byte[] DataToDecrypt = Convert.FromBase64String(Message);
                    // Step 5. Attempt to decrypt the string
                    try
                    {
                        ICryptoTransform Decryptor = TDESAlgorithm.CreateDecryptor();
                        Results = Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length);

                    }
                    finally
                    {
                        // Clear the Triple Des and Hash provider services of any sensitive information
                        TDESAlgorithm.Clear();
                        HashProvider.Clear();
                    }
                }
            }

            // Step 6. Return the decrypted string in UTF8 format
            return UTF8.GetString(Results);
        }

        #endregion


        
    }
}
