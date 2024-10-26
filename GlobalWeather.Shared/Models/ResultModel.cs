namespace GlobalWeather.Shared.Models;

public class ResultModel<T>
{
    public bool Success { get; private init; }
    public string? ErrorMessage { get; set; }
    public List<string> Errors { get; set; } = [];
    public T? Result { get; set; }

    public static ResultModel<T> SuccessResult(T result) => new()
    {
        Success = true,
        Result = result
    };

    public static ResultModel<T> ErrorResult(
        string errorMessage,
        List<string>? errors = null) => new()
    {
        ErrorMessage = errorMessage,
        Errors = errors ?? []
    };
}