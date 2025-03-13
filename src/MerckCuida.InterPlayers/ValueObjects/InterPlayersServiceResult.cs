namespace MerckCuida.InterPlayers.ValueObjects
{
    public class InterPlayersServiceResult<T>
    {
        public bool Success { get; set; }
        public string? RawResult { get; set; }

        public T? Data { get; set; }
        public InterPlayersError? ErrorData { get; set; }

        public InterPlayersServiceResult(bool success, string? rawResult, T? data, InterPlayersError? errorData)
        {
            Success = success;
            RawResult = rawResult;
            Data = data;
            ErrorData = errorData;
        }

        public static InterPlayersServiceResult<T> CreateSuccess(T? data, string? rawResult) => new InterPlayersServiceResult<T>(true, rawResult, data, null);
        public static InterPlayersServiceResult<T> CreateError(InterPlayersError error, string? rawResult) => new InterPlayersServiceResult<T>(false, rawResult, default(T), error);
    }
}