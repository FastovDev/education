**Clean Architecture** — это архитектурный паттерн, предложенный Робертом Мартином (Uncle Bob), который нацелен на создание хорошо организованных, модульных, поддерживаемых и тестируемых приложений. Основная идея заключается в разделении приложения на слои, где каждый слой имеет чётко определённую ответственность, и зависимости между слоями направлены только внутрь. Это помогает избежать сильной связности и улучшает гибкость архитектуры.

В **Clean Architecture** выделяются несколько слоёв, каждый из которых отвечает за определённый аспект системы:

1. **Core (внутренний уровень)**: Содержит бизнес-логику и правила. Этот слой не зависит от других слоёв.
2. **Use Cases**: Описание бизнес-операций и действий пользователя.
3. **Interface Adapters**: Адаптирует данные между внутренними слоями и внешними (например, запросы и ответы от веб-приложений).
4. **External Interfaces (внешний уровень)**: Взаимодействие с внешними компонентами, такими как базы данных, API, UI и т.д.

### Основные принципы Clean Architecture:

1. **Разделение ответственности**: Каждый слой в архитектуре отвечает за свою конкретную задачу. Например, бизнес-логика не должна зависеть от базы данных или пользовательского интерфейса.
2. **Независимость от фреймворков и технологий**: Внутренние слои не должны зависеть от конкретных технологий (например, от базы данных или веб-фреймворков). Это позволяет легко менять внешние компоненты, не затрагивая бизнес-логику.
3. **Тестируемость**: Логика приложения и её бизнес-правила независимы от UI, базы данных или сторонних сервисов, что делает приложение более тестируемым.
4. **Инверсия зависимостей (Dependency Inversion Principle)**: Зависимости должны быть направлены внутрь, т.е. внешний код зависит от абстракций, а не от конкретных реализаций.

### Основные слои Clean Architecture

1. **Entities (Сущности)**:
    
    - Это объекты с бизнес-логикой и правилами, которые являются независимыми от внешних технологий.
    - Например, это могут быть объекты, представляющие пользователей, заказы и другие сущности в приложении.
2. **Use Cases (Применение)**:
    
    - Этот слой определяет, какие операции выполняются в системе, и как они должны быть выполнены.
    - Например, обработка запроса на создание нового пользователя или выполнение транзакции.
3. **Interface Adapters (Адаптеры интерфейсов)**:
    
    - Адаптирует данные из внешнего мира в формат, понятный внутренним слоям (и наоборот).
    - Этот слой включает в себя контроллеры, представления и репозитории.
4. **External Interfaces (Внешние интерфейсы)**:
    
    - Сюда входят базы данных, веб-фреймворки, библиотеки, API и другие внешние зависимости.
    - Здесь происходит взаимодействие с внешними системами, но внешний мир не должен напрямую влиять на бизнес-логику.

### Реализация Clean Architecture

Пример проекта на основе Clean Architecture:

#### 1. **Entities (Сущности)**

Сущности представляют бизнес-объекты и их логику. Например, класс `User`:

```C#
public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}

```

#### 2. **Use Cases (Применение)**

Пример Use Case для создания пользователя:

```C#
public interface ICreateUserUseCase
{
    Task<User> ExecuteAsync(string name, string email);
}

public class CreateUserUseCase : ICreateUserUseCase
{
    private readonly IUserRepository _userRepository;

    public CreateUserUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User> ExecuteAsync(string name, string email)
    {
        var user = new User
        {
            Name = name,
            Email = email
        };

        await _userRepository.AddAsync(user);
        return user;
    }
}

```

#### 3. **Interface Adapters (Адаптеры интерфейсов)**

Этот слой адаптирует данные между внутренними и внешними слоями. Пример контроллера API, который использует Use Case для создания пользователя:

