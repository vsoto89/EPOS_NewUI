using System;
using System.Windows;
using System.Windows.Controls;

namespace EPOS_NewUI.Views
{
    public partial class LoginUC : UserControl
    {
        public LoginUC()
        {
            InitializeComponent();
            txtPassword.Focus();
        }

        private void btnEntrar_Click(object sender, RoutedEventArgs e)
        {
            string perfilSeleccionado = (cbUsuario.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Administrador";
            string clave = txtPassword.Password ?? string.Empty;

            if (perfilSeleccionado == "Administrador" && clave == "admin123")
            {
                AbrirMenu("Administrador");
            }
            else if (perfilSeleccionado == "Vendedor" && clave == "vendedor123")
            {
                AbrirMenu("Vendedor");
            }
            else
            {
                MessageBox.Show("Credenciales incorrectas. Intente nuevamente.", "Error de Acceso", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPassword.Clear();
                txtPassword.Focus();
            }
        }

        private void AbrirMenu(string rol)
        {
            MainWindow ventanaRaiz = (MainWindow)Window.GetWindow(this);
            ventanaRaiz.UsuarioActual = rol;
            ventanaRaiz.CambiarPantalla(new MenuUC(rol));
        }

        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            // Cierra la aplicación completa
            Application.Current.Shutdown();
        }
    }
}