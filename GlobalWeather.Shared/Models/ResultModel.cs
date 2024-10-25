namespace GlobalWeather.Shared.Models;

public class ResultModel<T>
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public T? Result { get; set; }

    public static ResultModel<T> SuccessResult(T result) => new()
    {
        Success = true,
        Result = result
    };

    public static ResultModel<T> ErrorResult(string error) => new()
    {
        Error = error
    };
}