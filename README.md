# Plainquire

Easy filtering, sorting and paging for ASP.NET Core.

Dynamically creates required expressions to filter, sort and page enumerable and database queries using LINQ.

## Features

* Filtering, sorting and pagination for ASP.NET Core
* Customizable syntax 
* Support for Swagger / OpenUI and code generators via [Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)
* Support for Entity Framework an other ORM mapper using `IQueryable<T>`
* Support for In-memory lists / arrays via `IEnumerable<T>`
* Binding for HTTP query parameters
* Natural language date/time interpretation (e.g. yesterday, last week Tuesday, ...)
* Filters, sorts and pages are serializable, e.g. to persist user defined filters
* Customizable expressions via interceptors

## Demo

[https://www.plainquire.com/demo](https://www.plainquire.com/demo)

[https://www.plainquire.com/api](https://www.plainquire.com/api)

## Overview

**HTTP request** (Syntax: [Filter Operators / Syntax](#filter-operators--syntax))

```bash
BASE_URL=https://www.plainquire.com/api/Freelancer
curl -O "$BASE_URL/GetFreelancers?firstName=Joe&orderBy=lastName&page=3&pageSize=5"
```

**MVC action**

```csharp
[HttpGet]
public List<FreelancerDto> GetFreelancers(
    [FromQuery] EntityFilter<Freelancer> filter,
    [FromQuery] EntitySort<Freelancer> sort,
    [FromQuery] EntityPage page)
{
    return dbContext.Freelancers
        .Where(filter)
        .OrderBy(sort)
        .Page(page);
}
```

**Results in SQL statement** (SQLite syntax)

```sql
SELECT *
FROM "Freelancer"
WHERE instr(upper("FirstName"), 'JOE') > 0
ORDER BY "LastName"
LIMIT 5 OFFSET 10
```

# Table of content

- [Getting started](#getting-started)
- [Filter entities](#filter-entities)
  - [Basic usage](#basic-usage)
  - [REST / MVC](#rest--mvc)
  - [Swagger / OpenAPI](#swagger--openapi)
  - [Filter operators / syntax](#filter-operators--syntax)
  - [Configuration](#configuration)
  - [Interception](#interception)
  - [Default configuration and interception](#default-configuration-and-interception)
  - [Support for Newtonsoft.Json](#support-for-newtonsoftjson)
  - [Advanced scenarios](#advanced-scenarios)
- [Sort entities](#sort-entities)
  - [Basic usage](#basic-usage)
  - [REST / MVC](#rest--mvc)
  - [Swagger / OpenAPI](#swagger--openapi)
  - [Sort operators / syntax](#sort-operators--syntax)
  - [Configuration](#configuration)
  - [Interception](#interception)
  - [Default configuration and interception](#default-configuration-and-interception)
  - [Support for Newtonsoft.Json](#support-for-newtonsoftjson)
  - [Advanced scenarios](#advanced-scenarios)
- [Page Entities](#page-entities)
  - [Basic usage](#basic-usage)
  - [REST / MVC](#rest--mvc)
  - [Swagger / OpenAPI](#swagger--openapi)
  - [Configuration](#configuration)
  - [Interception](#interception)
  - [Default configuration and interception](#default-configuration-and-interception)
  - [Support for Newtonsoft.Json](#support-for-newtonsoftjson)
  - [Advanced Scenarios](#advanced-scenarios)
- [Upgrade from FilterExpressionCreator](#upgrade-from-filterexpressioncreator)

# Getting started

## Web / MVC application

```cmd
dotnet add package Plainquire.Filter
dotnet add package Plainquire.Filter.Mvc
dotnet add package Plainquire.Filter.Swashbuckle
dotnet add package Plainquire.Page
dotnet add package Plainquire.Page.Mvc
dotnet add package Plainquire.Page.Swashbuckle
dotnet add package Plainquire.Sort
dotnet add package Plainquire.Sort.Mvc
dotnet add package Plainquire.Sort.Swashbuckle
```

```csharp
using Plainquire.Filter.Mvc;
using Plainquire.Filter.Swashbuckle;
using Plainquire.Page.Mvc;
using Plainquire.Page.Swashbuckle;
using Plainquire.Sort.Mvc;
using Plainquire.Sort.Swashbuckle;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddFilterSupport()
    .AddSortSupport()
    .AddPageSupport();

builder.Services.AddSwaggerGen(options => options
    .AddFilterSupport()
    .AddSortSupport()
    .AddPageSupport());
```

```csharp
using Plainquire.Filter;
using Plainquire.Page;
using Plainquire.Sort;

[FilterEntity]
public record Order(int Number, string Customer);

[HttpGet(Name = "GetOrders")]
public IEnumerable<Order> GetOrders(
    [FromQuery] EntityFilter<Order> filter,
    [FromQuery] EntitySort<Order> sort,
    [FromQuery] EntityPage page)
{
    var orders = GenerateOrders(); // See code below
    var requestedOrders = orders.Where(filter).OrderBy(sort).Page(page);
    return requestedOrders;
}
```

## Non-web application

```cmd
dotnet add package Plainquire.Filter
dotnet add package Plainquire.Sort
dotnet add package Plainquire.Page
```

```cs
using Plainquire.Filter;
using Plainquire.Sort;
using Plainquire.Page;

[FilterEntity]
public record Order(int Number, string Customer);

public IEnumerable<Order> GetOrders()
{
    var orders = GenerateOrders(); // See code below

    var filter = new EntityFilter<Order>().Add(x => x.Customer, "~Santos");
    var sort = new EntitySort<Order>().Add(x => x.Customer, SortDirection.Ascending);
    var page = new EntityPage { PageNumber = 2, PageSize = 3 };

    var requestedOrders = orders.Where(filter).OrderBy(sort).Page(page);
    return requestedOrders;
}
```

## Generate sample orders

```csharp
private static readonly string[] _customerNames =
[
    "Brock Luettgen",
    "Santos Rath",
    "Camden Goldner",
    "Santos Marks"
];

private static List<Order> GenerateOrders()
{
    return Enumerable
        .Range(1, 5)
        .Select(index => new Order
        (
            Number: index,
            Customer: _customerNames[Random.Shared.Next(_customerNames.Length)]
        ))
        .ToList();
}
```

# Filter entities

## Basic usage

**Install NuGet packages**

```
Package Manager : Install-Package Plainquire.Filter
CLI : dotnet add package Plainquire.Filter
```
**Create a filter**

 ```csharp
using Plainquire.Filter;

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

**Or bind sort from query-parameters**

```csharp
using Plainquire.Filter;

[HttpGet]
public Task<List<Order>> GetOrders([FromQuery] EntityFilter<Order> order)
{
    return dbContext.Orders.Where(filter).ToList();
}
```

## REST / MVC

To filter an entity via model binding, the entity must be marked with `FilterEntityAttribute`

### Register model binders

```
Package Manager : Install-Package Plainquire.Filter.Mvc
CLI : dotnet add package Plainquire.Filter.Mvc
```

```csharp
using Plainquire.Filter.Mvc;

// Register required stuff by calling 'AddFilterSupport()' on IMvcBuilder instance
services.AddControllers().AddFilterSupport();
```

### Map HTTP query parameters to `EntityFilter`

With model binding enabled, REST requests can be filtered using query parameters:

```csharp
using Plainquire.Filter;

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

### Configure model binding

By default, parameters for properties of filtered entity are named `{Entity}{Property}`.
By default, all public non-complex properties (`string`, `int`, `DateTime`, ...) are recognized.
Parameters can be renamed or removed using  `FilterAttribute` and `FilterEntityAttribute`.

For the code below `Number` is not mapped anymore and `Customer` becomes `CustomerName`:

```csharp
using Plainquire.Filter.Abstractions;

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
### Filter sets

Multiple entity filters can be combined to a set of filters using the `EntityFilterSetAttribute`.

```csharp
using Plainquire.Filter;
using Plainquire.Filter.Abstractions;

// Use
[HttpGet]
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
### Register OpenAPI support
Swagger / OpenAPI is supported when using [Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore).
```
Package Manager : Install-Package Plainquire.Filter.Swashbuckle
CLI : dotnet add package Plainquire.Filter.Swashbuckle
```

```csharp
using Plainquire.Filter.Swashbuckle;

services.AddSwaggerGen(options =>
{
    // Register filters used to modify swagger.json
    options.AddFilterSupport();
});
```

### Register XML documentation

To get descriptions for generated parameters from XML documentation, paths to documentation files can be provided.

```csharp
services.AddSwaggerGen(options =>
{
    var filterDoc = Path.Combine(AppContext.BaseDirectory, "Plainquire.Filter.xml");
    options.AddFilterSupport(filterDoc);
    options.IncludeXmlComments(filterDoc);
});
```

## Support for Newtonsoft.Json

By default `System.Text.Json` is used to serialize/convert Plainquire specific stuff. If you like to use Newtonsoft.Json you must register it:

```
Package Manager : Install-Package Plainquire.Filter.Mvc.Newtonsoft
CLI : dotnet add package Plainquire.Filter.Mvc.Newtonsoft
```

```csharp
using Plainquire.Filter.Mvc.Newtonsoft;

// Register support for Newtonsoft by calling
// 'AddFilterNewtonsoftSupport()' on IMvcBuilder instance
services.AddControllers().AddFilterNewtonsoftSupport();
```

## Filter operators / syntax

The filter micro syntax consists of a comma separated list of an operator shortcut and a value (e.g. `~Joe,=Doe`). When a value contains a comma itself, it must be escaped by a backslash.

| Operator             | Micro syntax | Description                                                  |
| -------------------- | ------------ | ------------------------------------------------------------ |
| Default              |              | Selects the operator according to the filtered type. When filtering `string` the default is `Contains`; otherwise `EqualCaseInsensitive` |
| Contains             | ~            | Hits when the filtered property contains the filter value    |
| EqualCaseInsensitive | =            | Hits when the filtered property equals the filter value (case-insensitive) |
| EqualCaseSensitive   | ==           | Hits when the filtered property equals the filter value (case-sensitive) |
| NotEqual             | !            | Negates the `Default` operator. Operators other than `Default` cannot be negated (currently) |
| LessThan             | <            | Hits when the filtered property is less than the filter value |
| LessThanOrEqual      | <=           | Hits when the filtered property is less than or equal to the filter value |
| GreaterThan          | >            | Hits when the filtered property is greater than the filter value |
| GreaterThanOrEqual   | >=           | Hits when the filtered property is greater than or equals the filter value |
| IsNull               | ISNULL       | Hits when the filtered property is `null`                    |
| NotNull              | NOTNULL      | Hits when the filtered property is not `null`                |

### Add/Replace filter using logical OR

Multiple values given to one call are combined using conditional `OR`.

```csharp
// Customer contains `Joe` || `Doe`

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

```csharp
// Customer contains `Joe` && `Doe`

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

### Date/Time with natural language

Thanks to [nChronic.Core](https://github.com/robbell/nChronic.Core) natural language for date/time is supported.

```csharp
// This
filter.Add(x => x.Created, ">yesterday");

// works as well as
filter.Add(x => x.Created, ">3-months-ago-saturday-at-5-pm");
```

Details can be found here: [https://github.com/mojombo/chronic](https://github.com/mojombo/chronic#simple)

### Enum

`Enum` values can be filtered by its name as well as by it's numeric representation.

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

### Nested filters

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

### Retrieve syntax and filter values

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

### Syntax examples

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

Creation of filter expression can be configured via `FilterConfiguration`.

```csharp
using Plainquire.Filter;

// Parse filter values using german locale (e.g. "5,5" => 5.5f).
var configuration = new FilterConfiguration { CultureInfo = new CultureInfo("de-DE") };
```

The configuration can be provided as follows and is used according to the listed order of precedence

```csharp
// For MVC model binding via dependency injection
services.Configure<FilterConfiguration>(c => c.IgnoreParseExceptions = true);

// Via constructor
new EntityFilter<Order>(configuration);

// Via static default
FilterConfiguration.Default
```

## Interception

Creation of filter expression can be intercepted via `IFilterInterceptor`. While implicit conversions to `Func<TEntity, bool>` and `Expression<Func<TEntity, bool>>` exists, explicit filter conversion is required to apply an interceptor.

```csharp
var filter = new EntityFilter<Order>();
var interceptor = new FilterStringsCaseInsensitiveInterceptor();
var filterExpression = filter.CreateFilter(interceptor) ?? (x => true);
var filteredList = orders.Where(filterExpression.Compile());
var filteredDb = _dbContext.Orders.Where(filterExpression);
```

A default interceptor can be provided via static `IFilterInterceptor.Default`.

## Advanced scenarios

### Deep copy

The `EntityFilter<T>` class supports deep cloning by calling the `Clone()` method

```csharp
var copy = filter.Clone();
```

### Casting

Filters can be cast between entities, e.g. to convert them between DTOs and database models.

Properties are matched by type (check if assignable) and name (case-sensitive)

```csharp
var dtoFilter = new EntityFilter<OrderDto>().Add(...);
var orderFilter = dtoFilter.Cast<Order>();
```

### Serialization

#### Using `System.Text.Json`

Objects of type `EntityFilter<T>` can be serialized via `System.Text.Json.JsonSerializer` without further requirements

```csharp
var json = JsonSerializer.Serialize(filter);
filter = JsonSerializer.Deserialize<EntityFilter<Order>>(json);
```

#### Using `Newtonsoft.Json`

When using `Newtonsoft.Json` additional converters are required

```
Package Manager : Install-Package Plainquire.Filter.Newtonsoft
CLI : dotnet add package Plainquire.Filter.Newtonsoft
```

```csharp
using Plainquire.Filter.Newtonsoft;

var json = JsonConvert.SerializeObject(filter, JsonConverterExtensions.NewtonsoftConverters);
filter = JsonConvert.DeserializeObject<EntityFilter<Order>>(json, JsonConverterExtensions.NewtonsoftConverters);
```

### Combine filter expressions

To add custom checks to a filter either call `.Where(...)` again

```csharp
var filteredOrders = orders
    .Where(filter)
    .Where(item => item.Items.Count > 2);
```

or where this isn't possible combine filters with `CombineWithConditionalAnd`

```csharp
using Plainquire.Filter.Abstractions;

var extendedFilter = new[]
    {
        filter.CreateFilter(),
        item => item.Items.Count > 2
    }
    .CombineWithConditionalAnd();

var filteredOrders = orders.Where(extendedFilter.Compile());
```

# Sort entities

## Basic usage

**Install NuGet packages**

```
Package Manager : Install-Package Plainquire.Sort
CLI : dotnet add package Plainquire.Sort
```
**Create a sort**

 ```csharp
using Plainquire.Sort;

var orders = new[] {
    new Order { Customer = "Joe Miller", Number = 100 },
    new Order { Customer = "Joe Smith", Number = 200 },
    new Order { Customer = "Joe Smith", Number = 300 },
};

// Create sort
 var sort = new EntitySort<Order>()
     .Add(x => x.Customer, SortDirection.Ascending)
     .Add(x => x.Number, SortDirection.Descending);

// Print sort
Console.WriteLine($"{orders.OrderBy(sort)}");
// Output: orders.OrderBy(x => IIF((x == null), null, x.Customer)).ThenByDescending(x => x.Number)

// Use sort with LINQ
var sortedOrders = orders.OrderBy(sort).ToList();
// Or queryables (e.g. Entity Framework)
var sortedOrders = dbContext.Orders.OrderBy(sort).ToList();

[FilterEntity]
public class Order
{
    public int Number { get; set; }
    public string Customer { get; set; }
}
 ```

**Or bind sort from query-parameters**

```csharp
using Plainquire.Sort;

[HttpGet]
public Task<List<Order>> GetOrders([FromQuery] EntitySort<Order> sort)
{
    return dbContext.Orders.OrderBy(sort).ToList();
}
```

## REST / MVC

To sort an entity via model binding, the entity must be marked with `FilterEntityAttribute`

### Register model binders

```
Package Manager : Install-Package Plainquire.Sort.Mvc
CLI : dotnet add package Plainquire.Sort.Mvc
```

```csharp
using Plainquire.Sort.Mvc;

// Register required stuff by calling 'AddSortSupport()' on IMvcBuilder instance
services.AddControllers().AddSortSupport();
```

### Map HTTP query parameter to `EntitySort`

With model binding enabled, REST requests can be sorted using query parameter `orderBy`.

```csharp
using Plainquire.Sort;

var getOrdersUrl = "/GetOrders?orderBy=customer,number-desc"

[HttpGet]
public Task<List<Order>> GetOrders([FromQuery] EntitySort<Order> sort)
{
    var orders = new List<Order>();
    var sortedOrders = orders.OrderBy(sort);
    Console.WriteLine($"{sortedOrders.OrderBy(sort)}");
    // Output: orders.OrderBy(x => IIF((x == null), null, x.Customer)).ThenByDescending(x => x.Number)

    var queryParams = sort.ToString();
    // Output: Customer-asc, Number-desc
}
```

### Configure model binding

By default, parameters for properties of sorted entity are named `{Entity}{Property}`.
By default, all public non-complex properties (`string`, `int`, `DateTime`, ...) are recognized.
Parameters can be renamed or removed using  `FilterAttribute` and `FilterEntityAttribute`.

For the code below `Number` is not mapped anymore and `Customer` becomes `CustomerName`.

```csharp
using Plainquire.Filter.Abstractions;

// Remove prefix, e.g. property 'Number' is mapped from 'number', not 'orderNumber'
// Use 'sortBy' as query parameter name instead of default 'orderBy'
[FilterEntity(Prefix = "")]
public class Order
{
     // 'Number' is removed from sort and will be ignored
    [Filter(Sortable = false)]
    public int Number { get; set; }

    // 'Customer' is mapped from query-parameter 'customerName'
    [Filter(Name = "CustomerName")]
    public string Customer { get; set; }
}
```
### Order sets

Multiple entity sorts can be combined to a set of filters using the `EntitySortSetAttribute`.

```csharp
using Plainquire.Sort;
using Plainquire.Sort.Abstractions;

// Use
[HttpGet]
public Task<List<Order>> GetOrders([FromQuery] OrderSortSet orderSet)
{
    var orderSort = orderSet.Order;
    var orderItemSort = orderSet.OrderItem;
}

// Instead of
public Task<List<Order>> GetOrders([FromQuery] EntitySort<Order> orderSort, EntitySort<OrderItem> orderItemSort) { ... }

[EntitySortSet]
public class OrderSortSet
{
    public EntitySort<Order> Order { get; set; }
    public EntitySort<OrderItem> OrderItem { get; set; }
}
```
## Swagger / OpenAPI
### Register OpenAPI support
Swagger / OpenAPI is supported when using [Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore).
```
Package Manager : Install-Package Plainquire.Sort.Swashbuckle
CLI : dotnet add package Plainquire.Sort.Swashbuckle
```

```csharp
using Plainquire.Sort.Swashbuckle;

services.AddSwaggerGen(options =>
{
    // Register filters used to modify swagger.json
    options.AddSortSupport();
});
```

## Support for Newtonsoft.Json

By default, `System.Text.Json` is used to serialize/convert Plainquire specific stuff. If you like to use Newtonsoft.Json you must register it.

```
Package Manager : Install-Package Plainquire.Sort.Mvc.Newtonsoft
CLI : dotnet add package Plainquire.Sort.Mvc.Newtonsoft
```

```csharp
using Plainquire.Sort.Mvc.Newtonsoft;

// Register support for Newtonsoft by calling
// 'AddSortNewtonsoftSupport()' on IMvcBuilder instance
services.AddControllers().AddSortNewtonsoftSupport();
```

## Sort operators / syntax

The sort micro syntax consists of a property to sort with an optional sort direction marker before or after (e.g. `customer-asc`). For the HTTP query parameter, a comma separated list of properties is allowed (`orderBy=customer,number-desc`).

Allowed sort direction markers are:

Ascending prefix: `+`, `asc-`, `asc ` (with trailing space)
Ascending postfix: `+`, `-asc`, ` asc` (with leading space)
Descending prefix: `-`, `~`, `desc-`, `desc ` (with trailing space)
Descending postfix: `-`, `~`, `-desc`, `-dsc`, ` desc`, ` dsc` (with leading space)

When no sort marker is given, sort is done in ascending order.

### Add sorting

Multiple values given to one call are combined using conditional `OR`.

```csharp
// Order is sorted by `Address` ascending.

var sort = new EntitySort<Order>();

// via operator
sort.Add(x => x.Address, SortDirection.Ascending);

// via syntax
sort.Add("Address-asc")

// via query parameter
var getOrdersUrl = "/GetOrders?orderBy=customer-asc"
```

### Nested sorting

Nested objects are sorted directly (`x=> x.OrderBy(order => order.Customer)`).
Deep property paths (e.g. `order => order.Customer.Length`) are supported.
Methods calls (e.g. `order => order.Customer.SubString(1)`) are not supported for security reasons.

Nested lists cannot be sorted directly. You can create an own `EntitySort` for it and sort the nested list by.

```csharp
// Create sort
var addressSort = new EntitySort<Address>()
    .Add(x => x.City);

// AddNested() is equivalent to adding the paths directly
var orderSort = new EntitySort<Order>()
    .AddNested(x => x.Address, addressSort);

// Is equivalent to AddNested() above
var orderSort = new EntitySort<Order>()
    .Add(x => x.Address.City, SortDirection.Ascending);

// Print sort
Console.WriteLine(orders.OrderBy(orderSort).ToString());
// Output:
// orders => orders.OrderBy(x => IIF((IIF((x == null), null, x.Address) == null), null, x.Address.City))

public class Order
{
    public int Number { get; set; }
    public string Customer { get; set; }
    public Address Address { get; set; }
}

public record Address(string Street, string City);
```

### Retrieve syntax and sort direction

```csharp
var orderSort = new EntitySort<Order>()
    .Add(x => x.Customer, SortDirection.Ascending);

// Retrive sort syntax
var syntax = orderSort.GetPropertySortSyntax(x => x.Customer);
// Output: Customer-asc

// Retrive sort direction
var direction = orderSort.GetPropertySortDirection(x => x.Customer);
// Output: Ascending

// Retrive sort expression string:
var orderExpression = orders.OrderBy(orderSort).ToString()
```

## Configuration

Creation of sort expression can be configured via `SortConfiguration`.

```csharp
using Plainquire.Sort.Abstractions;

var configuration = new SortConfiguration();
configuration.AscendingPostfixes.Add("^");
var sort = new EntitySort<Order>(configuration);
var sortedOrders = orders.OrderBy(sort);
```

The configuration can be provided as follows and is used according to the listed order of precedence

```csharp
// For MVC model binding via dependency injection
services.Configure<SortConfiguration>(c => c.IgnoreParseExceptions = true);

// Via constructor
new EntitySort<Order>(configuration);

// Via static default
SortConfiguration.Default
```

## Interception

Creation of sort expression can be intercepted via `ISortInterceptor`.

```csharp
var sort = new EntitySort<Order>();
var interceptor = new CaseInsensitiveSortInterceptor();
var filtered = orders.OrderBy(sort, interceptor);
```

A default interceptor can be provided via static `ISortInterceptor.Default`.

## Advanced scenarios

### Deep copy

The `EntitySort<T>` class supports deep cloning by calling the `Clone()` method

```csharp
var copy = sort.Clone();
```

### Casting

Sorting can be cast between entities, e.g. to convert them between DTOs and database models.

Properties are matched by type (check if assignable) and name (case-sensitive)

```csharp
var dtoSort = new EntitySort<OrderDto>().Add(...);
var orderSort = dtoSort.Cast<Order>();
```

### Serialization

#### Using `System.Text.Json`

Objects of type `EntitySort<T>` can be serialized via `System.Text.Json.JsonSerializer` without further requirements

```csharp
var json = JsonSerializer.Serialize(sort);
sort = JsonSerializer.Deserialize<EntitySort<Order>>(json);
```

#### Using `Newtonsoft.Json`

When using `Newtonsoft.Json` additional converters are required

```
Package Manager : Install-Package Plainquire.Sort.Newtonsoft
CLI : dotnet add package Plainquire.Sort.Newtonsoft
```

```csharp
using Plainquire.Sort.Newtonsoft;

var json = JsonConvert.SerializeObject(sort, JsonConverterExtensions.NewtonsoftConverters);
sort = JsonConvert.DeserializeObject<EntitySort<Order>>(json, JsonConverterExtensions.NewtonsoftConverters);
```

# Page Entities

## Basic usage

**Install NuGet packages**

```
Package Manager : Install-Package Plainquire.Page
CLI : dotnet add package Plainquire.Page
```
**Create a page**

 ```csharp
using Plainquire.Page;

// Direct pageing is the preferred way
var pagedOrders = orders.Page(pageNumber: 2, pageSize: 3).ToList();

// Alternative, create a EntityPage object
 var page = new EntityPage(pageNumber: 2, pageSize: 3);

// Use page with LINQ
var pagedOrders = orders.Page(page).ToList();
// Or queryables (e.g. Entity Framework)
var pagedOrders = dbContext.Orders.Page(page).ToList();

 ```

## REST / MVC

To page an entity via model binding, the entity must be marked with `FilterEntityAttribute`

### Register model binders

```
Package Manager : Install-Package Plainquire.Page.Mvc
CLI : dotnet add package Plainquire.Page.Mvc
```

```csharp
using Plainquire.Page.Mvc;

// Register required stuff by calling 'AddPageSupport()' on IMvcBuilder instance
services.AddControllers().AddPageSupport();
```

### Map HTTP query parameter to `EntityPage`

With model binding enabled, REST requests can be paged using query parameters `page` and `pageSize`.

```csharp
using Plainquire.Page;

var getOrdersUrl = "/GetOrders?page=2&pageSize=3"

[HttpGet]
public Task<List<Order>> GetOrders([FromQuery] EntityPage<Order> page)
{
    return dbContext.Orders.Page(page).ToList();
}
```

### Configure model binding

Parameters can be renamed `FilterEntityAttribute`.

For the code below page number is taken from query parameter `pageNumber` and page size from `size`.

```csharp
using Plainquire.Filter.Abstractions;

[FilterEntity(PageNumberParameter = "pageNumber", PageSizeParameter = "size")]
public class Order
{
    public string Customer { get; set; }
}
```
## Swagger / OpenAPI
### Register OpenAPI support
Swagger / OpenAPI is supported when using [Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore).
```
Package Manager : Install-Package Plainquire.Page.Swashbuckle
CLI : dotnet add package Plainquire.Page.Swashbuckle
```

```csharp
using Plainquire.Page.Swashbuckle;

services.AddSwaggerGen(options =>
{
    // Register filters used to modify swagger.json
    options.AddPageSupport();
});
```

## Support for Newtonsoft.Json

By default, `System.Text.Json` is used to serialize/convert Plainquire specific stuff. If you like to use Newtonsoft.Json you must register it.

```
Package Manager : Install-Package Plainquire.Page.Mvc.Newtonsoft
CLI : dotnet add package Plainquire.Page.Mvc.Newtonsoft
```

```csharp
using Plainquire.Page.Mvc.Newtonsoft;

// Register support for Newtonsoft by calling
// 'AddPageNewtonsoftSupport()' on IMvcBuilder instance
services.AddControllers().AddPageNewtonsoftSupport();
```

## Configuration

Creation of page expression can be configured via `PageConfiguration`.

```csharp
using Plainquire.Page.Abstractions;

var configuration = new PageConfiguration() { IgnoreParseExceptions = true };
var page = new EntityPage<Order>(configuration);
var pagedOrders = orders.Page(page);
```

The configuration can be provided as follows and is used according to the listed order of precedence

```csharp
// For MVC model binding via dependency injection
services.Configure<PageConfiguration>(c => c.IgnoreParseExceptions = true);

// Via constructor
new EntityPage<Order>(configuration);

// Via static default
PageConfiguration.Default
```

## Interception

Creation of page expression can be intercepted via `IPageInterceptor`.

```csharp
var page = new EntityPage();
var interceptor = new PageBackwardInterceptor();
var paged = orders.Page(page, interceptor);
```

A default interceptor can be provided via static `IPageInterceptor.Default`.

## Advanced Scenarios

### Deep copy

The `EntityPage<T>` class supports deep cloning by calling the `Clone()` method

```csharp
var copy = page.Clone();
```

### Serialization

#### Using `System.Text.Json`

Objects of type `EntityPage<T>` can be serialized via `System.Text.Json.JsonSerializer` without further requirements

```csharp
var json = JsonSerializer.Serialize(page);
page = JsonSerializer.Deserialize<EntityPage<Order>>(json);
```

#### Using `Newtonsoft.Json`

When using `Newtonsoft.Json` additional converters are required

```
Package Manager : Install-Package Plainquire.Page.Newtonsoft
CLI : dotnet add package Plainquire.Page.Newtonsoft
```

```csharp
using Plainquire.Page.Newtonsoft;

var json = JsonConvert.SerializeObject(page, JsonConverterExtensions.NewtonsoftConverters);
sort = JsonConvert.DeserializeObject<EntityPage<Order>>(json, JsonConverterExtensions.NewtonsoftConverters);
```

# Upgrade from FilterExpressionCreator

* Install `Schick.FilterExpressionCreator*` 4.7.x.

* Fix all warnings. This can largely be done by sear and replacing with regular expressions
  * Search for: `FS.FilterExpressionCreator(\.Abstractions|Mvc|Mvc\.Newtonsoft|Newtonsoft)?(\.\w+)?`
  * Replace with: `Plainquire.Filter$1`
  * Search for: `\[FilterEntity(\(.*\))]`
  * Replace with: `[EntityFilter$1]`
* Fix remaining errors and warnings following description of [breaking changes](https://github.com/plainquire/plainquire/blob/main/HISTORY.md#breaking-changes)
* Uninstall all `Schick.FilterExpressionCreator*` stuff
* Install corresponding `Plainquire.*` packages
