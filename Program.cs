using System;
using System.Data.SqlClient;
using System.Net;
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

            // SAST Vulnerabilidad 1: Inyección SQL (capturada para evitar caída del contenedor)
            try
            {
                ExecuteSqlVulnerable(userInput);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[SAST Log] Excepción SQL esperada: " + ex.Message);
            }

            // SAST Vulnerabilidad 2: Algoritmo de Hash inseguro MD5
            string hashed = HashPassword(DbPassword);
            Console.WriteLine("Hash generado: " + hashed);

            // Servidor HTTP continuo para permitir el escaneo DAST
            StartHttpServer();
        }

        static void StartHttpServer()
        {
            int port = 8080;
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add($"http://*:{port}/");
            listener.Start();
            Console.WriteLine($"[DAST] Servidor HTTP activo y escuchando en el puerto {port}...");

            while (true)
            {
                HttpListenerContext context = listener.GetContext();
                HttpListenerResponse response = context.Response;

                string responseString = "<html><body><h1>DevSecOps Demo App</h1><p>Aplicacion web activa para pruebas DAST.</p></body></html>";
                byte[] buffer = Encoding.UTF8.GetBytes(responseString);

                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);
                response.OutputStream.Close();
            }
        }

        static void ExecuteSqlVulnerable(string userParam)
        {
            string connectionString = "Server=10.0.0.5;Database=ProdDB;User=admin;Password=" + DbPassword;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Users WHERE Username = '" + userParam + "'";
                SqlCommand command = new SqlCommand(query, connection);

                // SonarCloud detecta esta línea como ejecución del exploit
                command.ExecuteReader();
            }
        }

        static string HashPassword(string password)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(password);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                return Convert.ToHexString(hashBytes);
            }
        }
    }
}