namespace CacheSmartProject.Application.Exceptions
{
    public class RedisUnavailableException : Exception
    {
        public RedisUnavailableException(string message) : base(message) { }
    }
}
