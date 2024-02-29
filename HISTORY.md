# 5.0.0

## New Features

...

## Breaking Changes

### Common

* Obsolete methods removed

* `FilterConfiguration` moved to namespace `Schick.Plainquire.Filter.Abstractions`
* `EntityFilter<TEntity>.Clear<TProperty>(...)`  renamed to `EntityFilter<TEntity>.Remove<TProperty>(...)`
* `EntitySort<TEntity>.Clear<TProperty>(...)`  renamed to `EntitySort<TEntity>.Remove<TProperty>(...)`
* `EntitySort<TEntity>.ClearNested<TProperty>(...)`  renamed to `EntitySort<TEntity>.RemoveNested<TProperty>(...)`

### Namespaces Simplified

| 4.x namespace                                                | 5.x namespace                      |
| ------------------------------------------------------------ | ---------------------------------- |
| `Plainquire.Filter.Enums`<br/>`Plainquire.Filter.Exceptions`<br/>`Plainquire.Filter.Extensions`<br/>`Plainquire.Filter.Filters`<br/>`Plainquire.Filter.Interfaces` | `Plainquire.Filter`                |
| `Plainquire.Filter.Abstractions.Attributes`<br/>`Plainquire.Filter.Abstractions.Configurations`<br/>`Plainquire.Filter.Abstractions.Extensions`<br/>`Plainquire.Filter.Abstractions.Models` | `Plainquire.Filter.Abstractions`   |
| `Plainquire.Filter.Mvc.Extensions`                           | `Plainquire.Filter.Mvc`            |
| `Plainquire.Filter.Mvc.Newtonsoft.Extensions`                | `Plainquire.Filter.Mvc.Newtonsoft` |
| `Plainquire.Filter.Newtonsoft.Extensions`                    | `Plainquire.Filter.Newtonsoft`     |
| `Plainquire.Sort.Enums`<br/>`Plainquire.Sort.Extensions`<br/>`Plainquire.Sort.Interfaces`<br/>`Plainquire.Sort.Sorts` | `Plainquire.Sort`                  |
| `Plainquire.Sort.Abstractions.Configurations`                | `Plainquire.Sort.Abstractions`     |
| `Plainquire.Sort.Mvc.Extensions`                             | `Plainquire.Sort.Mvc`              |
| `Plainquire.Sort.Mvc.Newtonsoft.Extensions`                  | `Plainquire.Sort.Mvc.Newtonsoft`   |
| `Plainquire.Sort.Newtonsoft.Extensions`                      | `Plainquire.Sort.Newtonsoft`       |
| `Plainquire.Sort.Swashbuckle.Extensions`                     | `Plainquire.Sort.Swashbuckle`      |

### Class / Method / Property Names Cleanup / Renames

| 4.x class / method / property name     | 5.x class / method / property name |
| -------------------------------------- | ---------------------------------- |
| `FilterAttribute.Visible`              | `FilterAttribute.Filterable`       |
| `AddFilterExpressionSupport`           | `AddFilterSupport`                 |
| `AddFilterExpressionNewtonsoftSupport` | `AddFilterNewtonsoftSupport`       |
| `AddSortQueryableSupport`              | `AddSortSupport`                   |
| `AddSortQueryableNewtonsoftSupport`    | `AddSortNewtonsoftSupport`         |
| `ValueFilterExtensions.Create(...)`    | `ValueFiltersFactory.Create(...)`  |
| `*FilterExpressionCreator*` classes    | `*FilterExpression*`               |
| `IPropertyFilterInterceptor`           | `IFilterInterceptor`               |
| `IPropertySortQueryableInterceptor`    | `ISortInterceptor`                 |
| `FilterExpressionCreationException`    | `FilterExpressionException`        |

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