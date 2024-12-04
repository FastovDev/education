Фабричный метод (Factory Method) - это паттерн, который определяет интерфейс для создания объектов некоторого класса, но непосредственное решение о том, объект какого класса создавать происходит в подклассах. То есть паттерн предполагает, что базовый класс делегирует создание объектов классам-наследникам.

Формальное определение паттерна на языке C# может выглядеть следующим образом:

```
abstract class Product
{}
 
class ConcreteProductA : Product
{}
 
class ConcreteProductB : Product
{}
 
abstract class Creator
{
    public abstract Product FactoryMethod();
}
 
class ConcreteCreatorA : Creator
{
    public override Product FactoryMethod() { return new ConcreteProductA(); }
}
 
class ConcreteCreatorB : Creator
{
    public override Product FactoryMethod() { return new ConcreteProductB(); }
}
```

### Участники

- Абстрактный класс Product определяет интерфейс класса, объекты которого надо создавать.
    
- Конкретные классы ConcreteProductA и ConcreteProductB представляют реализацию класса Product. Таких классов может быть множество
    
- Абстрактный класс Creator определяет абстрактный фабричный метод `FactoryMethod()`, который возвращает объект Product.
    
- Конкретные классы ConcreteCreatorA и ConcreteCreatorB - наследники класса Creator, определяющие свою реализацию метода `FactoryMethod()`. Причем метод `FactoryMethod()` каждого отдельного класса-создателя возвращает определенный конкретный тип продукта. Для каждого конкретного класса продукта определяется свой конкретный класс создателя.

Таким образом, класс Creator делегирует создание объекта Product своим наследникам. А классы ConcreteCreatorA и ConcreteCreatorB могут самостоятельно выбирать какой конкретный тип продукта им создавать.

Теперь рассмотрим на реальном примере. Допустим, мы создаем программу для сферы строительства. Возможно, вначале мы захотим построить многоэтажный панельный дом. И для этого выбирается соответствующий подрядчик, который возводит каменные дома. Затем нам захочется построить деревянный дом и для этого также надо будет выбрать нужного подрядчика:

```
class Program
{
    static void Main(string[] args)
    {
        Developer dev = new PanelDeveloper("ООО КирпичСтрой");
        House house2 = dev.Create();
         
        dev = new WoodDeveloper("Частный застройщик");
        House house = dev.Create();
 
        Console.ReadLine();
    }
}

// абстрактный класс строительной компании
abstract class Developer
{
    public string Name { get; set; }
 
    public Developer (string n)
    { 
        Name = n; 
    }
    // фабричный метод
    abstract public House Create();
}

// строит панельные дома
class PanelDeveloper : Developer
{
    public PanelDeveloper(string n) : base(n)
    { }
 
    public override House Create()
    {
        return new PanelHouse();
    }
}

// строит деревянные дома
class WoodDeveloper : Developer
{ 
    public WoodDeveloper(string n) : base(n)
    { }
 
    public override House Create()
    {
        return new WoodHouse();
    }
}
 
abstract class House
{ }
 
class PanelHouse : House 
{ 
    public PanelHouse()
    {
        Console.WriteLine("Панельный дом построен");
    }
}
class WoodHouse : House
{ 
    public WoodHouse()
    {
        Console.WriteLine("Деревянный дом построен");
    }
}
```


В качестве абстрактного класса Product здесь выступает класс House. Его две конкретные реализации - PanelHouse и WoodHouse представляют типы домов, которые будут строить подрядчики. В качестве абстрактного класса создателя выступает Developer, определяющий абстрактный метод `Create()`. Этот метод реализуется в классах-наследниках WoodDeveloper и PanelDeveloper. И если в будущем нам потребуется построить дома какого-то другого типа, например, кирпичные, то мы можем с легкостью создать новый класс кирпичных домов, унаследованный от House, и определить класс соответствующего подрядчика. Таким образом, система получится легко расширяемой. Правда, недостатки паттерна тоже очевидны - для каждого нового продукта необходимо создавать свой класс создателя.


### Использование в стандартных библиотеках .NET


### **1. Task.Factory.StartNew (System.Threading.Tasks)**

**Почему используется:**  
Фабричный метод предоставляет удобный способ создавать задачи (`Task`) с определенными параметрами. Это позволяет абстрагироваться от деталей создания задач.

