# 5.0.0

## New Features

...

## Breaking Changes

Obsolete methods removed from `EntityFilter.cs`

`FilterAttribute.Visible` renamed to `FilterAttribute.Filterable`

`AddFilterExpressionsSupport` renamed to `AddFilterExpressionSupport`

`AddFilterExpressionsNewtonsoftSupport` renamed to `AddFilterExpressionNewtonsoftSupport`

`FilterConfiguration` moved to namespace `FS.FilterExpressionCreator.Abstractions.Configurations`

`ValueFilterExtensions.Create(...)` renamed to `ValueFiltersFactory.Create(...)`

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

Abstractions, extensions and attributes moved to NuGet package `FS.FilterExpressionCreator.Abstractions`

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