$target = ".\Assemblies"

robocopy ".\Source\Core\bin\Debug\net10.0-windows7.0" "$target" Core.dll 
robocopy ".\Source\Core.Internet\bin\Debug\net10.0-windows7.0" "$target" Core.Internet.dll
robocopy ".\Source\Core.Data\bin\Debug\net10.0-windows7.0" "$target" Core.Data.dll
robocopy ".\Source\Core.WinForms\bin\Debug\net10.0-windows7.0" "$target" Core.WinForms.dll
robocopy ".\Source\Core.Markup\bin\Debug\net10.0-windows7.0" "$target" Core.Markup.dll
robocopy ".\Source\Core.Json\bin\Debug\net10.0-windows7.0" "$target" Core.Json.dll
robocopy ".\Source\Core.Services\bin\Debug\net10.0-windows7.0" "$target" Core.Services.dll
robocopy ".\Source\Core.Zip\bin\Debug\net10.0-windows7.0" "$target" Core.Zip.dll
robocopy ".\Source\Core.Io\bin\Debug\net10.0-windows7.0" "$target" Core.Io.dll
