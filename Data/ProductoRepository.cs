using EPOS_NewUI.Views;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.ObjectModel;

namespace EPOS_NewUI.Data
{
    public class ProductoRepository
    {
        public ObservableCollection<ProductoModel> ObtenerTodos()
        {
            var productos = new ObservableCollection<ProductoModel>();
            using var connection = new SqliteConnection($"Data Source={SqliteDatabase.DatabasePath}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Categoria, Nombre, Precio, Stock, Activo FROM Productos ORDER BY Id";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                productos.Add(new ProductoModel
                {
                    Id = reader.GetInt32(0),
                    Categoria = reader.GetString(1),
                    Nombre = reader.GetString(2),
                    Precio = reader.GetDecimal(3),
                    Stock = reader.GetInt32(4),
                    Activo = reader.GetInt32(5) == 1
                });
            }

            return productos;
        }

        public void Guardar(ProductoModel producto)
        {
            using var connection = new SqliteConnection($"Data Source={SqliteDatabase.DatabasePath}");
            connection.Open();

            var command = connection.CreateCommand();
            if (producto.Id > 0)
            {
                command.CommandText = @"
                    UPDATE Productos
                    SET Categoria = $categoria, Nombre = $nombre, Precio = $precio, Stock = $stock, Activo = $activo
                    WHERE Id = $id;
                ";
                command.Parameters.AddWithValue("$id", producto.Id);
            }
            else
            {
                command.CommandText = @"
                    INSERT INTO Productos (Categoria, Nombre, Precio, Stock, Activo)
                    VALUES ($categoria, $nombre, $precio, $stock, $activo);
                ";
            }

            command.Parameters.AddWithValue("$categoria", producto.Categoria);
            command.Parameters.AddWithValue("$nombre", producto.Nombre);
            command.Parameters.AddWithValue("$precio", producto.Precio);
            command.Parameters.AddWithValue("$stock", producto.Stock);
            command.Parameters.AddWithValue("$activo", producto.Activo ? 1 : 0);
            command.ExecuteNonQuery();
        }

        public void Eliminar(int id)
        {
            using var connection = new SqliteConnection($"Data Source={SqliteDatabase.DatabasePath}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Productos WHERE Id = $id";
            command.Parameters.AddWithValue("$id", id);
            command.ExecuteNonQuery();
        }
    }
}
