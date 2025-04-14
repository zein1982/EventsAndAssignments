#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
#Переменные окружения, которые должны прокидываться в контейнер
#ENV DATABASE_HOST RUK-SQL-XD067\\PRJ10252_DEV
#ENV DATABASE_NAME EventsAndAssignments
#ENV DATABASE_MIGRATE_USERNAME SIB\10252.migrate.dev
#ENV DATEBASE_MIGRATE_PASSWORD password
#ENV DATABASE_BACKEND_USERNAME SIB\10252.backend.dev
#ENV DATEBASE_BACKEND_PASSWORD password
#   DATABASE_BACKEND_PASSWORD
#ENV DATABASE_EMAIL_USERNAME SIB\10252.mail



FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["EventsAndAssignments/EventsAndAssignments.csproj", "EventsAndAssignments/"]
RUN dotnet restore "EventsAndAssignments/EventsAndAssignments.csproj"
COPY . .
WORKDIR "/src/EventsAndAssignments"
RUN dotnet build "EventsAndAssignments.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EventsAndAssignments.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EventsAndAssignments.dll"]