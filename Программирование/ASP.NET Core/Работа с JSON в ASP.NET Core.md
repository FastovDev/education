ASP.NET Core поддерживает работу с JSON через два основных инструмента:

1. **`System.Text.Json`** – встроенная библиотека, оптимизированная для высокой производительности.
2. **`Newtonsoft.Json`** – популярная сторонняя библиотека, обладающая более широкими возможностями.

Разберёмся с их возможностями, настройками и случаями использования.

---

# **1. System.Text.Json (встроенный парсер JSON)**

### **1.1. Подключение**

`System.Text.Json` встроен в .NET Core 3.0+ и не требует дополнительных зависимостей.

Импорт пространства имён:

```C#
using System.Text.Json;
using System.Text.Json.Serialization;

```

### **1.2. Сериализация (объект → JSON)**

```C#
var product = new Product { Id = 1, Name = "Ноутбук", Price = 50000 };
string json = JsonSerializer.Serialize(product);
Console.WriteLine(json);

```

**Вывод:**

```json
{"Id":1,"Name":"Ноутбук","Price":50000}
```

### **1.3. Десериализация (JSON → объект)**

```C#
var json = "{\"Id\":1,\"Name\":\"Ноутбук\",\"Price\":50000}";
Product product = JsonSerializer.Deserialize<Product>(json);
Console.WriteLine(product.Name);

```

### **1.4. Настройки сериализации**

Можно изменять поведение парсинга, например, настраивать игнорирование `null`-значений, регистр полей, форматирование.

```C#
var options = new JsonSerializerOptions
{
    WriteIndented = true, // Форматированный вывод
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // camelCase
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull // Игнорировать null
};

string json = JsonSerializer.Serialize(product, options);
Console.WriteLine(json);

```

**Вывод:**

```json
{
  "id": 1,
  "name": "Ноутбук",
  "price": 50000
}

```

### **1.5. Атрибуты сериализации**

- **`[JsonIgnore]`** – исключает свойство из JSON.
- **`[JsonPropertyName("custom_name")]`** – задаёт кастомное имя.

```C#
public class Product
{
    public int Id { get; set; }

    [JsonPropertyName("product_name")]
    public string Name { get; set; }

    [JsonIgnore]
    public decimal Price { get; set; }
}

```

**Вывод:**

```json
{
  "id": 1,
  "product_name": "Ноутбук"
}

```

---

# **2. Newtonsoft.Json (Json.NET)**

**Когда использовать?**

- Нужно поддерживать старый код с `Newtonsoft.Json`.
- Требуются гибкие настройки сериализации (`ReferenceLoopHandling`, `Converters`).
- Нужно работать со сложными структурами JSON.

### **2.1. Подключение**

Добавить NuGet-пакет:

```powershell
dotnet add package Newtonsoft.Json

```

Импорт пространства имён:

```C#
using Newtonsoft.Json;
```

### **2.2. Сериализация**

```C#
var product = new Product { Id = 1, Name = "Ноутбук", Price = 50000 };
string json = JsonConvert.SerializeObject(product, Formatting.Indented);
Console.WriteLine(json);

```

**Вывод:**

```json
{
  "Id": 1,
  "Name": "Ноутбук",
  "Price": 50000
}

```

### **2.3. Десериализация**

```C#
string json = "{\"Id\":1,\"Name\":\"Ноутбук\",\"Price\":50000}";
Product product = JsonConvert.DeserializeObject<Product>(json);
Console.WriteLine(product.Name);

```

### **2.4. Атрибуты**

- `[JsonIgnore]` – исключает поле.
- `[JsonProperty("custom_name")]` – задаёт кастомное имя.
- `[JsonConverter(typeof(CustomConverter))]` – кастомный конвертер.

```C#
public class Product
{
    public int Id { get; set; }

    [JsonProperty("product_name")]
    public string Name { get; set; }

    [JsonIgnore]
    public decimal Price { get; set; }
}
```

**Вывод:**

```json
{
  "Id": 1,
  "product_name": "Ноутбук"
}

```

---

# **3. Подключение Newtonsoft.Json в ASP.NET Core**

По умолчанию в ASP.NET Core 3.0+ используется `System.Text.Json`. Чтобы переключиться на `Newtonsoft.Json`, нужно зарегистрировать его в `Program.cs`:

```C#
builder.Services
    .AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    });

```

---

# **4. Различия между System.Text.Json и Newtonsoft.Json**

|Функция|System.Text.Json|Newtonsoft.Json|
|---|---|---|
|Производительность|Выше|Ниже|
|Поддержка старого кода|Нет|Да|
|Атрибуты|`[JsonPropertyName]`|`[JsonProperty]`|
|Поддержка конвертеров|Ограниченная|Полноценная|
|Поддержка `ReferenceLoopHandling`|Нет|Да|

Если вам нужна **скорость**, используйте `System.Text.Json`. Если **гибкость** – `Newtonsoft.Json`.

---

# **5. Примеры работы с JSON в Web API**

### **5.1. Получение JSON-данных**

```C#
[HttpGet]
public IActionResult Get()
{
    var product = new Product { Id = 1, Name = "Ноутбук", Price = 50000 };
    return Ok(product); // JSON автоматически сериализуется
}

```

### **5.2. Получение JSON-данных с настройками**

```C#
[HttpGet]
public IActionResult Get()
{
    var options = new JsonSerializerOptions { WriteIndented = true };
    var product = new Product { Id = 1, Name = "Ноутбук", Price = 50000 };
    string json = JsonSerializer.Serialize(product, options);
    return Content(json, "application/json");
}

```

### **5.3. Получение JSON через `Newtonsoft.Json`**

```C#
[HttpGet]
public IActionResult Get()
{
    var product = new Product { Id = 1, Name = "Ноутбук", Price = 50000 };
    string json = JsonConvert.SerializeObject(product, Formatting.Indented);
    return Content(json, "application/json");
}

```

---

# **6. Заключение**

- `System.Text.Json` – быстрый, встроенный, но менее гибкий.
- `Newtonsoft.Json` – мощный, но требует установки.
- В ASP.NET Core 3.0+ по умолчанию используется `System.Text.Json`, но можно переключиться на `Newtonsoft.Json`.

Если вам нужно больше настроек или старый код использует `Newtonsoft.Json`, его можно подключить.  
Если приоритет – **производительность**, лучше оставить `System.Text.Json`.