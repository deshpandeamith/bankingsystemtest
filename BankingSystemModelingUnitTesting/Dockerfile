#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["../BankingSystemModelingUnitTesting/BankingSystemModelingUnitTesting.csproj", "../BankingSystemModelingUnitTesting/"]
COPY ["BankingSystemModeling/BankingSystemModeling.csproj", "BankingSystemModeling/"]
RUN dotnet restore "../BankingSystemModelingUnitTesting/BankingSystemModelingUnitTesting.csproj"
COPY . .
WORKDIR "/src/../BankingSystemModelingUnitTesting"
RUN dotnet build "BankingSystemModelingUnitTesting.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BankingSystemModelingUnitTesting.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BankingSystemModelingUnitTesting.dll"]



FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
#COPY BankingSystemModeling.csproj .
#COPY "BankingSystemModeling/BankingSystemModeling.csproj" "BankingSystemModeling/"
COPY "BankingSystemModeling/BankingSystemModeling.csproj" "BankingSystemModeling/"
COPY "BankingSystemModelingUnitTesting/BankingSystemModelingUnitTesting.csproj" "BankingSystemModelingUnitTesting/"
RUN dotnet restore "BankingSystemModeling/BankingSystemModeling.csproj"
COPY . .

FROM build AS test
WORKDIR /src
LABEL test=true
RUN dotnet test -c Release --results-directory /testresults --logger "trx;LogFileName=test_results.trx" BankingSystemModelingUnitTesting/BankingSystemModelingUnitTesting.csproj


FROM build AS publish
RUN dotnet publish "BankingSystemModeling/BankingSystemModeling.csproj" --no-restore -c release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
EXPOSE 80
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BankingSystemModeling.dll"]


