Паттерн Прототип (Prototype) позволяет создавать объекты на основе уже ранее созданных объектов-прототипов. То есть по сути данный паттерн предлагает технику клонирования объектов.

Когда использовать Прототип?

- Когда конкретный тип создаваемого объекта должен определяться динамически во время выполнения
    
- Когда нежелательно создание отдельной иерархии классов фабрик для создания объектов-продуктов из параллельной иерархии классов (как это делается, например, при использовании паттерна Абстрактная фабрика)
    
- Когда клонирование объекта является более предпочтительным вариантом нежели его создание и инициализация с помощью конструктора. Особенно когда известно, что объект может принимать небольшое ограниченное число возможных состояний.


Формальная структура паттерна на C# могла бы выглядеть следующим образом:

```C#
class Client
{
    void Operation()
    {
        Prototype prototype = new ConcretePrototype1(1);
        Prototype clone = prototype.Clone();
        prototype = new ConcretePrototype2(2);
        clone = prototype.Clone();
    }
}
 
abstract class Prototype
{
    public int Id { get; private set; }
    public Prototype(int id)
    {
        this.Id = id;
    }
    public abstract Prototype Clone();
}
 
class ConcretePrototype1 : Prototype
{
    public ConcretePrototype1(int id)
        : base(id)
    { }
    public override Prototype Clone()
    {
        return new ConcretePrototype1(Id);
    }
}
 
class ConcretePrototype2 : Prototype
{
    public ConcretePrototype2(int id)
        : base(id)
    { }
    public override Prototype Clone()
    {
        return new ConcretePrototype2(Id);
    }
}
```
### Участники

- Prototype: определяет интерфейс для клонирования самого себя, который, как правило, представляет метод `Clone()`
    
- ConcretePrototype1 и ConcretePrototype2: конкретные реализации прототипа. Реализуют метод `Clone()`
    
- Client: создает объекты прототипов с помощью метода `Clone()`
    

Рассмотрим клонирование на примере фигур - прямоугольников и кругов:

```C#
class Program
{
    static void Main(string[] args)
    {
        IFigure figure = new Rectangle(30,40);
        IFigure clonedFigure = figure.Clone();
        figure.GetInfo();
        clonedFigure.GetInfo();
 
        figure = new Circle(30);
        clonedFigure=figure.Clone();
        figure.GetInfo();
        clonedFigure.GetInfo();
 
        Console.Read();
    }
}
 
interface IFigure
{
    IFigure Clone();
    void GetInfo();
}
 
class Rectangle: IFigure
{
    int width;
    int height;
    public Rectangle(int w, int h)
    {
        width = w;
        height = h;
    }
 
    public IFigure Clone()
    {
        return new Rectangle(this.width, this.height);
    }
    public void GetInfo()
    {
        Console.WriteLine("Прямоугольник длиной {0} и шириной {1}", height, width);
    }
}
 
class Circle : IFigure
{
    int radius;
    public Circle(int r)
    {
        radius = r;
    }
 
    public IFigure Clone()
    {
        return new Circle(this.radius);
    }
    public void GetInfo()
    {
        Console.WriteLine("Круг радиусом {0}", radius);
    }
}
```

Здесь в качестве прототипа используется интерфейс IFigure, который реализуется классами Circle и Rectangle.

Но в данном случае надо заметить, что фреймворк .NET предлагает функционал для копирования в виде метода MemberwiseClone(). Например, мы могли бы изменить реализацию метода `Clone()` в классах прямоугольника и круга следующим образом:

```C#
public IFigure Clone()
{
    return this.MemberwiseClone() as IFigure;
}
```
Причем данный метод был бы общим для обоих классов. И работа программы никак бы не изменилась.

В то же время надо учитывать, что метод `MemberwiseClone()` осуществляет неполное копирование - то есть копирование значимых типов. Если же класс фигуры содержал бы объекты ссылочных типов, то оба объекта после клонирования содержали бы ссылку на один и тот же ссылочный объект. Например, пусть фигура круг имеет свойство ссылочного типа:
```C#
class Point
{
    public int X { get; set; }
    public int Y { get; set; }
}
class Circle : IFigure
{
    int radius;
    public Point Point { get; set; }
    public Circle(int r, int x, int y)
    {
        radius = r;
        this.Point = new Point { X = x, Y = y };
    }
 
    public IFigure Clone()
    {
        return this.MemberwiseClone() as IFigure;
    }
    public void GetInfo()
    {
        Console.WriteLine("Круг радиусом {0} и центром в точке ({1}, {2})", radius, Point.X, Point.Y);
    }
}
```
В этом случае при изменении значений в свойстве Point начальной фигуры автоматически бы изменилось соответствующее значение и у клонированной фигуры:
```C#
Circle figure = new Circle(30, 50, 60);
Circle clonedFigure=figure.Clone() as Circle;
figure.Point.X = 100; // изменяем координаты начальной фигуры
figure.GetInfo(); // figure.Point.X = 100
clonedFigure.GetInfo(); // clonedFigure.Point.X = 100
```
Чтобы избежать подобной ситуации, надо применить полное копирование:
```C#
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
 
//........................
class Program
{
    static void Main(string[] args)
    {
        Circle figure = new Circle(30, 50, 60);
        // применяем глубокое копирование
        Circle clonedFigure=figure.DeepCopy() as Circle;
        figure.Point.X = 100;
        figure.GetInfo();
        clonedFigure.GetInfo();
 
        Console.Read();
    }
}
//.........................
     
[Serializable]
class Point
{
    public int X { get; set; }
    public int Y { get; set; }
}
[Serializable]
class Circle : IFigure
{
    int radius;
    public Point Point { get; set; }
    public Circle(int r, int x, int y)
    {
        radius = r;
        this.Point = new Point { X = x, Y = y };
    }
 
    public IFigure Clone()
    {
        return this.MemberwiseClone() as IFigure;
    }
 
    public object DeepCopy()
    {
        object figure = null;
        using (MemoryStream tempStream = new MemoryStream())
        {
            BinaryFormatter binFormatter = new BinaryFormatter(null,
                new StreamingContext(StreamingContextStates.Clone));
 
            binFormatter.Serialize(tempStream, this);
            tempStream.Seek(0, SeekOrigin.Begin);
 
            figure = binFormatter.Deserialize(tempStream);
        }
        return figure;
    }
    public void GetInfo()
    {
        Console.WriteLine("Круг радиусом {0} и центром в точке ({1}, {2})", radius, Point.X, Point.Y);
    }
}
```

