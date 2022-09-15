rm -r src/Utilities/nupkg
dotnet pack
dotnet tool update --global --configfile ./disable_nuget.config --add-source ./src/Utilities/nupkg/ Utilities
