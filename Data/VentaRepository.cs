using Microsoft.Data.Sqlite;
using System;
using System.Collections.ObjectModel;
using EPOS_NewUI.Views;

namespace EPOS_NewUI.Data
{
    public class VentaRepository
    {
        public void Guardar(VentaRegistro venta)
        {
            using var connection = new SqliteConnection($"Data Source={SqliteDatabase.DatabasePath}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Ventas (Detalle, Total, Fecha)
                VALUES ($detalle, $total, $fecha);
            ";
            command.Parameters.AddWithValue("$detalle", venta.Detalle);
            command.Parameters.AddWithValue("$total", venta.Total);
            command.Parameters.AddWithValue("$fecha", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            command.ExecuteNonQuery();
        }

        public ObservableCollection<VentaRegistro> ObtenerUltimas(int cantidad)
        {
            var ventas = new ObservableCollection<VentaRegistro>();
            using var connection = new SqliteConnection($"Data Source={SqliteDatabase.DatabasePath}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT Detalle, Total FROM Ventas ORDER BY Id DESC LIMIT $cantidad";
            command.Parameters.AddWithValue("$cantidad", cantidad);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                ventas.Add(new VentaRegistro
                {
                    Detalle = reader.GetString(0),
                    Total = reader.GetDecimal(1)
                });
            }

            return ventas;
        }
    }
}
