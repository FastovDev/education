## **1. Что такое Middleware?**

Middleware – это промежуточный слой обработки HTTP-запросов в ASP.NET Core. Он выполняет логику перед передачей запроса следующему компоненту в конвейере обработки.

**Принцип работы**:

1. Запрос поступает в первый middleware.
2. Middleware выполняет свою логику.
3. Передаёт управление следующему middleware или возвращает ответ.
4. Если запрос прошёл через все middleware, то ответ формируется и проходит обратный путь.

**Пример конвейера middleware:**

`Клиент → Middleware 1 → Middleware 2 → Контроллер → Middleware 2 → Middleware 1 → Клиент`

---

## **2. Использование встроенных Middleware**

ASP.NET Core предоставляет множество встроенных middleware.

### **2.1. Конфигурация Middleware в `Program.cs`**

Все middleware регистрируются в файле `Program.cs` с помощью метода `Use*`.

```C#
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Встроенные middleware
app.UseRouting();      // Определяет маршруты
app.UseAuthorization();// Проверяет авторизацию
app.UseStaticFiles();  // Обслуживает статические файлы (CSS, JS, изображения)

// Завершающий обработчик
app.MapControllers();  // Запускает контроллеры

app.Run();

```

---

## **3. Основные встроенные Middleware**

### **3.1. `UseRouting`**

Определяет маршруты, которые используются в приложении.

`app.UseRouting();`

### **3.2. `UseEndpoints`**

Обрабатывает конечные точки маршрутизации (контроллеры, API, Razor Pages).

`app.UseEndpoints(endpoints => { endpoints.MapControllers(); });`

### **3.3. `UseStaticFiles`**

Позволяет обслуживать статические файлы (HTML, CSS, JS, изображения).

`app.UseStaticFiles();`

### **3.4. `UseAuthorization`**

Проверяет авторизацию пользователя. Работает только после `UseRouting`.

`app.UseAuthorization();`

### **3.5. `UseExceptionHandler`**

Глобальная обработка ошибок.

`app.UseExceptionHandler("/Home/Error");`

### **3.6. `UseHttpsRedirection`**

Перенаправляет HTTP-запросы на HTTPS.

`app.UseHttpsRedirection();`

---

## **4. Создание кастомного Middleware**

### **4.1. Через `Use`**

Самый простой способ создать middleware – использовать `Use`.

```C#
app.Use(async (context, next) =>
{
    Console.WriteLine("Middleware 1: Перед запросом");
    await next(); // Передаём управление дальше
    Console.WriteLine("Middleware 1: После запроса");
});

```

**Вывод в консоль при обработке запроса:**

```
Middleware 1: Перед запросом 
Middleware 1: После запроса
```

### **4.2. Через `Map`**

`Map` позволяет разделять обработку запросов по определённому пути.

```C#
app.Map("/hello", appBuilder =>
{
    appBuilder.Run(async context =>
    {
        await context.Response.WriteAsync("Привет, мир!");
    });
});
```

**Запрос `http://localhost/hello` → ответ:**

`Привет, мир!`

### **4.3. Через класс Middleware**

Можно создать middleware в виде отдельного класса.

**Шаг 1: Создать класс Middleware**

```C#
public class CustomMiddleware
{
    private readonly RequestDelegate _next;

    public CustomMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        Console.WriteLine("Кастомный Middleware: Запрос обработан");
        await _next(context);
    }
}

```

**Шаг 2: Зарегистрировать Middleware в `Program.cs`**

```C#
app.UseMiddleware<CustomMiddleware>();

```

---

## **5. Порядок выполнения Middleware**

Middleware выполняются в порядке регистрации.

Пример:

```C#
app.Use(async (context, next) =>
{
    Console.WriteLine("Middleware 1: До next()");
    await next();
    Console.WriteLine("Middleware 1: После next()");
});

app.Use(async (context, next) =>
{
    Console.WriteLine("Middleware 2: До next()");
    await next();
    Console.WriteLine("Middleware 2: После next()");
});

```

**Вывод в консоль при запросе:**

```
Middleware 1: До next()
Middleware 2: До next()
Middleware 2: После next()
Middleware 1: После next()

```

**Выводит подтверждение того, что middleware работают в виде стека (LIFO).**

---

## **6. Разница между `Use`, `Run` и `Map`**

|Метод|Описание|
|---|---|
|`Use`|Передаёт запрос дальше (`next()`), но может выполнить код до и после него.|
|`Run`|Финальный обработчик, не передаёт запрос дальше.|
|`Map`|Разветвляет обработку запросов по URL.|

**Пример:**

```C#
app.Use(async (context, next) =>
{
    Console.WriteLine("Use Middleware");
    await next();
});

app.Run(async (context) =>
{
    Console.WriteLine("Run Middleware");
    await context.Response.WriteAsync("Ответ от Run()");
});

```

**Вывод:**

```
Use Middleware 
Run Middleware
```

---

## **7. Вывод**

- Middleware – это конвейер обработки запросов в ASP.NET Core.
- Используются встроенные middleware (`UseRouting`, `UseAuthorization`, `UseStaticFiles`).
- Можно создавать кастомные middleware (через `Use`, `Map`, `Run` или классы).
- Порядок регистрации middleware важен!

Middleware позволяет гибко управлять обработкой запросов, добавлять логику авторизации, логирования, кеширования и многого другого.