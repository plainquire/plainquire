﻿using System;

namespace Plainquire.Filter;

/// <summary>
/// Converter to create the body of filter expressions for <see cref="DateTime"/> values.
/// Implements the <see cref="IValueFilterExpression"/>
/// </summary>
/// <seealso cref="IValueFilterExpression"/>
public interface IDateTimeFilterExpression : IValueFilterExpression;