## 1. Что такое Reflection?

**Reflection (Рефлексия)** в C# — это механизм, позволяющий изучать и изменять метаданные типов (классов, методов, свойств и т. д.) во время выполнения программы. Reflection находится в пространстве имен `System.Reflection` и позволяет:

- Получать информацию о сборках, модулях, типах
- Динамически создавать экземпляры классов
- Вызывать методы и устанавливать значения полей/свойств
- Работать с атрибутами

---

## 2. Где применяется Reflection?

### 1️⃣ **Фреймворки ORM (например, Entity Framework, Dapper)**

Используется для маппинга классов-моделей на таблицы базы данных.

### 2️⃣ **Сериализация и десериализация (JSON, XML)**

Фреймворки типа Newtonsoft.Json используют Reflection для автоматической сериализации/десериализации объектов.

### 3️⃣ **Dependency Injection (DI)**

Контейнеры внедрения зависимостей (ASP.NET Core DI, Autofac) используют Reflection для поиска и создания экземпляров классов.

### 4️⃣ **Тестирование (Unit-тесты, Moq, xUnit, NUnit)**

Фреймворки тестирования используют Reflection для поиска тестовых методов и их запуска.

### 5️⃣ **Плагины и динамическая загрузка сборок**

Позволяет загружать плагины (`.dll`) во время работы приложения и вызывать их методы.

---

## 3. Основные возможности Reflection

### 📌 3.1 Получение информации о типе

Для работы с Reflection используется пространство имен `System.Reflection`.

Пример получения информации о типе (`Type`):

```C#
using System;
using System.Reflection;

class Program
{
    static void Main()
    {
        Type type = typeof(Person);
        
        Console.WriteLine($"Имя класса: {type.Name}");
        Console.WriteLine($"Пространство имен: {type.Namespace}");

        Console.WriteLine("\nСвойства:");
        foreach (var prop in type.GetProperties())
        {
            Console.WriteLine($"- {prop.Name} ({prop.PropertyType.Name})");
        }

        Console.WriteLine("\nМетоды:");
        foreach (var method in type.GetMethods())
        {
            Console.WriteLine($"- {method.Name}");
        }
    }
}

public class Person
{
    public string Name { get; set; }
    public int Age { get; set; }

    public void SayHello() => Console.WriteLine("Hello!");
}

```

**Вывод:**

```diff
Имя класса: Person  
Пространство имен: <текущее пространство>  

Свойства:  
- Name (String)  
- Age (Int32)  

Методы:  
- get_Name  
- set_Name  
- get_Age  
- set_Age  
- SayHello  
- ToString  
- Equals  
- GetHashCode  
- GetType  

```

---

### 📌 3.2 Создание объекта через Reflection

Можно создавать экземпляры классов динамически:

```C#
Type type = typeof(Person);
object obj = Activator.CreateInstance(type);

Console.WriteLine($"Создан объект: {obj.GetType().Name}");

```

Если у класса есть параметризованный конструктор:

```C#
Type type = typeof(Person);
object obj = Activator.CreateInstance(type, "John", 30);

```

---

### 📌 3.3 Вызов методов через Reflection

```C#
Type type = typeof(Person);
object obj = Activator.CreateInstance(type);

MethodInfo method = type.GetMethod("SayHello");
method.Invoke(obj, null);
```
---

### 📌 3.4 Работа с полями и свойствами

```C#
Type type = typeof(Person);
object obj = Activator.CreateInstance(type);

PropertyInfo prop = type.GetProperty("Name");
prop.SetValue(obj, "Alice");

Console.WriteLine(prop.GetValue(obj)); // Выведет: Alice

```

---

## 4. Атрибуты в `C#`

**Атрибуты** — это метаданные, которые можно добавлять к классам, методам, свойствам и т. д. для хранения дополнительной информации.

### 📌 4.1 Встроенные атрибуты

Примеры стандартных атрибутов:
```C#
[Obsolete("Метод устарел, используйте новый метод NewMethod")]
public void OldMethod()
{
    Console.WriteLine("Этот метод устарел");
}

```

```C#
[Serializable] // Позволяет сериализовать класс
public class Person { }

```

---

### 📌 4.2 Создание собственных атрибутов

Можно создавать кастомные атрибуты, унаследовавшись от `Attribute`.

```C#
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
public class AuthorAttribute : Attribute
{
    public string Name { get; }
    public AuthorAttribute(string name) => Name = name;
}

```

Использование:

```C#
[Author("Иван Иванов")]
public class MyClass { }

```

---

### 📌 4.3 Чтение атрибутов через Reflection

```C#
Type type = typeof(MyClass);
AuthorAttribute attr = (AuthorAttribute)Attribute.GetCustomAttribute(type, typeof(AuthorAttribute));

if (attr != null)
{
    Console.WriteLine($"Автор класса: {attr.Name}");
}

```

---

## 5. Динамическая загрузка сборок (Plugins)

Можно загружать `.dll` во время работы:

```C#
Assembly asm = Assembly.LoadFrom("MyPlugin.dll");

Type pluginType = asm.GetType("MyPlugin.PluginClass");
object pluginInstance = Activator.CreateInstance(pluginType);

MethodInfo method = pluginType.GetMethod("Execute");
method.Invoke(pluginInstance, null);

```

---

## 6. Опасности и недостатки Reflection

- **Производительность**: Reflection медленнее обычного кода, так как требует больше операций в рантайме.
- **Отсутствие проверок на этапе компиляции**: Ошибки обнаруживаются только во время выполнения.
- **Безопасность**: Через Reflection можно получить доступ к приватным данным, что потенциально небезопасно.

**Как избежать проблем?**

- Используйте Reflection только при необходимости.
- Кэшируйте `Type`, `MethodInfo`, `PropertyInfo`, если вызываете их часто.
- Для DI и сериализации используйте существующие фреймворки (ASP.NET Core DI, Newtonsoft.Json).

---

## 7. Заключение

Reflection – мощный инструмент, который позволяет работать с метаданными и динамически изменять поведение программ. Его разумное использование помогает реализовать гибкие и расширяемые системы, но требует внимательного подхода из-за потенциальных проблем с производительностью и безопасностью.

💡 **Когда использовать?**  
✅ В DI-контейнерах  
✅ В ORM-фреймворках  
✅ В динамической загрузке плагинов  
✅ В автоматическом тестировании

❌ **Когда НЕ использовать?**  
❌ В высоконагруженных местах  
❌ Если можно обойтись обычным кодом