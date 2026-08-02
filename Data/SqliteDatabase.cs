using Microsoft.Data.Sqlite;
using System;
using System.IO;

namespace EPOS_NewUI.Data
{
    public static class SqliteDatabase
    {
        public static string DatabasePath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "epos.db");

        public static void Initialize()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(DatabasePath) ?? AppDomain.CurrentDomain.BaseDirectory);

            using var connection = new SqliteConnection($"Data Source={DatabasePath}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Productos (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Categoria TEXT NOT NULL,
                    Nombre TEXT NOT NULL,
                    Precio REAL NOT NULL,
                    Stock INTEGER NOT NULL,
                    Activo INTEGER NOT NULL
                );

                CREATE TABLE IF NOT EXISTS Ventas (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Detalle TEXT NOT NULL,
                    Total REAL NOT NULL,
                    Fecha TEXT NOT NULL
                );
            ";

            command.ExecuteNonQuery();
        }
    }
}
