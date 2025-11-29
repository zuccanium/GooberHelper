# feel free to change
$CompilerPath = "D:\Other Crap\Internet Downloads\CelesteShaderCompiler1.0.0\CelesteShaderCompiler.exe"

Set-Location ./Effects/

Get-ChildItem -Recurse -Directory | ForEach-Object {    
    Push-Location $_.FullName

    Start-Process -FilePath $CompilerPath -NoNewWindow

    Pop-Location
}