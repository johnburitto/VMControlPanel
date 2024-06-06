# Система віддаленої взаємодії з вірутальними машинами
- [Постановка задачі](#постановка-задачі)
  - [Актуальність](#актуальність)
  - [Мета](#мета)
  - [Завдання](#завдання)
  - [Об'єкт дослідження](#обєкт-дослідження)
- [Технології](#технології)
- [Архітектура](#архітектура)
- [Реалізація](#реалізація)
- [Розгортання](#розгортання)

## Постановка задачі
### Актуальність
Багато девопсів стикаються з тим, що проекти, над якими вони працюють, потребують прямого втручання, проте люди не можуть постійно перебувати на робочому місці. Багатьом із них стало б пригоді рішення, яке надавало б можливість взаємодіяти зі своїми машинами віддалено.
### Мета
Метою розроблюваного застосунку є надання можливості девопсам віддалено, з будь-якої точки світу, виправляти проблеми, що виникають або просто слідкувати за станом їх віртуальних машин.
### Завдання
- [x] Розробити можливість авторизації та реєстрації
- [x] Розробити  клієнт SSH-підключення
- [x] Розробити клієнт SFTP-підключення
- [x] Розробити клієнт збору метрик
- [x] Реалізувати можливість додання віртуальних машин
- [x] Розробити захищений API
- [x] Локалізувати систему
- [x] Інтегрувати в проект Telegram Bot Api
### Об'єкт дослідження
Реалізація системи віддаленої взаємодії із віртуальними машинами.

## Технології
- C#
- Python
- Microsoft SQL Server
- Redis
- Entity Framework Core [![](https://badgen.net/badge/EntityFrameworkCore/efcore/512bd4?icon=github)](https://github.com/dotnet/efcore) [![](https://badgen.net/badge/nuget/v8.0.6/0274b5)](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore)
- SSH.NET [![](https://badgen.net/badge/SSH.NET/sshnet/0b8e36?icon=github)](https://github.com/sshnet/SSH.NET) [![](https://badgen.net/badge/nuget/v2024.0.0/0274b5)](https://www.nuget.org/packages/SSH.NET)
- Telegram.Bot [![](https://badgen.net/badge/Telegram.Bot/telegrambot/e0d8cc?icon=github)](https://github.com/TelegramBots/Telegram.Bot) [![](https://badgen.net/badge/nuget/19.0.0/0274b5)](https://www.nuget.org/packages/Telegram.Bot)
- ASP.NET Core [![](https://badgen.net/badge/ASP.NETCore/aspnetcore/512bd4?icon=github)](https://github.com/dotnet/aspnetcore)
- Docker

## Архітектура
Архітектурою додатка була обрана N-tier архітектура. Для Api ж була використана архітектура REST Api.

![image](https://github.com/johnburitto/VMControlPanel/assets/79087305/598df988-937b-4f38-b171-98ccb1f465ad)

## Реалізація
https://github.com/johnburitto/VMControlPanel/assets/79087305/3543b0e4-5776-4fa1-877c-962e69aca56d

## Розгортання
Для розгортання застосунку необхідно виконати наступні кроки:
1. Зробити копію всіх даних з бази даних Microsoft SQL Server. Для того, щоб зробити дамп бази наних необхідно виконати наступну команду ``sqlcmd -S [server_name] -Q "BACKUP DATABASE [database_name] TO DISK='C:\Backup\backup_file.bak' WITH INIT"``. Для того, щоб відновити дані з дампу на новому місці необхідно виконати команду ``sqlcmd -S [server_name] -Q "RESTORE DATABASE [database_name] FROM DISK='C:\Backup\backup_file.bak' WITH RECOVERY``.
2. Розгорнути в Docker імеджі Microsoft SQL Server та Redis. Команда ``docker compose up -d``.
```docker-compose.yml
version: '3.9'

services:
    sql-server-db:
        image: mcr.microsoft.com/mssql/server
        container_name: sql-server-db
        restart: always
        environment:
            - MSSQL_SA_PASSWORD=Strong2@PWD12
            - ACCEPT_EULA=Y
        ports:
            - "5434:1433"
        volumes:
            - sqlvolume:/var/opt/mssql
    redis:
        image: redis:latest
        container_name: redis
        restart: always
        ports:
          - "6379:6379"
        volumes:
          - /path/to/local/data:/root/redis
          - /path/to/local/redis.conf:/usr/local/etc/redis/redis.conf
        environment:
          - REDIS_PORT=6379

volumes:
    sqlvolume:
```
3. Після розгортання бази даних необхідно виконати міграції за допомогою команди ``Update-Database``.
4. Запустити проєкт.
