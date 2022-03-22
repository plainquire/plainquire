

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

Link to API added to demo page

README updated

# 1.0.1

## Changes

Link added to syntax to demo

`ArgumentException` is thrown when given value cannot be filtered

README updated

# 1.0.0

Initial release