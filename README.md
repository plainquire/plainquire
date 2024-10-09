# Plainquire

Unlock seamless **filtering**, **sorting**, and **paging** in .NET Standard 2.1 with Plainquire. Fully customizable, model binding support and Swagger UI  integration.

## Demo

Application: [https://www.plainquire.com/demo](https://www.plainquire.com/demo)

Swagger UI: [https://www.plainquire.com/api](https://www.plainquire.com/api)

## Usage for ASP.NET Core

### 1. Install NuGet packages

```cmd
dotnet add package Plainquire.Filter
dotnet add package Plainquire.Filter.Mvc
dotnet add package Plainquire.Filter.Swashbuckle
```

### 2. Register services

```csharp
using Plainquire.Filter.Mvc;
using Plainquire.Filter.Swashbuckle;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers().AddFilterSupport();
builder.Services.AddSwaggerGen(options => options.AddFilterSupport());
```

### 3. Setup entity

```csharp
using Plainquire.Filter;

[EntityFilter(Prefix = "")]
public class Freelancer
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
```

### 4. Create HTTP endpoint

```csharp
using Plainquire.Filter;

[HttpGet]
public IEnumerable<Freelancer> GetFreelancers([FromQuery] EntityFilter<Freelancer> filter
{
    var freelancers = GetFreelancersFromDatabase();
    var filteredFreelancers = freelancers.Where(filter);
    return filteredFreelancers;
}
```

### 5. Send HTTP request

```bash
BASE_URL=https://www.plainquire.com/api/Freelancer
curl -O "$BASE_URL/GetFreelancers?firstName=Joe"
```

### 6. **Results in SQL statement**

```sql
SELECT *
FROM "Freelancer"
WHERE instr(upper("FirstName"), 'JOE') > 0
```

## Usage for non-web applications

### 1. Install NuGet package

```cmd
dotnet add package Plainquire.Filter
```

### 2. Setup entity

See [setup entity](#3-setup-entity) from above.

### 3. Create repository

```cs
public IEnumerable<Freelancer> GetFreelancers()
{
    var filter = new EntityFilter<Freelancer>().Add(x => x.FirstName, "~Joe");
    
    var freelancers = GetFreelancersFromDatabase();
    var filteredFreelancers = freelancers.Where(filter);
    return filteredFreelancers;
}
```

# Table of contents

- [Features](#features)
- [Syntax overview](#syntax-overview)
  - [Filter syntax](#filter-syntax)
  - [Sort syntax](#sort-syntax)
- [Filter entities](#filter-entities)
  - [Basic usage](#basic-usage)
  - [Configure filters](#configure-filters)
  - [Filter by special values](#filter-by-special-values)
  - [Logical Operators](#logical-operators)
  - [Nested filters](#nested-filters)
  - [Retrieve syntax and filter values](#retrieve-syntax-and-filter-values)
  - [REST / MVC](#rest--mvc)
  - [Swagger / OpenAPI](#swagger--openapi)
  - [Support for Newtonsoft.Json](#support-for-newtonsoftjson)
  - [Interception](#interception)
  - [Advanced scenarios](#advanced-scenarios)
- [Sort entities](#sort-entities)
  - [Basic usage](#basic-usage)
  - [Configure sorting](#configure-sorting)
  - [Sort entities](#sort-entities)
  - [Sort nested entities](#sort-nested-entities)
  - [Retrieve syntax and sort direction](#retrieve-syntax-and-sort-direction)
  - [REST / MVC](#rest--mvc)
  - [Swagger / OpenAPI](#swagger--openapi)
  - [Support for Newtonsoft.Json](#support-for-newtonsoftjson)
  - [Interception](#interception)
  - [Advanced scenarios](#advanced-scenarios)
- [Page Entities](#page-entities)
  - [Basic usage](#basic-usage)
  - [Configure pagination](#configure-pagination)
  - [REST / MVC](#rest--mvc)
  - [Swagger / OpenAPI](#swagger--openapi)
  - [Support for Newtonsoft.Json](#support-for-newtonsoftjson)
  - [Interception](#interception)
  - [Advanced Scenarios](#advanced-scenarios)
- [Upgrade from FilterExpressionCreator](#upgrade-from-filterexpressioncreator)

# Features

* Filtering, sorting and pagination for ASP.NET Core
* Customizable syntax
* Support for Swagger / OpenUI and code generators via [Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)
* Support for Entity Framework an other ORM mapper using `IQueryable<T>`
* Support for In-memory lists / arrays via `IEnumerable<T>`
* Binding for HTTP query parameters
* Natural language date/time interpretation (e.g. yesterday, last week Tuesday, ...)
* Filters, sorts and pages are serializable, e.g. to persist user defined filters
* Customizable expressions via interceptors

# Syntax overview

## Filter syntax

**Samples (HTTP query parameter)**

| Query parameter         | Description                                   |
| ----------------------- | --------------------------------------------- |
| `gender=male,female`    | Gender equals `male` OR `female`              |
| `gender=~male`          | Gender contains `male` (fetches `female` too) |
| `size=<100`             | Size is lower than `100`                      |
| `size=>100&size<200`    | Size is between `100` and `200`               |
| `created=>two-days-ago` | Created within the last `2 days`              |
| `created=yesterday`     | Created `yesterday`                           |
| `created=>2020-03`      | Created after `Sunday, March 1, 2020`         |
| `created=2020`          | Crated in the year `2020`                     |
| `name=`                 | Name equals ""                                |

**Escape filter value**

The backslash (`\`) is used as escape character within values. To filter for a backslash itself, double it (`\\`).

Characters used by filter operators as well as characters used to separate values (see below) must be escaped to search for them themselves. 

Sample: Use `=\=A\;B` to do a case insensitive search (`=`) for value `=A;B`.

**Syntax reference**

The filter micro syntax consists of a list of operator/value pairs separated by comma (`,`), semicolon (`;`) or a pipe (`|`).

Comma separated values are combined with logical `OR`. To combine values with logical `AND`, specify the filter multiple times.

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

## Sort syntax

**Samples (HTTP query parameter)**

| Query parameter               | Description                                                  |
| ----------------------------- | ------------------------------------------------------------ |
| `orderBy=lastName`            | Sort by `lastName` ascending                                 |
| `orderBy=lastName-`           | Sort by `lastName` descending                                |
| `orderBy=lastName,-firstName` | Sort by `lastName` ascending, than by `firstName` descending |
| `orderBy=lastName.length`     | Sort by `length of lastName` ascending                       |

**Syntax reference**

The sort micro syntax consists of a property name to sort with an optional sort direction marker before or after (e.g. `customer-asc`). For the HTTP query parameter, a comma separated list of properties is allowed (`orderBy=customer,number-desc`).

| Position | Direction  | Values                                       |
| -------- | ---------- | -------------------------------------------- |
| prefix   | ascending  | `+`, `asc-`, `asc `                          |
| postfix  | ascending  | `+`, `-asc`, `  asc`                         |
| prefix   | descending | `-`, `~`, `desc-`, `dsc-`, `desc `, `dsc `   |
| postfix  | descending | `-`, `~`, `-desc`, `-dsc`, `  desc`, `  dsc` |

# Filter entities

## Basic usage

**Install NuGet packages**

```
Package Manager : Install-Package Plainquire.Filter
CLI : dotnet add package Plainquire.Filter
```
**Bind filter from query-parameters**

```csharp
using Plainquire.Filter;

[HttpGet]
public Task<List<Order>> GetOrders([FromQuery] EntityFilter<Order> order)
{
    return dbContext.Orders.Where(filter).ToList();
}
```

**Create filter from code**

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

// Filter using queryables (e.g. Entity Framework)
var filteredOrders = dbContext.Orders.Where(filter).ToList();

// Filter using LINQ
var filteredOrders = orders.Where(filter).ToList();
 ```

## Configure filters

Generated filter expressions can be configured via `FilterConfiguration`.

### Create configuration

```csharp
using Plainquire.Filter;

// Parse filter values using german locale (e.g. "5,5" => 5.5f).
var configuration = new FilterConfiguration { CultureInfo = new CultureInfo("de-DE") };
```

### Provide configuration

```csharp
// For MVC model binding via dependency injection
services.Configure<FilterConfiguration>(c => c.IgnoreParseExceptions = true);

// Via constructor
new EntityFilter<Order>(configuration);

// Via static default
FilterConfiguration.Default
```

### **Configuration reference**

| Configuration           | Description                                                  |
| ----------------------- | ------------------------------------------------------------ |
| `CultureName`           | Culture used for paring in the format languagecode2-country/regioncode2 (e.g. 'en-US'). |
| `UseConditionalAccess`  | Controls the use of conditional access to navigation properties. |
| `IgnoreParseExceptions` | Return `x => true` in case of any exception while parsing the value. |
| `FilterOperatorMap`     | Map between micro syntax and filter operator. Micro syntax is case-sensitive. |

## Filter by special values

### Filter by `== null` / `!= null`

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

### Filter by `""` / `string.Empty`

```csharp
// For 'Customer == ""'
filter.Add(x => x.Customer, string.Empty);
// Output: x => (x.Customer == "")

// For 'Customer is not null'
filter.Add(x => x.Customer, FilterOperator.NotEqual, string.Empty);
// Output: x => (x.Customer != "")

// via query parameter
var getOrdersUrl = "/GetOrders?customer="
```

### Filter by Date/Time

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

### Filter by Date/Time with natural language

Thanks to [nChronic.Core](https://github.com/robbell/nChronic.Core) natural language for date/time is supported.

```csharp
// This
filter.Add(x => x.Created, ">yesterday");

// works as well as
filter.Add(x => x.Created, ">3-months-ago-saturday-at-5-pm");
```

Details can be found here: [https://github.com/mojombo/chronic](https://github.com/mojombo/chronic#simple)

### Filter by enum

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

### Filter by numbers

Filter for numbers support `contains` operator but may be less performant.

```csharp
// Equals
filter.Add(x => x.Number, "1");
// Output: x => (x.Number == 1)

// Contains
filter.Add(x => x.Number, "~1");
// Output: x => x.Number.ToString().ToUpper().Contains("1")
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

## Logical Operators

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

## Nested filters

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

## Retrieve syntax and filter values

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

## REST / MVC

To filter an entity via model binding, the entity must be marked with `EntityFilterAttribute`

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
Parameters can be renamed or removed using  `FilterAttribute` and `EntityFilterAttribute`.

For the code below `Number` is not mapped anymore and `Customer` becomes `CustomerName`:

```csharp
using Plainquire.Filter.Abstractions;

// Remove prefix, e.g. property 'Number' is mapped from 'number', not 'orderNumber'
[EntityFilter(Prefix = "")]
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

## Interception

Creation of filter expression can be intercepted via `IFilterInterceptor`. While implicit conversions to `Func<TEntity, bool>` and `Expression<Func<TEntity, bool>>` exists, explicit filter conversion is required to apply an interceptor.

```csharp
var filter = new EntityFilter<Order>();
var interceptor = new FilterStringsCaseInsensitiveInterceptor();
var filterExpression = filter.CreateFilter(interceptor) ?? (x => true);
var filteredList = orders.Where(filterExpression.Compile());
var filteredDb = _dbContext.Orders.Where(filterExpression);
```

### Default interceptor

A default interceptor can be provided via static `IFilterInterceptor.Default`.

### Sample interceptor

Interceptor to omit filter values having an empty value. Allows to omit filters added by empty query parameters (`&birthday=`) but prevents filtering for empty strings (`&name=`).

```csharp
public class OmitEmptyFilterInterceptor : IFilterInterceptor
{
    public Expression<Func<TEntity, bool>>? CreatePropertyFilter<TEntity>(PropertyInfo propertyInfo, IEnumerable<ValueFilter> filters, FilterConfiguration configuration)
    {
        var nonEmptyFilters = filters.Where(ValueIsNotNullOrEmpty).ToList();

        var noFilterRequired = nonEmptyFilters.Count == 0;
        return noFilterRequired
            ? PropertyFilterExpression.EmptyFilter<TEntity>()
            : PropertyFilterExpression.CreateFilter<TEntity>(propertyInfo, nonEmptyFilters, configuration, this);
    }

    private static bool ValueIsNotNullOrEmpty(ValueFilter valueFilter)
        => !string.IsNullOrEmpty(valueFilter.Value);

    Func<DateTimeOffset> IFilterInterceptor.Now => () => DateTimeOffset.Now;
}
```

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

[EntityFilter]
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

## Configure sorting

Generated sort expression can be configured via `SortConfiguration`.

### Create configuration

```csharp
using Plainquire.Sort.Abstractions;

var configuration = new SortConfiguration();
configuration.AscendingPostfixes.Add("^");
```

### Provide configuration

```csharp
// For MVC model binding via dependency injection
services.Configure<SortConfiguration>(c => c.IgnoreParseExceptions = true);

// Via constructor
new EntitySort<Order>(configuration);

// Via static default
SortConfiguration.Default
```

### **Configuration reference**

| Configuration                     | Description                                                  |
| --------------------------------- | ------------------------------------------------------------ |
| `AscendingPrefixes`               | Prefixes used to identify an ascending sort order            |
| `AscendingPostfixes`              | Postfixes used to identify an ascending sort order           |
| `DescendingPrefixes`              | Prefixes used to identify a descending sort order            |
| `DescendingPostfixes`             | Postfixes used to identify a descending sort order           |
| `IgnoreParseExceptions`           | Return `source.OrderBy(x => 0)` in case of any exception while parsing the value |
| `UseConditionalAccess`            | Controls the use of conditional access to navigation properties (e.g. `person => person?.Name`) |
| `CaseInsensitivePropertyMatching` | Indicates whether to use case-insensitive property matching  |

## Sort entities

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

## Sort nested entities

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

## Retrieve syntax and sort direction

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

## REST / MVC

To sort an entity via model binding, the entity must be marked with `EntityFilterAttribute`

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
Parameters can be renamed or removed using  `FilterAttribute` and `EntityFilterAttribute`.

For the code below `Number` is not mapped anymore and `Customer` becomes `CustomerName`.

```csharp
using Plainquire.Filter.Abstractions;

// Remove prefix, e.g. property 'Number' is mapped from 'number', not 'orderNumber'
// Use 'sortBy' as query parameter name instead of default 'orderBy'
[EntityFilter(Prefix = "")]
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

## Configure pagination

### Create configuration

```csharp
using Plainquire.Page.Abstractions;

var configuration = new PageConfiguration() { IgnoreParseExceptions = true };
```

### Provide configuration

```csharp
// For MVC model binding via dependency injection
services.Configure<PageConfiguration>(c => c.IgnoreParseExceptions = true);

// Via constructor
new EntityPage<Order>(configuration);

// Via static default
PageConfiguration.Default
```

### **Configuration reference**

| Configuration           | Description                                                  |
| ----------------------- | ------------------------------------------------------------ |
| `IgnoreParseExceptions` | Omit paging in case of any exception while parsing the value |

## REST / MVC

To page an entity via model binding, the entity must be marked with `EntityFilterAttribute`

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

Parameters can be renamed `EntityFilterAttribute`.

For the code below page number is taken from query parameter `pageNumber` and page size from `size`.

```csharp
using Plainquire.Filter.Abstractions;

[EntityFilter(PageNumberParameter = "pageNumber", PageSizeParameter = "size")]
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
