# Pager Duty errors

This tool aims to calculate weekly error statistic for Team Raven.


## Instructions

To install

1. Pack the nupkg package by running.

```powershell
dotnet pack
```
2. Install the tool globaly by running the command below.

```ps1
dotnet tool install --global --add-source ./nupkg pg-errors
```

3. Use the tool 

```ps1
pg-errors path/to/your/pager/duty/file
```

## Un-install
If you want to uninstall, run the follwoing.

```ps1
dotnet tool uninstall --global pg-errors
```