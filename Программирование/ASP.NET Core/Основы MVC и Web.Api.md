ASP.NET Core предоставляет две основные модели для построения веб-приложений:

- **MVC (Model-View-Controller)** — используется для построения традиционных веб-приложений с HTML-интерфейсом.
- **Web API** — предназначен для создания RESTful API, которые возвращают данные (обычно в формате JSON или XML).

Далее подробно разберём каждую из моделей.

---

## **1. MVC (Model-View-Controller) в ASP.NET Core**

### **Что такое MVC?**

MVC — это архитектурный шаблон, разделяющий логику приложения на три компонента:

- **Model (Модель):** отвечает за данные, их логику и правила обработки.
- **View (Представление):** отображает данные пользователю и получает ввод.
- **Controller (Контроллер):** обрабатывает запросы, вызывает соответствующие модели и передаёт данные представлениям.

### **Как работает MVC?**

1. **Клиент отправляет HTTP-запрос** (например, `GET /products`).
2. **Контроллер получает запрос** и определяет, что делать (например, обратиться к базе данных за списком товаров).
3. **Модель извлекает данные** и передаёт их контроллеру.
4. **Контроллер передаёт данные в представление**, которое рендерит HTML-страницу.
5. **Пользователь видит готовую страницу в браузере**.

---

### **1.1. Создание MVC-приложения в ASP.NET Core**

#### **Установка ASP.NET Core MVC**

Если проект создаётся с нуля, можно запустить команду:

```bash
dotnet new mvc -o MyMvcApp
cd MyMvcApp
dotnet run

```

Этот шаблон создаст базовую структуру MVC-приложения.

---

### **1.2. Компоненты MVC**

#### **1.2.1. Модель (Model)**

Модель отвечает за работу с данными. Обычно это класс с полями и методами для доступа к БД.

**Пример модели:**

```C#
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}

```

#### **1.2.2. Контроллер (Controller)**

Контроллер отвечает за обработку HTTP-запросов и работу с моделью.

**Пример контроллера:**

```C#
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

public class ProductController : Controller
{
    public IActionResult Index()
    {
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Ноутбук", Price = 50000 },
            new Product { Id = 2, Name = "Телефон", Price = 30000 }
        };

        return View(products);
    }
}

```

#### **1.2.3. Представление (View)**

Это HTML-шаблон с данными, полученными от контроллера.

**Пример `Views/Product/Index.cshtml`:**

```html
@model List<Product>

<h2>Список товаров</h2>
<ul>
    @foreach (var product in Model)
    {
        <li>@product.Name - @product.Price руб.</li>
    }
</ul>

```

---

### **1.3. Добавление маршрутов (Routing)**

В `Program.cs` необходимо зарегистрировать маршруты:

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

Теперь по адресу `/product/index` будет вызван метод `Index` в `ProductController`.

---

## **2. Web API в ASP.NET Core**

### **2.1. Что такое Web API?**

Web API — это модель разработки серверного API, работающего по HTTP и использующего JSON/XML для обмена данными.

### **2.2. Создание Web API**

Запускаем команду для создания API-проекта:

```bash
dotnet new webapi -o MyWebApi
cd MyWebApi
dotnet run
```
По умолчанию Web API использует `Minimal API`, но мы рассмотрим классический вариант с `Controller`.

---

### **2.3. Компоненты Web API**

Web API не использует представления, а возвращает данные в формате JSON.

#### **2.3.1. Контроллер Web API**

**Пример API-контроллера `ProductsController.cs`:**

```C#
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private static readonly List<Product> products = new()
    {
        new Product { Id = 1, Name = "Ноутбук", Price = 50000 },
        new Product { Id = 2, Name = "Телефон", Price = 30000 }
    };

    [HttpGet]
    public IEnumerable<Product> Get()
    {
        return products;
    }

    [HttpGet("{id}")]
    public ActionResult<Product> Get(int id)
    {
        var product = products.Find(p => p.Id == id);
        if (product == null)
            return NotFound();
        return product;
    }

    [HttpPost]
    public IActionResult Create([FromBody] Product product)
    {
        products.Add(product);
        return CreatedAtAction(nameof(Get), new { id = product.Id }, product);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] Product product)
    {
        var existing = products.Find(p => p.Id == id);
        if (existing == null)
            return NotFound();

        existing.Name = product.Name;
        existing.Price = product.Price;
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var product = products.Find(p => p.Id == id);
        if (product == null)
            return NotFound();

        products.Remove(product);
        return NoContent();
    }
}

```

---

### **2.4. Запросы к Web API**

Используем `curl` или `Postman` для тестирования API:

```bash
# Получение всех товаров
curl -X GET http://localhost:5000/api/products

# Получение товара с ID 1
curl -X GET http://localhost:5000/api/products/1

# Добавление товара
curl -X POST http://localhost:5000/api/products -H "Content-Type: application/json" -d '{"id":3,"name":"Монитор","price":10000}'

# Обновление товара
curl -X PUT http://localhost:5000/api/products/1 -H "Content-Type: application/json" -d '{"id":1,"name":"Ультрабук","price":60000}'

# Удаление товара
curl -X DELETE http://localhost:5000/api/products/1

```

---

## **3. Отличия MVC и Web API**

|**Функция**|**MVC**|**Web API**|
|---|---|---|
|Представления|Использует Razor Views|Отдаёт JSON/XML|
|Контроллеры|Наследуются от `Controller`|Наследуются от `ControllerBase`|
|Использование|Веб-приложения с UI|RESTful API|
|Формат ответов|HTML|JSON|

---

## **Заключение**

- **MVC** подходит для создания веб-приложений с пользовательским интерфейсом.
- **Web API** используется для разработки RESTful API.
- Оба подхода могут использоваться в одном проекте.