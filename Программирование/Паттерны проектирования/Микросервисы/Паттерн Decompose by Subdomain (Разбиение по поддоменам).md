Паттерн "Decompose by Subdomain" предполагает разбиение системы на микросервисы в соответствии с доменными подпространствами (субдоменами). Этот подход основывается на концепциях **Domain-Driven Design** (DDD) и предполагает, что каждый микросервис должен соответствовать отдельному бизнес-поддомену. Сервисы взаимодействуют друг с другом через явные API, и каждый из них имеет свою собственную область ответственности, что способствует лучшему разделению логики.
 <h3>Пример реализации на C#</h3>

Допустим, у нас есть система для управления библиотекой. Мы можем выделить несколько поддоменов:

- **Управление книгами** (Book Management)
- **Управление пользователями** (User Management)
- **Управление заказами** (Order Management)

Каждый из этих поддоменов будет отдельным микросервисом.

**1. Сервис управления книгами (Book Management)**

```
public class BookService
{
    private readonly IBookRepository _bookRepository;

    public BookService(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public void AddBook(string title, string author)
    {
        // Логика добавления книги в систему
        var book = new Book { Title = title, Author = author };
        _bookRepository.Add(book);
    }

    public List<Book> GetAllBooks()
    {
        // Логика получения всех книг
        return _bookRepository.GetAll();
    }
}

```

**2. Сервис управления пользователями (User Management)**

```
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
        var user = new User { Username = username, Password = password };
        _userRepository.Add(user);
    }

    public User GetUser(string username)
    {
        // Логика получения данных о пользователе
        return _userRepository.GetByUsername(username);
    }
}

```

**3. Сервис управления заказами (Order Management)**

```
public class OrderService
{
    private readonly IOrderRepository _orderRepository;

    public OrderService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public void CreateOrder(string userId, List<string> bookIds)
    {
        // Логика создания заказа на книги
        var order = new Order { UserId = userId, BookIds = bookIds };
        _orderRepository.Add(order);
    }

    public Order GetOrderDetails(int orderId)
    {
        // Логика получения информации о заказе
        return _orderRepository.GetById(orderId);
    }
}

```

### Объяснение:

- **BookService**: Сервис, который управляет книгами (добавление, поиск).
- **UserService**: Сервис для работы с пользователями (регистрация, получение информации).
- **OrderService**: Сервис для обработки заказов (создание заказа, получение информации).

Каждый сервис отвечает за отдельный **субдомен**. Это позволяет масштабировать, разрабатывать и обновлять каждый из микросервисов независимо.