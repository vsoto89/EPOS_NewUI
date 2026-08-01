using System.Windows;
using System.Windows.Controls;

namespace EPOS_NewUI.Views
{
    public partial class MenuUC : UserControl
    {
        public MenuUC(string rolUsuario)
        {
            InitializeComponent();
            AplicarPermisos(rolUsuario);
        }

        private void AplicarPermisos(string rolUsuario)
        {
            bool esAdmin = rolUsuario == "Administrador";

            btnClientes.Visibility = esAdmin ? Visibility.Visible : Visibility.Collapsed;
            btnAdmin.Visibility = esAdmin ? Visibility.Visible : Visibility.Collapsed;
            btnInformes.Visibility = esAdmin ? Visibility.Visible : Visibility.Collapsed;
        }

        // Navegar al Punto de Venta
        private void BtnPos_Click(object sender, RoutedEventArgs e)
        {
            MainWindow ventanaRaiz = (MainWindow)Window.GetWindow(this);
            ventanaRaiz.CambiarPantalla(new PosPremiumUC());
        }

        // Regresar al Login
        private void BtnCerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            MainWindow ventanaRaiz = (MainWindow)Window.GetWindow(this);
            ventanaRaiz.CambiarPantalla(new LoginUC());
        }

        private void BtnAdministracion_Click(object sender, RoutedEventArgs e)
        {
            MainWindow ventanaRaiz = (MainWindow)Window.GetWindow(this);
            ventanaRaiz.CambiarPantalla(new AdministracionUC());
        }

        private void BtnInformes_Click(object sender, RoutedEventArgs e)
        {
            MainWindow ventanaRaiz = (MainWindow)Window.GetWindow(this);
            ventanaRaiz.CambiarPantalla(new InformesUC());
        }

        private void BtnProximamente_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Este módulo se construirá más adelante.", "En Construcción", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnClientes_Click(object sender, RoutedEventArgs e)
        {
            MainWindow ventanaRaiz = (MainWindow)Window.GetWindow(this);
            ventanaRaiz.CambiarPantalla(new MaestroClientesUC());
        }
    }
}