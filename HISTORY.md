# 6.0.0

## New Features

Support for filter operators `StartsWith` and `EndsWith` added

Characters used to separate filter values are now configurable

Character used to escape filter values are configurable

Handling for escaped values in micro syntax implemented

## Breaking Changes

### Conditional member access for generated filter expressions (not functions) removed

By default, an implicit cast to `Expression<Func<TEntity, bool>>` omits conditional property access (`x => x?.Value`) from now, while an implicit cast to `Func<TEntity, bool>` includes it. Users employing the common syntax `items.Where(filter)` should rarely notice any difference.

A new configuration value, `FilterConfiguration.UseConditionalAccess`, has been added to control the generation of conditional access. For compiled LINQ queries, conditional access is typically required. However, this can often be omitted for expressions used by ORM mappers.

To restore the previous behavior, set `FilterConfiguration.UseConditionalAccess` to `FilterConditionalAccess.Always`.

The enum `ConditionalAccess` of `SortConfiguration` was renamed to `SortConditionalAccess` for clearer distinction.

### Sorting and pagination query parameters are now case-insensitive

There is no longer a difference between `&orderBy=<property>` and `&OrderBy=<property>`

### Filter `contains` for enum now uses the enum's string representation

Previously the filter `property=~1` (property contains `1`) for `enum MyEnum { Value0 = 1, Value1 = 2 }` matched `Value0` because of it's underlying value `1`. Now it matches `Value1`.

The default operator `EqualCaseInsensitive` (filter `property=1`) still matches `Value0`.

### Page sets removed

`PageSetAttribute` and related features removed because they were too complex and impractical.

### Namespaces renamed

| 5.x namespace                              | 6.x namespace                               |
| ------------------------------------------ | ------------------------------------------- |
| Plainquire.Filter.PropertyFilterExpression | Plainquire.Filter.PropertyFilterExpressions |
| Plainquire.Filter.ValueFilterExpression    | Plainquire.Filter.ValueFilterExpressions    |

# 5.2.0

## Changes

Support for parameter binding via PageModel added

# 5.1.1

## Fixes

Fix null-reference exception when unused parameters from route are accessed

# 5.1.0

## Changes

Convince methods for filter interception added

Filter interception sample added to documentation

# 5.0.1

## Fixes

Handling of later-bound sort pre/postfix strings fixed

# 5.0.0

## New Features

Support for pagination

## Breaking Changes

### Common

Obsolete methods removed

`FilterConfiguration` removed from `EntityFilter.CreateFilter`. Use `EntityFilter.Configuration` instead

`SortConfiguration` removed from `QueryableExtensions.OrderBy`. Use `EntitySort.Configuration` instead

`FilterEntityAttribute.SortByParameter` removed. Parameter name is taken from controller action parameter name or `IFromQueryMetadata.Name` (`[FromQuery(Name = "sortBy")]`)

Static `EntityFilter.DefaultConfiguration` replaced by `FilterConfiguration.Default`

Static `EntitySort.DefaultConfiguration` replaced by `SortConfiguration.Default`

`FilterConfiguration.Now` moved to `IFilterInterceptor.Now`

Static `EntityFilter.DefaultInterceptor` replaced by `IFilterInterceptor.Default`

Static `EntitySort.DefaultInterceptor` replaced by `ISortInterceptor.Default`

Serialized filters are incompatible between 4.x and 5.x. If you have stored filters and need to convert it, open an issue

### Namespaces simplified

| 4.x namespace                                                | 5.x namespace                      |
| ------------------------------------------------------------ | ---------------------------------- |
| `FS.FilterExpressionCreator.Enums`<br/>`FS.FilterExpressionCreator.Exceptions`<br/>`FS.FilterExpressionCreator.Extensions`<br/>`FS.FilterExpressionCreator.Filters`<br/>`FS.FilterExpressionCreator.Interfaces` | `Plainquire.Filter`                |
| `FS.FilterExpressionCreator.Abstractions.Attributes`<br/>`FS.FilterExpressionCreator.Abstractions.Configurations`<br/>`FS.FilterExpressionCreator.Abstractions.Extensions`<br/>`FS.FilterExpressionCreator.Abstractions.Models` | `Plainquire.Filter.Abstractions`   |
| `FS.FilterExpressionCreator.Mvc.Extensions`                  | `Plainquire.Filter.Mvc`            |
| `FS.FilterExpressionCreator.Mvc.Newtonsoft.Extensions`       | `Plainquire.Filter.Mvc.Newtonsoft` |
| `FS.FilterExpressionCreator.Newtonsoft.Extensions`           | `Plainquire.Filter.Newtonsoft`     |
| `FS.SortQueryableCreator.Enums`<br/>`FS.SortQueryableCreator.Extensions`<br/>`FS.SortQueryableCreator.Interfaces`<br/>`FS.SortQueryableCreator.Sorts` | `Plainquire.Sort`                  |
| `FS.SortQueryableCreator.Abstractions.Configurations`        | `Plainquire.Sort.Abstractions`     |
| `FS.SortQueryableCreator.Mvc.Extensions`                     | `Plainquire.Sort.Mvc`              |
| `FS.SortQueryableCreator.Mvc.Newtonsoft.Extensions`          | `Plainquire.Sort.Mvc.Newtonsoft`   |
| `FS.SortQueryableCreator.Newtonsoft.Extensions`              | `Plainquire.Sort.Newtonsoft`       |
| `FS.SortQueryableCreator.Swashbuckle.Extensions`             | `Plainquire.Sort.Swashbuckle`      |

