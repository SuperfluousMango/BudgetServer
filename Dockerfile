FROM mcr.microsoft.com/dotnet/sdk:6.0

WORKDIR /publish

COPY ./BudgetServer/bin/Release/net6.0 .

ENV ASPNETCORE_URLS="http://0.0.0.0:5501"

ENTRYPOINT ["dotnet", "BudgetServer.dll"]
