## **1. Что такое юнит-тестирование?**

Юнит-тестирование (unit testing) — это процесс тестирования отдельных модулей кода (функций, классов, методов) в изоляции. Цель — проверить, что каждая часть приложения работает корректно.

**Преимущества юнит-тестов:**  
✅ Раннее выявление ошибок  
✅ Упрощение рефакторинга  
✅ Автоматизация проверки кода  
✅ Повышение уверенности в коде

**Основные фреймворки для юнит-тестирования в C#:**

- **xUnit** – современный, широко используется в .NET Core
- **NUnit** – мощный, но менее популярный
- **MSTest** – встроен в Visual Studio

---

## **2. Установка фреймворков юнит-тестирования**

Можно создать проект для тестов командой:

```sh
dotnet new xunit -n MyProject.Tests

```

Или через **Visual Studio**:

1. **File** → **New Project**
2. Выбрать **xUnit Test Project** (или NUnit, MSTest)
3. Добавить ссылку на тестируемый проект (`MyProject`)

```sh
dotnet add reference ../MyProject/MyProject.csproj

```

---

## **3. Написание первых тестов**

Рассмотрим тестируемый код (`Calculator.cs`):

```C#
public class Calculator
{
    public int Add(int a, int b) => a + b;
    public int Divide(int a, int b) => b == 0 ? throw new DivideByZeroException() : a / b;
}

```

Теперь напишем тесты.

### **3.1. xUnit**

```C#
using Xunit;

public class CalculatorTests
{
    [Fact]
    public void Add_ReturnsCorrectSum()
    {
        var calculator = new Calculator();
        int result = calculator.Add(2, 3);
        Assert.Equal(5, result);
    }

    [Fact]
    public void Divide_ByZero_ThrowsException()
    {
        var calculator = new Calculator();
        Assert.Throws<DivideByZeroException>(() => calculator.Divide(10, 0));
    }
}

```

Запуск тестов:

```sh
dotnet test

```

### **3.2. NUnit**

```C#
sing NUnit.Framework;

[TestFixture]
public class CalculatorTests
{
    [Test]
    public void Add_ReturnsCorrectSum()
    {
        var calculator = new Calculator();
        Assert.AreEqual(5, calculator.Add(2, 3));
    }

    [Test]
    public void Divide_ByZero_ThrowsException()
    {
        var calculator = new Calculator();
        Assert.Throws<DivideByZeroException>(() => calculator.Divide(10, 0));
    }
}

```

### **3.3. MSTest**

```C#
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class CalculatorTests
{
    [TestMethod]
    public void Add_ReturnsCorrectSum()
    {
        var calculator = new Calculator();
        Assert.AreEqual(5, calculator.Add(2, 3));
    }

    [TestMethod]
    [ExpectedException(typeof(DivideByZeroException))]
    public void Divide_ByZero_ThrowsException()
    {
        var calculator = new Calculator();
        calculator.Divide(10, 0);
    }
}

```

---

## **4. Теория и параметризованные тесты**

### **4.1. Параметризованные тесты в xUnit**

```C#
public class CalculatorTests
{
    [Theory]
    [InlineData(2, 3, 5)]
    [InlineData(10, 5, 15)]
    public void Add_ReturnsCorrectSum(int a, int b, int expected)
    {
        var calculator = new Calculator();
        Assert.Equal(expected, calculator.Add(a, b));
    }
}

```

### **4.2. Параметризованные тесты в NUnit**

```C#
[Test]
[TestCase(2, 3, 5)]
[TestCase(10, 5, 15)]
public void Add_ReturnsCorrectSum(int a, int b, int expected)
{
    var calculator = new Calculator();
    Assert.AreEqual(expected, calculator.Add(a, b));
}

```

---

## **5. Mocking и тестирование зависимостей**

Для тестирования зависимостей (например, базы данных) используется `Moq`.

Пример теста с моками:

```C#
using Moq;
using Xunit;

public interface ICalculatorService
{
    int Multiply(int a, int b);
}

public class Calculator
{
    private readonly ICalculatorService _service;
    
    public Calculator(ICalculatorService service)
    {
        _service = service;
    }

    public int Multiply(int a, int b) => _service.Multiply(a, b);
}

public class CalculatorTests
{
    [Fact]
    public void Multiply_CallsServiceMethod()
    {
        var mockService = new Mock<ICalculatorService>();
        mockService.Setup(s => s.Multiply(2, 3)).Returns(6);
        
        var calculator = new Calculator(mockService.Object);
        int result = calculator.Multiply(2, 3);
        
        Assert.Equal(6, result);
        mockService.Verify(s => s.Multiply(2, 3), Times.Once);
    }
}

```

---

## **6. Покрытие кода тестами**

Можно измерить покрытие кода с помощью `coverlet`:

```sh
dotnet add package coverlet.collector
dotnet test --collect:"XPlat Code Coverage"

```

Анализ результатов через `ReportGenerator`:

```sh
reportgenerator -reports:coverage.cobertura.xml -targetdir:coverageReport

```

---

## **7. Вывод**

|Фреймворк|Особенности|
|---|---|
|**xUnit**|Рекомендуется для .NET Core, поддерживает `Theory`, `Fact`, не использует атрибуты `[Test]`|
|**NUnit**|Гибкий, популярен в старых проектах, поддерживает `[TestCase]`|
|**MSTest**|Встроен в Visual Studio, используется в корпоративных проектах|

### **Выбор фреймворка**

- Если новый проект — **xUnit**
- Если старый проект на NUnit — **NUnit**
- Если используется MSTest в компании — **MSTest**

> Важно: хорошее покрытие юнит-тестами увеличивает надежность кода и снижает вероятность багов.