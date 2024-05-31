#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SensorSimulator/SensorSimulator.csproj", "SensorSimulator/"]
RUN dotnet restore "./SensorSimulator/SensorSimulator.csproj"
COPY . .
WORKDIR "/src/SensorSimulator"
RUN dotnet build "./SensorSimulator.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SensorSimulator.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# ENV MQTT_BROKER_URL=c93e99eaf93b4f81aa4db3ba6fee2780.s1.eu.hivemq.cloud
# ENV MQTT_BROKER_PORT=8883
# ENV MQTT_TOPIC=room/simulator2
# ENV MQTT_BROKER_USERNAME=sensor-simulator-2
# ENV MQTT_BROKER_PASSWORD=SensorSim123

ENTRYPOINT ["dotnet", "SensorSimulator.dll"]