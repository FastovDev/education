Строитель (Builder) - шаблон проектирования, который инкапсулирует создание объекта и позволяет разделить его на различные этапы.

### Когда использовать паттерн Строитель?

- Когда процесс создания нового объекта не должен зависеть от того, из каких частей этот объект состоит и как эти части связаны между собой
    
- Когда необходимо обеспечить получение различных вариаций объекта в процессе его создания


Формальное определение на C# могло бы выглядеть так:

```
class Client
{
    void Main()
    {
        Builder builder = new ConcreteBuilder();
        Director director = new Director(builder);
        director.Construct();
        Product product = builder.GetResult();
    }
}
class Director
{
    Builder builder;
    public Director(Builder builder)
    {
        this.builder = builder;
    }
    public void Construct()
    {
        builder.BuildPartA();
        builder.BuildPartB();
        builder.BuildPartC();
    }
}
 
abstract class Builder
{
    public abstract void BuildPartA();
    public abstract void BuildPartB();
    public abstract void BuildPartC();
    public abstract Product GetResult();
}
 
class Product
{
    List<object> parts = new List<object>();
    public void Add(string part)
    {
        parts.Add(part);
    }
}
 
class ConcreteBuilder : Builder
{
    Product product = new Product();
    public override void BuildPartA()
    {
        product.Add("Part A");
    }
    public override void BuildPartB()
    {
        product.Add("Part B");
    }
    public override void BuildPartC()
    {
        product.Add("Part C");
    }
    public override Product GetResult()
    {
        return product;
    }
}
```

### Участники

- Product: представляет объект, который должен быть создан. В данном случае все части объекта заключены в списке parts.
    
- Builder: определяет интерфейс для создания различных частей объекта Product
    
- ConcreteBuilder: конкретная реализация Buildera. Создает объект Product и определяет интерфейс для доступа к нему
    
- Director: распорядитель - создает объект, используя объекты Builder
    

Рассмотрим применение паттерна на примере выпечки хлеба. Как известно, даже обычный хлеб включает множество компонентов. Мы можем использовать для представления хлеба и его компонентов следующие классы:

```
//мука
class Flour
{
    // какого сорта мука
    public string Sort { get; set; }
}
// соль
class Salt
{}
// пищевые добавки
class Additives
{
    public string Name { get; set; }
}
 
class Bread
{
    // мука
    public Flour Flour { get; set; }
    // соль
    public Salt Salt { get; set; }
    // пищевые добавки
    public Additives Additives { get; set; }
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
 
        if (Flour != null)
            sb.Append(Flour.Sort + "\n");
        if (Salt != null)
            sb.Append("Соль \n");
        if (Additives != null)
            sb.Append("Добавки: "+Additives.Name+" \n");
        return sb.ToString();
    }
}
```
Кстати в данном случае для построения строки используется класс StringBuilder.

Хлеб может иметь различную комбинацию компонентов: ржаной и пшеничной муки, соли, пищевых добавок. И нам надо обеспечить выпечку разных сортов хлеба. Для разных сортов хлеба может варьироваться конкретный набор компонентов, не все компоненты могут использоваться. И для этой задачи применим паттерн Builder:

```
class Program
{
    static void Main(string[] args)
    {
        // содаем объект пекаря
        Baker baker = new Baker();
        // создаем билдер для ржаного хлеба
        BreadBuilder builder = new RyeBreadBuilder();
        // выпекаем
        Bread ryeBread = baker.Bake(builder);
        Console.WriteLine(ryeBread.ToString());
        // оздаем билдер для пшеничного хлеба
        builder = new WheatBreadBuilder();
        Bread wheatBread = baker.Bake(builder);
        Console.WriteLine(wheatBread.ToString());
 
        Console.Read();
    }
}
// абстрактный класс строителя
abstract class BreadBuilder
{
    public Bread Bread { get; private set; }
    public void CreateBread()
    {
        Bread = new Bread();
    }
    public abstract void SetFlour();
    public abstract void SetSalt();
    public abstract void SetAdditives();
}
// пекарь
class Baker
{
    public Bread Bake(BreadBuilder breadBuilder)
    {
        breadBuilder.CreateBread();
        breadBuilder.SetFlour();
        breadBuilder.SetSalt();
        breadBuilder.SetAdditives();
        return breadBuilder.Bread;
    }
}
// строитель для ржаного хлеба
class RyeBreadBuilder : BreadBuilder
{
    public override void SetFlour()
    {
        this.Bread.Flour = new Flour { Sort = "Ржаная мука 1 сорт" };
    }
 
    public override void SetSalt()
    {
        this.Bread.Salt = new Salt();
    }
 
    public override void SetAdditives()
    {
        // не используется
    }
}
// строитель для пшеничного хлеба
class WheatBreadBuilder : BreadBuilder
{
    public override void SetFlour()
    {
        this.Bread.Flour = new Flour { Sort = "Пшеничная мука высший сорт" };
    }
 
    public override void SetSalt()
    {
        this.Bread.Salt = new Salt();
    }
 
    public override void SetAdditives()
    {
        this.Bread.Additives = new Additives { Name = "улучшитель хлебопекарный" };
    }
}
```

