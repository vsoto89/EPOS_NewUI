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
            
            // Cargar automáticamente la fecha de hoy en el selector
            dpFechaSistema.SelectedDate = DateTime.Now;
            
            // Poner el cursor directamente en la contraseña al abrir
            txtPassword.Focus(); 
        }

        private void btnEntrar_Click(object sender, RoutedEventArgs e)
        {
            // OBTENER LA CONTRASEÑA ESCRITA
            string clave = txtPassword.Password;

            // VALIDACIÓN DE PRUEBA (puedes cambiar "1234" por la clave real)
            if (clave == "1234")
            {
                // Buscamos la ventana principal contenedora
                MainWindow ventanaRaiz = (MainWindow)Window.GetWindow(this);

                // Aquí deberías pasar a MenuUC, pero para probar directamente tu avance, 
                // pasaremos a PosPremiumUC (Asegúrate de que este archivo exista en tu carpeta Views)
                ventanaRaiz.CambiarPantalla(new MenuUC());
            }
            else
            {
                MessageBox.Show("Contraseña incorrecta. Intente nuevamente.", "Error de Acceso", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPassword.Clear();
                txtPassword.Focus();
            }
        }

        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            // Cierra la aplicación completa
            Application.Current.Shutdown();
        }
    }
}