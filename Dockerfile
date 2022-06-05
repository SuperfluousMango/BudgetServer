FROM mcr.microsoft.com/dotnet/sdk:6.0

WORKDIR /publish

COPY ./BudgetServer/bin/Release/net6.0 .

ENTRYPOINT ["dotnet", "BudgetServer.dll"]
