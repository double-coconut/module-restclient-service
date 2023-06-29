using System.Collections.Generic;
using BestHTTP;

namespace Services.Networking.RestClient.Requests
{
    public static class RequestProvider
    {
        public static Request Get(string url, Dictionary<string, string> headers = null)
        {
            var request = new Request(url, HTTPMethods.Get);
            request.TryAddHeaders(headers);
            return request;
        }

        public static Request<TResponse> Get<TResponse>(string url, Dictionary<string, string> headers = null)
        {
            var request = new Request<TResponse>(url, HTTPMethods.Get);
            request.TryAddHeaders(headers);
            return request;
        }

        public static Request<TRequest, TResponse> Get<TRequest, TResponse>(string url,
            Dictionary<string, string> headers = null)
        {
            var request = new Request<TRequest, TResponse>(url, HTTPMethods.Get);
            request.TryAddHeaders(headers);
            return request;
        }


        public static Request Post(string url, Dictionary<string, string> headers = null)
        {
            var request = new Request(url, HTTPMethods.Post);
            request.TryAddHeaders(headers);
            return request;
        }

        public static Request<TResponse> Post<TResponse>(string url, Dictionary<string, string> headers = null)
        {
            var request = new Request<TResponse>(url, HTTPMethods.Post);
            request.TryAddHeaders(headers);
            return request;
        }

        public static Request<TRequest, TResponse> Post<TRequest, TResponse>(string url,
            Dictionary<string, string> headers = null)
        {
            var request = new Request<TRequest, TResponse>(url, HTTPMethods.Post);
            request.TryAddHeaders(headers);
            return request;
        }

        
        public static Request Put(string url, Dictionary<string, string> headers = null)
        {
            var request = new Request(url, HTTPMethods.Put);
            request.TryAddHeaders(headers);
            return request;
        }

        public static Request<TRequest, TResponse> Put<TRequest, TResponse>(string url,
            Dictionary<string, string> headers = null)
        {
            var request = new Request<TRequest, TResponse>(url, HTTPMethods.Put);
            request.TryAddHeaders(headers);
            return request;
        }

        private static bool TryAddHeaders(this Request request, Dictionary<string, string> headers)
        {
            if (headers == null || headers.Count == 0)
            {
                return false;
            }

            request.AddHeaders(headers);
            return true;
        }

        public static Request Clone(Request request, bool withHeaders = false)
        {
            var newRequest = new Request(request.Url, request.MethodType);
            newRequest.InitByTarget(request);
            return newRequest;
        }

        public static Request<TResponse> Clone<TResponse>(Request request, bool withHeaders = false)
        {
            var newRequest = new Request<TResponse>(request.Url, request.MethodType);
            newRequest.InitByTarget(request, withHeaders);
            return newRequest;
        }

        public static Request<TRequest, TResponse> Clone<TRequest, TResponse>(Request request)
        {
            var newRequest = new Request<TRequest, TResponse>(request.Url, request.MethodType);
            newRequest.InitByTarget(request);
            return newRequest;
        }

        private static void InitByTarget(this Request origin, Request target, bool withHeaders = false)
        {
            origin.SetHttpRequest(target.CopyHttpRequest(withHeaders));
            origin.Retries = target.Retries;
            origin.RetryDelay = target.RetryDelay;
            origin.TimeOut = target.TimeOut;
        }
    }
}