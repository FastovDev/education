Библиотека для работы в режиме реального времени. Например, чаты, социальные сети, игровые приложения, мониторинг данных и т.д.

Работа библиотеки строится на WebSockets, Server-Sent Events, Long Polling, Forever Frame.  Приложение по умолчанию будет пытаться использовать эти технологии в порядки написания. Если не удается построить приложение на WebSockets, то будет осуществлена попытка повторить запрос с помощью SSE, иначе дойдет дело до Long Polling. 

В качестве клиентов для приложения могут выступать:

JavaScript приложение запущенное на Node.js
JavaScript приложение запущенное в браузерах 
Приложение .Net
Java приложение
Экспериментальная поддержка swift и C++

Для работы SignalR в серверной части необходимо подключить:

`using` `SignalRApp;`
...
`builder.Services.AddSignalR();`
...
`app.MapHub<ChatHub>(``"/chat"``);`

Подключаем пространство с реализацией интерфейса Hub в виде ChatHub. 
Добавляем в приложение сервисы SignalR.
Добавляем маршрут с реализацией ChatHub по адресу /chat. 

Подключение сервисов в приложение представляет собой следующую реализацию
`ISignalRServerBuilder AddSignalR(Action<HubOptions> configure);`

Перегрузка метода позволяет сконфигурировать следующие свойства:

- ClientTimeoutInterval. Определяет время, в течение которого клиент должен отправить серверу сообщение. Если в течение данного времени никаких сообщений от клиента на сервер не пришло, то сервер закрывает соединение. (30 секунд дефолт)

- HandshakeTimeout. После подключения к серверу клиент должен отправить серверу в качестве самого первого сообщения специальное сообщение - HandshakeRequest. Это свойство устанавливает допустимое время таймаута, которое может пройти до получения от клиента первого сообщения об установки соединения. Если в течение этого периода клиент не отправит первое сообщение, то подключение закрывается. (15 секунд дефолт)
   
- KeepAliveInterval: если в течение этого периода сервер не отправит никаких сообщений, то автоматически отправляется ping-сообщение для поддержания подключения открытым. При изменении этого свойства Microsoft рекомендует изменить на стороне клиента параметр serverTimeoutInMilliseconds (клиент javascript)/ ServerTimeout(клиент .NET), которое рекомендуется устанавливать в два раза больше, чем KeepAliveInterval. (15 сек)

- SupportedProtocols определяет поддерживаемые протоколы. По умолчанию поддерживаются все протоколы.

- EnableDetailedErrors при значении true возвращает клиенту детальное описание возникшей ошибки (при ее возникновении). Поскольку подобные сообщения могут содержать критически важную для безопасности информацию, то по умолчанию имеет значение false.

- StreamBufferCapacity определяет максимальный размер буфера для входящего потока клиента. По умолчанию равно 10.

- MaximumReceiveMessageSize определяет максимальный размер для входящего сообщения. По умолчанию - 32 кб

- MaximumParallelInvocationsPerClient определяет максимальное количество методов хаба, которые клиент может вызвать параллельно. По умолчанию равно 1.

Настройки можно менять как глобально, так и персонально для какого-то хаба.
Глобально:
`builder.Services.AddSignalR(hubOptions =>`

`{`

    `hubOptions.EnableDetailedErrors =` `true``;`

    `hubOptions.KeepAliveInterval = TimeSpan.FromMinutes(1);`

`});`

Персонально:

`var builder = WebApplication.CreateBuilder(args);`

`builder.Services.AddSignalR().AddHubOptions<ChatHub>(options =>`

`{`

    `options.EnableDetailedErrors =` `true``;`

    `options.KeepAliveInterval = System.TimeSpan.FromMinutes(1);`

`});`

Комбинированно:
`builder.Services`

    `.AddSignalR(hubOptions =>`

    `{`

        `hubOptions.EnableDetailedErrors =` `true``;`

        `hubOptions.KeepAliveInterval = TimeSpan.FromMinutes(1);`

    `})`

    `.AddHubOptions<ChatHub>(options =>`

    `{`

        `options.EnableDetailedErrors =` `false``;`

        `options.KeepAliveInterval = TimeSpan.FromMinutes(5);`

    `});`

Локальные настройки хаба имеют более высокий приоритет, чем глобальные. 


Как подключить Middleware (minimalApi, MVC) глобально, для отдельного роута, 