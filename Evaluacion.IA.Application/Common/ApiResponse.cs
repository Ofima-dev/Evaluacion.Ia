namespace Evaluacion.IA.Application.Common
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new();

        private ApiResponse(bool isSuccess, string message, T? data = default, List<string>? errors = null)
        {
            IsSuccess = isSuccess;
            Message = message;
            Data = data;
            Errors = errors ?? new List<string>();
        }

        public static ApiResponse<T> Success(T data, string message = "Operación exitosa")
        {
            return new ApiResponse<T>(true, message, data);
        }

        public static ApiResponse<T> Success(string message = "Operación exitosa")
        {
            return new ApiResponse<T>(true, message);
        }

        public static ApiResponse<T> Failure(string message, List<string>? errors = null)
        {
            return new ApiResponse<T>(false, message, default, errors);
        }

        public static ApiResponse<T> Failure(List<string> errors, string message = "Operación fallida")
        {
            return new ApiResponse<T>(false, message, default, errors);
        }
    }
}
