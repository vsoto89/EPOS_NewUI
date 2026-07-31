# EPOS_NewUI

Proyecto WPF .NET para la nueva aplicación POS de Windows.

## Ruta del proyecto
`E:\OneDrive - Corporación Santo Tomas\Santo Tomas 4to año\Segundo semestre\Practica\Semana 1\backup\EPOS_NewUI\EPOS_NewUI`

## Qué contiene
- `EPOS_NewUI.csproj`: proyecto WPF .NET
- `App.xaml` / `App.xaml.cs`: arranque de la aplicación
- `MainWindow.xaml` / `MainWindow.xaml.cs`: ventana principal inicial

## Requisitos
- .NET SDK 9.0 está instalado en este equipo.
- VS Code puede usarse para editar el proyecto.
- Se recomienda instalar la extensión de C# de Microsoft en VS Code para mejor experiencia.

## Cómo abrirlo en VS Code
1. Abre VS Code.
2. Selecciona `File > Open Folder...`.
3. Elige la carpeta:
   `E:\OneDrive - Corporación Santo Tomas\Santo Tomas 4to año\Segundo semestre\Practica\Semana 1\backup\EPOS_NewUI\EPOS_NewUI`

## Comandos útiles
- Restaurar dependencias y compilar:
  ```powershell
  dotnet build
  ```
- Ejecutar la aplicación:
  ```powershell
  dotnet run
  ```

## Siguiente paso sugerido
- Modificar `MainWindow.xaml` para diseñar la primera pantalla del POS.
- Crear un modelo de datos simple para `Vendedor`, `Producto` y `Ticket`.
- Agregar una capa de datos en memoria para simular la base antigua mientras definimos la conexión real.
