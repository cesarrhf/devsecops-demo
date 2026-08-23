using System;

namespace DevSecOpsDemo
{
    class Program
    {
        // SAST Alerta 1: Credencial expuesta en texto plano (Hardcoded Secret)
        private const string DbPassword = "AdminPassword_2026_SuperSecret!";
        private const string ConnectionString = "Server=10.0.0.5;Database=ProdDB;User=admin;Password=" + DbPassword;

        static void Main(string[] args)
        {
            Console.WriteLine("Iniciando aplicación de prueba...");

            string userParam = "' OR '1'='1";
            
            // SAST Alerta 2: Construcción insegura de consulta SQL (SQL Injection Risk)
            string sqlQuery = "SELECT * FROM Users WHERE Username = '" + userParam + "'";
            
            ExecuteQuery(sqlQuery);
        }

        static void ExecuteQuery(string query)
        {
            Console.WriteLine("Ejecutando SQL: " + query);
        }
    }
}
