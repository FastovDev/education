`app.UseWhen(`
    `context => context.Request.Path ==` `"/time"``,` `// условие: если путь запроса "/time"`
    `appBuilder =>`
    `{`
        `appBuilder.Use(``async` `(context, next) =>`
        `{`
            `var time = DateTime.Now.ToShortTimeString();`
            `Console.WriteLine($``"Time: {time}"``);`
            `await` `next();`   `// вызываем следующий middleware`
        `});`
`});`