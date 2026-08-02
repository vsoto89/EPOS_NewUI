using EPOS_NewUI.Data;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace EPOS_NewUI.Views
{
    public class TicketItem
    {
        public string Nombre { get; set; } = string.Empty;
        public int PrecioUnitario { get; set; }
        public int Cantidad { get; set; }
        public int Total => PrecioUnitario * Cantidad;
        public string TotalFormateado => $"${Total.ToString("N0")}";
    }

    public partial class PosPremiumUC : UserControl
    {
        private readonly ObservableCollection<TicketItem> listaTicket = new ObservableCollection<TicketItem>();
        private string categoriaActual = "Todas";
        private readonly ProductoRepository productoRepository = new ProductoRepository();
        private ObservableCollection<ProductoModel> productosCatalogo;

        public PosPremiumUC()
        {
            InitializeComponent();
            IniciarReloj();
            productosCatalogo = productoRepository.ObtenerTodos();
            icTicket.ItemsSource = listaTicket;
            ActualizarProductosVista();
        }

        private void IniciarReloj()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (s, e) => { txtHora.Text = DateTime.Now.ToString("HH:mm:ss"); };
            timer.Start();
        }

        private void TxtBuscar_TextChanged(object sender, TextChangedEventArgs e)
        {
            FiltrarProductos();
        }

        private void BtnCategoria_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag != null)
            {
                categoriaActual = btn.Tag.ToString() ?? "Todas";
                FiltrarProductos();
            }
        }

        private void ActualizarProductosVista()
        {
            wpProductos.Children.Clear();

            foreach (var producto in productosCatalogo.Where(p => p.Activo))
            {
                var stack = new StackPanel();
                stack.Children.Add(new TextBlock
                {
                    Text = producto.Nombre,
                    Foreground = Brushes.White,
                    FontWeight = FontWeights.SemiBold,
                    FontSize = 14
                });
                stack.Children.Add(new TextBlock
                {
                    Text = "$ " + producto.Precio.ToString("N0"),
                    Foreground = Brushes.LimeGreen,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 5, 0, 0)
                });

                var boton = new Button
                {
                    Content = stack,
                    Tag = producto,
                    Background = Brushes.DimGray,
                    Margin = new Thickness(0, 0, 15, 15),
                    Padding = new Thickness(15),
                    BorderThickness = new Thickness(0),
                    Cursor = Cursors.Hand
                };

                boton.Click += BtnProducto_Click;
                wpProductos.Children.Add(boton);
            }

            FiltrarProductos();
        }

        private void FiltrarProductos()
        {
            string textoBuscado = txtBuscar.Text?.ToLower() ?? string.Empty;

            foreach (UIElement elemento in wpProductos.Children)
            {
                if (elemento is Button btnProducto && btnProducto.Tag is ProductoModel producto)
                {
                    bool pasaFiltroCategoria = categoriaActual == "Todas" || producto.Categoria == categoriaActual;
                    bool pasaFiltroTexto = string.IsNullOrEmpty(textoBuscado) || producto.Nombre.ToLower().StartsWith(textoBuscado);

                    btnProducto.Visibility = pasaFiltroCategoria && pasaFiltroTexto ? Visibility.Visible : Visibility.Collapsed;
                }
            }
        }

        private void BtnProducto_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btnClickeado && btnClickeado.Tag is ProductoModel producto)
            {
                TicketItem itemExistente = listaTicket.FirstOrDefault(i => i.Nombre == producto.Nombre);

                if (itemExistente != null)
                {
                    itemExistente.Cantidad++;
                }
                else
                {
                    listaTicket.Add(new TicketItem { Nombre = producto.Nombre, PrecioUnitario = (int)producto.Precio, Cantidad = 1 });
                }

                producto.Id = producto.Id;
                productoRepository.Guardar(producto);
                RefrescarTicket();
                ActualizarProductosVista();
            }
        }

        private void BtnSumarItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is TicketItem item)
            {
                item.Cantidad++;
                RefrescarTicket();
            }
        }

        private void BtnRestarItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is TicketItem item)
            {
                item.Cantidad--;

                if (item.Cantidad <= 0)
                {
                    listaTicket.Remove(item);
                }

                RefrescarTicket();
            }
        }

        private void RefrescarTicket()
        {
            icTicket.Items.Refresh();

            int subtotalTicket = listaTicket.Sum(i => i.Total);
            int iva = (int)(subtotalTicket * 0.19);
            int total = subtotalTicket + iva;

            txtSubtotal.Text = $"Subtotal: ${subtotalTicket.ToString("N0")}";
            txtIva.Text = $"IVA (19%): ${iva.ToString("N0")}";
            txtTotal.Text = $"Total: ${total.ToString("N0")}";

            int cantidadArticulos = listaTicket.Sum(i => i.Cantidad);
            txtContadorArticulos.Text = $"{cantidadArticulos} artículos";
        }

        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            listaTicket.Clear();
            RefrescarTicket();
            ActualizarProductosVista();
        }

        private void BtnCobrar_Click(object sender, RoutedEventArgs e)
        {
            if (listaTicket.Count > 0)
            {
                MessageBox.Show("¡Cobro realizado con éxito! Imprimiendo ticket...", "Venta Exitosa", MessageBoxButton.OK, MessageBoxImage.Information);
                BtnLimpiar_Click(null, null);
            }
            else
            {
                MessageBox.Show("Agregue productos al ticket antes de cobrar.", "Ticket Vacío", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnVolver_Click(object sender, RoutedEventArgs e)
        {
            MainWindow ventanaRaiz = (MainWindow)Window.GetWindow(this);
            ventanaRaiz.CambiarPantalla(new MenuUC(ventanaRaiz.UsuarioActual));
        }
    }
}