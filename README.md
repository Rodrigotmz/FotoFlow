# FotoFlow

FotoFlow es una aplicacion de escritorio para Windows que recibe fotografias desde un dispositivo Android mediante ADB y las guarda en una carpeta local.

## Funcionalidades

- Deteccion periodica de nuevas imagenes en `/sdcard/DCIM/Camera`.
- Transferencia de fotografias al equipo usando `adb pull`.
- Filtro de formatos de imagen comunes: JPG, PNG, GIF, BMP, TIFF, WEBP, HEIC, HEIF y DNG.
- Modo Basico para recibir y guardar archivos directamente.
- Modo Advance para previsualizar, renombrar y guardar fotografias manualmente.
- Persistencia del ultimo modo usado y de la ultima ruta de guardado.
- Proyecto de instalacion incluido en `SetupFotoFlow/`.

## Requisitos

- Windows.
- Visual Studio 2022.
- .NET 8 SDK.
- Un dispositivo Android con depuracion USB habilitada.
- ADB disponible en la carpeta `adb/` del proyecto o en la salida compilada.

## Ejecucion

Desde Visual Studio:

1. Abre `FotoFlow.sln`.
2. Selecciona el proyecto `FotoFlow` como proyecto de inicio.
3. Compila y ejecuta con `F5`.

Desde terminal:

```bash
dotnet build FotoFlow.sln
dotnet run --project FotoFlow.csproj
```

## Uso rapido

1. Conecta el dispositivo Android por USB.
2. Acepta la depuracion USB en el dispositivo.
3. Selecciona la carpeta destino en FotoFlow.
4. Inicia la transferencia.
5. En modo Basico las fotos se guardan automaticamente; en modo Advance puedes revisar, renombrar y guardar cada archivo.

## Estructura principal

- `Program.cs`: punto de entrada y seleccion del ultimo modo usado.
- `FrmFotoFlow.cs`: formulario del modo Basico.
- `FrmFotoFlowAdvance.cs`: formulario del modo Advance.
- `Core/FotoFlowService.cs`: servicio de monitoreo y transferencia con ADB.
- `Core/AdbRunner.cs`: ejecucion de comandos ADB.
- `Core/AppUserSettings.cs`: persistencia de configuracion del usuario.
- `SetupFotoFlow/`: proyecto de instalacion.

## Notas

- FotoFlow solo procesa archivos con extension de imagen permitida e ignora carpetas o entradas de cache.
- Si ADB no existe en la carpeta esperada, la aplicacion muestra un error al iniciar la transferencia.
- El modo Advance usa una carpeta temporal antes del guardado final.
