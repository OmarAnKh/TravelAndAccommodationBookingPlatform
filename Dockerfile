# ========================
# STAGE 1: BUILD
# ========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy everything
COPY . .

# Restore and publish only the API project
WORKDIR /app/TravelAndAccommodationBookingPlatform.API
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

# ========================
# STAGE 2: RUNTIME
# ========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:80

EXPOSE 80

ENTRYPOINT ["dotnet", "TravelAndAccommodationBookingPlatform.API.dll"]
