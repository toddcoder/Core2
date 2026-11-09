$target = ".\Assemblies"

robocopy ".\Source\Core\bin\Debug\net10.0-windows7.0" $target Core.dll /xf *.xml *.config *.pdb /NDL /NJH /NJS /nc /ns
robocopy ".\Source\Core.Internet\bin\Debug\net10.0-windows7.0" $target Core.Internet.dll /xf *.xml *.config *.pdb /NDL /NJH /NJS /nc /ns
robocopy ".\Source\Core.Data\bin\Debug\net10.0-windows7.0" $target Core.Data.dll /xf *.xml *.config *.pdb /NDL /NJH /NJS /nc /ns
robocopy ".\Source\Core.WinForms\bin\Debug\net10.0-windows7.0" $target Core.WinForms.dll /xf *.xml *.config *.pdb /NDL /NJH /NJS /nc /ns
robocopy ".\Source\Core.Markup\bin\Debug\net10.0-windows7.0" $target Core.Markup.dll /xf *.xml *.config *.pdb /NDL /NJH /NJS /nc /ns
robocopy ".\Source\Core.Json\bin\Debug\net10.0-windows7.0" $target Core.Json.dll Newtonsoft.Json.dll /xf *.xml *.config *.pdb /NDL /NJH /NJS /nc /ns
robocopy ".\Source\Core.Services\bin\Debug\net10.0-windows7.0" $target Core.Services.dll /xf *.xml *.config *.pdb /NDL /NJH /NJS /nc /ns
robocopy ".\Source\Core.Zip\bin\Debug\net10.0-windows7.0" $target Core.Zip.dll /xf *.xml *.config *.pdb /NDL /NJH /NJS /nc /ns
robocopy ".\Source\Core.Io\bin\Debug\net10.0-windows7.0" $target Core.Io.dll /xf *.xml *.config *.pdb /NDL /NJH /NJS /nc /ns
robocopy ".\Source\Core.Markdown\bin\Debug\net10.0-windows7.0" $target Core.Markdown.dll /xf *.xml *.config *.pdb /NDL /NJH /NJS /nc /ns
