using Microsoft.AspNetCore.DataProtection;
using System.Text;

namespace WebVersionStore.Models.Local
{
    public class JwtSettings
    {
        public string Secret { get; set; }
        public byte[] SecretBytes { get 
            {
                return AsBytes(Secret); 
            } 
        }
        public static byte[] AsBytes(string secret)
        {
            return Encoding.ASCII.GetBytes(secret);
        }
    }
}
