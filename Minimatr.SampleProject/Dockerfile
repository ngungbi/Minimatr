FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Minimatr.SampleProject/Minimatr.SampleProject.csproj", "Minimatr.SampleProject/"]
RUN dotnet restore "Minimatr.SampleProject/Minimatr.SampleProject.csproj"
COPY . .
WORKDIR "/src/Minimatr.SampleProject"
RUN dotnet build "Minimatr.SampleProject.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Minimatr.SampleProject.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Minimatr.SampleProject.dll"]
