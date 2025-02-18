Event Sourcing — это паттерн проектирования, при котором изменения состояния приложения не сохраняются напрямую, а записываются в виде последовательности событий. Вместо того чтобы хранить актуальное состояние объекта, система хранит все произошедшие с ним изменения, а текущее состояние вычисляется путём последовательного «проигрывания» этих событий. Это позволяет не только реконструировать состояние в любой момент времени, но и обеспечивает полноценный аудит и возможность отката изменений.

---

## 1. Основные Принципы Event Sourcing

- **Запись событий вместо состояния:** Вместо обновления текущего состояния сущности, каждое изменение фиксируется в виде события. Например, создание заказа, обновление статуса или оплата — все это фиксируется как отдельные события.
- **Агрегация событий:** Текущее состояние объекта определяется путем последовательного применения всех его событий. При необходимости можно «пересобрать» состояние, воспроизведя историю событий.
- **Неизменяемость событий:** После записи событие не изменяется. Это обеспечивает надёжный аудит и историческую точность.
- **Поддержка CQRS:** Event Sourcing часто используется вместе с разделением команд и запросов (Command Query Responsibility Segregation), что позволяет оптимизировать чтение и запись данных по-разному.

---

## 2. Привязка к ASP.NET Core

ASP.NET Core предоставляет все необходимые инструменты для реализации Event Sourcing:

- **Dependency Injection:** Регистрация сервисов, таких как хранилище событий (Event Store) или обработчики событий, становится интуитивной благодаря встроенной поддержке DI.
- **Middleware и Фильтры:** Можно интегрировать в пайплайн обработки запросов механизмы логирования или публикации событий.
- **Web API:** Контроллеры ASP.NET Core могут выступать в роли входных точек для команд, инициирующих изменения в агрегатах.

### Структура проекта

При реализации Event Sourcing в ASP.NET Core проект можно разделить на следующие слои:

- **Domain:** Содержит определения агрегатов, событий и интерфейсов для хранилища событий.
- **Infrastructure:** Реализация хранилища событий (например, с использованием базы данных, EventStoreDB или даже in-memory решения для прототипов).
- **Application:** Сервисы, управляющие агрегатами, применяющие события и публикующие их при необходимости.
- **Presentation:** API-контроллеры, принимающие команды от клиента и инициирующие обработку событий.

---

## 3. Пример Использования

### Определение доменных событий

```C#
public interface IDomainEvent { }

public class OrderCreatedEvent : IDomainEvent
{
    public Guid OrderId { get; }
    public string CustomerName { get; }
    public DateTime CreatedAt { get; }

    public OrderCreatedEvent(Guid orderId, string customerName, DateTime createdAt)
    {
        OrderId = orderId;
        CustomerName = customerName;
        CreatedAt = createdAt;
    }
}

public class OrderCompletedEvent : IDomainEvent
{
    public Guid OrderId { get; }
    public DateTime CompletedAt { get; }

    public OrderCompletedEvent(Guid orderId, DateTime completedAt)
    {
        OrderId = orderId;
        CompletedAt = completedAt;
    }
}

```

### Базовый класс агрегата с Event Sourcing

```C#
public abstract class AggregateRoot
{
    public Guid Id { get; protected set; }
    private readonly List<IDomainEvent> _uncommittedEvents = new List<IDomainEvent>();

    public IEnumerable<IDomainEvent> GetUncommittedEvents() => _uncommittedEvents.AsReadOnly();

    protected void ApplyChange(IDomainEvent @event, bool isNew = true)
    {
        // Обновляем состояние агрегата с помощью метода When
        ((dynamic)this).When((dynamic)@event);
        
        if (isNew)
            _uncommittedEvents.Add(@event);
    }

    public void MarkEventsAsCommitted()
    {
        _uncommittedEvents.Clear();
    }
}

```

### Пример агрегата — Заказ

```C#
public class OrderAggregate : AggregateRoot
{
    public string CustomerName { get; private set; }
    public bool IsCompleted { get; private set; }

    // Пустой конструктор для восстановления из истории событий
    public OrderAggregate() { }

    // Фабричный метод для создания нового заказа
    public static OrderAggregate Create(Guid id, string customerName)
    {
        var order = new OrderAggregate();
        var @event = new OrderCreatedEvent(id, customerName, DateTime.UtcNow);
        order.ApplyChange(@event);
        return order;
    }

    public void CompleteOrder()
    {
        if (IsCompleted)
            throw new InvalidOperationException("Заказ уже завершён.");

        var @event = new OrderCompletedEvent(this.Id, DateTime.UtcNow);
        ApplyChange(@event);
    }

    // Обработчики событий (методы When)
    private void When(OrderCreatedEvent @event)
    {
        Id = @event.OrderId;
        CustomerName = @event.CustomerName;
        IsCompleted = false;
    }

    private void When(OrderCompletedEvent @event)
    {
        IsCompleted = true;
    }
}

```

### Интерфейс и реализация хранилища событий

