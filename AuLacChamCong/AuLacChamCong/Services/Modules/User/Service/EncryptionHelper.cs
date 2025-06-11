using System.Text;

namespace AuLacChamCong.Services.Modules.User.Service
{
    public class EncryptionHelper
    {
        public static string EncodeString(string strInput)
        {
            bool isEmpty = string.IsNullOrEmpty(strInput);
            if (isEmpty)
            {
                strInput = "a2z";
            }
            else
            {
                strInput = "zd" + strInput + "2a";
            }

            char[] array = strInput.ToCharArray();
            int num = (array.Length % 2 != 0) ? 155 : 115;

            StringBuilder encryptedText = new StringBuilder();
            for (int i = 0; i < array.Length; i++)
            {
                encryptedText.Append((char)(array[i] + num + i));
            }

            return encryptedText.ToString();
        }
        public static string DecodeString(string strInput)
        {
            if (string.IsNullOrEmpty(strInput))
                return "";

            char[] array = strInput.ToCharArray();
            int num = (array.Length % 2 != 0) ? 155 : 115;

            StringBuilder decryptedText = new StringBuilder();
            for (int i = 0; i < array.Length; i++)
            {
                decryptedText.Append((char)(array[i] - num - i));
            }

            string result = decryptedText.ToString();

            if (result == "a2z")
                return "";

            if (result.StartsWith("zd") && result.EndsWith("2a"))
            {
                result = result.Substring(2, result.Length - 4);
            }

            return result;
        }
    }

}
