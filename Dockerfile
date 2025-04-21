# Use the official image as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

# Use SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["NextUp/NextUp.csproj", "NextUp/"]
RUN dotnet restore "NextUp/NextUp.csproj"
COPY . .
WORKDIR "/src/NextUp"
RUN dotnet build "NextUp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NextUp.csproj" -c Release -o /app/publish

# Copy build artifacts to the base image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NextUp.dll"]