using System.Collections.Generic;
using System.Linq;
using Services.Networking.RestClient.Requests;

namespace Services.Networking.RestClient.Interceptors
{
    public class InterceptorsSet : IInterceptor
    {
        private readonly LinkedList<IInterceptor> _interceptors;

        public InterceptorsSet()
        {
            _interceptors = new LinkedList<IInterceptor>();
        }

        public Request ProcessRequest(Request request)
        {
            return _interceptors.Aggregate(request, (current, interceptor) => interceptor.ProcessRequest(current));
        }

        public void AddInterceptor(IInterceptor newInterceptor)
        {
            _interceptors.AddLast(newInterceptor);
        }

        public void RemoveInterceptor(IInterceptor interceptorToRemove)
        {
            _interceptors.Remove(interceptorToRemove);
        }

        public void Clear()
        {
            _interceptors.Clear();
        }
    }
}