**Middleware (промежуточное ПО)** — это компонент в ASP.NET Core, который обрабатывает HTTP-запросы и ответы.  
**Custom Middleware** — это собственные промежуточные компоненты, выполняющие специфическую логику, например, **логирование, аутентификацию, кеширование, обработку ошибок и др.**

---

## **🔹 Как работает Middleware?**

ASP.NET Core обрабатывает запросы **каскадно**, проходя через **цепочку Middleware**.

📌 **Цепочка обработки запроса**

1. **Middleware A** → передаёт запрос дальше
2. **Middleware B** → передаёт запрос дальше
3. **Middleware C** → выполняет конечную логику
4. **Middleware C** → возвращает ответ
5. **Middleware B** → обрабатывает ответ
6. **Middleware A** → обрабатывает ответ

Каждый Middleware **может передавать управление** следующему или **прерывать цепочку**, отправляя ответ клиенту.

---

## **1️⃣ Регистрация Middleware в ASP.NET Core**

В файле `Program.cs`:

```C#
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.Use(async (context, next) =>
{
    Console.WriteLine("🔹 Middleware 1: Начало обработки");
    await next();  // Передаём управление следующему Middleware
    Console.WriteLine("✅ Middleware 1: Завершение обработки");
});

app.Use(async (context, next) =>
{
    Console.WriteLine("🔹 Middleware 2: Начало обработки");
    await next();
    Console.WriteLine("✅ Middleware 2: Завершение обработки");
});

app.Run(async (context) =>
{
    Console.WriteLine("🔥 Конечный Middleware: Обрабатываем запрос");
    await context.Response.WriteAsync("Hello from Middleware!");
});

app.Run();

```

### **🔍 Что здесь происходит?**

1. **Middleware 1** выполняет код → передаёт управление дальше
2. **Middleware 2** выполняет код → передаёт управление дальше
3. **Конечный Middleware** (`app.Run()`) отправляет ответ
4. Управление возвращается назад, выполняя **обратный проход** через Middleware

---

## **2️⃣ Создание Custom Middleware**

**Способы создания Middleware:**  
✔ Через метод `app.Use()`  
✔ Через `app.UseMiddleware<T>()`  
✔ Через расширяющий метод `UseCustomMiddleware()`

---

### **📌 Способ 1: Middleware через app.Use()**

```C#
app.Use(async (context, next) =>
{
    Console.WriteLine($"➡ Запрос: {context.Request.Method} {context.Request.Path}");
    await next();  // Передаём управление дальше
    Console.WriteLine($"⬅ Ответ отправлен: {context.Response.StatusCode}");
});

```

✅ **Быстро, но не переиспользуемо**.

---

### **📌 Способ 2: Middleware как класс (рекомендуемый способ)**

**Создадим Middleware-класс:**

```C#
public class LoggingMiddleware
{
    private readonly RequestDelegate _next;

    public LoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        Console.WriteLine($"➡ Запрос: {context.Request.Method} {context.Request.Path}");
        
        await _next(context); // Передаём запрос дальше
        
        Console.WriteLine($"⬅ Ответ: {context.Response.StatusCode}");
    }
}

```

**Добавим Middleware в `Program.cs`:**

```C#
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseMiddleware<LoggingMiddleware>();

app.Run(async (context) =>
{
    await context.Response.WriteAsync("Middleware работает!");
});

app.Run();

```

✅ **Гибкость**: этот класс можно переиспользовать в разных проектах.

---

### **📌 Способ 3: Middleware через расширяющий метод**

Создадим **метод-расширение**, чтобы упрощать подключение Middleware:

```C#
public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseLoggingMiddleware(this IApplicationBuilder app)
    {
        return app.UseMiddleware<LoggingMiddleware>();
    }
}

```

Теперь подключаем его так:

```C#
app.UseLoggingMiddleware();

```

✅ **Чистый код**: приложение остаётся читаемым.

---

## **3️⃣ Управление потоком запросов**

### **📌 Прерывание Middleware (без передачи запроса дальше)**

```C#
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/blocked")
    {
        context.Response.StatusCode = 403;
        await context.Response.WriteAsync("Доступ запрещён!");
        return; // Прерываем выполнение
    }

    await next();
});

```

🔹 Если путь **"/blocked"**, Middleware **не передаёт** запрос дальше и сразу **возвращает 403**.

---

## **4️⃣ Middleware для обработки ошибок**

### **📌 Перехват и обработка исключений**

Создадим Middleware для обработки **ошибок**:

```C#
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context); // Передаём запрос дальше
        }
        catch (Exception ex)
        {
            Console.WriteLine($"🔥 Ошибка: {ex.Message}");
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("Внутренняя ошибка сервера");
        }
    }
}

```

**Регистрация Middleware:**

```C#
app.UseMiddleware<ExceptionHandlingMiddleware>();

```

✅ **Теперь все ошибки обрабатываются централизованно!**

---

## **5️⃣ Middleware для измерения времени выполнения запроса**

```C#
public class RequestTimingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestTimingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();

        await _next(context);

        watch.Stop();
        Console.WriteLine($"⏳ Запрос обработан за {watch.ElapsedMilliseconds} мс");
    }
}

```

**Добавление в `Program.cs`:**

```C#
app.UseMiddleware<RequestTimingMiddleware>();

```

---

## **6️⃣ Middleware для модификации заголовков**

```C#
public class CustomHeaderMiddleware
{
    private readonly RequestDelegate _next;

    public CustomHeaderMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        context.Response.Headers.Add("X-Custom-Header", "Hello from Middleware");

        await _next(context);
    }
}

```

🔹 **Теперь все ответы включают заголовок `X-Custom-Header`!**

---

## **Выводы**

✅ **Middleware — мощный инструмент** для обработки запросов и ответов.  
✅ **Можно создавать свои Middleware** для логирования, безопасности, обработки ошибок и др.  
✅ **Цепочка Middleware** позволяет централизовать обработку данных.  
✅ **Можно прерывать поток**, изменять запросы и ответы.  
✅ **Middleware легко переиспользовать** в разных проектах.

🔥 Используйте **Custom Middleware** для создания **гибкого и расширяемого ASP.NET Core API!** 🚀