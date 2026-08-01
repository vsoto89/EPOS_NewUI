using System.Windows;
using System.Windows.Controls;

namespace EPOS_NewUI.Views
{
    public partial class MaestroClientesUC : UserControl
    {
        public MaestroClientesUC()
        {
            InitializeComponent();
        }

        // 1. VOLVER AL MENÚ
        private void BtnVolver_Click(object sender, RoutedEventArgs e)
        {
            MainWindow ventanaRaiz = (MainWindow)Window.GetWindow(this);
            ventanaRaiz.CambiarPantalla(new MenuUC());
        }

        // 2. LIMPIAR TODOS LOS CAMPOS
        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            txtRut.Clear();
            txtNombre.Clear();
            txtGiro.Clear();
            txtEmail.Clear();
            txtTelefono.Clear();
            txtDireccion.Clear();
            txtComuna.Clear();
            txtCiudad.Clear();
            
            txtRut.Focus(); // Pone el cursor en el RUT para empezar de nuevo
        }

        // 3. GUARDAR (Simulación visual por ahora)
        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtRut.Text) || string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("Por favor, ingrese al menos el RUT y el Nombre del cliente.", "Campos incompletos", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Aquí a futuro irá la conexión a SQL Server ("INSERT INTO Clientes...")
            
            MessageBox.Show("Cliente guardado correctamente en el sistema.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            BtnLimpiar_Click(null, null); // Limpiamos después de guardar
        }

        // 4. ELIMINAR (Simulación)
        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtRut.Text))
            {
                MessageBox.Show("Ingrese el RUT del cliente que desea eliminar.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBoxResult confirmacion = MessageBox.Show($"¿Está seguro que desea eliminar al cliente RUT: {txtRut.Text}?", "Confirmar Eliminación", MessageBoxButton.YesNo, MessageBoxImage.Question);
            
            if (confirmacion == MessageBoxResult.Yes)
            {
                // Aquí a futuro irá el código SQL ("DELETE FROM Clientes WHERE Rut = ...")
                MessageBox.Show("Cliente eliminado.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                BtnLimpiar_Click(null, null);
            }
        }
    }
}