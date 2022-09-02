# Filter Expression Creator

Library to dynamically create lambda expressions to filter lists and database queries.

# Demo #

Demo: [https://filterexpressioncreator.schick-software.de/demo](https://filterexpressioncreator.schick-software.de/demo)

API: https://filterexpressioncreator.schick-software.de/openapi/

# Table of Content
- [Getting Started](#getting-started)
- [Creating Filters](#creating-filters)
  - [General](#general)
  - [Combine values with AND and OR](#combine-values-with-and-and-or)
  - [Nested Filters](#nested-filters)
  - [Filter Operators](#filter-operators)
  - [Filter to `null`](#filter-to-null)
  - [Filter Micro Syntax](#filter-micro-syntax)
    - [Examples](#examples)
    - [Date/Time](#date/time)
    - [Enumerations](#enumerations)
  - [Configuration](#configuration)
  - [Interception](#interception)
  - [Default Configuration and Interception](#default-configuration-and-interception)
- [Using With MVC Controllers](#using-with-mvc-controllers)
  - [Model Binding](#model-binding)
  - [Register Model Binders](#register-model-binders)
  - [Configure Model Binding](#configure-model-binding)
  - [Nested Objects/Lists](#nested-objects/lists)
  - [Retrieve HTTP query parameters from filter](#Retrieve-HTTP-query-parameters-from-filter)
- [Support for OpenAPI / Swashbuckle.AspNetCore](#support-for-openapi-/-swashbuckleaspnetcore)
  - [Register OpenAPI Support](#register-openapi-support)
  - [Register XML Documentation](#register-xml-documentation)
- [Support for Newtonsoft.Json](#support-for-newtonsoftjson)
- [Advanced Scenarios](#advanced-scenarios)
  - [Deep Copy Filters](#deep-copy-filters)
  - [Cast Filters](#cast-filters)
  - [Serialize Filters](#serialize-filters)
  - [Combine Filter Expressions](#combine-filter-expressions)

# Getting Started

1. Install the standard NuGet package into your ASP.NET Core application
```
Package Manager : Install-Package Schick.FilterExpressionCreator
CLI : dotnet add package Schick.FilterExpressionCreator
```
2. Create a new filter
 ```csharp
 using FS.FilterExpressionCreator.Enums;
 using FS.FilterExpressionCreator.Extensions;
 using FS.FilterExpressionCreator.Filters;
 
 public class Order
 {
     public int Number { get; set; }
     public string Customer { get; set; }
 }
 
 var filter = new EntityFilter<Order>()
     .Add(x => x.Number, FilterOperator.GreaterThan, 200)
     .Add(x => x.Customer, "Joe");
 
 // Show filter
 System.Console.WriteLine(filter);
 // x => (((x.Customer != null) AndAlso x.Customer.ToUpper().Contains("JOE")) AndAlso (x.Number > 200))
 ```
3. Start filtering
```csharp
using FS.FilterExpressionCreator.Enums;
using FS.FilterExpressionCreator.Extensions;
using FS.FilterExpressionCreator.Filters;
using System.Linq;

var orders = new[] {
    new Order { Customer = "Joe Miller", Number = 100 },
    new Order { Customer = "Joe Smith", Number = 200 },
    new Order { Customer = "Joe Smith", Number = 300 },
};

// Using IEnumerable<>
var filteredOrders = orders.Where(filter).ToList();
// filteredOrders equals
// new[] { new Order { Customer = "Joe Smith", Number = 300 } };

// Or using queryables (e.g. Entity Framework)
var filteredOrders = dbContext.Orders.Where(filter).ToList();
```

# Creating Filters

## General

Filters can be added/replaced using operator/value(s) pairs

```csharp
// Operator/Value(s): Customer contains 'Joe' or 'Doe'
filter.Add(x => x.Customer, FilterOperator.Contains, "Joe", "Doe");

// Operator/Value(s): Customer contains 'Joe' or equals 'Doe'
filter.Add(x => x.Customer, ValueFilter.Create(
    FilterOperator.EqualCaseInsensitive, "Joe"), 
    ValueFilter.Create(FilterOperator.EqualCaseInsensitive, "Doe")
);
```

or via [filter micro syntax](#filter-micro-syntax)

```csharp
// Filter micro syntax: Customer contains 'Joe' or 'Doe'.
filter.Add(x => x.Customer, "~Joe,~Doe");

// Filter micro syntax: Customer contains 'Joe' or equals 'Doe'
filter.Add(x => x.Customer, "~Joe,=Doe");
```

## Combine values with AND and OR

```csharp
// OR: Customer contains 'Joe' or 'Doe'.
// Multiple values given to one call of Add/Replace are combined using conditional `OR`.
filter.Add(x => x.Customer, "~Joe,~Doe");

// AND: Customer contains 'Joe' and 'Doe'.
// Multiple calls to Add/Replace are combined using conditional `AND`.
filter.Add(x => x.Customer, "~Joe");
filter.Add(x => x.Customer, "~Doe");
```

## Nested Filters

Filtering nested objects/lists is supported.

### Nested Objects

Nested objects are filtered directly

```csharp
using FS.FilterExpressionCreator.Enums;
using FS.FilterExpressionCreator.Extensions;
using FS.FilterExpressionCreator.Filters;
using System.Collections.Generic;

public class Order
{
    public int Number { get; set; }
    public string Customer { get; set; }
    
    public Address Address { get; set; }
    public List<OrderItem> Items { get; set; }
}

public record Address(string Street, string City) { }
public record OrderItem(int Position, string Article) { }

var addressFilter = new EntityFilter<Address>()
    .Add(x => x.City, "==Berlin");

var filter = new EntityFilter<Order>()
    .Add(x => x.Address, addressFilter);

// Print
System.Console.WriteLine(filter);
// x => ((x.Address != null) AndAlso (x.Address.City == "Berlin"))
```
### Nested Lists

Nested lists are filtered using `IEnumerable<T>.Any()`

```csharp
var itemFilter = new EntityFilter<OrderItem>()
    .Add(x => x.Article, "==Laptop");

var filter = new EntityFilter<Order>()
    .Add(x => x.Items, itemFilter);

// Print
System.Console.WriteLine(filter);
// x => ((x.Items != null) AndAlso x.Items.Any(x => (x.Article == "Laptop")))
```

## Filter Operators

| Operator             | Micro Syntax | Description                                                  |
| -------------------- | ------------ | ------------------------------------------------------------ |
| Default              |              | Selects the operator according to the filtered type. When filtering `string` the default is `Contains`; otherwise `EqualCaseInsensitive` |
| Contains             | ~            | Hits when the filtered property contains the filter value    |
| EqualCaseInsensitive | =            | Hits when the filtered property equals the filter value (case insensitive) |
| EqualCaseSensitive   | ==           | Hits when the filtered property equals the filter value (case sensitive) |
| NotEqual             | !            | Negates the `Default` operator. Operators other than `Default` can not be negated (currently) |
| LessThan             | <            | Hits when the filtered property is less than the filter value |
| LessThanOrEqual      | <=           | Hits when the filtered property is less than or equal to the filter value |
| GreaterThan          | >            | Hits when the filtered property is greater than the filter value |
| GreaterThanOrEqual   | >=           | Hits when the filtered property is greater than or equals the filter value |
| IsNull               | ISNULL       | Hits when the filtered property is `null`                    |
| NotNull              | NOTNULL      | Hits when the filtered property is not `null`                |

## Filter to `null`

To filter to `== null` / `!= null` special filter operators exists

```csharp
// Filtering for values are NULL
filter.Add(x => x.Customer, FilterOperator.IsNull);

// Filtering for values are NOT NULL
filter.Add(x => x.Customer, FilterOperator.NotNull, "values", "are", "ignored");
```

While filtering for `== null` / `!= null`, given values are ignored.

## Filter Micro Syntax

The filter syntax consists of an operator shortcut (see [filter operators](#filter-operators) above) and a list of comma separated values. When a value contains a comma itself, it must be escaped by a backslash. The backslash itself is escaped by another backslash.

### Examples

| Syntax         | Description                                                  |
| -------------- | ------------------------------------------------------------ |
| Joe            | For `string` filtered value contains 'Joe', for `Enum` filtered value is 'Joe' |
| ~Joe           | Filtered value contains 'Joe', even for `Enum`               |
| ~1,2           | Filtered value contains 1 or 2                               |
| =1\\,2         | Filtered value equals '1,2'                                  |
| <4,>10         | Filtered value is less than 4 or greater than 10             |
| ~Joe<br />~Doe | Filtered value contains Joe and Doe                          |
| ISNULL         | Filtered value is `null`                                     |
| >one-week-ago  | For `DateTime` filtered value is greater than one week ago, for others types see above |
| 2020           | For `DateTime` filtered value is between 01/01/2020 and 12/31/2020, for others types see above |
| 2020-01        | For `DateTime` filtered value is between 01/01/2020 and 1/31/2020, for others types see above |

### Date/Time

Date/Time values can be given as a representation in the form of a fault-tolerant [round-trip date/time pattern](https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings#Roundtrip)

`2020/01/01` works as well as `2020-01-01-12-30`

or thanks to [nChronic.Core](https://github.com/robbell/nChronic.Core) as natural language date string. More details can be found here: [https://github.com/mojombo/chronic](https://github.com/mojombo/chronic#simple)

`tomorrow` works as well as `3-months-ago-saturday-at-5-pm`

Partial date strings are supported too

`2020-01` filters date between `2020-01-01` and `2020-01-31`.

### Enumerations

`Enum` values can be filtered by it's numeric representation as well as by it's name.

When filtering using `Contains` the filter values are expanded ("~male" filters for `male` as well as `female`).

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

The class `EntityFilter` has static properties to provide a system-wide configuration and/or interceptors

```c#
/// <summary>
/// Gets or sets the default configuration. Can be used to set a system-wide configuration.
/// </summary>
public static FilterConfiguration DefaultConfiguration { get; set; } = new FilterConfiguration();

/// <summary>
/// Gets or sets the default interceptor. Can be used to set a system-wide interceptor.
/// </summary>
public static IPropertyFilterInterceptor DefaultInterceptor { get; set; }
```

# Using With MVC Controllers

## Model Binding

Model binding for MVC controllers is supported

```csharp
using FS.FilterExpressionCreator.Filters;

[HttpGet]
public Task<List<Order>> GetOrders([FromQuery] EntityFilter<Order> order, ...)
```

The above usage maps the query parameters `OrderNumber` and `OrderCustomer` to the filter parameter  `order`.

## Register Model Binders

```
Package Manager : Install-Package Schick.FilterExpressionCreator.Mvc
CLI : dotnet add package Schick.FilterExpressionCreator.Mvc
```

```csharp
using FS.FilterExpressionCreator.Mvc.Extensions;
```

```csharp
// Register required stuff by calling 'AddFilterExpressionsSupport()' on IMvcBuilder instance
services.AddControllers().AddFilterExpressionsSupport();
```

## Configure Model Binding

By default parameters for properties of filtered entity are named \<`Entity`\>\<`Property`\>.

By default all public non-complex properties (`string`, `int`, `DateTime`, ...) are recognized.

Parameters can be renamed or hidden using  `FilterAttribute` and `FilterEntityAttribute`. For the code below `OrderNumber` is not mapped anymore and `OrderCustomer` becomes `CustomerName`

```csharp
using FS.FilterExpressionCreator.Abstractions.Attributes;

[FilterEntity(Prefix = "")]
public class Order
{
    [Filter(Visible = false)]
    public int Number { get; set; }

    [Filter(Name = "CustomerName")]
    public string Customer { get; set; }
}
```

## Nested Objects/Lists

Nested objects and lists are not implicit recognized as parameters. But you can simply combine them

```csharp
[HttpGet]
public Task<List<Order>> GetOrders(
    [FromQuery] EntityFilter<Order> order, 
    [FromQuery] EntityFilter<Address> address
)
{
    var filter = order.Add(x => x.Address, address);
    // ...
}
```

## Retrieve HTTP query parameters from filter

Use the extension method `ToQueryParams()` to convert an `EntityFilter<TEntity>` back to it's HTTP query parameters

```csharp
var filter = new EntityFilter<Order>()
    .Add(x => x.Number, FilterOperator.GreaterThan, 200)
    .Add(x => x.Customer, "Joe");

var queryParams = filter.ToQueryParams();

// Print query parameters
System.Console.WriteLine(filter);
// number=>200&customer=Joe
```

# Support for OpenAPI / Swashbuckle.AspNetCore

Support for OpenAPI (formerly swagger) provided by Swashbuckle.AspNetCore is available.

## Register OpenAPI Support

``` 
Package Manager : Install-Package Schick.FilterExpressionCreator.Swashbuckle
CLI : dotnet add package Schick.FilterExpressionCreator.Swashbuckle
```

```csharp
using FS.FilterExpressionCreator.Swashbuckle.Extensions;
```

```csharp
services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("V1", new OpenApiInfo { Title = $"API", Version = "V1" });
    // Register required stuff by calling 'AddFilterExpressionsSupport()' on SwaggerGenOptions instance
    options.AddFilterExpressionsSupport();
});
```

## Register XML Documentation

To get descriptions for generated parameters from XML documentation, paths to documentation files can be provided

```csharp
services
.AddSwaggerGen(options =>
{
    options.SwaggerDoc("V1", new OpenApiInfo { Title = $"API", Version = "V1" });
    // Register required stuff including XML documentatioin files
    var filterCreatorDoc = Path.Combine(AppContext.BaseDirectory, "FS.FilterExpressionCreator.xml");
    options.AddFilterExpressionsSupport(filterCreatorDoc);
    options.IncludeXmlComments(filterCreatorDoc);
});
```

# Support for Newtonsoft.Json

By default `System.Text.Json` is used to serialize/convert Filter Expression Creator specific stuff. If you like to use Newtonsoft.Json instead you must register it

```
Package Manager : Install-Package Schick.FilterExpressionCreator.Mvc.Newtonsoft
CLI : dotnet add package Schick.FilterExpressionCreator.Mvc.Newtonsoft
```
```csharp
using FS.FilterExpressionCreator.Mvc.Newtonsoft;

// Register support for Newtonsoft by calling 
// 'AddFilterExpressionsNewtonsoftSupport()' on IMvcBuilder instance
services.AddControllers().AddFilterExpressionsNewtonsoftSupport();
```

# Advanced Scenarios

## Deep Copy Filters

The `EntityFilter<T>` class supports deep cloning by calling the `Clone()` method

```csharp
var copy = filter.Clone();
```

## Cast Filters

Filters can be cast between entities, e.g. to convert them between DTOs and database models.

Properties are matched by type (check if assignable) and name (case sensitive)

```csharp
var dtoFilter = new EntityFilter<OrderDto>().Add(...);
var orderFilter = dtoFilter.Cast<Order>();
```

## Serialize Filters

### Using `System.Text.Json`

Objects of type `EntityFilter<T>` can be serialized via `System.Text.Json.JsonSerializer` without further requirements

```csharp
var json = JsonSerializer.Serialize(filter);
filter = JsonSerializer.Deserialize<EntityFilter<EntityFilter<Order>>>(json);
```

### Using `Newtonsoft.Json`

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

## Combine Filter Expressions

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
