namespace CatchUpPlatform.API.Shared.Application.Patterns;

/// <summary>
///     Represents the outcome of an operation, which can either be a success with a value of type <typeparamref name="TValue"/>
///     or a failure with an error of type <typeparamref name="TError"/>.
/// </summary>
/// <typeparam name="TValue">The type of the successful result value.</typeparam>
/// <typeparam name="TError">The type of the failure error.</typeparam>
public abstract record Result<TValue, TError>
{
    /// <summary>
    ///     Represents a successful outcome containing a value.
    /// </summary>
    /// <param name="Value">The value associated with the successful result.</param>
    public sealed record Success(TValue Value) : Result<TValue, TError>;

    /// <summary>
    ///     Represents a failed outcome containing an error.
    /// </summary>
    /// <param name="Error">The error associated with the failed result.</param>
    public sealed record Failure(TError Error) : Result<TValue, TError>;
    
    /// <summary>
    ///     Gets a value indicating whether the result is successful.
    /// </summary>
    public bool IsSuccess => this is Success;

    /// <summary>
    ///     Gets a value indicating whether the result is a failure.
    /// </summary>
    public bool IsFailure => this is Failure;

    /// <summary>
    ///     Transforms the successful value using the specified mapping function, preserving any failure error.
    /// </summary>
    /// <typeparam name="TNext">The type of the transformed value.</typeparam>
    /// <param name="onSuccess">A function to apply to the successful value.</param>
    /// <returns>A new <see cref="Result{TNext, TError}"/> instance.</returns>
    public Result<TNext, TError> Map<TNext>(Func<TValue, TNext> onSuccess) =>
        this switch
        {
            Success s => new Result<TNext, TError>.Success(onSuccess(s.Value)),
            Failure f => new Result<TNext, TError>.Failure(f.Error),
            _ => throw new InvalidOperationException("Unknown Result type")
        };
    
    /// <summary>
    ///     Evaluates a matching function based on whether the result is a success or failure, returning the resulting value.
    /// </summary>
    /// <typeparam name="TResult">The return type of the folding functions.</typeparam>
    /// <param name="onSuccess">The function to execute if the result is successful.</param>
    /// <param name="onFailure">The function to execute if the result is a failure.</param>
    /// <returns>The result of evaluating either <paramref name="onSuccess"/> or <paramref name="onFailure"/>.</returns>
    public TResult Fold<TResult>(Func<TValue, TResult> onSuccess, Func<TError, TResult> onFailure) =>
        this switch
        {
            Success s => onSuccess(s.Value),
            Failure f => onFailure(f.Error),
            _ => throw new InvalidOperationException("Unknown Result type")
        };

    /// <summary>
    ///     Executes a matching action depending on whether the result is a success or failure.
    /// </summary>
    /// <param name="onSuccess">The action to execute if the result is successful.</param>
    /// <param name="onFailure">The action to execute if the result is a failure.</param>
    public void Match(Action<TValue> onSuccess, Action<TError> onFailure)
    {
        if (this is Success s) onSuccess(s.Value);
        else if (this is Failure f) onFailure(f.Error);
    }
}