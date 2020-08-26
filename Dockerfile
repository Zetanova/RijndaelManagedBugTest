FROM mcr.microsoft.com/dotnet/core/sdk:3.1 
WORKDIR /src
COPY . .
RUN dotnet restore RijndaelManagedBugTest.sln --verbosity minimal
RUN dotnet build RijndaelManagedBugTest.sln --no-restore -c Release 

CMD dotnet test RijndaelManagedBugTest.sln --no-build -c Release --logger:"trx" 