# FotoFlow

Pequeña aplicación de escritorio (Windows Forms) para visualizar y gestionar fotografías.

## Estado actual
- Funcionalidad mínima: abrir imágenes y navegar entre ellas desde la interfaz principal.

## Requisitos
- .NET 8 SDK
- Visual Studio 2022 (o uso de la CLI de .NET)

## Inicio rápido
1. Clona el repositorio y cambia a la rama `development`:
   ```bash
   git clone <repositorio>
   cd FotoFlow
   git checkout development
   ```

2. Abre la solución en Visual Studio 2022 y ejecuta (F5) o usa la CLI desde la carpeta de la solución:
   ```bash
   dotnet build
   dotnet run --project <ruta-al-proyecto-csproj>
   ```

## Archivos importantes
- `FrmFotoFlow.cs`, `FrmFotoFlow.Designer.cs`, `FrmFotoFlow.resx` — formulario principal y recursos.
- `README.md` — este archivo.

## Contribuir
- Crear issues para errores o propuestas de mejora.
- Enviar Pull Requests a la rama `development`.

## Licencia
- MIT (si quieres otro licencia, reemplaza esta sección).
