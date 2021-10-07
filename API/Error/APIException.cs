namespace API.Error
{
    public class APIException
    {
        private int statusCode;
        
        private string message;

        private string details;
        
        private  int _priority;

        public APIException(int statusCode, string message = null, string details = null, int priority = 1)
        {
            StatusCode = statusCode;
            Message = message;
            Details = details;
            _priority = priority;
        }

        public int StatusCode { get => statusCode; set => statusCode = value; }
        public string Message { get => message; set => message = value; }
        public string Details { get => details; set => details = value; }
        public int priority { get => _priority; set => _priority = value; }

    }
}