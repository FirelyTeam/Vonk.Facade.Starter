Param(
  [string]$configuration = "Debug",
  [string]$publicationDir = ""
)

Push-Location $PSScriptRoot\..\

#Remove binaries
Get-ChildItem * -Include obj -Recurse | Remove-Item -Recurse
Get-ChildItem * -Include bin -Recurse | Remove-Item -Recurse

#Restore packages
dotnet restore .\Vonk.Facade.Starter.sln

dotnet build .\Vonk.Facade.Starter.sln --configuration $configuration

If ($publicationDir -ne "")
{
    #remove the old publication directory
    If(Test-path "$publicationDir") {Remove-item "$publicationDir" -Recurse -Force}

    #publish to the publication directory
    dotnet publish .\Vonk.Facade.Starter\Vonk.Facade.Starter.csproj -c $configuration
}
Else #publish to the default outputdir, bin\<configuration>\<platform>\publish
{
    dotnet publish .\Vonk.Facade.Starter\Vonk.Facade.Starter.csproj -c $configuration 
}

Pop-Location
