namespace api.Common
{
    public class ExtServiceException : Exception
    {        
        public ExtServiceException(string message) : base(message) { }
        public ExtServiceException(string message, Exception inner) : base(message, inner) { }
    }
}
