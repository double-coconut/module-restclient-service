using Services.Networking.RestClient.Requests;

namespace Services.Networking.RestClient.Interceptors
{
    public class TestInterceptor : IInterceptor
    {
        private readonly string _token;

        public TestInterceptor(string token)
        {
            _token = token;
        }

        public Request ProcessRequest(Request request)
        {
            request.AddHeader("Authorization",$"Bearer {_token}");
            return request;
        }
    }
}