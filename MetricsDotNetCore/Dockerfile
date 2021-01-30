FROM mcr.microsoft.com/dotnet/sdk:5.0
COPY bin/Release/net5.0/publish/ App/
EXPOSE 5000
WORKDIR /App
ENTRYPOINT ["dotnet", "MetricsDotNetCore.dll"]