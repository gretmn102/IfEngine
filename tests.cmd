cls

dotnet tool restore
dotnet paket install
cd tests
dotnet run -c Release
