namespace CacheSmartProject.Application.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }

    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message) { }
    }

    public class CacheUpdateException : Exception
    {
        public CacheUpdateException(string message) : base(message) { }
    }

    public class RedisUnavailableException : Exception
    {
        public RedisUnavailableException(string message) : base(message) { }
    }

    public class DatabaseException : Exception
    {
        public DatabaseException(string message) : base(message) { }
    }
}
