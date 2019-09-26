FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY dotnet-express-mapper.csproj ./dotnet-express-mapper/
RUN dotnet restore ./dotnet-express-mapper/dotnet-express-mapper.csproj

# copy everything else and build app
COPY . ./dotnet-express-mapper/
WORKDIR /app/dotnet-express-mapper
RUN dotnet publish dotnet-express-mapper.csproj -c Release -o out


FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS runtime
WORKDIR /app
COPY --from=build /app/dotnet-express-mapper/out ./
ENTRYPOINT ["dotnet", "dotnet-express-mapper.dll"]