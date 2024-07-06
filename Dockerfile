FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

COPY ./notification.api/*.csproj ./notification.api/
COPY ./notification.bll/*.csproj ./notification.bll/
RUN dotnet restore ./notification.api/notification.api.csproj

COPY . .
RUN dotnet publish ./notification.api/notification.api.csproj -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Expose ports http & https
EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "notification.api.dll"]