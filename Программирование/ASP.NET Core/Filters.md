**Filters (фильтры)** в ASP.NET Core — это механизм для выполнения логики **до, после или во время** обработки запроса. Они позволяют централизовать код, который **повторяется** в разных частях приложения (например, логирование, обработка ошибок, валидация и аутентификация).

## **📌 Виды фильтров в ASP.NET Core**

| Вид фильтра       | Интерфейс              | Когда выполняется                                                       |
| ----------------- | ---------------------- | ----------------------------------------------------------------------- |
| **Authorization** | `IAuthorizationFilter` | Перед выполнением контроллера                                           |
| **Resource**      | `IResourceFilter`      | До и после выполнения контроллера (но до выполнения модели)             |
| **Action**        | `IActionFilter`        | До и после выполнения метода контроллера                                |
| **Exception**     | `IExceptionFilter`     | При возникновении исключений                                            |
| **Result**        | `IResultFilter`        | До и после формирования результата (например, перед рендерингом `View`) |

---

## **1️⃣ Authorization Filters (Фильтры авторизации)**

Эти фильтры **определяют**, имеет ли пользователь **право** выполнять запрос. Обычно используются атрибуты **[Authorize]**.

```C#
[Authorize]
public class MyController : Controller
{
    public IActionResult SecureData()
    {
        return Ok("Этот метод доступен только авторизованным пользователям.");
    }
}

```

🔹 **Кастомный фильтр авторизации:**  
Можно создать собственный фильтр, реализовав `IAuthorizationFilter`:

```C#
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class CustomAuthFilter : IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        bool isAuthenticated = false; // Здесь может быть проверка токена или другого механизма авторизации

        if (!isAuthenticated)
        {
            context.Result = new UnauthorizedResult();
        }
    }
}

```

**Применение:**

```C#
[ServiceFilter(typeof(CustomAuthFilter))]
public class SecureController : Controller
{
    public IActionResult Secret()
    {
        return Ok("Вы авторизованы!");
    }
}

```

---

## **2️⃣ Resource Filters (Фильтры ресурсов)**

Эти фильтры выполняются **до и после** обработки запроса контроллером, но **до выполнения привязки модели**. Они полезны для кеширования и модификации входных данных.

### **Пример: Кастомный Resource Filter**

```C#
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

public class ResourceLogFilter : IResourceFilter
{
    public void OnResourceExecuting(ResourceExecutingContext context)
    {
        Console.WriteLine("🚀 Начало обработки ресурса");
    }

    public void OnResourceExecuted(ResourceExecutedContext context)
    {
        Console.WriteLine("✅ Завершение обработки ресурса");
    }
}

```

**Применение:**

```C#
[ServiceFilter(typeof(ResourceLogFilter))]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return Ok("Hello, ASP.NET Core!");
    }
}

```

---

## **3️⃣ Action Filters (Фильтры действий)**

Эти фильтры выполняются **до и после** выполнения метода контроллера. Они **модифицируют** входные данные или результат метода.

### **Пример: Логирование выполнения методов контроллера**

```C#
using Microsoft.AspNetCore.Mvc.Filters;

public class ActionLoggingFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        Console.WriteLine($"➡ Выполняется метод: {context.ActionDescriptor.DisplayName}");
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        Console.WriteLine($"✔ Завершено выполнение метода: {context.ActionDescriptor.DisplayName}");
    }
}

```

**Применение:**

```C#
[ServiceFilter(typeof(ActionLoggingFilter))]
public class DemoController : Controller
{
    public IActionResult Test()
    {
        return Ok("Фильтр логирования работает!");
    }
}

```

---

## **4️⃣ Exception Filters (Фильтры обработки исключений)**

Фильтры исключений позволяют **ловить** ошибки и **обрабатывать их централизованно**. Например, можно **логировать ошибки** или возвращать **кастомный JSON-ответ**.

### **Пример: Глобальная обработка исключений**

```C#
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

public class GlobalExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        Console.WriteLine($"🔥 Ошибка: {context.Exception.Message}");

        context.Result = new ObjectResult(new
        {
            Message = "Произошла ошибка, попробуйте позже.",
            Error = context.Exception.Message
        })
        {
            StatusCode = 500
        };

        context.ExceptionHandled = true;
    }
}

```

**Применение:**

```C#
[ServiceFilter(typeof(GlobalExceptionFilter))]
public class ErrorController : Controller
{
    public IActionResult ThrowError()
    {
        throw new Exception("Это тестовая ошибка!");
    }
}

```

🔹 Теперь если в контроллере возникнет ошибка, фильтр **автоматически вернёт JSON-ответ**.

---

## **5️⃣ Result Filters (Фильтры результатов)**

Result Filters **изменяют** или **логируют** выходные данные перед их отправкой клиенту.

### **Пример: Добавление заголовка к ответу**

```C#
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

public class AddHeaderFilter : IResultFilter
{
    public void OnResultExecuting(ResultExecutingContext context)
    {
        context.HttpContext.Response.Headers.Add("X-Custom-Header", "Hello from Filter!");
    }

    public void OnResultExecuted(ResultExecutedContext context)
    {
        Console.WriteLine("📩 Заголовок добавлен!");
    }
}

```

**Применение:**

```C#
[ServiceFilter(typeof(AddHeaderFilter))]
public class HeaderController : Controller
{
    public IActionResult Index()
    {
        return Ok("Проверьте заголовки ответа!");
    }
}

```

---

## **6️⃣ Глобальные фильтры**

Фильтры можно применять **не только на уровне контроллера**, но и **глобально для всего приложения**.

### **Добавление глобального фильтра**

В `Program.cs`:

```C#
builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(GlobalExceptionFilter)); // Добавляем фильтр для обработки исключений глобально
});

```

Теперь **все контроллеры** используют `GlobalExceptionFilter`.

---

## **7️⃣ Как фильтры работают вместе?**

Фильтры выполняются **в следующем порядке**:

1️⃣ **Authorization** (Авторизация)  
2️⃣ **Resource (до контроллера)**  
3️⃣ **Action (до метода контроллера)**  
4️⃣ **Метод контроллера**  
5️⃣ **Action (после метода контроллера)**  
6️⃣ **Result (обработка ответа)**  
7️⃣ **Resource (после контроллера)**  
8️⃣ **Exception (если есть ошибка)**

---

## **Выводы**

✅ **Фильтры помогают централизовать** повторяющийся код (логирование, ошибки, кеширование).  
✅ **Можно создавать кастомные фильтры**, наследуясь от `IActionFilter`, `IExceptionFilter` и других.  
✅ **Фильтры можно применять** локально (в контроллере) или глобально (для всего приложения).  
✅ **PLINQ + фильтры** — мощный инструмент для оптимизации ASP.NET Core API.

🔹 **Где полезны фильтры?**  
✔ Логирование запросов/ответов  
✔ Валидация данных  
✔ Обработка ошибок  
✔ Аудит безопасности

🔥 Используйте фильтры **правильно**, и ваш код станет **чище и гибче!** 😃🚀