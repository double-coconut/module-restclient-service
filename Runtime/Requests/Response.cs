namespace Services.Networking.RestClient.Requests
{
    public class Response
    {
        public int StatusCode;
        public string Message;
        public string DataText;
    }

    public class Response<T> : Response
    {
        public T Value;
    }
}