**Domain-Driven Design (DDD)** — это **подход к разработке программного обеспечения**, который фокусируется на построении **модели предметной области (Domain Model)**.

### **Основные принципы DDD:**

1. **Модель предметной области в центре внимания** — код должен точно отражать бизнес-логику.
2. **Единый язык (Ubiquitous Language)** — разработчики и бизнес должны использовать **одинаковые термины**.
3. **Разделение модели на слои** — предметная область, инфраструктура, UI и приложение четко разделены.
4. **Инкапсуляция бизнес-логики** — правила предметной области не должны находиться в сервисах, контроллерах или репозиториях.
5. **Гибкость и возможность эволюции** — модель должна **адаптироваться к изменениям** в бизнес-правилах.

---

## **Ключевые понятия DDD**

DDD делит код на **основные компоненты**, которые формируют **модель предметной области**.

### **1. Entities (Сущности)**

Сущность — это объект, который имеет **уникальный идентификатор (ID)** и **изменяемое состояние**.

```C#
public class Order
{
    public Guid Id { get; private set; }
    public string CustomerName { get; private set; }
    public List<OrderItem> Items { get; private set; } = new();

    public Order(Guid id, string customerName)
    {
        Id = id;
        CustomerName = customerName;
    }

    public void AddItem(OrderItem item)
    {
        Items.Add(item);
    }
}

```

✅ **Когда использовать:**

- Если объект **имеет идентичность** (например, пользователь, заказ, товар).
- Если его **состояние меняется со временем**.

---

### **2. Value Objects (Объекты-значения)**

Объекты-значения **не имеют уникального ID** и **неизменяемы (immutable)**.

**Пример:**

```C#
public class Address
{
    public string Street { get; }
    public string City { get; }
    public string ZipCode { get; }

    public Address(string street, string city, string zipCode)
    {
        Street = street;
        City = city;
        ZipCode = zipCode;
    }
}

```

✅ **Когда использовать:**

- Если **идентичность не важна** (например, адрес, деньги, координаты).
- Если **объект неизменяемый** (immutable).

---

### **3. Aggregate Root (Корень агрегата)**

Агрегат — это **группа связанных сущностей** с **единым корнем (Aggregate Root)**.

**Пример:**

```C#
public class Order : IAggregateRoot
{
    public Guid Id { get; private set; }
    public List<OrderItem> Items { get; private set; } = new();

    public void AddItem(OrderItem item)
    {
        Items.Add(item);
    }
}

```

✅ **Когда использовать:**

- Если сущности должны **изменяться как единое целое**.
- Если нужна **гарантия согласованности данных**.

---

### **4. Domain Events (События предметной области)**

Позволяют моделировать **реальные события в системе**.

**Пример:**

```C#
public class OrderCreatedEvent : IDomainEvent
{
    public Guid OrderId { get; }

    public OrderCreatedEvent(Guid orderId)
    {
        OrderId = orderId;
    }
}

```

**Обработчик события:**

```C#
public class OrderCreatedEventHandler : INotificationHandler<OrderCreatedEvent>
{
    public Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Order {notification.OrderId} created!");
        return Task.CompletedTask;
    }
}

```

✅ **Когда использовать:**

- Если нужно **разделить бизнес-логику**.
- Если важна **реакция на изменения состояния системы**.

---

### **5. Repositories (Репозитории)**

Репозитории скрывают **детали работы с базой данных** и обеспечивают доступ к агрегатам.

**Пример:**

```C#
public interface IOrderRepository
{
    Order GetById(Guid id);
    void Save(Order order);
}

public class OrderRepository : IOrderRepository
{
    private readonly DbContext _context;

    public OrderRepository(DbContext context)
    {
        _context = context;
    }

    public Order GetById(Guid id)
    {
        return _context.Set<Order>().Find(id);
    }

    public void Save(Order order)
    {
        _context.Set<Order>().Update(order);
        _context.SaveChanges();
    }
}

```

✅ **Когда использовать:**

- Если агрегаты **должны загружаться единым объектом**.
- Если **важна инкапсуляция работы с БД**.

---

### **6. Services (Доменные сервисы)**

Сервисы нужны, когда **логика не принадлежит одной сущности**.

**Пример:**

```C#
public class OrderService
{
    private readonly IOrderRepository _orderRepository;

    public OrderService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public void PlaceOrder(Order order)
    {
        _orderRepository.Save(order);
    }
}

```

✅ **Когда использовать:**

- Если бизнес-логика **не принадлежит одной сущности**.
- Если логика **оперирует несколькими агрегатами**.

---

## **Архитектура DDD в ASP.NET Core**

DDD предполагает **четкое разделение на слои**:

- **Application Layer** — отвечает за API и сценарии приложения.
- **Domain Layer** — содержит **модель предметной области** (сущности, агрегаты, сервисы).
- **Infrastructure Layer** — отвечает за **БД, логирование, внешние API**.
- **Presentation Layer** — UI (например, Razor Pages, Blazor, MVC).

### **Пример структуры проекта**

📂 MyApp
 ├── 📂 MyApp.Application
 │   ├── Services/
 │   ├── DTOs/
 │   ├── Commands/
 │   ├── Queries/
 ├── 📂 MyApp.Domain
 │   ├── Entities/
 │   ├── ValueObjects/
 │   ├── Services/
 │   ├── Events/
 ├── 📂 MyApp.Infrastructure
 │   ├── Repositories/
 │   ├── Data/
 ├── 📂 MyApp.API
 │   ├── Controllers/
 │   ├── Middlewares/


✅ **Плюсы структуры:**

- Четкое **разделение ответственности**
- Гибкость и возможность **масштабирования**
- Код легко **тестируется**

---

## **Когда применять DDD?**

✅ Сложные **бизнес-домены** (финансы, здравоохранение, логистика)  
✅ **Часто изменяющиеся бизнес-правила**  
✅ Проекты с долгим жизненным циклом

## **Когда НЕ применять DDD?**

❌ **Простые CRUD-приложения**  
❌ **Маленькие проекты**, где DDD избыточен  
❌ Когда **нет сложных бизнес-правил**

---

## **Вывод**

📌 **DDD помогает строить чистую архитектуру**, разделять логику и работать с **реальными бизнес-процессами**. В ASP.NET Core можно легко применять DDD вместе с **EF Core, MediatR, CQRS и Event Sourcing**.

📌 Но **DDD усложняет разработку**, поэтому его стоит применять **только для сложных предметных областей**.