using EPOS_NewUI.Data;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace EPOS_NewUI.Views
{
    public class VentaResumen
    {
        public string Producto { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal Total { get; set; }
        public string Resumen => $"{Producto} - {Cantidad} und. - ${Total:N0}";
    }

    public class VentaRegistro
    {
        public string Detalle { get; set; } = string.Empty;
        public decimal Total { get; set; }
    }

    public partial class InformesUC : UserControl
    {
        private readonly ObservableCollection<VentaResumen> productosVendidos = new ObservableCollection<VentaResumen>();
        private readonly ObservableCollection<VentaRegistro> ultimasVentas = new ObservableCollection<VentaRegistro>();
        private readonly VentaRepository ventaRepository = new VentaRepository();

        public InformesUC()
        {
            InitializeComponent();
            lbTopProductos.ItemsSource = productosVendidos;
            lbUltimasVentas.ItemsSource = ultimasVentas;
            CargarDatos();
        }

        private void CargarDatos()
        {
            productosVendidos.Clear();
            ultimasVentas.Clear();

            var ventas = ventaRepository.ObtenerUltimas(5);
            foreach (var venta in ventas)
            {
                ultimasVentas.Add(venta);
            }

            productosVendidos.Add(new VentaResumen { Producto = "Café Americano", Cantidad = 8, Total = 24000 });
            productosVendidos.Add(new VentaResumen { Producto = "Sandwich Club", Cantidad = 5, Total = 20000 });
            productosVendidos.Add(new VentaResumen { Producto = "Cheesecake", Cantidad = 3, Total = 15000 });

            ActualizarResumen();
        }

        private void ActualizarResumen()
        {
            decimal ventasTotales = productosVendidos.Sum(p => p.Total);
            int tickets = ultimasVentas.Count;
            decimal promedio = tickets > 0 ? ventasTotales / tickets : 0;

            txtVentasTotales.Text = "$" + ventasTotales.ToString("N0");
            txtTickets.Text = tickets.ToString();
            txtPromedio.Text = "$" + promedio.ToString("N0");
        }

        private void BtnActualizar_Click(object sender, RoutedEventArgs e)
        {
            CargarDatos();
            MessageBox.Show("Resumen actualizado.", "Informes", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnVolver_Click(object sender, RoutedEventArgs e)
        {
            MainWindow ventanaRaiz = (MainWindow)Window.GetWindow(this);
            ventanaRaiz.CambiarPantalla(new MenuUC(ventanaRaiz.UsuarioActual));
        }
    }
}
