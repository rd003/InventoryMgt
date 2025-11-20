FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.sln .
COPY InventoryMgt.Api/*.csproj ./InventoryMgt.Api/
COPY InventoryMgt.Data/*.csproj ./InventoryMgt.Data/
COPY InventoryMgt.Shared/*.csproj ./InventoryMgt.Shared/
RUN dotnet restore

# copy everything else and build app
COPY InventoryMgt.Api/. ./InventoryMgt.Api/
COPY InventoryMgt.Data/. ./InventoryMgt.Data/
COPY InventoryMgt.Shared/. ./InventoryMgt.Shared/

WORKDIR /source/InventoryMgt.Api
RUN dotnet publish -c release -o /app

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app ./

EXPOSE 8081

ENTRYPOINT ["dotnet", "InventoryMgt.Api.dll"]
