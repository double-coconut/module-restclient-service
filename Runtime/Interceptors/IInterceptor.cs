using Services.Networking.RestClient.Requests;

namespace Services.Networking.RestClient.Interceptors
{
    public interface IInterceptor
    {
        Request ProcessRequest(Request request);
    }
}