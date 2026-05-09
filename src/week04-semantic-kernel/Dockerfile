# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY ["Week04.csproj", "."]
RUN dotnet restore "Week04.csproj"

# Copy everything else and build
COPY . .
RUN dotnet build "Week04.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "Week04.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# The app is interactive, so we need a TTY if running via docker run -it
# For docker-compose, it will just start and wait for input, then exit or hang
# We'll set it to run the integration test if --test is passed, or just stay alive
ENTRYPOINT ["dotnet", "Week04.dll"]
