using System.Collections.Generic;

namespace JPlayer.Lib.Contract
{
    public class ApiError
    {
        public string Error { get; set; }

        public IEnumerable<string> Details { get; set; }

        public string StackTrace { get; set; }

        public string InnerException { get; set; }
    }

    public static class ApiErrorExtension
    {
        public static ApiError AsApiError(this System.Exception exception, bool useStackTrace = false, System.Exception innerException = null) => new ApiError
        {
            Error = exception.Message,
            StackTrace = useStackTrace ? exception.StackTrace : string.Empty,
            InnerException = innerException?.Message,
            Details = new[] {innerException?.Message}
        };
    }
}