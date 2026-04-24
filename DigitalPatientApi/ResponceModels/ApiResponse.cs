namespace DigitalPatientApi.ResponceModels
{
  public class ApiResponse<T>
  {
    public bool Success { get; set; }
    public T Data { get; set; }
    public string Message { get; set; }
    public string ErrorCode { get; set; }
    public object Pagination { get; set; }

    public static ApiResponse<T> Ok(T data, string message = null, object pagination = null)
    {
      return new ApiResponse<T>
      {
        Success = true,
        Data = data,
        Message = message,
        Pagination = pagination
      };
    }

    public static ApiResponse<T> Error(string message, string errorCode = null)
    {
      return new ApiResponse<T>
      {
        Success = false,
        Data = default,
        Message = message,
        ErrorCode = errorCode,
      };
    }
  }
}
