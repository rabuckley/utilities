# Custom Utilities

To install run:

```sh
dotnet tool install --global --configfile ./disable_nuget.config --add-source ./src/Utilities/nupkg/ Utilities
```

To update the tool run:

```sh
dotnet tool update --global --configfile ./disable_nuget.config --add-source ./src/Utilities/nupkg/ Utilities
```

Includes:

- File renamer, roughly to kebab-case.
- File extractor - moves all files in subdirectories into a parent folder.
