# Multi-Stage Dockerfile for SpacePulse .NET 9 Web API
# Stage 1: Build & Publish
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY RentalPeAPI/RentalPeAPI.csproj RentalPeAPI/
RUN dotnet restore "RentalPeAPI/RentalPeAPI.csproj"
COPY . .
WORKDIR "/src/RentalPeAPI"
RUN dotnet build "RentalPeAPI.csproj" -c Release -o /app/build
RUN dotnet publish "RentalPeAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Production Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
EXPOSE 5000
ENV ASPNETCORE_HTTP_PORTS=5000
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "RentalPeAPI.dll"]