```C#
public interface IEventStore
{
    Task SaveEventsAsync(Guid aggregateId, IEnumerable<IDomainEvent> events, int expectedVersion);
    Task<List<IDomainEvent>> GetEventsAsync(Guid aggregateId);
}

// Пример простой in-memory реализации
public class InMemoryEventStore : IEventStore
{
    private readonly Dictionary<Guid, List<IDomainEvent>> _store = new Dictionary<Guid, List<IDomainEvent>>();

    public Task SaveEventsAsync(Guid aggregateId, IEnumerable<IDomainEvent> events, int expectedVersion)
    {
        if (!_store.ContainsKey(aggregateId))
            _store[aggregateId] = new List<IDomainEvent>();

        // В production-решениях следует проверять версию для избежания конфликтов
        _store[aggregateId].AddRange(events);
        return Task.CompletedTask;
    }

    public Task<List<IDomainEvent>> GetEventsAsync(Guid aggregateId)
    {
        if (_store.ContainsKey(aggregateId))
            return Task.FromResult(_store[aggregateId]);

        throw new Exception("Агрегат не найден");
    }
}

```

### Интеграция с ASP.NET Core (Контроллер)

```C#
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IEventStore _eventStore;

    public OrdersController(IEventStore eventStore)
    {
        _eventStore = eventStore;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateOrder([FromBody] string customerName)
    {
        var orderId = Guid.NewGuid();
        var order = OrderAggregate.Create(orderId, customerName);

        await _eventStore.SaveEventsAsync(orderId, order.GetUncommittedEvents(), 0);
        order.MarkEventsAsCommitted();

        return Ok(new { OrderId = orderId, Message = "Заказ создан" });
    }

    [HttpPost("{id}/complete")]
    public async Task<IActionResult> CompleteOrder(Guid id)
    {
        // Восстанавливаем агрегат из истории событий
        var events = await _eventStore.GetEventsAsync(id);
        var order = new OrderAggregate();
        foreach (var @event in events)
        {
            order.ApplyChange(@event, isNew: false);
        }

        order.CompleteOrder();
        await _eventStore.SaveEventsAsync(id, order.GetUncommittedEvents(), events.Count);
        order.MarkEventsAsCommitted();

        return Ok("Заказ завершён");
    }
}

```

### Регистрация зависимостей в `Program.cs` (или `Startup.cs`)

`builder.Services.AddSingleton<IEventStore, InMemoryEventStore>();`

---

## 4. Сравнительный Анализ с Другими Подходами

### Традиционные CRUD операции

- **CRUD:** Изменения состояния записываются напрямую в базу данных (например, через обновление строк).  
    **Плюсы:** Простота реализации, понятность.  
    **Минусы:** Нет истории изменений, сложнее отслеживать причины ошибок и проводить аудит.
    
- **Event Sourcing:** Вместо непосредственного изменения состояния сохраняется последовательность событий.  
    **Плюсы:** Полная история изменений, возможность воспроизведения состояния на любой момент времени, улучшенная трассировка и аудит, поддержка аналитики и отладки.  
    **Минусы:** Сложность реализации, необходимость в механизмах для оптимизации (например, создание снимков — snapshots), возможная сложность в обработке версионирования событий.
    

### CQRS (Command Query Responsibility Segregation)

- **CQRS:** Разделение операций чтения и записи может улучшить масштабируемость и производительность.
- **Связь с Event Sourcing:** Часто используются вместе, так как события могут асинхронно реплицироваться в модели чтения, позволяя поддерживать разные представления данных.

### Ситуации, когда Event Sourcing особенно полезен

- **Сложные доменные модели:** Когда бизнес-логика включает сложные изменения состояния с большим числом сценариев.
- **Требования аудита и истории:** Если необходимо иметь детальную историю всех изменений для аудита, отладки или соответствия требованиям.
- **Аналитика и прогнозирование:** Возможность анализировать цепочку событий для извлечения бизнес-инсайтов.

---

## 5. Когда Использовать Event Sourcing

Использование Event Sourcing оправдано, когда:

- Требуется полный аудит и отслеживание истории изменений.
- Приложение имеет сложную бизнес-логику с множеством состояний и переходов.
- Важна возможность восстановления состояния системы на любую точку во времени.
- Применяется CQRS для разделения операций записи и чтения.

Однако для простых CRUD-приложений или MVP данная архитектура может оказаться избыточной из-за дополнительной сложности в реализации и поддержки.

---

## 6. Заключение

Event Sourcing — мощный паттерн, позволяющий сохранять всю историю изменений системы в виде событий. В ASP.NET Core его можно реализовать с помощью встроенной системы Dependency Injection, контроллеров и middleware. Такой подход обеспечивает:

- Полноту аудита и возможность восстановления состояния;
- Гибкость в обработке изменений и анализе бизнес-процессов;
- Возможность интеграции с CQRS для повышения масштабируемости.

При выборе архитектуры важно оценивать сложность бизнес-логики и требования к аудиту: для небольших систем традиционные CRUD-операции могут быть более подходящими, а для сложных доменных моделей с критическими требованиями к истории изменений Event Sourcing станет отличным решением.