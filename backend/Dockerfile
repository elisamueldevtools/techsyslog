# ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY global.json ./
COPY src/TechsysLog.Domain/TechsysLog.Domain.csproj         src/TechsysLog.Domain/
COPY src/TechsysLog.Application/TechsysLog.Application.csproj src/TechsysLog.Application/
COPY src/TechsysLog.Infrastructure/TechsysLog.Infrastructure.csproj src/TechsysLog.Infrastructure/
COPY src/TechsysLog.API/TechsysLog.API.csproj             src/TechsysLog.API/

RUN dotnet restore src/TechsysLog.API/TechsysLog.API.csproj

COPY src/ src/
RUN dotnet publish src/TechsysLog.API/TechsysLog.API.csproj -c Release -o /app/publish /p:UseAppHost=false

# ---- Runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development
EXPOSE 8080

ENTRYPOINT ["dotnet", "TechsysLog.API.dll"]
