using System.Threading;
using System.Threading.Tasks;
using Services.Networking.RestClient.Requests;

namespace Services.Networking.RestClient.Controllers
{
    public abstract class AbstractController
    {
        protected Request Request { get; private set; }
        

        public abstract Task ProcessRequest(CancellationToken cancellationToken = default);
        public abstract Task<Response<TResponse>> ProcessResponse<TResponse>(Response<TResponse> response, CancellationToken cancellationToken = default);
        internal void InitRequest(Request request)
        {
            Request = request;
        }

    }
}