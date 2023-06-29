using RestClientService.Requests;

namespace RestClientService.Interceptors
{
    public interface IInterceptor
    {
        Request ProcessRequest(Request request);
    }
}