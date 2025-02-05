Паттерн "Абстрактная фабрика" (Abstract Factory) предоставляет интерфейс для создания семейств взаимосвязанных объектов с определенными интерфейсами без указания конкретных типов данных объектов.

### Когда использовать абстрактную фабрику

- Когда система не должна зависеть от способа создания и компоновки новых объектов
    
- Когда создаваемые объекты должны использоваться вместе и являются взаимосвязанными

Формальное определение паттерна на языке C# может выглядеть следующим образом:

```C#
abstract class AbstractFactory
{
    public abstract AbstractProductA CreateProductA();
    public abstract AbstractProductB CreateProductB();
}
class ConcreteFactory1: AbstractFactory
{
    public override AbstractProductA CreateProductA()
    {
        return new ProductA1();
    }
         
    public override AbstractProductB CreateProductB()   
    {
        return new ProductB1(); 
    }
}
class ConcreteFactory2: AbstractFactory
{
    public override AbstractProductA CreateProductA()
    {
        return new ProductA2();
    }
         
    public override AbstractProductB CreateProductB()
    {
        return new ProductB2();
    }
}
 
abstract class AbstractProductA
{}
             
abstract class AbstractProductB     
{}
                 
class ProductA1: AbstractProductA   
{}
     
class ProductB1: AbstractProductB   
{}
 
class ProductA2: AbstractProductA   
{}
                 
class ProductB2: AbstractProductB       
{}      
             
class Client
{
    private AbstractProductA abstractProductA;
    private AbstractProductB abstractProductB;
 
    public Client(AbstractFactory factory)
    {
        abstractProductB = factory.CreateProductB();
        abstractProductA = factory.CreateProductA();
    }
    public void Run()
    { }
}
```

Паттерн определяет следующих участников:

- Абстрактные классы AbstractProductA и AbstractProductB определяют интерфейс для классов, объекты которых будут создаваться в программе.
    
- Конкретные классы ProductA1 / ProductA2 и ProductB1 / ProductB2 представляют конкретную реализацию абстрактных классов
    
- Абстрактный класс фабрики AbstractFactory определяет методы для создания объектов. Причем методы возвращают абстрактные продукты, а не их конкретные реализации.
    
- Конкретные классы фабрик ConcreteFactory1 и ConcreteFactory2 реализуют абстрактные методы базового класса и непосредственно определяют какие конкретные продукты использовать
    
- Класс клиента Client использует класс фабрики для создания объектов. При этом он использует исключительно абстрактный класс фабрики AbstractFactory и абстрактные классы продуктов AbstractProductA и AbstractProductB и никак не зависит от их конкретных реализаций
    

Посмотрим, как мы можем применить паттерн. Например, мы делаем игру, где пользователь должен управлять некими супергероями, при этом каждый супергерой имеет определенное оружие и определенную модель передвижения. Различные супергерои могут определяться комплексом признаков. Например, эльф может летать и должен стрелять из арбалета, другой супергерой должен бегать и управлять мечом. Таким образом, получается, что сущность оружия и модель передвижения являются взаимосвязанными и используются в комплексе. То есть имеется один из доводов в пользу использования абстрактной фабрики.

И кроме того, наша задача при проектировании игры абстрагировать создание супергероев от самого класса супергероя, чтобы создать более гибкую архитектуру. И для этого применим абстрактную фабрику:


```C#
class Program
{
    static void Main(string[] args)
    {
        Hero elf = new Hero(new ElfFactory());
        elf.Hit();
        elf.Run();
 
        Hero voin = new Hero(new VoinFactory());
        voin.Hit();
        voin.Run();
 
        Console.ReadLine();
    }
}

//абстрактный класс - оружие
abstract class Weapon
{
    public abstract void Hit();
}

// абстрактный класс движение
abstract class Movement
{
    public abstract void Move();
}
 
// класс арбалет
class Arbalet : Weapon
{
    public override void Hit()
    {
        Console.WriteLine("Стреляем из арбалета");
    }
}

// класс меч
class Sword : Weapon
{
    public override void Hit()
    {
        Console.WriteLine("Бьем мечом");
    }
}

// движение полета
class FlyMovement : Movement
{
    public override void Move()
    {
        Console.WriteLine("Летим");
    }
}

// движение - бег
class RunMovement : Movement
{
    public override void Move()
    {
        Console.WriteLine("Бежим");
    }
}

// класс абстрактной фабрики
abstract class HeroFactory
{
    public abstract Movement CreateMovement();
    public abstract Weapon CreateWeapon();
}

// Фабрика создания летящего героя с арбалетом
class ElfFactory : HeroFactory
{
    public override Movement CreateMovement()
    {
        return new FlyMovement();
    }
 
    public override Weapon CreateWeapon()
    {
            return new Arbalet();
    }
}

// Фабрика создания бегущего героя с мечом
class VoinFactory : HeroFactory
{
    public override Movement CreateMovement()
    {
        return new RunMovement();
    }
 
    public override Weapon CreateWeapon()
    {
        return new Sword();
    }
}

// клиент - сам супергерой
class Hero
{
    private Weapon weapon;
    private Movement movement;
    public Hero(HeroFactory factory)
    {
        weapon = factory.CreateWeapon();
        movement = factory.CreateMovement();
    }
    public void Run()
    {
        movement.Move();
    }
    public void Hit()
    {
        weapon.Hit();
    }
}
```

