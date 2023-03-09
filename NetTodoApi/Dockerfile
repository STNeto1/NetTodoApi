FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["NetTodoApi/NetTodoApi.csproj", "NetTodoApi/"]
RUN dotnet restore "NetTodoApi/NetTodoApi.csproj"
COPY . .
WORKDIR "/src/NetTodoApi"
RUN dotnet build "NetTodoApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NetTodoApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NetTodoApi.dll"]
