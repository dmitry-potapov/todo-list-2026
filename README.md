# Todo List 2026 (aka Yet Another Todo List)

A simple todo list app. You can add, view and manage your tasks.

- **Backend:** .NET Web API (in-memory database, so data is lost on restart)
- **Frontend:** Angular (`todo-list-ui`)

## Run the backend

```bash
dotnet run --project YetAnother.TodoList.Api --launch-profile http
```

The API runs at http://localhost:5107.

## Run the frontend

```bash
cd todo-list-ui
npm install
npm start
```

The app opens at http://localhost:4200. Start the backend first.

## Run the tests

Backend (integration tests need Docker running, they use a PostgreSQL container):

```bash
dotnet test Tests/YetAnother.TodoList.UnitTests
dotnet test Tests/YetAnother.TodoList.IntegrationTests
```

Frontend:

```bash
cd todo-list-ui
npm test
```
