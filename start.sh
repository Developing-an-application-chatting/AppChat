#!/bin/bash
# start.sh cho Railway .NET 9

cd AppChat
dotnet restore
dotnet build --configuration Release
dotnet run --urls "http://0.0.0.0:$PORT"