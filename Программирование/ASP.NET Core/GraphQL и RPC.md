В современных микросервисных и API-ориентированных архитектурах **GraphQL** и **gRPC** являются популярными альтернативами REST API. Разберем их основные принципы, преимущества, недостатки и примеры реализации в **ASP.NET Core**.

---

# **📌 1. GraphQL в ASP.NET Core**

### 🔹 Что такое GraphQL?

**GraphQL** — это язык запросов к API, который позволяет клиенту **самому определять, какие данные получать**. В отличие от REST, где есть фиксированные эндпоинты (`/users`, `/orders`), в GraphQL клиент может запрашивать только нужные поля.

---

## **✅ Преимущества GraphQL**

✔ **Гибкость** – можно запрашивать только нужные данные  
✔ **Меньше запросов** – можно получать связанные данные одним запросом  
✔ **Схема API** – строго типизированная схема облегчает разработку  
✔ **Поддержка реального времени** – подписки (subscriptions)

## **❌ Недостатки GraphQL**

❌ **Сложность** – требует продвинутой настройки  
❌ **Производительность** – запросы могут быть тяжелее, чем REST  
❌ **Кэширование** – сложнее реализовать, чем в REST

---

## **📌 Настройка GraphQL в ASP.NET Core**

### **📍 Установка необходимых пакетов**

```sh
dotnet add package HotChocolate.AspNetCore
dotnet add package HotChocolate.AspNetCore.Playground
```
### **📍 Настройка GraphQL в `Program.cs`**

```C#
var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>();

var app = builder.Build();

app.MapGraphQL(); // Подключаем GraphQL
app.Run();

```

---

## **📌 Определение схемы GraphQL**

### **📍 1. Создаем модель данных**

```C#
public class Book
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
}

```

### **📍 2. Создаем Query для получения данных**

```C#
public class Query
{
    public List<Book> GetBooks() => new()
    {
        new Book { Id = 1, Title = "C# in Depth", Author = "Jon Skeet" },
        new Book { Id = 2, Title = "CLR via C#", Author = "Jeffrey Richter" }
    };
}

```

---

## **📌 Запросы в GraphQL**

После запуска приложения GraphQL API будет доступен по адресу:  
📍 `http://localhost:5000/graphql`

### **Пример запроса**

```graphql
query {
  books {
    id
    title
    author
  }
}

```

### **Ответ**

```json
{
  "data": {
    "books": [
      { "id": 1, "title": "C# in Depth", "author": "Jon Skeet" },
      { "id": 2, "title": "CLR via C#", "author": "Jeffrey Richter" }
    ]
  }
}

```

---

## **📌 GraphQL Mutations (изменение данных)**

```C#
public class Mutation
{
    public Book AddBook(string title, string author)
    {
        return new Book { Id = 3, Title = title, Author = author };
    }
}

```

Подключаем мутацию в `Program.cs`:

```C#
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>();

```

**Запрос на создание книги:**

```graphql
mutation {
  addBook(title: "ASP.NET Core GraphQL", author: "John Doe") {
    id
    title
    author
  }
}

```

---

## **📌 GraphQL Subscriptions (подписка на события)**

Позволяет клиенту получать **обновления в реальном времени**.

```C#
public class Subscription
{
    [Subscribe]
    public Book OnBookAdded([EventMessage] Book book) => book;
}

```

Добавляем подписки в `Program.cs`:

```C#
builder.Services
    .AddGraphQLServer()
    .AddSubscriptionType<Subscription>();

```

**Пример подписки:**

```graphql
subscription {
  onBookAdded {
    id
    title
    author
  }
}

```

---

# **📌 2. gRPC в ASP.NET Core**

### 🔹 Что такое gRPC?

**gRPC** (Google Remote Procedure Call) — это **высокопроизводительный** RPC-фреймворк, который использует **Protocol Buffers (protobuf)** вместо JSON.

---

## **✅ Преимущества gRPC**

✔ **Высокая производительность** – работает поверх HTTP/2  
✔ **Компактность** – данные сериализуются в бинарном формате  
✔ **Строгая типизация** – контракт задается через `.proto` файлы  
✔ **Поддержка потоков** – bidirectional streaming

## **❌ Недостатки gRPC**

❌ **Не поддерживается в браузерах** (из-за HTTP/2)  
❌ **Сложнее отлаживать**

---

## **📌 Настройка gRPC в ASP.NET Core**

### **📍 1. Устанавливаем пакет gRPC**

```sh
dotnet add package Grpc.AspNetCore

```

### **📍 2. Добавляем gRPC в `Program.cs`**

```C#
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc();

var app = builder.Build();
app.MapGrpcService<BookService>();
app.Run();

```

---

## **📌 Определение gRPC-сервиса**

### **📍 1. Создаем файл `book.proto`**

```proto
syntax = "proto3";

service BookService {
  rpc GetBooks (Empty) returns (BookList);
}

message Empty {}

message Book {
  int32 id = 1;
  string title = 2;
  string author = 3;
}

message BookList {
  repeated Book books = 1;
}

```

### **📍 2. Компилируем `.proto`**

В `csproj` добавляем:

```xml
<ItemGroup>
  <Protobuf Include="Protos/book.proto" GrpcServices="Server" />
</ItemGroup>

```

### **📍 3. Реализуем сервис**

```C#
using Grpc.Core;
using System.Threading.Tasks;

public class BookService : BookService.BookServiceBase
{
    public override Task<BookList> GetBooks(Empty request, ServerCallContext context)
    {
        var books = new BookList();
        books.Books.Add(new Book { Id = 1, Title = "C# in Depth", Author = "Jon Skeet" });
        books.Books.Add(new Book { Id = 2, Title = "CLR via C#", Author = "Jeffrey Richter" });

        return Task.FromResult(books);
    }
}

```

---

## **📌 Клиент для gRPC**

### **📍 1. Создаем gRPC-клиент**

```sh
dotnet add package Grpc.Net.Client

```

### **📍 2. Подключаем к серверу**

```C#
using Grpc.Net.Client;
using System.Threading.Tasks;

var channel = GrpcChannel.ForAddress("https://localhost:5001");
var client = new BookService.BookServiceClient(channel);

var response = await client.GetBooksAsync(new Empty());
foreach (var book in response.Books)
{
    Console.WriteLine($"{book.Id}: {book.Title} - {book.Author}");
}

```

# **📌 Итог**

|                        |          **GraphQL**          |      **gRPC**       |
| :--------------------: | :---------------------------: | :-----------------: |
|   **Формат данных**    |             JSON              | Protobuf (бинарный) |
| **Производительность** |            Средняя            |       Высокая       |
| **Где используется?**  | Web API, мобильные приложения |    Микросервисы     |
|   **Реальное время**   |      Да (subscriptions)       |   Да (streaming)    |
🔹 **GraphQL** – лучше для **клиент-серверного** взаимодействия  
🔹 **gRPC** – лучше для **взаимодействия микросервисов**

🚀 **Выбор зависит от задачи!**