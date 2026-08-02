using EPOS_NewUI.Data;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace EPOS_NewUI.Views
{
    public class ProductoModel
    {
        public int Id { get; set; }
        public string Categoria { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public bool Activo { get; set; } = true;
        public string PrecioFormateado => "$" + Precio.ToString("N0");
    }

    public partial class AdministracionUC : UserControl
    {
        private readonly ObservableCollection<ProductoModel> productos;
        private ProductoModel? productoSeleccionado;
        private readonly ProductoRepository productoRepository = new ProductoRepository();

        public AdministracionUC()
        {
            InitializeComponent();
            productos = productoRepository.ObtenerTodos();
            dgProductos.ItemsSource = productos;
        }

        private void BtnNuevo_Click(object sender, RoutedEventArgs e)
        {
            LimpiarFormulario();
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtPrecio.Text))
            {
                MessageBox.Show("Complete al menos el nombre y el precio del producto.", "Datos incompletos", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtPrecio.Text, out decimal precio))
            {
                MessageBox.Show("El precio debe ser un número válido.", "Valor inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(txtStock.Text, out int stock))
            {
                stock = 0;
            }

            string categoria = (cbCategoria.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Bebidas";

            if (productoSeleccionado != null)
            {
                productoSeleccionado.Categoria = categoria;
                productoSeleccionado.Nombre = txtNombre.Text.Trim();
                productoSeleccionado.Precio = precio;
                productoSeleccionado.Stock = stock;
                productoSeleccionado.Activo = chkActivo.IsChecked == true;
                dgProductos.Items.Refresh();
                MessageBox.Show("Producto actualizado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                var nuevoProducto = new ProductoModel
                {
                    Categoria = categoria,
                    Nombre = txtNombre.Text.Trim(),
                    Precio = precio,
                    Stock = stock,
                    Activo = chkActivo.IsChecked == true
                };

                productoRepository.Guardar(nuevoProducto);
                productos.Clear();
                foreach (var producto in productoRepository.ObtenerTodos())
                {
                    productos.Add(producto);
                }
                MessageBox.Show("Producto agregado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            LimpiarFormulario();
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (productoSeleccionado == null)
            {
                MessageBox.Show("Seleccione un producto de la lista para eliminarlo.", "Sin selección", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var resultado = MessageBox.Show($"¿Desea eliminar el producto '{productoSeleccionado.Nombre}'?", "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (resultado == MessageBoxResult.Yes)
            {
                productoRepository.Eliminar(productoSeleccionado.Id);
                productos.Remove(productoSeleccionado);
                LimpiarFormulario();
                MessageBox.Show("Producto eliminado.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void DgProductos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            productoSeleccionado = dgProductos.SelectedItem as ProductoModel;
            if (productoSeleccionado != null)
            {
                cbCategoria.SelectedItem = cbCategoria.Items.Cast<ComboBoxItem>().FirstOrDefault(i => i.Content?.ToString() == productoSeleccionado.Categoria);
                txtNombre.Text = productoSeleccionado.Nombre;
                txtPrecio.Text = productoSeleccionado.Precio.ToString();
                txtStock.Text = productoSeleccionado.Stock.ToString();
                chkActivo.IsChecked = productoSeleccionado.Activo;
            }
        }

        private void LimpiarFormulario()
        {
            productoSeleccionado = null;
            cbCategoria.SelectedIndex = 0;
            txtNombre.Clear();
            txtPrecio.Clear();
            txtStock.Text = "0";
            chkActivo.IsChecked = true;
            dgProductos.SelectedItem = null;
        }

        private void BtnVolver_Click(object sender, RoutedEventArgs e)
        {
            MainWindow ventanaRaiz = (MainWindow)Window.GetWindow(this);
            ventanaRaiz.CambiarPantalla(new MenuUC(ventanaRaiz.UsuarioActual));
        }
    }
}
