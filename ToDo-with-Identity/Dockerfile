FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ToDo-with-Identity/ToDo-with-Identity.csproj", "ToDo-with-Identity/"]
RUN dotnet restore "ToDo-with-Identity/ToDo-with-Identity.csproj"
COPY . .
WORKDIR "/src/ToDo-with-Identity"
RUN dotnet build "ToDo-with-Identity.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ToDo-with-Identity.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ToDo-with-Identity.dll"]