```C#
[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly ICreateUserUseCase _createUserUseCase;

    public UserController(ICreateUserUseCase createUserUseCase)
    {
        _createUserUseCase = createUserUseCase;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        var user = await _createUserUseCase.ExecuteAsync(request.Name, request.Email);
        return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
    }
}

```

#### 4. **External Interfaces (Внешние интерфейсы)**

Этот слой отвечает за взаимодействие с внешними системами, например, с базой данных. Пример репозитория, который взаимодействует с базой данных:

```C#
public interface IUserRepository
{
    Task AddAsync(User user);
}

public class UserRepository : IUserRepository
{
    private readonly DbContext _context;

    public UserRepository(DbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }
}

```

### Преимущества Clean Architecture

1. **Лёгкость в тестировании**: Каждый слой можно тестировать независимо, а зависимости можно подменять mock-объектами, что облегчает юнит-тестирование.
2. **Модульность**: Каждый слой отвечает за определённую задачу, что делает систему более гибкой и расширяемой.
3. **Независимость от технологий**: Внутренние слои (например, бизнес-логика) не зависят от внешних технологий, таких как база данных, веб-фреймворк или пользовательский интерфейс. Это позволяет легко сменить технологии, не затрагивая бизнес-логику.
4. **Масштабируемость и поддерживаемость**: В проекте с Clean Architecture легче добавлять новые функциональности и адаптировать систему к изменениям, поскольку изменения в одном слое минимально влияют на другие слои.
5. **Гибкость**: Система становится более гибкой и адаптируемой к изменениям в бизнес-логике или внешней среде.

### Когда стоит использовать Clean Architecture?

6. **Сложные и масштабируемые проекты**: Когда проект становится более сложным, и необходимо обеспечить его поддержку и расширение в будущем.
7. **Проекты с часто меняющимися требованиями**: Когда необходимо поддерживать гибкость архитектуры для быстрого реагирования на изменения в требованиях.
8. **Микросервисы**: Clean Architecture хорошо подходит для микросервисной архитектуры, где каждый сервис имеет свою бизнес-логику и внешние интерфейсы.
9. **Проекты с большим количеством зависимостей**: Если проект взаимодействует с множеством внешних сервисов и технологий, разделение на слои помогает упорядочить систему.

### Когда не стоит использовать Clean Architecture?

10. **Простые приложения**: Если приложение маленькое и не имеет сложной бизнес-логики, применение Clean Architecture может быть избыточным и привести к ненужной сложности.
11. **Проекты с ограниченными сроками**: В случае, когда нужно быстро разработать MVP или решение с минимальными требованиями, Clean Architecture может замедлить разработку, так как требует чёткого разделения слоёв и дополнительных абстракций.
12. **Реализация без достаточной подготовки**: Без опыта в использовании Clean Architecture можно легко "переусложнить" проект, создав лишние абстракции и зависимости.

### Плохие примеры реализации

- **Жёсткая привязка слоёв**: Например, если бизнес-логика напрямую зависит от веб-фреймворка или базы данных. В таком случае изменения в базе данных потребуют изменений в бизнес-логике, что нарушает основную цель Clean Architecture.
- **Излишняя абстракция**: Слишком глубокая иерархия слоёв, где между компонентами добавляется избыточная абстракция, делает код трудным для понимания и поддержания.
- **Сложности с тестированием**: Если зависимости не инвертированы должным образом, и компоненты взаимодействуют напрямую, это может затруднить тестирование.

### Заключение

**Clean Architecture** — это мощный паттерн для построения масштабируемых, поддерживаемых и гибких приложений. Он идеально подходит для сложных и крупных проектов, где важно обеспечить чёткое разделение ответственности между слоями. Однако для простых приложений его использование может быть избыточным. Основные принципы Clean Architecture помогают создавать системы, которые легко изменяются и развиваются, при этом оставаясь тестируемыми и понятными.