Таким образом, создание супергероя абстрагируется от самого класса супергероя. В то же время нельзя не отметить и недостатки шаблона. В частности, если нам захочется добавить в конфигурацию супергероя новый объект, например, тип одежды, то придется переделывать классы фабрик и класс супергероя. Поэтому возможности по расширению в данном паттерне имеют некоторые ограничения.

### Использование в стандартных библиотеках .NET

Паттерн **Абстрактная фабрика (Abstract Factory)** используется в платформе .NET для создания семейств связанных объектов, не указывая конкретные классы. Этот подход особенно полезен, когда необходимо создавать объекты, которые должны работать вместе, или при реализации зависимости от платформы/окружения.

### **1. DbProviderFactory (System.Data.Common)**

**Почему используется:**  
`DbProviderFactory` позволяет абстрагироваться от конкретного типа базы данных (SQL Server, MySQL, Oracle и т.д.) и создавать соответствующие объекты для работы с БД, такие как `DbConnection`, `DbCommand` и `DbDataAdapter`.

**Как применяется паттерн:**  
Абстрактная фабрика предоставляет методы для создания объектов конкретных типов, используя фабрики провайдеров.

```C#
string providerName = "System.Data.SqlClient";
DbProviderFactory factory = DbProviderFactories.GetFactory(providerName);

using DbConnection connection = factory.CreateConnection();
connection.ConnectionString = "your_connection_string";
connection.Open();

```

### **2. System.Text.Encoding**

**Почему используется:**  
`Encoding` предоставляет фабричные методы, которые возвращают кодировки для работы с текстом (UTF-8, ASCII и др.). Абстрактная фабрика позволяет скрыть детали реализации кодировок.

**Как применяется паттерн:**  
Вместо создания объектов вручную используется статические методы, возвращающие кодировки.

```C#
Encoding utf8 = Encoding.UTF8;
Encoding ascii = Encoding.ASCII;
byte[] bytes = utf8.GetBytes("Hello, World!");

```


### **3. System.Security.Cryptography**

**Почему используется:**  
Классы криптографии, такие как `Aes`, `RSA`, `SHA256`, используют фабричные методы для создания объектов алгоритмов шифрования. Это позволяет абстрагироваться от конкретной реализации алгоритма.

**Как применяется паттерн:**  
Фабрики обеспечивают создание конкретных реализаций интерфейсов или абстрактных классов, таких как `SymmetricAlgorithm` или `HashAlgorithm`.

```C#
using var aes = Aes.Create();
aes.Key = new byte[32]; // Установка ключа
aes.GenerateIV();       // Генерация вектора инициализации

```


### **4. IServiceProvider и DI-контейнеры (Microsoft.Extensions.DependencyInjection)**

**Почему используется:**  
Службы и зависимости в ASP.NET Core создаются через фабрики DI-контейнера. `IServiceProvider` по сути реализует паттерн "Абстрактная фабрика", предоставляя объекты по запросу.

**Как применяется паттерн:**  
DI-контейнер регистрирует зависимости и создает объекты, управляя их временем жизни.

```C#
var serviceProvider = new ServiceCollection()
    .AddTransient<IMyService, MyService>()
    .BuildServiceProvider();

IMyService myService = serviceProvider.GetService<IMyService>();

```


### **5. DependencyProperty и WPF (Windows Presentation Foundation)**

**Почему используется:**  
В WPF паттерн "Абстрактная фабрика" применяется для создания метаданных зависимостей (`DependencyProperty`) и их регистрации. Это позволяет гибко определять свойства и поведение UI-компонентов.

**Как применяется паттерн:**  
Для создания объектов `DependencyProperty` используется фабричный метод `DependencyProperty.Register`.

```C#
public static readonly DependencyProperty MyProperty = DependencyProperty.Register(
    "MyProperty", typeof(string), typeof(MyControl), new PropertyMetadata(default(string)));
```

### **Аналоги и альтернативы паттерну "Абстрактная фабрика":**

1. **Фабричный метод:**  
    Вместо создания семейств объектов создается один объект определенного типа. Например, `Task.Factory.StartNew()` в `System.Threading.Tasks`.
    
2. **Builder (Строитель):**  
    Используется для пошагового создания сложных объектов. В отличие от абстрактной фабрики, акцент на поэтапной настройке.
    
3. **DI-контейнеры:**  
    Современные контейнеры внедрения зависимостей (например, в ASP.NET Core) заменяют многие сценарии использования абстрактной фабрики.
    
4. **Прототип:**  
    Иногда для создания семейств объектов используется клонирование вместо фабрики.


### **Когда использовать абстрактную фабрику:**

1. Когда нужно создавать семейства связанных объектов (например, для работы с разными БД или алгоритмами).
2. Когда детали реализации должны быть скрыты за интерфейсами или базовыми классами.
3. Когда объекты, создаваемые фабрикой, должны быть взаимосвязаны.

Абстрактная фабрика особенно полезна в проектах, где важна независимость от платформы, библиотеки или технологии.