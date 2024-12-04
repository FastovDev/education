Одиночка (Singleton, Синглтон) - порождающий паттерн, который гарантирует, что для определенного класса будет создан только один объект, а также предоставит к этому объекту точку доступа.

Когда надо использовать Синглтон? Когда необходимо, чтобы для класса существовал только один экземпляр

Синглтон позволяет создать объект только при его необходимости. Если объект не нужен, то он не будет создан. В этом отличие синглтона от глобальных переменных.

Классическая реализация данного шаблона проектирования на C# выглядит следующим образом:

```
class Singleton
{
    private static Singleton instance;
 
    private Singleton()
    {}
 
    public static Singleton getInstance()
    {
        if (instance == null)
            instance = new Singleton();
        return instance;
    }
}
```

В классе определяется статическая переменная - ссылка на конкретный экземпляр данного объекта и приватный конструктор. В статическом методе `getInstance()` этот конструктор вызывается для создания объекта, если, конечно, объект отсутствует и равен null.

Для применения паттерна Одиночка создадим небольшую программу. Например, на каждом компьютере можно одномоментно запустить только одну операционную систему. В этом плане операционная система будет реализоваться через паттерн синглтон:

```
class Program
{
    static void Main(string[] args)
    {
        Computer comp = new Computer();
        comp.Launch("Windows 8.1");
        Console.WriteLine(comp.OS.Name);
         
        // у нас не получится изменить ОС, так как объект уже создан    
        comp.OS = OS.getInstance("Windows 10");
        Console.WriteLine(comp.OS.Name);
         
        Console.ReadLine();
    }
}
class Computer
{
    public OS OS { get; set; }
    public void Launch(string osName)
    {
        OS = OS.getInstance(osName);
    }
}
class OS
{
    private static OS instance;
 
    public string Name { get; private set; }
 
    protected OS(string name)
    {
        this.Name=name;
    }
 
    public static OS getInstance(string name)
    {
        if (instance == null)
            instance = new OS(name);
        return instance;
    }
}
```

### Синглтон и многопоточность

При применении паттерна синглтон в многопоточным программах мы можем столкнуться с проблемой, которую можно описать следующим образом:

```
static void Main(string[] args)
{
    (new Thread(() =>
    {
        Computer comp2 = new Computer();
        comp2.OS = OS.getInstance("Windows 10");
        Console.WriteLine(comp2.OS.Name);
 
    })).Start();
 
    Computer comp = new Computer();
    comp.Launch("Windows 8.1");
    Console.WriteLine(comp.OS.Name);
    Console.ReadLine();
}
```

Здесь запускается дополнительный поток, который получает доступ к синглтону. Параллельно выполняется тот код, который идет запуска потока и который также обращается к синглтону. Таким образом, и главный, и дополнительный поток пытаются инициализировать синглтон нужным значением - "Windows 10", либо "Windows 8.1". Какое значение синглтон получит в итоге, предсказать в данном случае невозможно.

В итоге мы сталкиваемся с проблемой инициализации синглтона, когда оба потока одновременно обращаются к коду:

```
if (instance == null)
    instance = new OS(name);
```

Чтобы решить эту проблему, перепишем класс синглтона следующим образом:
```
class OS
{
    private static OS instance;
 
    public string Name { get; private set; }
    private static object syncRoot = new Object();
 
    protected OS(string name)
    {
        this.Name = name;
    }
 
    public static OS getInstance(string name)
    {
        if (instance == null)
        {
            lock (syncRoot)
            {
                if (instance == null)
                    instance = new OS(name);
            }
        }
        return instance;
    }
}
```

Чтобы избежать одновременного доступа к коду из разных потоков критическая секция заключается в блок lock.
### Другие реализации синглтона

Выше были рассмотрены общие стандартные реализации: потоконебезопасная и потокобезопасная реализации паттерна. Но есть еще ряд дополнительных реализаций, которые можно рассмотреть.
### Потокобезопасная реализация без использования lock

