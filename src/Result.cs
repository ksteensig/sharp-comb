using System;

namespace MonadicParserCombinator
{
    public class Result<T>
    {
        bool _isSuccess;
        readonly T _value;
        readonly Input _remainder;
        readonly string _msg;

        public readonly bool End;

        public T Value
        {
            get
            {
                if (!_isSuccess)
                {
                    throw new InvalidOperationException();
                }

                return _value;
            }
        }

        public bool IsSuccess
        {
            get
            {
                return _isSuccess;
            }
        }

        public Input Remainder
        {
            get
            {
                return _remainder;
            }
        }

        public string Message
        {
            get
            {
                return _msg;
            }
        }

        Result(T value, Input remainder)
        {
            _isSuccess = true;
            _value = value;
            _remainder = remainder;
        }

        Result(Input remainder, string msg)
        {
            _isSuccess = false;
            _remainder = remainder;
            _msg = msg;
        }

        public static Result<T> Success(T value, Input remainder)
        {
            return new Result<T>(value, remainder);
        }

        public static Result<T> Failure(Input remainder, string msg)
        {
            return new Result<T>(remainder, msg);
        }
    }

    public static class ResultExtensions
    {
        public static Result<U> IfSuccess<T, U>(this Result<T> result, Func<Result<T>, Result<U>> apply)
        {
            switch (result.IsSuccess)
            {
                case true: return apply(result);
                default  : return Result<U>.Failure(result.Remainder, result.Message);
            }
        }

        public static Result<T> IfFailure<T>(this Result<T> result, Func<Result<T>, Result<T>> apply)
        {
            switch (result.IsSuccess)
            {
                case false: return apply(result);
                default   : return Result<T>.Success(result.Value, result.Remainder);
            }
        }
    }
}