#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
EXPOSE 80
EXPOSE 443 

# Install SSL/TLS certificates
#RUN apt-get update && \
#    apt-get install -y ca-certificates && \
#    update-ca-certificates

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["RoomSense-Backend.csproj", "."]
RUN dotnet restore "./RoomSense-Backend.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./RoomSense-Backend.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./RoomSense-Backend.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Copy SSL/TLS certificate files
##COPY roomsense.crt /app/roomsense.crt
##COPY roomsense.key /app/roomsense.key

ENTRYPOINT ["dotnet", "RoomSense-Backend.dll"]