### Class / Method / Property renames

| 4.x class / method / property name                | 5.x class / method / property name                 |
| ------------------------------------------------- | -------------------------------------------------- |
| `FilterEntityAttribute`                           | `EntityFilterAttribute`                            |
| `FilterAttribute.Visible`                         | `FilterAttribute.Filterable`                       |
| `AddFilterExpressionSupport`                      | `AddFilterSupport`                                 |
| `AddFilterExpressionNewtonsoftSupport`            | `AddFilterNewtonsoftSupport`                       |
| `AddSortQueryableSupport`                         | `AddSortSupport`                                   |
| `AddSortQueryableNewtonsoftSupport`               | `AddSortNewtonsoftSupport`                         |
| `*FilterExpressionCreator*` classes               | `*FilterExpression*`                               |
| `ValueFilterExtensions.Create(...)`               | `ValueFiltersFactory.Create(...)`                  |
| `EntityFilter<TEntity>.Clear<TProperty>(...)`     | `EntityFilter<TEntity>.Remove<TProperty>(...)`     |
| `EntitySort<TEntity>.Clear<TProperty>(...)`       | `EntitySort<TEntity>.Remove<TProperty>(...)`       |
| `EntitySort<TEntity>.ClearNested<TProperty>(...)` | `EntitySort<TEntity>.RemoveNested<TProperty>(...)` |
| `EntityFilter.DefaultConfiguration`               | `EntityFilter.DefaultFilterConfiguration`          |
| `FilterExpressionCreationException`               | `FilterExpressionException`                        |
| `IPropertyFilterInterceptor`                      | `IFilterInterceptor`                               |
| `IPropertySortQueryableInterceptor`               | `ISortInterceptor`                                 |

# 4.6.1

## Fixes

Version 4.6.0 is broken because the tag was created on an incorrect commit.

# 4.6.0

## New Features

Support for ordering added (Packages `Schick.SortQueryableCreator.*`)

## Changes

Filter-syntax allows `;` as separator: `Joe;Eve` becomes the same as `Joe,Eve`, it filters for `Joe` or `Eve`

# 4.5.0

## New Features

Method `GetPropertyFilterValues` added to `EntityFilter` to retrieve filter values

## Changes

Method to get filter syntax for property renamed from `GetPropertyFilter` to `GetPropertyFilterSyntax`

# 4.4.0

## New Features

Added extended type information for parameters created by filters

## Changes

Methods to add/replace for nested filters renamed from `Add|Replace(...)` to `AddNested|ReplaceNested(...)`

# 4.3.0

## Changes

Support for numeric contains operator added

Support for GUID contains operator added

# 4.2.0

## Changes

Support for entity filter sets added

# 4.1.0

## Changes

Extension method to convert `EntityFilter<TEntity>` to HTTP query parameters added

# 4.0.1

## Fixes

Dependencies set to lowest possible version

# 4.0.0

## Breaking Changes

Classes `Section`/`Section<TType>` renamed to `Range`/`Range<TType>`

Target Framework updated to .NET 6 where applicable

## Changes

`Intersect`/`Contains` methods of `Range<TType>` implemented as extension methods

Null value handling added to `Range<TType>` extension methods

Creation of union added to `Range<TType>` extension methods

# 3.1.0

## Changes

Abstractions, extensions and attributes moved to NuGet package `Schick.Plainquire.Filter.Abstractions`

# 3.0.0

## Breaking Changes

Class `DateTimeSpan` replaced by generic class `Section<TType>`

Allow individual filter operators for multiple values combined with OR, see [Combine values with AND and OR](https://github.com/fschick/FilterExpressionCreator#combine-values-with-and-and-or)

## Fixes

Adding ISNULL/NOTNULL filter without values fixed

# 2.3.1

## Changes

`EntityFilter.IsEmpty()` added

# 2.3.0

## Changes

Filter value gets cleared when it is replaced by NULL

# 2.2.1

## Changes

`DateTimeSpan` intersection calculation added

# 2.2.0

## New Features

`DateTimeSpan` equality and comparison functions added

# 2.1.1

## Changes

Parsing of (partial) date/time value (ranges) changed/fixed

# 2.0.0

## New Features

Interceptor added to provide custom filter manipulation/translation

# 1.2.0

## New Features

Support for `DateTimeOffset` added

# 1.1.0

## Changes

Names of generated API parameters changed to first-char-lower-cased

# 1.0.3

## Changes

NuGet packages updated

# 1.0.2

## Changes

API-Link added to demo page

# 1.0.1

## Changes

Throw `ArgumentException` when given value cannot be filtered

README updated

# 1.0.0

Initial release