namespace JPlayer.Lib.Contract
{
    public class ApiResult<T>
    {
        public string ResourceType { get; set; }

        public T Data { get; set; }
    }

    public static class ApiResultExtension
    {
        public static ApiResult<T> AsApiResult<T>(this T result, string resourceType = "") => new ApiResult<T>
        {
            Data = result,
            ResourceType = resourceType
        };
    }
}