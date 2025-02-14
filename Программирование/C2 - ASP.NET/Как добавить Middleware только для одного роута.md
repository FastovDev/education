Метод UseWhen() на основании некоторого условия позволяет создать ответвление конвейера при обработке запроса:

`public` `static` `IApplicationBuilder UseWhen (this` `IApplicationBuilder app, Func<HttpContext,``bool``> predicate, Action<IApplicationBuilder> configuration);`

Рассмотрим небольшой пример:

`var builder = WebApplication.CreateBuilder();`
`var app = builder.Build();`
`app.UseWhen(`
    `context => context.Request.Path ==` `"/time"``,` `// если путь запроса "/time"`
    `appBuilder =>`
    `{`
        `// логгируем данные - выводим на консоль приложения`
        `appBuilder.Use(``async` `(context, next) =>`
        `{`
            `var time = DateTime.Now.ToShortTimeString();`
            `Console.WriteLine($``"Time: {time}"``);`
            `await` `next();`   `// вызываем следующий middleware`
        `});`
        `// отправляем ответ`
        `appBuilder.Run(``async` `context =>`
        `{`
            `var time = DateTime.Now.ToShortTimeString();`
            `await` `context.Response.WriteAsync($``"Time: {time}"``);`
        `});`
`});`
`app.Run(``async` `context =>`
`{`
    `await` `context.Response.WriteAsync(``"Hello METANIT.COM"``);`
`});`
`app.Run();`