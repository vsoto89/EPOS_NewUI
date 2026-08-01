using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace EPOS_NewUI.Views
{
    public class ProductoCatalogService
    {
        private static readonly ProductoCatalogService _instancia = new ProductoCatalogService();

        private readonly ObservableCollection<ProductoModel> _productos = new ObservableCollection<ProductoModel>();

        private ProductoCatalogService()
        {
            CargarProductosIniciales();
        }

        public static ProductoCatalogService Instancia => _instancia;

        public ObservableCollection<ProductoModel> Productos => _productos;

        public void CargarProductosIniciales()
        {
            if (_productos.Any())
            {
                return;
            }

            _productos.Add(new ProductoModel { Id = 1, Categoria = "Bebidas", Nombre = "Café Americano", Precio = 3000, Stock = 20, Activo = true });
            _productos.Add(new ProductoModel { Id = 2, Categoria = "Bebidas", Nombre = "Capuccino", Precio = 4000, Stock = 15, Activo = true });
            _productos.Add(new ProductoModel { Id = 3, Categoria = "Bebidas", Nombre = "Jugo Natural", Precio = 3000, Stock = 12, Activo = true });
            _productos.Add(new ProductoModel { Id = 4, Categoria = "Comidas", Nombre = "Sandwich Club", Precio = 4000, Stock = 10, Activo = true });
            _productos.Add(new ProductoModel { Id = 5, Categoria = "Comidas", Nombre = "Bagel de Jamón", Precio = 6000, Stock = 8, Activo = true });
            _productos.Add(new ProductoModel { Id = 6, Categoria = "Comidas", Nombre = "Ensalada César", Precio = 6000, Stock = 7, Activo = true });
            _productos.Add(new ProductoModel { Id = 7, Categoria = "Postres", Nombre = "Cheesecake", Precio = 5000, Stock = 6, Activo = true });
        }

        public void AgregarOActualizar(ProductoModel producto)
        {
            if (producto == null)
            {
                throw new ArgumentNullException(nameof(producto));
            }

            var existente = _productos.FirstOrDefault(p => p.Id == producto.Id);
            if (existente != null)
            {
                existente.Categoria = producto.Categoria;
                existente.Nombre = producto.Nombre;
                existente.Precio = producto.Precio;
                existente.Stock = producto.Stock;
                existente.Activo = producto.Activo;
            }
            else
            {
                producto.Id = _productos.Any() ? _productos.Max(p => p.Id) + 1 : 1;
                _productos.Add(producto);
            }
        }

        public void Eliminar(ProductoModel producto)
        {
            if (producto == null)
            {
                return;
            }

            var existente = _productos.FirstOrDefault(p => p.Id == producto.Id);
            if (existente != null)
            {
                _productos.Remove(existente);
            }
        }

        public void ReducirStock(int productoId)
        {
            var producto = _productos.FirstOrDefault(p => p.Id == productoId);
            if (producto != null && producto.Stock > 0)
            {
                producto.Stock--;
            }
        }
    }
}
