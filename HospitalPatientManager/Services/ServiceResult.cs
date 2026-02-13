namespace HospitalPatientManager.Services;

public class ServiceResult<T>
{
    public bool Success { get; private set; }
    public string Message { get; private set; } = string.Empty;
    public T? Data { get; private set; }
    public List<string> Errors { get; private set; } = new();

    public static ServiceResult<T> Ok(T data, string message = "Success")
        => new() { Success = true, Data = data, Message = message };

    public static ServiceResult<T> Fail(string message, params string[] errors)
        => new() { Success = false, Message = message, Errors = errors.ToList() };
}
