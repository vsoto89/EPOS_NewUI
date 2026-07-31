using System.Windows;
using System.Windows.Controls;
using EPOS_NewUI.Views;

namespace EPOS_NewUI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Arrancamos la aplicación cargando la pantalla de Login.
            // (Si te marca error es porque aún no creas el archivo LoginUC)
            CambiarPantalla(new LoginUC()); 
        }

        /// <summary>
        /// Método público para cambiar el contenido de la ventana.
        /// </summary>
        /// <param name="nuevaPantalla">El UserControl que se desea mostrar</param>
        public void CambiarPantalla(UserControl nuevaPantalla)
        {
            ContenedorPrincipal.Content = nuevaPantalla;
        }
    }
}