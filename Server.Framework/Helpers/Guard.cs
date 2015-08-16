#region BSD License
/* 
Copyright (c) 2011, NETFx
All rights reserved.

Redistribution and use in source and binary forms, with or without modification, 
are permitted provided that the following conditions are met:

* Redistributions of source code must retain the above copyright notice, this list 
  of conditions and the following disclaimer.

* Redistributions in binary form must reproduce the above copyright notice, this 
  list of conditions and the following disclaimer in the documentation and/or other 
  materials provided with the distribution.

* Neither the name of Clarius Consulting nor the names of its contributors may be 
  used to endorse or promote products derived from this software without specific 
  prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY 
EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES 
OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT 
SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, 
INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED 
TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR 
BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH 
DAMAGE.
*/
#endregion

using System;
using System.Diagnostics;
using System.Linq.Expressions;

/// <summary>
/// Common guard class for argument validation.
/// </summary>
[DebuggerStepThrough]
public static class Guard
{
    /// <summary>
    /// Ensures the given <paramref name="reference"/> is not null.
    /// Throws <see cref="ArgumentNullException"/> otherwise.
    /// </summary>
    /// <param name="reference">The expression to check.</param>
    /// <typeparam name="T">Type of the reference.</typeparam>
    /// <exception cref="System.ArgumentException">The <paramref name="reference"/> is null.</exception>
    public static void NotNull<T>(Expression<Func<T>> reference)
    {
        T value = reference.Compile()();

        if (value == null)
        {
            throw new ArgumentNullException(GetParameterName(reference), "Parameter cannot be null.");
        }
    }

    /// <summary>
    /// Ensures the given string <paramref name="reference"/> is not null or empty.
    /// Throws <see cref="ArgumentNullException"/> in the first case, or 
    /// <see cref="ArgumentException"/> in the latter.
    /// </summary>
    /// <param name="reference">The expression to check.</param>
    /// <exception cref="System.ArgumentException">The <paramref name="reference"/> is null or an empty string.</exception>
    public static void NotNullOrEmpty(Expression<Func<string>> reference)
    {
        Guard.NotNull<string>(reference);

        string value = reference.Compile()();

        if (value.Length == 0)
        {
            throw new ArgumentException("Parameter cannot be empty. ", GetParameterName(reference));
        }
    }

    /// <summary>
    /// Ensures the given string <paramref name="reference"/> is valid according 
    /// to the <paramref name="validate"/> function. Throws <see cref="ArgumentNullException"/> 
    /// otherwise.
    /// </summary>
    /// <param name="reference">The expression to check.</param>
    /// <param name="validate">The validation function.</param>
    /// <param name="message">The message to return on failed validation.</param>
    /// <typeparam name="T">Type of the reference.</typeparam>
    /// <exception cref="System.ArgumentException">The <paramref name="reference"/> is not valid according 
    /// to the <paramref name="validate"/> function.</exception>
    public static void IsValid<T>(Expression<Func<T>> reference, Func<T, bool> validate, string message)
    {
        T value = reference.Compile()();

        if (!validate(value))
        {
            throw new ArgumentException(message, GetParameterName(reference));
        }
    }

    /// <summary>
    /// Ensures the given string <paramref name="reference"/> is valid according 
    /// to the <paramref name="validate"/> function. Throws <see cref="ArgumentNullException"/> 
    /// otherwise.
    /// </summary>
    /// <param name="reference">The expression to check.</param>
    /// <param name="validate">The validation function.</param>
    /// <param name="format">The string formatted message to return on failed validation.</param>
    /// <param name="args">The arguments for the message string.</param>
    /// <typeparam name="T">Type of the reference.</typeparam>
    /// <exception cref="System.ArgumentException">The <paramref name="value"/> is not valid according 
    /// to the <paramref name="validate"/> function.</exception>
    public static void IsValid<T>(Expression<Func<T>> reference, Func<T, bool> validate, string format, params object[] args)
    {
        T value = reference.Compile()();

        if (!validate(value))
        {
            throw new ArgumentException(string.Format(format, args), GetParameterName(reference));
        }
    }

    /// <summary>
    /// Gets the name of the given parameter.
    /// </summary>
    /// <param name="reference">The expression tree to get the parameter name from.</param>
    /// <returns>The name of the given parameter.</returns>
    private static string GetParameterName(Expression reference)
    {
        var lambda = reference as LambdaExpression;
        var member = lambda.Body as MemberExpression;

        return member.Member.Name;
    }
}