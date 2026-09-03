FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["BackEnd/BackEnd/BackEnd.csproj", "BackEnd/BackEnd/"]
RUN dotnet restore "BackEnd/BackEnd/BackEnd.csproj"
COPY . .
WORKDIR "/src/BackEnd/BackEnd"
RUN dotnet publish "BackEnd.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "BackEnd.dll"]
