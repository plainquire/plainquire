# Filter Expression Creator

Dynamically creates lambda expressions to filter enumerable and database queries via `System.Linq.Enumerable.Where(...)`

# Demo

Demo
[https://filterexpressioncreator.schick-software.de/demo](https://filterexpressioncreator.schick-software.de/demo)

Swagger / OpenAPI
https://filterexpressioncreator.schick-software.de/openapi/

# Table of Content
- [Filter Entities](#filter-entities)
  - [Quick Start](#quick-start)
  - [REST / MVC](#rest-mvc)
  - [Swagger / OpenAPI](#swagger-openapi)
  - [Filter Operators / Syntax](#filter-operators-syntax)
  - [Configuration](#configuration)
  - [Interception](#interception)
  - [Default Configuration and Interception](#default-configuration-and-interception)
  - [Support for Newtonsoft.Json](#support-for-newtonsoftjson)
  - [Advanced Scenarios](#advanced-scenarios)
- [Sort Entities](#sort-entities)

# Filter Entities

## Quick Start

Install the NuGet packages:
```
Package Manager : Install-Package Schick.FilterExpressionCreator
CLI : dotnet add package Schick.FilterExpressionCreator
```
Create a filter:
 ```csharp
using FS.FilterExpressionCreator.Enums;
using FS.FilterExpressionCreator.Extensions;
using FS.FilterExpressionCreator.Filters;

var orders = new[] {
    new Order { Customer = "Joe Miller", Number = 100 },
    new Order { Customer = "Joe Smith", Number = 200 },
    new Order { Customer = "Joe Smith", Number = 300 },
};

// Create filter
var filter = new EntityFilter<Order>()
    .Add(x => x.Customer, "Joe")
    .Add(x => x.Number, FilterOperator.GreaterThan, 250);

// Print filter
Console.WriteLine(filter);
// Output: x => (((x.Customer != null) AndAlso x.Customer.ToUpper().Contains("JOE")) AndAlso (x.Number > 250))

// Use filter with LINQ
var filteredOrders = orders.Where(filter).ToList();
// Or queryables (e.g. Entity Framework)
var filteredOrders = dbContext.Orders.Where(filter).ToList();
// Output: new[] { new Order { Customer = "Joe Smith", Number = 300 } };

[FilterEntity]
public class Order
{
    public int Number { get; set; }
    public string Customer { get; set; }
}
 ```

Or bind filter from query-parameters:

```csharp
using FS.FilterExpressionCreator.Filters;

[HttpGet]
public Task<List<Order>> GetOrders([FromQuery] EntityFilter<Order> order)
{
    return dbContext.Orders.Where(filter).ToList();
}
```

## REST / MVC

Model binding for MVC controllers is supported.

To filter an entity via model binding, the entity must be marked with `FilterEntityAttribute`

### Register Model Binders

```
Package Manager : Install-Package Schick.FilterExpressionCreator.Mvc
CLI : dotnet add package Schick.FilterExpressionCreator.Mvc
```

```csharp
using FS.FilterExpressionCreator.Mvc.Extensions;

// Register required stuff by calling 'AddFilterExpressionSupport()' on IMvcBuilder instance
services.AddControllers().AddFilterExpressionSupport();
```

### Map HTTP query parameters to `EntityFilter`

With model binding enabled, REST requests can be filtered using query parameters:

```csharp
using FS.FilterExpressionCreator.Filters;

var getOrdersUrl = "/GetOrders?customer==Joe&number=>4711"

[HttpGet]
public Task<List<Order>> GetOrders([FromQuery] EntityFilter<Order> filter)
{
    Console.WriteLine(filter);
    // Output:
    // x => (
    //   ((x.Customer != null) AndAlso (x.Customer.ToUpper() == "JOE"))
    //   AndAlso (x.Number > 4711)
    // )
    
    var queryParams = filter.ToQueryParams();
    // Output: customer==Joe&number=>4711
}
```

### Configure Model Binding

By default parameters for properties of filtered entity are named `{Entity}{Property}`.
By default all public non-complex properties (`string`, `int`, `DateTime`, ...) are recognized.
Parameters can be renamed or removed using  `FilterAttribute` and `FilterEntityAttribute`.

For the code below `Number` is not mapped anymore and `Customer` becomes `CustomerName`:

```csharp
using FS.FilterExpressionCreator.Abstractions.Attributes;

// Remove prefix, e.g. property 'Number' is mapped from 'number', not 'orderNumber'
[FilterEntity(Prefix = "")]
public class Order
{
     // 'Number' is removed from filter and will be ignored
    [Filter(Filterable = false)]
    public int Number { get; set; }

    // 'Customer' is mapped from query-parameter 'customerName'
    [Filter(Name = "CustomerName")]
    public string Customer { get; set; }
}
```
### Filter Sets

Multiple entity filters can be combined to a set of filters using the `EntityFilterSetAttribute`.

You can write:

```csharp
using FS.FilterExpressionCreator.Filters;
using FS.FilterExpressionCreator.Abstractions.Attributes;

[HttpGet]
// Use
public Task<List<Order>> GetOrders([FromQuery] OrderFilterSet filterSet)
{ 
    var order = filterSet.Order;
    var orderItem = filterSet.OrderItem;
}

// Instead of
public Task<List<Order>> GetOrders([FromQuery] EntityFilter<Order> order, EntityFilter<OrderItem> orderItem) { ... }

[EntityFilterSet]
public class OrderFilterSet
{
	public EntityFilter<Order> Order { get; set; }
	public EntityFilter<OrderItem> OrderItem { get; set; }
}
```
## Swagger / OpenAPI
### Register OpenAPI Support
Swagger / OpenAPI is supported when using [Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore).
``` 
Package Manager : Install-Package Schick.FilterExpressionCreator.Swashbuckle
CLI : dotnet add package Schick.FilterExpressionCreator.Swashbuckle
```

```csharp
using FS.FilterExpressionCreator.Swashbuckle.Extensions;

services.AddSwaggerGen(options =>
{    
    // Register filters used to modify swagger.json
    options.AddFilterExpressionSupport();
});
```

### Register XML Documentation

To get descriptions for generated parameters from XML documentation, paths to documentation files can be provided:

```csharp
services.AddSwaggerGen(options =>
{
    var filterCreatorDoc = Path.Combine(AppContext.BaseDirectory, "FS.FilterExpressionCreator.xml");
    options.AddFilterExpressionSupport(filterCreatorDoc);
    options.IncludeXmlComments(filterCreatorDoc);
});
```

## Filter Operators / Syntax

The filter micro syntax consists of a comma separated list of an operator shortcut and a value (e.g. `~Joe,=Doe`). When a value contains a comma itself, it must be escaped by a backslash.

| Operator             | Micro Syntax | Description                                                  |
| -------------------- | ------------ | ------------------------------------------------------------ |
| Default              |              | Selects the operator according to the filtered type. When filtering `string` the default is `Contains`; otherwise `EqualCaseInsensitive` |
| Contains             | ~            | Hits when the filtered property contains the filter value    |
| EqualCaseInsensitive | =            | Hits when the filtered property equals the filter value (case-insensitive) |
| EqualCaseSensitive   | ==           | Hits when the filtered property equals the filter value (case-sensitive) |
| NotEqual             | !            | Negates the `Default` operator. Operators other than `Default` can not be negated (currently) |
| LessThan             | <            | Hits when the filtered property is less than the filter value |
| LessThanOrEqual      | <=           | Hits when the filtered property is less than or equal to the filter value |
| GreaterThan          | >            | Hits when the filtered property is greater than the filter value |
| GreaterThanOrEqual   | >=           | Hits when the filtered property is greater than or equals the filter value |
| IsNull               | ISNULL       | Hits when the filtered property is `null`                    |
| NotNull              | NOTNULL      | Hits when the filtered property is not `null`                |

### Add/Replace filter using logical OR

Multiple values given to one call are combined using conditional `OR`.

Customer contains `Joe` || `Doe`:

```csharp
var filter = new EntityFilter<Order>();

// via operator
filter.Add(x => x.Customer, FilterOperator.Contains, "Joe", "Doe");
filter.Replace(x => x.Customer, FilterOperator.Contains, "Joe", "Doe");

// via syntax
filter.Add(x => x.Customer, "~Joe,~Doe");
filter.Replace(x => x.Customer, "~Joe,~Doe");

// via query parameter
var getOrdersUrl = "/GetOrders?customer=~Joe,~Doe"
```

### Add/Replace filter using logical AND

Multiple calls are combined using conditional `AND`. 

Customer contains `Joe` && `Doe`:

```csharp
var filter = new EntityFilter<Order>();
    
// via operator
filter
    .Add(x => x.Customer, FilterOperator.Contains, "Joe")
    .Add(x => x.Customer, FilterOperator.Contains, "Doe");

// via syntax
filter
    .Add(x => x.Customer, "~Joe")
    .Add(x => x.Customer, "~Doe");

// via query parameter
var getOrdersUrl = "/GetOrders?customer=~Joe&customer=~Doe"
```

### Filter to `== null` / `!= null`

```csharp
// For 'Customer is null'
filter.Add(x => x.Customer, FilterOperator.IsNull);
// Output: x => (x.Customer == null)

// For 'Customer is not null'
filter.Add(x => x.Customer, FilterOperator.NotNull);
// Output: x => (x.Customer != null)

// via query parameter
var getOrdersUrl = "/GetOrders?customer=ISNULL"
var getOrdersUrl = "/GetOrders?customer=NOTNULL"
```

While filtered for `== null` / `!= null`, (accidently) given values are ignored:

```csharp
filter.Add(x => x.Customer, FilterOperator.NotNull, "values", "are", "ignored");
```

### Filter to Date/Time

Date/Time values can be given in the form of a fault-tolerant [round-trip date/time pattern](https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings#Roundtrip)

```csharp
// Date
filter.Add(x => x.Created, ">2020/01/01");
// Output: x => (x.Created > 01.01.2020 00:00:00)

// Date/Time
filter.Add(x => x.Created, ">2020-01-01-12-30");
// Output: x => (x.Created > 01.01.2020 12:30:00)
    
// Partial values are supported too
filter.Add(x => x.Created, "2020-01");
// Output: x => ((x.Created >= 01.01.2020 00:00:00) AndAlso (x.Created < 01.02.2020 00:00:00))
```

### Date/Time with Natural Language

Thanks to [nChronic.Core](https://github.com/robbell/nChronic.Core) natural language for date/time is supported. 

```csharp
// This
filter.Add(x => x.Created, ">yesterday");

// works as well as
filter.Add(x => x.Created, ">3-months-ago-saturday-at-5-pm");
```

Details can be found here: [https://github.com/mojombo/chronic](https://github.com/mojombo/chronic#simple)

### Enum

`Enum` values can be filtered by it's name as well as by it's numeric representation.

```csharp
// Equals by name
filter.Add(x => x.Gender, "=divers");
// Output: x => (x.Gender == Divers)

// Equals by numeric value
filter.Add(x => x.Gender, "=1");
// Output: x => (Convert(x.Gender, Int64) == 1)

// Contains, value is expanded
filter.Add(x => x.Gender, "~male");
// Output: x => ((x.Gender == Male) OrElse (x.Gender == Female))

enum Gender { Divers, Male, Female }
```

### Numbers

Filter for numbers support `contains` operator but may be less performant.

```csharp
// Equals
filter.Add(x => x.Number, "1");
// Output: x => (x.Number == 1)

// Contains
filter.Add(x => x.Number, "~1");
// Output: x => x.Number.ToString().ToUpper().Contains("1")
```

### Nested Filters

Nested objects are filtered directly (`x => x.Address.City == "Berlin"`)

Nested lists are filtered using `.Any()` (`x => x.Items.Any(item => (item.Article == "Laptop"))`)

```csharp
// Create filters
var addressFilter = new EntityFilter<Address>()
    .Add(x => x.City, "==Berlin");

var itemFilter = new EntityFilter<OrderItem>()
    .Add(x => x.Article, "==Laptop");

var orderFilter = new EntityFilter<Order>()
    .AddNested(x => x.Address, addressFilter)
    .AddNested(x => x.Items, itemFilter);

// Print filter
Console.WriteLine(orderFilter);
// Output:
// x => ((x.Address != null) AndAlso (x.Address.City == "Berlin"))
// x => ((x.Items != null) AndAlso x.Items.Any(x => (x.Article == "Laptop")))

public class Order
{
    public int Number { get; set; }
    public string Customer { get; set; }
    
    public Address Address { get; set; }
    public List<OrderItem> Items { get; set; }
}

public record Address(string Street, string City);
public record OrderItem(int Position, string Article);
```

### Retrieve Syntax and Filter Values

```csharp
var filter = new EntityFilter<Order>()
    .Add(x => x.Customer, FilterOperator.Contains, "Joe", "Doe");

// Retrive filter syntax
string filterSytax = filter.GetPropertyFilterSyntax(x => x.Customer);
// Output: ~Joe,~Doe

// Retrive filter values
ValueFilter[] filterValues = filter.GetPropertyFilterValues(x => x.Customer);
// Output:
// [{
//   "Operator": "Contains",
//   "Value": "Joe",
//   "IsEmpty": false
// }, {
//   "Operator": "Contains",
//   "Value": "Doe",
//   "IsEmpty": false
// }]
```

### Syntax Examples

| Syntax        | Description                                                  |
| ------------- | ------------------------------------------------------------ |
| Joe           | For `string` filtered value contains 'Joe', for `Enum` filtered value is 'Joe' |
| ~Joe          | Filtered value contains 'Joe', even for `Enum`               |
| ~1,~2         | Filtered value contains `1` or `2`                           |
| =1\\,2        | Filtered value equals `1,2`                                  |
| ~Joe,=Doe     | Filtered value contains `Joe` or equals `Doe`                |
| <4,>10        | Filtered value is less than 4 or greater than 10             |
| ISNULL        | Filtered value is `null`                                     |
| >one-week-ago | For `DateTime` filtered value is greater than one week ago   |
| 2020          | For `DateTime` filtered value is between 01/01/2020 and 12/31/2020 |
| 2020-01       | For `DateTime` filtered value is between 01/01/2020 and 1/31/2020 |

## Configuration

Creation of filter expression can be configured via `FilterConfiguration`. While implicit conversions to `Func<TEntity, bool>` and `Expression<Func<TEntity, bool>>` exists, explicit filter conversion is required to apply a configuration

```csharp
using FS.FilterExpressionCreator.Models;

// Parse filter values using german locale (e.g. "5,5" => 5.5f).
var configuration = new FilterConfiguration { CultureInfo = new CultureInfo("de-DE") };

// Explicit filter conversion
var filterExpression = filter.CreateFilter(configuration);

// Filter IEnumerable<T> by compiling filter expression
var filteredOrders = orders.Where(filterExpression.Compile()).ToList();
```

## Interception

Creation of filter expression can be intercepted via `IPropertyFilterInterceptor`. While implicit conversions to `Func<TEntity, bool>` and `Expression<Func<TEntity, bool>>` exists, explicit filter conversion is required to apply an interceptor.

An example can be found in the test code [InterceptorTests](https://github.com/fschick/FilterExpressionCreator/blob/main/FS.FilterExpressionCreator.Tests/Tests/EntityFilter/InterceptorTests.cs)

## Default Configuration and Interception

`EntityFilter` has static properties to provide a system-wide configuration and/or interceptors

```c#
public class EntityFilter
{
	public static FilterConfiguration DefaultConfiguration { get; set; } = new FilterConfiguration();

	public static IPropertyFilterInterceptor? DefaultInterceptor { get; set; }
}
```

## Support for Newtonsoft.Json

By default `System.Text.Json` is used to serialize/convert Filter Expression Creator specific stuff. If you like to use Newtonsoft.Json you must register it:

```
Package Manager : Install-Package Schick.FilterExpressionCreator.Mvc.Newtonsoft
CLI : dotnet add package Schick.FilterExpressionCreator.Mvc.Newtonsoft
```

```csharp
using FS.FilterExpressionCreator.Mvc.Newtonsoft;

// Register support for Newtonsoft by calling 
// 'AddFilterExpressionNewtonsoftSupport()' on IMvcBuilder instance
services.AddControllers().AddFilterExpressionNewtonsoftSupport();
```

## Advanced Scenarios

### Deep Copy

The `EntityFilter<T>` class supports deep cloning by calling the `Clone()` method

```csharp
var copy = filter.Clone();
```

### Cast Filters

Filters can be cast between entities, e.g. to convert them between DTOs and database models.

Properties are matched by type (check if assignable) and name (case-sensitive)

```csharp
var dtoFilter = new EntityFilter<OrderDto>().Add(...);
var orderFilter = dtoFilter.Cast<Order>();
```

### Serialize Filters

#### Using `System.Text.Json`

Objects of type `EntityFilter<T>` can be serialized via `System.Text.Json.JsonSerializer` without further requirements

```csharp
var json = JsonSerializer.Serialize(filter);
filter = JsonSerializer.Deserialize<EntityFilter<Order>>(json);
```

#### Using `Newtonsoft.Json`

When using `Newtonsoft.Json` additional converters are required

``` 
Package Manager : Install-Package Schick.FilterExpressionCreator.Newtonsoft
CLI : dotnet add package Schick.FilterExpressionCreator.Newtonsoft
```

```csharp
using FS.FilterExpressionCreator.Newtonsoft.Extensions;

var json = JsonConvert.SerializeObject(filter, JsonConverterExtensions.NewtonsoftConverters);
filter = JsonConvert.DeserializeObject<EntityFilter<Order>>(json, JsonConverterExtensions.NewtonsoftConverters);
```

### Combine Filter Expressions

To add custom checks to a filter either call `.Where(...)` again

```csharp
var filteredOrders = orders
    .Where(filter)
    .Where(item => item.Items.Count > 2);
```

or where this isn't possible combine filters with `CombineWithConditionalAnd`

```csharp
using FS.FilterExpressionCreator.Abstractions.Extensions;

var extendedFilter = new[]
    {
        filter.CreateFilter(),
        item => item.Items.Count > 2
    }
    .CombineWithConditionalAnd();

var filteredOrders = orders.Where(extendedFilter.Compile());
```
