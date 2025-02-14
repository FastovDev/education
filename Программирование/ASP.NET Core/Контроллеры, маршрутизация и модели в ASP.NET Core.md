В ASP.NET Core контроллеры, маршрутизация и модели являются ключевыми компонентами, которые позволяют обрабатывать HTTP-запросы и управлять данными.

Разберёмся с каждым из них подробно.

---

# **1. Контроллеры (Controllers) в ASP.NET Core**

### **1.1. Что такое контроллер?**

Контроллер — это класс, который обрабатывает входящие HTTP-запросы и возвращает соответствующий ответ.

В зависимости от типа приложения контроллер может:

- Возвращать HTML (в MVC-приложениях).
- Возвращать данные в формате JSON/XML (в Web API).

### **1.2. Создание контроллера**

Все контроллеры должны наследоваться от `Controller` (для MVC) или `ControllerBase` (для Web API).

**Пример MVC-контроллера:**

```C#
using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult About()
    {
        return View();
    }
}

```

Этот контроллер обрабатывает запросы к `/Home/Index` и `/Home/About`.

**Пример API-контроллера:**

```C#
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private static readonly List<string> products = new() { "Ноутбук", "Телефон", "Монитор" };

    [HttpGet]
    public IEnumerable<string> Get() => products;

    [HttpGet("{id}")]
    public ActionResult<string> Get(int id)
    {
        if (id < 0 || id >= products.Count)
            return NotFound();
        return products[id];
    }
}

```

Этот контроллер будет обрабатывать запросы, например:

- `GET /api/products` → возвращает список товаров.
- `GET /api/products/1` → возвращает товар с ID 1.

---

## **2. Маршрутизация (Routing) в ASP.NET Core**

Маршрутизация определяет, какие URL-адреса соответствуют каким методам контроллеров.

### **2.1. Атрибутная маршрутизация**

В атрибутной маршрутизации маршруты указываются прямо в контроллере:

```C#
[Route("api/products")]
[ApiController]
public class ProductsController : ControllerBase
{
    [HttpGet] // GET /api/products
    public IEnumerable<string> Get() => new string[] { "Товар1", "Товар2" };

    [HttpGet("{id}")] // GET /api/products/{id}
    public ActionResult<string> Get(int id)
    {
        return "Товар " + id;
    }
}

```

### **2.2. Конвенциональная маршрутизация**

Маршруты задаются в `Program.cs`:

```C#
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
var app = builder.Build();

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();

```

Здесь определён маршрут по умолчанию:

- `/Home/Index` вызовет `Index` в `HomeController`.
- `/Product/Details/5` вызовет `Details(5)` в `ProductController`.

---

## **3. Модели (Models) в ASP.NET Core**

Модель — это класс, который описывает данные и логику их обработки.

### **3.1. Простая модель**

```C#
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}

```

### **3.2. Использование модели в контроллере**

```C#
[Route("api/products")]
[ApiController]
public class ProductsController : ControllerBase
{
    private static readonly List<Product> products = new()
    {
        new Product { Id = 1, Name = "Ноутбук", Price = 50000 },
        new Product { Id = 2, Name = "Телефон", Price = 30000 }
    };

    [HttpGet]
    public IEnumerable<Product> Get() => products;

    [HttpPost]
    public IActionResult Create([FromBody] Product product)
    {
        products.Add(product);
        return CreatedAtAction(nameof(Get), new { id = product.Id }, product);
    }
}

```

Запрос `POST /api/products` с JSON-данными:

```json
{
    "id": 3,
    "name": "Монитор",
    "price": 15000
}

```

Добавит новый товар в список.

---

## **4. Валидация моделей**

ASP.NET Core поддерживает валидацию моделей с помощью атрибутов `DataAnnotations`.

### **4.1. Пример модели с валидацией**

```C#
using System.ComponentModel.DataAnnotations;

public class Product
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [Range(1, 100000)]
    public decimal Price { get; set; }
}

```

### **4.2. Валидация в контроллере**

```C#
[HttpPost]
public IActionResult Create([FromBody] Product product)
{
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }

    products.Add(product);
    return CreatedAtAction(nameof(Get), new { id = product.Id }, product);
}

```

Если в запросе не будет `Name` или `Price` выйдет ошибка `400 Bad Request`.

---

## **5. Разница между MVC и API контроллерами**

|**Функция**|**MVC Controller**|**API Controller**|
|---|---|---|
|Наследуется от|`Controller`|`ControllerBase`|
|Возвращает|`ViewResult`, `JsonResult`, `RedirectResult`|`JsonResult`, `ActionResult<T>`|
|Использует Razor|Да|Нет|
|Атрибут `[ApiController]`|Нет|Да|

---

## **6. Заключение**

- **Контроллеры** обрабатывают HTTP-запросы и возвращают HTML или JSON.
- **Маршрутизация** определяет, какие URL соответствуют каким методам.
- **Модели** используются для работы с данными и могут включать валидацию.