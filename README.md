# Plainquire

Dynamically creates lambda expressions to filter enumerable and database queries via `System.Linq.Enumerable.Where(...)`

# Overview

```csharp
var getFreelancersUrl = "/GetFreelancers?firstName=Joe&orderBy=lastName&page=2&pageSize=1"

[HttpGet]
public List<Order> GetFreelancers(
    [FromQuery] EntityFilter<Freelancer> filter,
    [FromQuery] EntitySort<Freelancer> sort,
    [FromQuery] EntityPage page)
{   
    var orders = dbContext.Freelancers
        .Where(filter)
        .OrderBy(sort)
        .Page(page);
    
	// Freelancers in database
    new Freelancer { FirstName = "Joe", LastName = "Smith" };
    new Freelancer { FirstName = "Eve", LastName = "Smith" };
    new Freelancer { FirstName = "Joe", LastName = "Jones" };
}
```

# Demo

[https://filterexpressioncreator.schick-software.de/demo](https://filterexpressioncreator.schick-software.de/demo)

# Table of Content

- [Filter Entities](#filter-entities)
   * [Quick Start](#quick-start)
   * [REST / MVC](#rest-mvc)
   * [Swagger / OpenAPI](#swagger-openapi)
   * [Filter Operators / Syntax](#filter-operators-syntax)
   * [Configuration](#configuration)
   * [Interception](#interception)
   * [Default Configuration and Interception](#default-configuration-and-interception)
   * [Support for Newtonsoft.Json](#support-for-newtonsoftjson)
   * [Advanced Scenarios](#advanced-scenarios)
- [Sort Entities](#sort-entities)
   * [Quick Start](#quick-start-1)
   * [REST / MVC](#rest-mvc-1)
   * [Swagger / OpenAPI](#swagger-openapi-1)
   * [Sort Operators / Syntax](#sort-operators-syntax)
   * [Configuration](#configuration-1)
   * [Interception](#interception-1)
   * [Default Configuration and Interception](#default-configuration-and-interception-1)
   * [Support for Newtonsoft.Json](#support-for-newtonsoftjson-1)
   * [Advanced Scenarios](#advanced-scenarios-1)

# Filter Entities

## Getting Started

Install the NuGet packages:
```
Package Manager : Install-Package Schick.Plainquire.
CLI : dotnet add package Schick.Plainquire.Filter
```
Create a filter:
 ```csharp
using Schick.Plainquire.Filter.Enums;
using Schick.Plainquire.Filter.Extensions;
using Schick.Plainquire.Filter.Filters;

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
using Schick.Plainquire.Filter.Filters;

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
Package Manager : Install-Package Schick.Plainquire.Filter.Mvc
CLI : dotnet add package Schick.Plainquire.Filter.Mvc
```

```csharp
using Schick.Plainquire.Filter.Mvc.Extensions;

// Register required stuff by calling 'AddFilterSupport()' on IMvcBuilder instance
services.AddControllers().AddFilterSupport();
```

### Map HTTP query parameters to `EntityFilter`

With model binding enabled, REST requests can be filtered using query parameters:

```csharp
using Schick.Plainquire.Filter.Filters;

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

By default, parameters for properties of filtered entity are named `{Entity}{Property}`.
By default, all public non-complex properties (`string`, `int`, `DateTime`, ...) are recognized.
Parameters can be renamed or removed using  `FilterAttribute` and `FilterEntityAttribute`.

For the code below `Number` is not mapped anymore and `Customer` becomes `CustomerName`:

```csharp
using Schick.Plainquire.Filter.Abstractions.Attributes;

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
using Schick.Plainquire.Filter.Filters;
using Schick.Plainquire.Filter.Abstractions.Attributes;

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
Package Manager : Install-Package Schick.Plainquire.Filter.Swashbuckle
CLI : dotnet add package Schick.Plainquire.Filter.Swashbuckle
```

```csharp
using Schick.Plainquire.Filter.Swashbuckle.Extensions;

services.AddSwaggerGen(options =>
{    
    // Register filters used to modify swagger.json
    options.AddFilterSupport();
});
```

### Register XML Documentation

To get descriptions for generated parameters from XML documentation, paths to documentation files can be provided:

```csharp
services.AddSwaggerGen(options =>
{
    var filterDoc = Path.Combine(AppContext.BaseDirectory, "Schick.Plainquire.Filter.xml");
    options.AddFilterSupport(filterDoc);
    options.IncludeXmlComments(filterDoc);
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
| NotEqual             | !            | Negates the `Default` operator. Operators other than `Default` cannot be negated (currently) |
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
using Schick.Plainquire.Filter.Models;

// Parse filter values using german locale (e.g. "5,5" => 5.5f).
var configuration = new FilterConfiguration { CultureInfo = new CultureInfo("de-DE") };

// Explicit filter conversion
var filterExpression = filter.CreateFilter(configuration);

// Filter IEnumerable<T> by compiling filter expression
var filteredOrders = orders.Where(filterExpression.Compile()).ToList();
```

## Interception

Creation of filter expression can be intercepted via `IFilterInterceptor`. While implicit conversions to `Func<TEntity, bool>` and `Expression<Func<TEntity, bool>>` exists, explicit filter conversion is required to apply an interceptor.

An example can be found in the test code [InterceptorTests](https://github.com/fschick/Plainquire/blob/main/Schick.Plainquire.Filter.Tests/Tests/EntityFilter/InterceptorTests.cs)

## Default Configuration and Interception

`EntityFilter` has static properties to provide a system-wide configuration and/or interceptors

```c#
public class EntityFilter
{
	public static FilterConfiguration DefaultConfiguration { get; set; } = new FilterConfiguration();

	public static IFilterInterceptor? DefaultInterceptor { get; set; }
}
```

## Support for Newtonsoft.Json

By default `System.Text.Json` is used to serialize/convert Plainquire specific stuff. If you like to use Newtonsoft.Json you must register it:

```
Package Manager : Install-Package Schick.Plainquire.Filter.Mvc.Newtonsoft
CLI : dotnet add package Schick.Plainquire.Filter.Mvc.Newtonsoft
```

```csharp
using Schick.Plainquire.Filter.Mvc.Newtonsoft;

// Register support for Newtonsoft by calling 
// 'AddFilterNewtonsoftSupport()' on IMvcBuilder instance
services.AddControllers().AddFilterNewtonsoftSupport();
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
Package Manager : Install-Package Schick.Plainquire.Filter.Newtonsoft
CLI : dotnet add package Schick.Plainquire.Filter.Newtonsoft
```

```csharp
using Schick.Plainquire.Filter.Newtonsoft.Extensions;

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
using Schick.Plainquire.Filter.Abstractions.Extensions;

var extendedFilter = new[]
    {
        filter.CreateFilter(),
        item => item.Items.Count > 2
    }
    .CombineWithConditionalAnd();

var filteredOrders = orders.Where(extendedFilter.Compile());
```

# Sort Entities

## Getting Started

Install the NuGet packages:
```
Package Manager : Install-Package Schick.Plainquire.Sort
CLI : dotnet add package Schick.Plainquire.Sort
```
Create a filter:
 ```csharp
using Schick.Plainquire.Sort.Enums;
using Schick.Plainquire.Sort.Extensions;
using Schick.Plainquire.Sort.Sorts;

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

Or bind filter from query-parameters:

```csharp
using Schick.Plainquire.Sort.Extensions;
using Schick.Plainquire.Sort.Sorts;

[HttpGet]
public Task<List<Order>> GetOrders([FromQuery] EntitySort<Order> sort)
{
    return dbContext.Orders.OrderBy(sort).ToList();
}
```

## REST / MVC

Model binding for MVC controllers is supported.

To sort an entity via model binding, the entity must be marked with `FilterEntityAttribute`

### Register Model Binders

```
Package Manager : Install-Package Schick.Plainquire.Sort.Mvc
CLI : dotnet add package Schick.Plainquire.Sort.Mvc
```

```csharp
using Schick.Plainquire.Sort.Mvc.Extensions;

// Register required stuff by calling 'AddSortQueryableSupport()' on IMvcBuilder instance
services.AddControllers().AddSortQueryableSupport();
```

### Map HTTP query parameter to `EntitySort`

With model binding enabled, REST requests can be sorted using query parameter `orderBy`:

```csharp
using Schick.Plainquire.Sort.Sorts;

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

### Configure Model Binding

By default, parameters for properties of sorted entity are named `{Entity}{Property}`.
By default, all public non-complex properties (`string`, `int`, `DateTime`, ...) are recognized.
Parameters can be renamed or removed using  `FilterAttribute` and `FilterEntityAttribute`.

For the code below `Number` is not mapped anymore and `Customer` becomes `CustomerName`:

```csharp
using Schick.Plainquire.Filter.Abstractions.Attributes;

// Remove prefix, e.g. property 'Number' is mapped from 'number', not 'orderNumber'
// Use 'sortBy' as query parameter name instead of default 'orderBy'
[FilterEntity(Prefix = "", SortByParameter = "sortBy")]
public class Order
{
     // 'Number' is removed from filter and will be ignored
    [Filter(Sortable = false)]
    public int Number { get; set; }

    // 'Customer' is mapped from query-parameter 'customerName'
    [Filter(Name = "CustomerName")]
    public string Customer { get; set; }
}
```
### Order Sets

Multiple entity sorts can be combined to a set of filters using the `EntitySortSetAttribute`.

You can write:

```csharp
using Schick.Plainquire.Filter.Filters;
using Schick.Plainquire.Filter.Abstractions.Attributes;

[HttpGet]
// Use
public Task<List<Order>> GetOrders([FromQuery] OrderSortSet orderSet)
{ 
    var orderSort = orderSet.Order;
    var orderItemSort = orderSet.OrderItem;
}

// Instead of
public Task<List<Order>> GetOrders([FromQuery] EntityFilter<Order> orderSort, EntityFilter<OrderItem> orderItemSort) { ... }

[EntityFilterSet]
public class OrderSortSet
{
	public EntitySort<Order> Order { get; set; }
	public EntitySort<OrderItem> OrderItem { get; set; }
}
```
## Swagger / OpenAPI
### Register OpenAPI Support
Swagger / OpenAPI is supported when using [Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore).
``` 
Package Manager : Install-Package Schick.Plainquire.Sort.Swashbuckle
CLI : dotnet add package Schick.Plainquire.Sort.Swashbuckle
```

```csharp
using Schick.Plainquire.Sort.Swashbuckle.Extensions;

services.AddSwaggerGen(options =>
{    
    // Register filters used to modify swagger.json
    options.AddSortQueryableSupport();
});
```

## Sort Operators / Syntax

The filter micro syntax consists of a property to sort with an optional sort direction marker before or after (e.g. `customer-asc`). For the HTTP query parameter, a comma separated list of properties is allowed (`orderBy=customer,number-desc`).

Allowed sort direction markers are:

Ascending prefix: `+`, `asc-`, `asc ` (with trailing space)
Ascending postfix: `+`, `-asc`, ` asc` (with leading space)
Descending prefix: `-`, `~`, `desc-`, `desc ` (with trailing space)
Descending postfix: `-`, `~`, `-desc`, `-dsc`, ` desc`, ` dsc` (with leading space)

When no sort marker is given, sort is done in ascending order.

### Add Sorting

Multiple values given to one call are combined using conditional `OR`.

Customer contains `Joe` || `Doe`:

```csharp
var sort = new EntitySort<Order>();

// via operator
sort.Add(x => x.Address, SortDirection.Ascending);

// via syntax
sort.Add("Address-asc")

// via query parameter
var getOrdersUrl = "/GetOrders?orderBy=customer-asc"
```

### Nested Sorting

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

// Print filter
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

### Retrieve Syntax and Sort Direction

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
using Schick.Plainquire.Sort.Abstractions.Configurations;

var configuration = new SortConfiguration { IgnoreParseExceptions = true };
var sortedOrders = orders.OrderBy(sortParam, configuration).ToList();
```

## Interception

Creation of sort expression can be intercepted via `ISortQueryableInterceptor`. 

An example can be found in the test code [InterceptorTests](https://github.com/fschick/Plainquire/blob/main/Schick.Plainquire.Sort.Tests/Tests/EntitySort/InterceptorTests.cs)

## Default Configuration and Interception

`EntitySort` has static properties to provide a system-wide configuration and/or interceptors

```c#
public class EntitySort
{
	public static SortConfiguration DefaultConfiguration { get; set; } = new FilterConfiguration();

	public static ISortQueryableInterceptor? DefaultInterceptor { get; set; }
}
```

## Support for Newtonsoft.Json

By default, `System.Text.Json` is used to serialize/convert Plainquire specific stuff. If you like to use Newtonsoft.Json you must register it:

```
Package Manager : Install-Package Schick.Plainquire.Sort.Mvc.Newtonsoft
CLI : dotnet add package Schick.Plainquire.Sort.Mvc.Newtonsoft
```

```csharp
using Schick.Plainquire.Sort.Mvc.Newtonsoft;

// Register support for Newtonsoft by calling 
// 'AddSortQueryableNewtonsoftSupport()' on IMvcBuilder instance
services.AddControllers().AddSortQueryableNewtonsoftSupport();
```

## Advanced Scenarios

### Deep Copy

The `EntitySort<T>` class supports deep cloning by calling the `Clone()` method

```csharp
var copy = filter.Clone();
```

### Cast Sorting

Sorting can be cast between entities, e.g. to convert them between DTOs and database models.

Properties are matched by type (check if assignable) and name (case-sensitive)

```csharp
var dtoSort = new EntitySort<OrderDto>().Add(...);
var orderSort = dtoFilter.Cast<Order>();
```

### Serialize Sort

#### Using `System.Text.Json`

Objects of type `EntitySort<T>` can be serialized via `System.Text.Json.JsonSerializer` without further requirements

```csharp
var json = JsonSerializer.Serialize(filter);
filter = JsonSerializer.Deserialize<EntitySort<Order>>(json);
```

#### Using `Newtonsoft.Json`

When using `Newtonsoft.Json` additional converters are required

``` 
Package Manager : Install-Package Schick.Plainquire.Sort.Newtonsoft
CLI : dotnet add package Schick.Plainquire.Sort.Newtonsoft
```

```csharp
using Schick.Plainquire.Filter.Newtonsoft.Extensions;

var json = JsonConvert.SerializeObject(sort, JsonConverterExtensions.NewtonsoftConverters);
sort = JsonConvert.DeserializeObject<EntitySort<Order>>(json, JsonConverterExtensions.NewtonsoftConverters);
```
