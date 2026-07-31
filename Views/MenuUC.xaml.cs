using System.Windows;
using System.Windows.Controls;

namespace EPOS_NewUI.Views
{
    public partial class MenuUC : UserControl
    {
        public MenuUC()
        {
            InitializeComponent();
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

        // Mensaje temporal para las vistas que harás más adelante
        private void BtnProximamente_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Este módulo se construirá más adelante.", "En Construcción", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}