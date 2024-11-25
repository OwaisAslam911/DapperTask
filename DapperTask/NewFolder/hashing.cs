using System.Text;
using System.Security.Cryptography;


namespace DapperTask.NewFolder
{
    public class hashing
    {


        public static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                // Send a sample text to hash.  
                var hashedPassword = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                
                // Print the string.   
                Console.WriteLine(hashedPassword);
                return Convert.ToBase64String(hashedPassword);
            }
        }

    }
}
