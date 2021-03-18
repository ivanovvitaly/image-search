FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY . .
ARG TEAMCITY_VERSION
ENV TEAMCITY_VERSION=$TEAMCITY_VERSION
RUN dotnet test "AE.Tests/AE.Tests.csproj"
RUN dotnet publish "AE.WebApi/AE.WebApi.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "AE.WebApi.dll"]