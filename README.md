Репозиторий GitHub:
https://github.com/nek00oo/ATOH

## ⚙️ Запуск проекта
Перейти в api
```bash
cd .\UserManagementApi\
```
Установить зависимости
```bash
dotnet restore
```
Поднять контейнер с бд
```bash
docker-compose up -d
```
Далее можно запускать проект, через Rider/Visual Studio автоматически откроется браузер со swagger.
Иначе, через команду:
```bash
dotnet run
```
Swagger будет по url: http://localhost:5120/swagger/index.html

*Миграция применится автоматически с запуском с готовым админом. Требуется только залогиниться: login: "admin", password: "admin"

# 🧑‍💼 User Management System

Это чистая и расширяемая система управления пользователями, реализованная на .NET с использованием луковой архитектуры (Onion Architecture).

---

## 📐 Архитектура проекта

Проект разделён на слои:

- **UserManagementApi** – HTTP API, контроллеры, middleware.
- **UserManagementApplication** – бизнес-логика, DTO, мапперы, сервисы.
- **UserManagementCore** – доменная модель, контракты, Result-обёртки.
- **UserManagementInfrastructure** – реализация репозиториев, EF Core, миграции, JWT, работа с БД.

---

## 🚀 Возможности

- 🔐 Аутентификация с использованием JWT (токен сохраняется в cookie + есть возможность аунтифицироваться через header)
- 👤 Разграничение доступа по ролям (администратор / пользователь)
- 📝 Создание, обновление, удаление и восстановление пользователей
- ✅ Валидация входных данных через FluentValidation
- 🧠 Soft delete через поля `RevokedOn` и `RevokedBy`
- 🔁 Уникальные логины, шифрование паролей

---

## 🛠️ Технологии

- .NET 9+
- ASP.NET Core WebAPI
- EF Core
- PostgreSQL
- Docker Compose
- FluentValidation
- JWT (сохранение токена в куки)
  
---
