﻿namespace Mav.Infrastructure.Exceptions;

public class RetryableException : Exception
{
    public RetryableException(string message, Exception inner) : base(message, inner)
    {
    }
    public RetryableException(string message) : base(message)
    {
    }
}