```
public class Singleton
{
    private static readonly Singleton instance = new Singleton();
 
    public string Date { get; private set; }
 
    private Singleton()
    {
        Date = System.DateTime.Now.TimeOfDay.ToString();
    }
 
    public static Singleton GetInstance()
    {
        return instance;
    }
}
```
Данная реализация также потокобезопасная, то есть мы можем использовать ее в потоках так:
```
(new Thread(() =>
{
    Singleton singleton1 = Singleton.GetInstance();
    Console.WriteLine(singleton1.Date);
})).Start();
 
Singleton singleton2 = Singleton.GetInstance();
Console.WriteLine(singleton2.Date);
```
### Lazy-реализация

Определение объекта синглтона в виде статического поля класса открывает нам дорогу к созданию Lazy-реализации паттерна Синглтон, то есть такой реализации, где данные будут инициализироваться только перед непосредственным использованием. Поскольку статические поля инициализируются перед первым доступом к статическому членам класса и перед вызовом статического конструктора (при его наличии). Однако здесь мы можем столкнуться с двумя трудностями.

Во-первых, класс синглтона может иметь множество статических переменных. Возможно, мы вообще не будем обращаться к объекту синглтона, а будем использовать какие-то другие статические переменные:
```
public class Singleton
{
    private static readonly Singleton instance = new Singleton();
    public static string text = "hello";
    public string Date { get; private set; }
         
    private Singleton()
    {
        Console.WriteLine($"Singleton ctor {DateTime.Now.TimeOfDay}");
        Date = System.DateTime.Now.TimeOfDay.ToString();
    }
 
    public static Singleton GetInstance()
    {
        Console.WriteLine($"GetInstance {DateTime.Now.TimeOfDay}");
        Thread.Sleep(500);
        return instance;
    }
}
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine($"Main {DateTime.Now.TimeOfDay}");
        Console.WriteLine(Singleton.text);
    }
}
```
В данном случае идет только обращение к переменной text, однако статическое поле instance также будет инициализировано. 

В данном случае мы видим, что статическое поле instance инициализировано.

Для решения этой проблемы выделим отдельный внутренний класс в рамках класса синглтона:
```
public class Singleton
{
    public string Date { get; private set; }
    public static string text = "hello";
    private Singleton()
    {
        Console.WriteLine($"Singleton ctor {DateTime.Now.TimeOfDay}");
        Date = DateTime.Now.TimeOfDay.ToString();
    }
 
    public static Singleton GetInstance()
    {
        Console.WriteLine($"GetInstance {DateTime.Now.TimeOfDay}");
        return Nested.instance;
    }
 
    private class Nested
    {
        internal static readonly Singleton instance = new Singleton();
    }
}
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine($"Main {DateTime.Now.TimeOfDay}");
        Console.WriteLine(Singleton.text);
    }
}
```
Теперь статическая переменная, которая представляет объект синглтона, определена во вложенном классе Nested. Чтобы к этой переменной можно было обращаться из класса синглтона, она имеет модификатор internal, в то же время сам класс Nested имеет модификатор private, что позволяет гарантировать, что данный класс будет доступен только из класса Singleton.

### Реализация через класс `Lazy<T>`

Еще один способ создания синглтона представляет использование класса `Lazy<T>:`

```
public class Singleton
{
    private static readonly Lazy<Singleton> lazy = 
        new Lazy<Singleton>(() => new Singleton());
 
    public string Name { get; private set; }
         
    private Singleton()
    {
        Name = System.Guid.NewGuid().ToString();
    }
     
    public static Singleton GetInstance()
    {
        return lazy.Value;
    }
}
```

### Использование в стандартных библиотеках .NET

