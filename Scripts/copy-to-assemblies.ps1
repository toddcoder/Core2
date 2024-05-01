Push-Location "C:\Users\tebennett\source\repos\Core2\Source"

$target = "C:\Users\tebennett\source\repos\Core2\Assemblies"

robocopy "Core\bin\Debug\net6.0-windows" $target Core.dll /xf *.xml *.config *.pdb /NDL /NJH /NJS /nc /ns
robocopy "Core.Internet\bin\Debug\net6.0-windows" $target Core.Internet.dll /xf *.xml *.config *.pdb /NDL /NJH /NJS /nc /ns
robocopy "Core.Data\bin\Debug\net6.0-windows" $target Core.Data.dll /xf *.xml *.config *.pdb /NDL /NJH /NJS /nc /ns
robocopy "Core.WinForms\bin\Debug\net6.0-windows" $target Core.WinForms.dll /xf *.xml *.config *.pdb /NDL /NJH /NJS /nc /ns
robocopy "Core.Markup\bin\Debug\net6.0-windows" $target Core.Markup.dll /xf *.xml *.config *.pdb /NDL /NJH /NJS /nc /ns
robocopy "Core.Json\bin\Debug\net6.0-windows" $target Core.Json.dll Newtonsoft.Json.dll /xf *.xml *.config *.pdb /NDL /NJH /NJS /nc /ns
robocopy "Core.Services\bin\Debug\net6.0-windows" $target Core.Services.dll /xf *.xml *.config *.pdb /NDL /NJH /NJS /nc /ns
robocopy "Core.Zip\bin\Debug\net6.0-windows" $target Core.Zip.dll /xf *.xml *.config *.pdb /NDL /NJH /NJS /nc /ns
robocopy "Core.Io\bin\Debug\net6.0-windows" $target Core.Io.dll /xf *.xml *.config *.pdb /NDL /NJH /NJS /nc /ns
Pop-Location