using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace EPOS_NewUI.Views
{
    // 1. CLASE MODELO PARA EL TICKET
    public class TicketItem
    {
        public string Nombre { get; set; }
        public int PrecioUnitario { get; set; }
        public int Cantidad { get; set; }
        
        // Calcula el total por línea
        public int Total => PrecioUnitario * Cantidad;
        public string TotalFormateado => $"${Total.ToString("N0")}";
    }

    public partial class PosPremiumUC : UserControl
    {
        // 2. LISTA OBSERVABLE PARA AGRUPAR EL CARRITO
        private ObservableCollection<TicketItem> listaTicket = new ObservableCollection<TicketItem>();
        
        // Variable para recordar la categoría seleccionada en los botones grises
        private string categoriaActual = "Todas";

        public PosPremiumUC()
        {
            InitializeComponent();
            IniciarReloj();
            
            // Le decimos al ItemsControl (la lista en pantalla) de dónde sacar los datos
            icTicket.ItemsSource = listaTicket;
        }

        // 3. FUNCIONALIDAD DEL RELOJ EN TIEMPO REAL
        private void IniciarReloj()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (s, e) => { txtHora.Text = DateTime.Now.ToString("HH:mm:ss"); };
            timer.Start();
        }

        // 4. EVENTO: CUANDO SE ESCRIBE EN EL BUSCADOR
        private void TxtBuscar_TextChanged(object sender, TextChangedEventArgs e)
        {
            FiltrarProductos();
        }

        // 5. EVENTO: CLIC EN CATEGORÍA
        private void BtnCategoria_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null && btn.Tag != null)
            {
                categoriaActual = btn.Tag.ToString(); // Guardamos la categoría actual
                FiltrarProductos();
            }
        }

        // 6. MÉTODO CENTRAL DE FILTRADO (Buscador + Categoría)
        private void FiltrarProductos()
        {
            // Pasamos todo a minúsculas para que no importe si escriben "C" o "c"
            string textoBuscado = txtBuscar.Text.ToLower();

            // Recorremos todos los botones de producto en el WrapPanel
            foreach (UIElement elemento in wpProductos.Children)
            {
                if (elemento is Button btnProducto && btnProducto.Tag != null)
                {
                    // Extraemos los datos ("Categoria|Nombre|Precio")
                    string[] datos = btnProducto.Tag.ToString().Split('|');
                    
                    // Verificamos que el Tag tenga la estructura correcta para evitar errores
                    if(datos.Length >= 2)
                    {
                        string categoria = datos[0];
                        string nombre = datos[1].ToLower(); // Pasamos el nombre a minúsculas también

                        // Verificamos si pasa el filtro de la categoría de los botones grises
                        bool pasaFiltroCategoria = (categoriaActual == "Todas" || categoria == categoriaActual);
                        
                        // Verificamos si pasa el filtro de texto
                        // Usamos .StartsWith para buscar por la primera letra.
                        bool pasaFiltroTexto = string.IsNullOrEmpty(textoBuscado) || nombre.StartsWith(textoBuscado);

                        // Si cumple AMBAS condiciones, se muestra. Si no, se oculta.
                        if (pasaFiltroCategoria && pasaFiltroTexto)
                        {
                            btnProducto.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            btnProducto.Visibility = Visibility.Collapsed;
                        }
                    }
                }
            }
        }

        // 7. EVENTO: CLIC EN UN PRODUCTO (AGREGAR AL TICKET)
        private void BtnProducto_Click(object sender, RoutedEventArgs e)
        {
            Button btnClickeado = sender as Button;
            
            if (btnClickeado != null && btnClickeado.Tag != null)
            {
                // Extraemos Categoria, Nombre y Precio del Tag ("Bebidas|Café Americano|3000")
                string[] datosProducto = btnClickeado.Tag.ToString().Split('|');
                
                if(datosProducto.Length >= 3)
                {
                    string nombre = datosProducto[1];
                    int precio = int.Parse(datosProducto[2]);

                    // Buscamos si el producto ya está en el ticket
                    TicketItem itemExistente = listaTicket.FirstOrDefault(i => i.Nombre == nombre);

                    if (itemExistente != null)
                    {
                        // Si existe, le sumamos 1 a la cantidad
                        itemExistente.Cantidad++;
                    }
                    else
                    {
                        // Si no existe, creamos una nueva línea
                        listaTicket.Add(new TicketItem { Nombre = nombre, PrecioUnitario = precio, Cantidad = 1 });
                    }

                    RefrescarTicket();
                }
            }
        }

        // 8. EVENTO: CLIC EN EL BOTÓN "+" DEL TICKET
        private void BtnSumarItem_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            TicketItem item = btn.Tag as TicketItem; // Rescatamos a qué item le hizo clic
            
            if(item != null)
            {
                item.Cantidad++;
                RefrescarTicket();
            }
        }

        // 9. EVENTO: CLIC EN EL BOTÓN "-" DEL TICKET
        private void BtnRestarItem_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            TicketItem item = btn.Tag as TicketItem;
            
            if(item != null)
            {
                item.Cantidad--;
                
                // Si la cantidad llega a 0, eliminamos el producto del ticket
                if (item.Cantidad <= 0)
                {
                    listaTicket.Remove(item);
                }

                RefrescarTicket();
            }
        }

        // 10. ACTUALIZAR TOTALES Y REFRESCAR VISTA
        private void RefrescarTicket()
        {
            // Forzamos a la lista visual a refrescar sus datos
            icTicket.Items.Refresh();

            // Sumamos el total de cada línea
            int subtotalTicket = listaTicket.Sum(i => i.Total);
            int iva = (int)(subtotalTicket * 0.19);
            int total = subtotalTicket + iva;

            txtSubtotal.Text = $"Subtotal: ${subtotalTicket.ToString("N0")}";
            txtIva.Text = $"IVA (19%): ${iva.ToString("N0")}";
            txtTotal.Text = $"Total: ${total.ToString("N0")}";
            
            // Sumamos la CANTIDAD total de artículos (ej: 2 cafés + 1 jugo = 3)
            int cantidadArticulos = listaTicket.Sum(i => i.Cantidad);
            txtContadorArticulos.Text = $"{cantidadArticulos} artículos";
        }

        // 11. FUNCIONALIDAD DE LIMPIAR LA VENTA
        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            listaTicket.Clear(); 
            RefrescarTicket();             
        }

        // 12. FUNCIONALIDAD DE COBRO
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

        // 13. FUNCIONALIDAD DE VOLVER AL MENÚ
        private void BtnVolver_Click(object sender, RoutedEventArgs e)
        {
            MainWindow ventanaRaiz = (MainWindow)Window.GetWindow(this);
            ventanaRaiz.CambiarPantalla(new MenuUC());
        }
    }
}