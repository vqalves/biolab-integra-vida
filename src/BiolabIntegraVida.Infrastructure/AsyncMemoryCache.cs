namespace BiolabIntegraVida.Infrastructure
{
    public class AsyncMemoryCache<T>
    {
        private readonly Func<Task<T>> _dataGenerator;
        private readonly Func<T, TimeSpan> _expirationTimeGenerator;
        private readonly SemaphoreSlim _semaphore;

        private T? _cachedData;
        private DateTime? _dateTimeLimit;

        public AsyncMemoryCache(Func<Task<T>> dataGenerator, Func<T, TimeSpan> expirationTimeGenerator)
        {
            _dataGenerator = dataGenerator ?? throw new ArgumentNullException(nameof(dataGenerator));
            _expirationTimeGenerator = expirationTimeGenerator ?? throw new ArgumentNullException(nameof(expirationTimeGenerator));
            _semaphore = new SemaphoreSlim(1, 1);
        }

        public async Task<T> GetDataAsync()
        {
            if (_dateTimeLimit == null || _dateTimeLimit <= DateTime.Now)
            {
                await _semaphore.WaitAsync();

                try
                {
                    if (_dateTimeLimit == null || _dateTimeLimit <= DateTime.Now)
                    {
                        _cachedData = await _dataGenerator();

                        var dataTimeSpan = _expirationTimeGenerator(_cachedData);
                        _dateTimeLimit = DateTime.Now.Add(dataTimeSpan);
                    }
                }
                finally
                {
                    _semaphore.Release();
                }
            }

            return _cachedData!;
        }
    }
}