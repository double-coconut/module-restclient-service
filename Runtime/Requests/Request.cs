using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BestHTTP;
using BestHTTP.Extensions;
using BestHTTP.Forms;
using Newtonsoft.Json;
using RestClientService.Controllers;
using RestClientService.Exceptions;
using RestClientService.Utils;

namespace RestClientService.Requests
{
    public class Request : IDisposable
    {
        private readonly string _url;
        private readonly HTTPMethods _methodType;
        private readonly LinkedList<AbstractController> _controller;


        public double TimeOut
        {
            get => HttpRequest.Timeout.TotalSeconds;
            set => HttpRequest.Timeout = TimeSpan.FromSeconds(value);
        }

        public string Url => _url;
        public HTTPMethods MethodType => _methodType;
        public int Retries { get; set; }
        public int CurrentRetries { get; private set; }
        public double RetryDelay { get; set; }
        public HTTPRequest HttpRequest { get; protected set; }


        internal Request(string url, HTTPMethods methodType)
        {
            _url = url;
            _methodType = methodType;
            Uri uri = new Uri(url);
#if REST_CLIENT_LOGGING
            Debug.Log($"<color=yellow>Create {methodType.ToString()} Request</color> to :{url} URL");
#endif
            HttpRequest = new HTTPRequest(uri, methodType);
            HttpRequest.Timeout = TimeSpan.FromSeconds(10);
            HttpRequest.ConnectTimeout = TimeSpan.FromSeconds(10);
            HttpRequest.SetHeader("Content-type", "application/json");
            Retries = 0;
            CurrentRetries = 0;
            RetryDelay = 0.5;
            _controller = new LinkedList<AbstractController>();
        }


        public async Task<Response<HTTPResponse>> Send(CancellationToken cancellationToken = default)
        {
            await ProcessControllersRequest(cancellationToken);
            var response = await SendRequest(cancellationToken);
            return await ProcessControllersResult(response, cancellationToken);
        }


        public Request WithController(params AbstractController[] controller)
        {
            foreach (var ctrl in controller)
            {
                ctrl.InitRequest(this);
                _controller.AddLast(ctrl);
            }

            return this;
        }

        public void Abort()
        {
            HttpRequest.Abort();
        }


        public Request AddHeader(string name, string value)
        {
#if REST_CLIENT_LOGGING
            Debug.Log($"<color=yellow>Add header</color> to :<color=orange>Key: {name}</color>, Value: {value}");
#endif
            HttpRequest.AddHeader(name, value);
            return this;
        }

        public Request AddHeaders(Dictionary<string, string> headers)
        {
            foreach (KeyValuePair<string, string> header in headers)
            {
                AddHeader(header.Key, header.Value);
            }

            return this;
        }

        internal void SetHttpRequest(HTTPRequest request)
        {
            HttpRequest = request;
        }


        protected async Task<Response<HTTPResponse>> SendRequest(CancellationToken cancellationToken)
        {
#if REST_CLIENT_LOGGING
            Debug.Log(
                $"<color=yellow>Making request</color> to :<color=orange>{Url}</color>, with request method :{HttpRequest.MethodType}");
#endif
            try
            {
                HTTPResponse response = await HttpRequest.GetHTTPResponseAsync(cancellationToken);
                if (response == null || string.IsNullOrEmpty(response.DataAsText))
                {
                    throw new RequestFailedException(
                        $"RequestFailedException :: Response is: {(response == null ? "NULL" : "Not Null")},\n" +
                        $"Data As Text is NULL, Status code : {response?.StatusCode}, Message : {response?.Message}");
                }

#if REST_CLIENT_LOGGING
                Debug.Log(
                    $"<color=green>Receive response</color> from :<color=orange>{Url}</color>, with request method :{HttpRequest.MethodType} and body :\n{response.DataAsText}");
#endif
                return new Response<HTTPResponse>
                {
                    StatusCode = response.StatusCode,
                    Message = response.Message,
                    DataText = response.DataAsText,
                    Value = response
                };
            }
            catch (Exception exception)
            {
                if (Retries > 0 && CurrentRetries < Retries)
                {
                    CurrentRetries++;
#if REST_CLIENT_LOGGING
                    Debug.LogWarning(
                        $"<color=orange>Retry Request to {Url}, Retries count is :{Retries}, Current Retries :{CurrentRetries}!</color>");
#endif
                    await Task.Delay(TimeSpan.FromSeconds(RetryDelay), cancellationToken);
                    HttpRequest = CopyHttpRequest(true);
                    return await Send(cancellationToken);
                }

                if (exception.GetType() == typeof(RequestFailedException) ||
                    exception.GetType() == typeof(TimeoutException) ||
                    exception.GetType() == typeof(OperationCanceledException))
                {
                    throw;
                }

                throw new RequestFailedException(exception.Message);
            }
            finally
            {
#if REST_CLIENT_LOGGING
                Debug.Log($"Dispose request to :{Url}");
#endif
                Dispose();
            }
        }


