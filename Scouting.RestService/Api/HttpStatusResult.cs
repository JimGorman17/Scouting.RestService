using System.Net;

namespace Scouting.RestService.Api
{
    public class HttpStatusResult
    {
        private readonly HttpStatusCode _httpStatusCode;
        public HttpStatusResult(HttpStatusCode httpStatusCode)
        {
            _httpStatusCode = httpStatusCode;
        }

        public int HttpStatusCode { get { return (int)_httpStatusCode; } }
        public string Message { get { return _httpStatusCode.ToString(); } }
    }
}