# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS vsdebug
USER $APP_UID
WORKDIR /app
EXPOSE 80
EXPOSE 443

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /src

COPY ["src/Mav.EventListener/Mav.EventListener.csproj", "Mav.EventListener/"]
COPY ["src/Mav.Infrastructure/Mav.Infrastructure.csproj", "Mav.Infrastructure/"]
COPY ["src/Mav.Domain/Mav.Domain.csproj", "Mav.Domain/"]

RUN dotnet restore "Mav.EventListener/Mav.EventListener.csproj" -r linux-x64 -v n --configfile nuget.config
RUN dotnet restore "Mav.Infrastructure/Mav.Infrastructure.csproj" -r linux-x64 -v n --configfile nuget.config
RUN dotnet restore "Mav.Domain/Mav.Domain.csproj" -r linux-x64 -v n --configfile nuget.config

RUN rm ./nuget.config
COPY ["src/", "."]

WORKDIR "Mav.EventListener"
RUN dotnet oublish "Mav.EventListener.csproj" -v n -c Release -o /app -r linux-x64 --no-restore /p:Version=${VERSION}

WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["./Mav.EventListener"]