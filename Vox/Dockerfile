FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env

# Copy everything else and build
COPY Vox ./Vox
COPY Vox.Data ./Vox.Data
COPY Vox.Services ./Vox.Services

RUN dotnet restore Vox/Vox.csproj
RUN dotnet publish Vox/*.csproj -c Release -o out

# Build runtime image
WORKDIR /
FROM mcr.microsoft.com/dotnet/aspnet:6.0
COPY --from=build-env ./out .
ENTRYPOINT ["dotnet", "Vox.dll"]