FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/MSG.BlogApi.Domain/MSG.BlogApi.Domain.csproj src/MSG.BlogApi.Domain/
COPY src/MSG.BlogApi.Application/MSG.BlogApi.Application.csproj src/MSG.BlogApi.Application/
COPY src/MSG.BlogApi.Infrastructure/MSG.BlogApi.Infrastructure.csproj src/MSG.BlogApi.Infrastructure/
COPY src/MSG.BlogApi.Api/MSG.BlogApi.Api.csproj src/MSG.BlogApi.Api/
RUN dotnet restore src/MSG.BlogApi.Api/MSG.BlogApi.Api.csproj

COPY src/ src/
RUN dotnet publish src/MSG.BlogApi.Api/MSG.BlogApi.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
RUN mkdir logs && chown $APP_UID logs
USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "MSG.BlogApi.Api.dll"]