Паттерн **Singleton (Одиночка)** широко используется в стандартных библиотеках .NET для создания единственного экземпляра класса, который доступен из любой точки приложения. Он помогает управлять состоянием или разделяемыми ресурсами, где нужно гарантировать, что существует только одна точка доступа.

#### **1. HttpClient (System.Net.Http)**

**Почему используется:**  
Создание нового экземпляра `HttpClient` для каждого запроса может привести к исчерпанию сокетов, так как `HttpClient` использует пул соединений. Вместо этого рекомендуется использовать один экземпляр для всего приложения.

**Как применяется паттерн:**  
Разработчики часто используют Singleton для хранения и повторного использования `HttpClient`.

```
public class HttpClientSingleton
{
    private static readonly HttpClient instance = new HttpClient();

    public static HttpClient Instance => instance;

    private HttpClientSingleton() { }
}
```


### **2. ConfigurationManager (System.Configuration)**

**Почему используется:**  
Этот класс предоставляет доступ к конфигурационным данным приложения. Конфигурация загружается один раз и затем используется повсеместно.

**Как применяется паттерн:**  
`ConfigurationManager` фактически работает как Singleton, предоставляя статический доступ к конфигурационным настройкам.

```
string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

```

### **3. LogManager (NLog, Serilog и другие библиотеки)**

**Почему используется:**  
Журналирование в большинстве случаев должно использовать единственный экземпляр логера, чтобы избежать конфликтов записи в один и тот же ресурс (например, файл или базу данных).

**Как применяется паттерн:**  
Большинство библиотек логирования предоставляют статические методы или объекты для доступа к логеру. Например, `LogManager` в NLog.

```
private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
logger.Info("Application started");

```


### **4. Random (System)**

**Почему используется:**  
Создание множества экземпляров `Random` в короткий промежуток времени может привести к генерации одинаковых чисел из-за одинакового начального времени (seed). Использование одного экземпляра решает эту проблему.

**Как применяется паттерн:**  
Обычно рекомендуется использовать общий экземпляр `Random` как Singleton.

```
public class RandomSingleton
{
    private static readonly Random instance = new Random();

    public static Random Instance => instance;

    private RandomSingleton() { }
}

```


### **5. TaskScheduler.Default (System.Threading.Tasks)**

**Почему используется:**  
`TaskScheduler.Default` предоставляет доступ к стандартному планировщику задач. Это глобальный объект, работающий как Singleton, чтобы задачи могли использовать общие ресурсы.

```
Task.Factory.StartNew(() => Console.WriteLine("Running"), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);

```

### **Преимущества использования Singleton в .NET:**

1. **Глобальная точка доступа:** Удобно для ресурсов, которые должны быть общими (например, конфигурации, соединения).
2. **Контроль жизненного цикла:** Один экземпляр управляется централизованно, что снижает риск утечек памяти.
3. **Экономия ресурсов:** Избегается создание дублирующих объектов.


### **Аналоги Singleton в .NET:**

1. **Dependency Injection (DI):**  
    Вместо Singleton часто используется DI для управления временем жизни объектов. Например, `AddSingleton` в ASP.NET Core:
    
    `services.AddSingleton<IMyService, MyService>();`
    
2. **Статические классы:**  
    В некоторых случаях вместо Singleton можно использовать статические классы, но они менее гибкие (нет интерфейсов, наследования и контроля за временем жизни).
    
3. **Пул объектов (Object Pooling):**  
    Позволяет переиспользовать объекты вместо создания нового экземпляра каждый раз. Это может быть полезно в ситуациях, где Singleton неуместен.


### **Когда использовать Singleton:**

1. Ресурс должен быть разделяемым и доступным из любой части приложения (например, логгер).
2. Объект должен быть создан только один раз и использоваться повторно (например, `HttpClient`).
3. Нужен централизованный контроль над состоянием (например, глобальная конфигурация).

Singleton — это мощный инструмент, но его следует использовать с осторожностью, чтобы избежать сложностей с тестированием и поддержкой кода.