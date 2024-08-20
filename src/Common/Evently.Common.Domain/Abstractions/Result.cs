﻿using System.Diagnostics.CodeAnalysis;

namespace Evently.Common.Domain.Abstractions;

public class Result
{
    protected Result(bool isSuccess, Error error)
    {
        Error = error;
        IsSuccess = isSuccess;
    }
 

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    public static Result Success() => new(true, Error.None);

    public static Result Failure(Error error) => new(false, error);

    public static Result<TValue> Success<TValue>(TValue value) =>  new(value, true, Error.None);
    public static Result<TValue> Failure<TValue>(Error error) =>  new(default, false, error);
    
    
}

public class Result<TValue> : Result
{

    private readonly TValue? _value;

    public Result(TValue? value, bool isSuccess, Error error) : base(isSuccess, error)
    {
        _value = value;
    }
    
    [NotNull]
    public TValue Value => IsSuccess ? _value! : throw new InvalidOperationException("Failure does not have a value");
    
    public static implicit operator Result<TValue>(TValue? value) => value is not null ? Success(value) : Failure<TValue>(Error.NullValue);

    public static Result<TValue> ValidationFailure(Error error) => Failure<TValue>(error);
}
