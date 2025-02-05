Паттерн "Decompose by Business Capability" предполагает разбиение системы на микросервисы, каждый из которых соответствует отдельной бизнес-функции или способу взаимодействия с клиентами. Это помогает избежать излишней сложности и повысить автономность сервисов.

<h3>Пример реализации на C#</h3>

Допустим, у нас есть система для электронной коммерции, и мы решаем разделить её на несколько сервисов, например, для управления пользователями и для обработки заказов.

1. Сервис управления пользователями:

```C#
public class UserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public void RegisterUser(string username, string password)
    {
        // Логика регистрации пользователя
        _userRepository.Add(new User { Username = username, Password = password });
    }

    public User GetUserDetails(string username)
    {
        // Логика получения данных пользователя
        return _userRepository.GetByUsername(username);
    }
}

```

2. Сервис обработки заказов:

```C#
public class OrderService
{
    private readonly IOrderRepository _orderRepository;

    public OrderService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public void CreateOrder(string userId, List<string> items)
    {
        // Логика создания заказа
        var order = new Order(userId, items);
        _orderRepository.Add(order);
    }

    public Order GetOrderDetails(int orderId)
    {
        // Логика получения данных о заказе
        return _orderRepository.GetById(orderId);
    }
}

```

Здесь два сервиса: **UserService** для работы с пользователями и **OrderService** для управления заказами. Каждый сервис фокусируется на своей бизнес-области, что соответствует паттерну.