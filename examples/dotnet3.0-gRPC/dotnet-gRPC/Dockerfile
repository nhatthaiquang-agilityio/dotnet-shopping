FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build
WORKDIR /app
EXPOSE 5000

# copy .NET standard
COPY ./service-proto ./service-proto

COPY ./dotnet-gRPC/ ./dotnet-gRPC
WORKDIR /app/dotnet-gRPC
RUN dotnet publish dotnet-gRPC.csproj -c Release -o out


FROM mcr.microsoft.com/dotnet/core/aspnet:3.0 AS runtime
WORKDIR /app
COPY --from=build /app/dotnet-gRPC/out ./
ENTRYPOINT ["dotnet", "dotnet-gRPC.dll"]