# Use the official .NET 8 SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the files and publish the app
COPY . ./
RUN dotnet publish -c Release -o out

# Use the official ASP.NET Core runtime image for the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy the published output from build stage
COPY --from=build /app/out .

# Start the app
ENTRYPOINT ["dotnet", "BrownieBackend.dll"]
