#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.


FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY "BankingSystemModeling/BankingSystemModeling.csproj" "BankingSystemModeling/"
COPY "BankingSystemModelingUnitTesting/BankingSystemModelingUnitTesting.csproj" "BankingSystemModelingUnitTesting/"
RUN dotnet restore "BankingSystemModeling/BankingSystemModeling.csproj"
COPY . .

FROM build AS test
WORKDIR /src
LABEL test=true
RUN dotnet test -c Release --results-directory . --logger "trx;LogFileName=test_results.trx" BankingSystemModelingUnitTesting/BankingSystemModelingUnitTesting.csproj


FROM build AS publish
RUN dotnet publish "BankingSystemModeling/BankingSystemModeling.csproj" --no-restore -c release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
EXPOSE 80
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BankingSystemModeling.dll"]