**Как применяется:**  
Метод `Task.Factory.StartNew` инкапсулирует создание задачи с заданными действиями, опциями и планировщиком.

```
Task task = Task.Factory.StartNew(() =>
{
    Console.WriteLine("Task is running");
});

```

### **2. DbProviderFactories.GetFactory (System.Data.Common)**

**Почему используется:**  
Этот фабричный метод позволяет создать объект `DbProviderFactory` для работы с конкретной базой данных. Это делает код независимым от провайдера, например, SQL Server, MySQL, Oracle.

**Как применяется:**  
Метод возвращает экземпляр фабрики для заданного провайдера.


```
DbProviderFactory factory = DbProviderFactories.GetFactory("System.Data.SqlClient");
DbConnection connection = factory.CreateConnection();
connection.ConnectionString = "your_connection_string";
connection.Open();

```


### **3. XmlReader.Create и XmlWriter.Create (System.Xml)**

**Почему используется:**  
Эти фабричные методы предоставляют абстракцию для работы с различными источниками данных (файлами, потоками и т.д.), скрывая детали реализации.

**Как применяется:**  
Методы `XmlReader.Create` и `XmlWriter.Create` возвращают объекты, сконфигурированные для чтения или записи XML.


```
using XmlReader reader = XmlReader.Create("data.xml");
while (reader.Read())
{
    Console.WriteLine(reader.Name);
}

```

### **4. HttpClientFactory.CreateClient (Microsoft.Extensions.Http)**

**Почему используется:**  
В ASP.NET Core фабрика `HttpClientFactory` используется для создания настроенных клиентов `HttpClient`. Это позволяет управлять временем жизни клиентов и их настройками через Dependency Injection.

**Как применяется:**  
Метод `CreateClient` возвращает объект `HttpClient`, сконфигурированный согласно определенным настройкам.

```
var client = httpClientFactory.CreateClient("MyClient");
var response = await client.GetAsync("https://api.example.com");

```

### **5. LoggerFactory.CreateLogger (Microsoft.Extensions.Logging)**

**Почему используется:**  
Фабрика `LoggerFactory` предоставляет способ создавать логеры для различных классов или категорий. Это упрощает настройку и использование логирования.

**Как применяется:**  
Метод `CreateLogger` возвращает объект логера для заданной категории.

```
ILogger logger = loggerFactory.CreateLogger("MyCategory");
logger.LogInformation("Application started");

```

### **6. Activator.CreateInstance (System)**

**Почему используется:**  
Этот фабричный метод позволяет создавать объекты динамически на основе типа, переданного в качестве аргумента. Это полезно в ситуациях, когда тип известен только во время выполнения.

**Как применяется:**  
Метод `CreateInstance` возвращает объект указанного типа.

```
Type type = typeof(StringBuilder);
object instance = Activator.CreateInstance(type);

```

### **7. ServiceProvider.GetService (Microsoft.Extensions.DependencyInjection)**

**Почему используется:**  
Фабричный метод `GetService` в DI-контейнерах используется для создания или получения объекта зарегистрированного типа. Это заменяет ручное управление зависимостями.

**Как применяется:**  
`GetService` создает объект с учетом его зависимостей, определенных в контейнере.

```
IMyService service = serviceProvider.GetService<IMyService>();

```

### **Аналоги и альтернативы фабричного метода в .NET:**

1. **Абстрактная фабрика:**  
    Используется для создания семейств связанных объектов, тогда как фабричный метод фокусируется на создании одного типа.
    
2. **Статические фабричные методы:**  
    Например, `Encoding.UTF8` или `Aes.Create`. Эти методы также предоставляют объекты, но не требуют создания экземпляра фабрики.
    
3. **Dependency Injection (DI):**  
    Современные DI-контейнеры заменяют фабричные методы в большинстве приложений, предоставляя более гибкий способ управления зависимостями.
    
4. **Прототип:**  
    Если объекты часто копируются, вместо фабрики можно использовать прототипы (`ICloneable` или `MemberwiseClone`).


### **Когда использовать фабричный метод:**

1. Если тип создаваемого объекта может изменяться в зависимости от условий.
2. Если требуется централизованное управление созданием объектов.
3. Если нужно избежать жесткой привязки к конкретным классам.

Фабричный метод — это мощный и универсальный паттерн, который активно применяется в стандартных библиотеках .NET для обеспечения гибкости и читаемости кода.