Фоновые сервисы в **ASP.NET Core** позволяют выполнять длительные операции **в фоновом режиме**, независимо от HTTP-запросов.

### **📌 Варианты фоновых сервисов:**

1. **IHostedService** – базовый интерфейс для фоновых задач
2. **BackgroundService** – упрощенный вариант `IHostedService`
3. **Worker Service** – отдельное консольное приложение для фоновых задач

---

# **1. IHostedService**

### **🔹 Что такое IHostedService?**

`IHostedService` – это интерфейс, позволяющий запустить фоновые процессы при старте приложения и корректно их завершить при остановке.

### **📌 Реализация IHostedService**

```C#
public class MyBackgroundService : IHostedService
{
    private Task _backgroundTask;
    private readonly CancellationTokenSource _cts = new();

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _backgroundTask = Task.Run(async () =>
        {
            while (!_cts.Token.IsCancellationRequested)
            {
                Console.WriteLine("Работаем в фоне...");
                await Task.Delay(1000, _cts.Token);
            }
        }, _cts.Token);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _cts.Cancel();
        return _backgroundTask ?? Task.CompletedTask;
    }
}

```

### **📌 Регистрация IHostedService в `Program.cs`**

```C#
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHostedService<MyBackgroundService>();

var app = builder.Build();
app.Run();

```

📌 **Задача будет работать в фоне, пока приложение запущено.**

---

# **2. BackgroundService (наследник IHostedService)**

### **🔹 Что такое BackgroundService?**

Это абстрактный класс, упрощающий работу с `IHostedService`.

### **📌 Реализация BackgroundService**

```C#
public class MyWorker : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            Console.WriteLine("Выполняем задачу в фоне...");
            await Task.Delay(1000, stoppingToken);
        }
    }
}

```

### **📌 Регистрация BackgroundService**

```C#
builder.Services.AddHostedService<MyWorker>();

```

📌 **Этот вариант удобнее, если нужно запустить бесконечный цикл с задержками.**

---

# **3. Worker Service (фоновый сервис в отдельном приложении)**

### **🔹 Что такое Worker Service?**

Это **самостоятельное приложение** для фоновых задач, не зависящее от веб-приложения.

### **📌 Создание Worker Service**

```sh
dotnet new worker -n MyWorkerService

```

📌 В проекте уже будет файл `Worker.cs` с кодом:

```C#
public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
        }
    }
}

```

📌 Запускается командой:

```sh
dotnet run

```

---

# **4. Использование Scoped сервисов в фоновых задачах**

Фоновые сервисы работают в **Singleton-контексте**, но иногда нужно использовать Scoped- или Transient-сервисы.

### **📌 Проблема: нельзя напрямую внедрять Scoped-сервис**

```C#
public class MyWorker : BackgroundService
{
    private readonly MyDbContext _dbContext; // ❌ Ошибка!

    public MyWorker(MyDbContext dbContext)
    {
        _dbContext = dbContext;
    }
}

```

📌 **Решение:** Внедрять `IServiceScopeFactory`

```C#
public class MyWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public MyWorker(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<MyDbContext>();
                // Работаем с БД
            }

            await Task.Delay(1000, stoppingToken);
        }
    }
}

```

📌 **Теперь `DbContext` создается внутри `scope` и не вызывает утечек памяти.**

---

# **5. Запуск фоновых задач по расписанию (Cron Jobs)**

Фоновые сервисы можно запускать **по расписанию**.

### **📌 Реализация через Timer**

```C#
public class TimedWorker : BackgroundService
{
    private Timer _timer;

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
        return Task.CompletedTask;
    }

    private void DoWork(object state)
    {
        Console.WriteLine($"Фоновая задача: {DateTime.Now}");
    }

    public override Task StopAsync(CancellationToken stoppingToken)
    {
        _timer?.Dispose();
        return Task.CompletedTask;
    }
}

```

📌 **Этот вариант подходит для периодических задач.**

---

# **📌 Итог**

|Вариант|Когда использовать?|Преимущества|Недостатки|
|---|---|---|---|
|**IHostedService**|Простые фоновые процессы|Полный контроль над процессом|Больше кода|
|**BackgroundService**|Длительные фоновые задачи|Упрощенная реализация|Меньше контроля|
|**Worker Service**|Отдельные фоновые процессы|Независимость от ASP.NET Core|Требует настройки деплоя|

🚀 **Выбирайте подходящий вариант в зависимости от требований проекта!**