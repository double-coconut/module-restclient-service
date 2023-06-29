using System;

namespace RestClientService.Exceptions
{
    public class RequestFailedException : Exception
    {
        public int ErrorCode { get; }
        public string ResponseText { get; }
       
        
        public RequestFailedException(string errorMessage) : this(484,string.Empty,errorMessage)
        {
        }
        
        public RequestFailedException(int errorCode, string errorMessage) : this(errorCode,string.Empty,errorMessage)
        {
        }

        public RequestFailedException(string responseText,string errorMessage) : this(484,responseText,errorMessage)
        {
        }
        
        public RequestFailedException(int errorCode, string responseText , string errorMessage) : base(
            $"Request Failed With Code:{errorCode} and message: {errorMessage}", new Exception(errorMessage))
        {
            ErrorCode = errorCode;
            ResponseText = responseText;
        }
    }
}