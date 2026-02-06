namespace SmartMicrobus.Core.DTO.Common
{
    public class ApiResponseWithData<T> : ApiResponse
    {
        public T? Data { get; set; }

        public ApiResponseWithData(string message, T data)
        {
            Success = true;
            Message = message;
            Data = data;
            StatusCode = 200;
        }
    }
}