В **Clean Architecture** структура слоёв, как правило, остаётся в рамках общих принципов, но названия и количество слоёв могут варьироваться в зависимости от специфики проекта или предпочтений команды. Важно понимать, что суть Clean Architecture заключается в принципах разделения ответственности и инверсии зависимостей, а не в жёстком соблюдении определённых названий слоёв. Основные концепции остаются постоянными: разделение на слои с чётко определённой ответственностью, независимость от внешних технологий и минимизация зависимости между слоями.

### Возможные изменения в названиях и структуре слоёв:

1. **Entities** могут быть названы по-разному, например:
    
    - **Domain Models** или **Domain Entities** — отражает, что это объекты домена, которые моделируют бизнес-логику.
    - **Core Models** — акцент на том, что это основные объекты системы, которые не зависят от внешних технологий.
2. **Use Cases** могут быть названы:
    
    - **Application Services** — когда акцент на том, что этот слой реализует сервисы приложения.
    - **Business Logic** — в некоторых случаях называют слой с бизнес-логикой именно так, подчёркивая, что именно в этом слое происходят основные операции системы.
3. **Interface Adapters** могут быть переименованы в:
    
    - **Adapters** или **API Layer** — если акцент на адаптации внешних интерфейсов (например, API).
    - **Application Interfaces** — если акцент на интерфейсах взаимодействия с внешними системами.
4. **External Interfaces** могут быть названы:
    
    - **Infrastructure** — это классическое название для слоя, взаимодействующего с внешними системами, такими как базы данных, файлы, сети.
    - **Persistence** — когда основной акцент делается на слоях работы с хранением данных (например, с базами данных).
    - **Frameworks and Drivers** — иногда используется такой термин в контексте взаимодействия с внешними фреймворками и драйверами, не относящимися непосредственно к бизнес-логике.

### Примеры структур и названий слоёв в зависимости от подхода:

#### 1. Пример с типичными слоями (стандартная реализация Clean Architecture):

- **Entities** (или **Domain Models**)
- **Use Cases** (или **Application Services**)
- **Interface Adapters** (или **Adapters**)
- **External Interfaces** (или **Infrastructure**)

#### 2. Пример с более плоской архитектурой:

Вместо многочисленных слоёв могут быть объединены некоторые уровни. Например:

- **Domain** — объединяет сущности и бизнес-логику (содержит как Entities, так и Use Cases).
- **Adapters** — включает интерфейсы, адаптеры и взаимодействие с внешними сервисами.
- **Infrastructure** — включает в себя работу с базой данных и внешними технологиями.

#### 3. Пример для микросервисной архитектуры:

- **Domain** (или **Core**): содержит бизнес-логику и сущности.
- **Application**: реализует основные функции приложения (например, сервисы, которые взаимодействуют с бизнес-логикой).
- **API** (или **Controllers**): взаимодействие с клиентами (контроллеры и маршруты).
- **Persistence**: работа с хранилищами данных (репозитории).
- **Integration**: взаимодействие с внешними сервисами или API.

### Примеры других вариантов названий:

- **Business Layer** вместо **Use Cases**.
- **Service Layer** вместо **Use Cases** или **Application Services**.
- **Data Layer** вместо **Infrastructure** (если акцент на хранении данных).
- **Infrastructure Layer** может быть разделён на несколько частей, например, **Persistence Layer**, **Communication Layer**, **External Systems** и т.д.

### Важно помнить:

- Названия и количество слоёв могут изменяться в зависимости от потребностей проекта и команды, но важнейшие принципы Clean Architecture (разделение ответственности и инверсия зависимостей) должны оставаться неизменными.
- Важнейшее отличие Clean Architecture от других подходов — это независимость бизнес-логики от технологий и возможность легко изменять внешний мир (например, менять базу данных или фреймворк), не затрагивая бизнес-логику.

### Заключение

Названия и количество слоёв могут меняться, но важно сохранять концепцию разделения ответственности и независимости внутренних слоёв от внешних технологий. У команды всегда есть гибкость в том, как именовать слои, адаптируя структуру под специфические нужды проекта.