Чтобы вручную не создавать у клонированного объекта вложенный объект Point, здесь используются механизмы бинарной сериализации. И в этом случае все классы, объекты которых подлежат копированию, должны быть помечены атрибутом Serializable.

### Использование в стандартных библиотеках .NET

В платформе .NET паттерн **Прототип (Prototype)** используется для создания новых объектов путем клонирования существующих. Он удобен, когда нужно создавать копии объектов с минимальными затратами, особенно если создание объекта с нуля трудоемко или дорого.

#### **1. ICloneable (System)**

**Почему используется:**  
Интерфейс `ICloneable` предоставляет метод `Clone`, который позволяет создать копию объекта. Этот интерфейс — прямое отражение паттерна "Прототип".

**Как применяется паттерн:**  
Классы, реализующие `ICloneable`, должны определять, как объект будет копироваться: поверхностно (shallow copy) или глубоко (deep copy).

```C#
public class Person : ICloneable
{
    public string Name { get; set; }
    public Address Address { get; set; }

    public object Clone()
    {
        return new Person
        {
            Name = this.Name,
            Address = this.Address // поверхностное копирование
        };
    }
}

public class Address
{
    public string City { get; set; }
}
```

#### **2. MemberwiseClone (System.Object)**

**Почему используется:**  
Метод `MemberwiseClone`, унаследованный от `System.Object`, позволяет быстро создать поверхностную копию объекта.

**Как применяется паттерн:**  
Этот метод используется в реализации пользовательских копий (например, внутри метода `Clone`), чтобы не переписывать вручную копирование всех полей.

```C#
public class Person
{
    public string Name { get; set; }

    public Person ShallowCopy()
    {
        return (Person)this.MemberwiseClone();
    }
}
```

#### **3. Serialization (System.Runtime.Serialization)**

**Почему используется:**  
Сериализация/десериализация объектов может быть использована для глубокого клонирования объектов, что также является примером реализации паттерна "Прототип".

**Как применяется:**  
Объект сериализуется в поток (например, в JSON или XML), а затем десериализуется обратно в новый объект, который является копией оригинала.

```C#
using System.Text.Json;

var original = new Person { Name = "Alice" };
var json = JsonSerializer.Serialize(original);
var clone = JsonSerializer.Deserialize<Person>(json);

```

#### **4. DataTable.Clone и DataTable.Copy (System.Data)**

**Почему используется:**  
`DataTable` из ADO.NET предоставляет два метода для копирования:

- `Clone`: создает пустую таблицу с той же структурой.
- `Copy`: создает полную копию таблицы вместе с данными.

```C#
DataTable table = new DataTable();
DataTable clonedTable = table.Clone(); // Только структура
DataTable copiedTable = table.Copy(); // Структура и данные

```

#### **Аналоги паттерна Прототип:**

1. **Фабричный метод:** Вместо копирования объекты создаются заново через фабрику.  
	Пример: `DbProviderFactories.GetFactory("System.Data.SqlClient")`.

2. **Конструкторы копирования:** Иногда используются вместо прототипа для создания новых объектов на основе существующих.  
	Пример:  

```C#
	public Person(Person other)
	{
		Name = other.Name;
		Address = new Address(other.Address);
	}
```

3. **Mapper-ы и AutoMapper:** Используются для копирования сложных объектов или их проекций, особенно в бизнес-логике.
#### **Когда используется паттерн Прототип в .NET?**

1. **Оптимизация производительности:** Когда создание объекта с нуля слишком затратное.
2. **Клонирование сложных объектов:** Сложные графы объектов, например, в данных или UI.
3. **Обеспечение неизменяемости:** Создание копий объектов для обеспечения функциональной изоляции.

Этот паттерн не так явно распространен, как "Строитель" или "Фабрика", но его применение можно встретить в библиотеках, требующих гибкости и производительности.