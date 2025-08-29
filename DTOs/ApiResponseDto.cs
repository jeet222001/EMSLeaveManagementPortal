namespace EMSLeaveManagementPortal.DTOs
{
    public class ApiResponseDto<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }
        public int StatusCode { get; set; }

        public ApiResponseDto(bool success, string message, T? data = default, int statusCode = 200)
        {
            Success = success;
            Message = message;
            Data = data;
            StatusCode = statusCode;
        }
    }
}