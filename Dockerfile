FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS builder
WORKDIR /src
COPY . .

WORKDIR "/src/MerchantIntegration.Api"
RUN ["rm", "-rf", "obj"]
RUN ["rm", "-rf", "bin"]
RUN ["dotnet", "restore"]
RUN ["dotnet", "publish", "MerchantIntegration.Api.csproj", "-c", "Release", "-o", "/out"]

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /out
COPY --from=builder /out .
ENTRYPOINT ["dotnet", "MerchantIntegration.Api.dll"]

