using System.Net;

namespace api.Common
{
    public class ExtServiceException : Exception
    {      
        public HttpStatusCode? StatusCode { get;}
        public ExtServiceException(string message, HttpStatusCode? statusCode ) : base(message) {

            StatusCode = statusCode;
        }
        public ExtServiceException(string message, Exception inner, HttpStatusCode? statusCode) : base(message, inner) {

            StatusCode = statusCode;
        }
    }
}
