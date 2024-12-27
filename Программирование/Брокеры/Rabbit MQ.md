RabbitMQ — это брокер сообщений (message broker), который реализует протокол AMQP (Advanced Message Queuing Protocol). Это программное обеспечение, которое помогает приложениям обмениваться сообщениями, обеспечивая надежную доставку, маршрутизацию и управление очередями.

#### Как работает RabbitMQ?

1. **Отправитель (Producer)**: Приложение отправляет сообщения в RabbitMQ.
2. **Обменник (Exchange)**: Сообщения маршрутизируются в очереди на основании правил маршрутизации (routing key, bindings).
3. **Очередь (Queue)**: Хранит сообщения до их получения получателем.
4. **Получатель (Consumer)**: Приложение получает сообщения из очереди.

RabbitMQ поддерживает различные типы обменников (direct, fanout, topic, headers), что позволяет гибко управлять доставкой сообщений.

#### Зачем нужен RabbitMQ?

RabbitMQ используется для:

- **Асинхронной коммуникации**: Разделение процессов между системами.
- **Разгрузки системы**: Балансировка нагрузки, чтобы приложения не перегружались.
- **Буферизации данных**: Сохранение сообщений в случае, если получатель временно недоступен.
- **Масштабирования**: Увеличение производительности за счёт обработки очередей.
#### Аналоги RabbitMQ

1. **Apache Kafka**: Фокусируется на высокоскоростной обработке потоков данных.
2. **ActiveMQ**: Популярен для систем с использованием протоколов STOMP и MQTT.
3. **Redis Streams**: Легковесная альтернатива с интеграцией в Redis.
4. **Amazon SQS**: Управляемый облачный брокер сообщений от AWS.

#### Преимущества RabbitMQ

- **Простота настройки**: Интуитивно понятная конфигурация.
- **Поддержка различных протоколов**: AMQP, STOMP, MQTT.
- **Надёжность**: Подтверждение доставки (acknowledgement), повторная отправка.
- **Расширяемость**: Поддержка кластеризации и федерации.

#### Недостатки RabbitMQ

- **Меньшая производительность в потоковой обработке** по сравнению с Kafka.
- **Зависимость от памяти**: При большом количестве сообщений требуется больше ресурсов.
- **Усложнение при горизонтальном масштабировании**.


### RabbitMQ vs Kafka

|**Характеристика**|**RabbitMQ**|**Kafka**|
|---|---|---|
|**Протокол**|AMQP|Проприетарный (Kafka Protocol)|
|**Скорость**|Подходит для транзакционных систем|Высокая для потоковых данных|
|**Обработка сообщений**|FIFO, подтверждения|Партиции, обратная совместимость|
|**Назначение**|Асинхронная коммуникация, RPC|Потоковая обработка данных|
|**Сложность**|Простая настройка|Требует настройки и мониторинга|

### Область применения RabbitMQ

- Обмен данными между микросервисами.
- Реализация механизма очередей задач (task queues).
- Поддержка систем с высокой нагрузкой (e-commerce, IoT).
- Асинхронные системы уведомлений.


### Спецификация AMQP

AMQP (Advanced Message Queuing Protocol) — это стандарт для обмена сообщениями между приложениями:

- **Exchange**: Определяет правила маршрутизации.
- **Queue**: Буфер для хранения сообщений.
- **Binding**: Связь между exchange и очередью.
- **Message**: Структурированная единица данных.


### Использование RabbitMQ с C#

Для работы с RabbitMQ в C# используется библиотека **RabbitMQ.Client**.

Пример отправки сообщения:

```
using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory() { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(queue: "hello",
                     durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

string message = "Hello, RabbitMQ!";
var body = Encoding.UTF8.GetBytes(message);

channel.BasicPublish(exchange: "",
                     routingKey: "hello",
                     basicProperties: null,
                     body: body);

Console.WriteLine(" [x] Sent {0}", message);

```


Пример получения сообщения:

```
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory() { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(queue: "hello",
                     durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine(" [x] Received {0}", message);
};

channel.BasicConsume(queue: "hello",
                     autoAck: true,
                     consumer: consumer);

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();

```


