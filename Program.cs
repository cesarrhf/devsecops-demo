using System;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace DevSecOpsDemo
{
    class Program
    {
        private const string DbPassword = "AdminPassword_2026_SuperSecret!";

        static void Main(string[] args)
        {
            Console.WriteLine("Iniciando aplicación...");

            string userInput = args.Length > 0 ? args[0] : "' OR '1'='1";

            // SAST Vulnerabilidad 1: Inyección SQL real con Sink de base de datos (S3649)
            ExecuteSqlVulnerable(userInput);

            // SAST Vulnerabilidad 2: Algoritmo de Hash inseguro y obsoleto (S2077 / S4790)
            string hashed = HashPassword(DbPassword);
            Console.WriteLine("Hash: " + hashed);
        }

        static void ExecuteSqlVulnerable(string userParam)
        {
            string connectionString = "Server=10.0.0.5;Database=ProdDB;User=admin;Password=" + DbPassword;
            
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Concatenación directa procesada por el lector de base de datos
                string query = "SELECT * FROM Users WHERE Username = '" + userParam + "'";
                SqlCommand command = new SqlCommand(query, connection);
                
                // SonarCloud detecta esta línea como la ejecución del exploit
                command.ExecuteReader(); 
            }
        }

        static string HashPassword(string password)
        {
            // Uso explícito de MD5 (Criptografía débil)
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(password);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                return Convert.ToHexString(hashBytes);
            }
        }
    }
}