В данном случае с помощью конкретных строителей RyeBreadBuilder и WheatBreadBuilder создаются объекты Bread с определенным набором. В роли распорядителя выступает класс пекаря Baker, который вызывает методы конкретных строителей для построения нового объекта.

### Использование в стандартных библиотеках .NET 

В платформе .NET паттерн **Строитель (Builder)** активно используется в стандартных библиотеках для создания объектов, которые требуют сложной или поэтапной настройки. Вот основные случаи применения:

#### **1. StringBuilder (System.Text)**

**Почему используется:**  
Класс `StringBuilder` применяется для создания и изменения строк без создания промежуточных объектов, что делает его эффективным в случае частых изменений текста.

**Как применяется паттерн:**  
`StringBuilder` позволяет поэтапно настраивать строку через методы вроде `Append`, `Insert`, `Remove` и т.д. После завершения сборки строка может быть получена вызовом `ToString()`.

```
var builder = new StringBuilder();
builder.Append("Hello");
builder.Append(", ");
builder.Append("World!");
string result = builder.ToString();
```

#### **2. ConfigurationBuilder (Microsoft.Extensions.Configuration)**

**Почему используется:**  
Этот класс применяется для последовательного добавления различных источников конфигурации (файлы, переменные среды, пользовательские классы и т.д.).

**Как применяется паттерн:**  
Класс `ConfigurationBuilder` позволяет поэтапно добавлять источники конфигурации, вызывая методы `AddJsonFile`, `AddEnvironmentVariables` и т.д., а затем создавать объект `IConfiguration`.

```
var builder = new StringBuilder();
builder.Append("Hello");
builder.Append(", ");
builder.Append("World!");
string result = builder.ToString();
```

#### **3. HttpClientFactory (Microsoft.Extensions.Http)**

**Почему используется:**  
Для создания и настройки экземпляров `HttpClient` с общей конфигурацией, такой как тайм-ауты, заголовки или политика повторных попыток.

**Как применяется паттерн:**  
Используется метод `IHttpClientBuilder` для добавления различных опций, таких как обработчики, базовый адрес, политики и т.д.

```
services.AddHttpClient("MyClient", client => 
{ 
	client.BaseAddress = new Uri("https://api.example.com"); 
	client.DefaultRequestHeaders.Add("User-Agent", "MyApp"); 
});
```

#### **4. ObjectContext (Entity Framework)**

**Почему используется:**  
В старых версиях Entity Framework использовался для поэтапного построения запросов, которые затем транслировались в SQL-запросы.

**Как применяется паттерн:**  
Методы LINQ позволяют добавлять фильтры, сортировки и прочие элементы, постепенно собирая запрос.

```
var query = context.Users 
	.Where(u => u.IsActive) 
	.OrderBy(u => u.LastName);
```

##### **Почему используется паттерн Строитель:**

1. Упрощает процесс создания сложных объектов.
2. Обеспечивает пошаговую настройку объектов.
3. Позволяет изолировать процесс создания от конечного объекта.
#### **Аналоги:**

1. **Фабричный метод:** Создание объекта с помощью фабрики, но без сложной пошаговой настройки.  
    Пример: `DbProviderFactories.GetFactory()`.
2. **Fluent API:** Обеспечивает похожий подход для поэтапного построения, например, в `Entity Framework`.

**Основное отличие:** Строитель акцентируется на поэтапном процессе создания, а фабричные методы просто возвращают готовый объект.