FROM mcr.microsoft.com/dotnet/core/sdk:3.1
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY . .

WORKDIR "/src/MerchantIntegration.Api"
RUN ["rm", "-rf", "obj"]
RUN ["rm", "-rf", "bin"]
RUN ["dotnet", "restore"]
RUN ["dotnet", "publish", "MerchantIntegration.Api.csproj", "-c", "Release", "-o", "out"]
ENTRYPOINT ["dotnet", "out/MerchantIntegration.Api.dll"]

