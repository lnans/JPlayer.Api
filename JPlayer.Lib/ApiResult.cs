namespace JPlayer.Lib
{
    public class ApiResult<T> where T : class
    {
        public string ResourceType { get; set; }

        public T Data { get; set; }
    }

    public static class ApiResultExtension
    {
        public static ApiResult<T> AsApiResult<T>(this T result, string resourceType = "") where T : class => new ApiResult<T>
        {
            Data = result,
            ResourceType = resourceType
        };
    }
}