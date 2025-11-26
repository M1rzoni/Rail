
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["BarcodeScannerAPI.csproj", "."]
RUN dotnet restore "BarcodeScannerAPI.csproj"

COPY . .
WORKDIR "/src"
RUN dotnet build "BarcodeScannerAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BarcodeScannerAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://0.0.0.0:5000
ENV PORT=5000
EXPOSE 5000

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BarcodeScannerAPI.dll"]