        protected async Task<Response<TResponse>> SendRequest<TResponse>(CancellationToken cancellationToken)
        {
            var response = await SendRequest(cancellationToken);
            try
            {
                var deserializedResp = JsonConvert.DeserializeObject<TResponse>(response.DataText);
                return new Response<TResponse>
                {
                    StatusCode = response.StatusCode,
                    Message = response.Message,
                    DataText = response.DataText,
                    Value = deserializedResp
                };
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        protected async Task ProcessControllersRequest(CancellationToken cancellationToken)
        {
            try
            {
                foreach (var controller in _controller)
                {
                    await controller.ProcessRequest(cancellationToken);
                }
            }
            finally
            {
                Dispose();
            }
        }

        protected async Task<Response<TResp>> ProcessControllersResult<TResp>(Response<TResp> resp,
            CancellationToken cancellationToken)
        {
            try
            {
                foreach (var controller in _controller)
                {
                    resp = await controller.ProcessResponse(resp, cancellationToken);
                }
            }
            finally
            {
                Dispose();
            }

            return resp;
        }


        internal HTTPRequest CopyHttpRequest(bool withHeaders)
        {
            var request = new HTTPRequest(HttpRequest.Uri, HttpRequest.MethodType);
            request.Timeout = HttpRequest.Timeout;
            request.ConnectTimeout = HttpRequest.ConnectTimeout;
            if (withHeaders)
            {
                HttpRequest.EnumerateHeaders((header, values) =>
                {
                    foreach (string value in values)
                    {
                        request.AddHeader(header, value);
                    }
                });
            }

            if (HttpRequest.GetFormFields() != null)
            {
                foreach (HTTPFieldData fieldData in HttpRequest.GetFormFields())
                {
                    request.AddBinaryData(fieldData.FileName, fieldData.Binary, fieldData.Name);
                }
            }

            request.FormUsage = HttpRequest.FormUsage;
            request.RawData = HttpRequest.RawData;
            return request;
        }

        public void Dispose()
        {
            HttpRequest?.Dispose();
        }
    }


    public class Request<TResponse> : Request
    {
        internal Request(string url, HTTPMethods methodType) : base(url, methodType)
        {
        }


        public new async Task<Response<TResponse>> Send(CancellationToken cancellationToken = default)
        {
            await ProcessControllersRequest(cancellationToken);
            var response = await SendRequest<TResponse>(cancellationToken);
            response = await ProcessControllersResult(response, cancellationToken);
            return response;
        }

        public async Task<Response<TResponse>> Send(string binaryFieldName, byte[] data, string fileName,
            HTTPFormUsage formUsage = HTTPFormUsage.Multipart,
            CancellationToken cancellationToken = default)
        {
            HttpRequest.AddBinaryData(binaryFieldName, data, fileName);
            HttpRequest.FormUsage = formUsage;
            await ProcessControllersRequest(cancellationToken);
            var result = await this.Send(cancellationToken);
            result = await ProcessControllersResult(result, cancellationToken);
            return result;
        }
    }


    public class Request<TRequest, TResponse> : Request<TResponse>
    {
        internal Request(string url, HTTPMethods methodType) : base(url, methodType)
        {
        }


        public async Task<Response<TResponse>> Send(TRequest requestBody, CancellationToken cancellationToken = default)
        {
            await ProcessControllersRequest(cancellationToken);
            try
            {
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ContractResolver = new WritablePropertiesOnlyResolver(),
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
                string json = JsonConvert.SerializeObject(requestBody, settings);
#if REST_CLIENT_LOGGING
                Debug.Log($"<color=yellow>Send Request</color> : {json}");
#endif
                byte[] rawData = json.GetASCIIBytes().Data;
                HttpRequest.RawData = rawData;

                var result = await SendRequest<TResponse>(cancellationToken);
                result = await ProcessControllersResult(result, cancellationToken);
                return result;
            }
            catch
            {
                Dispose();
                throw;
            }
        }
    }
}