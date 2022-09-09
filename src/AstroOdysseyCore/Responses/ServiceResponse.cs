using FluentValidation.Results;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace AstroOdysseyCore
{
    public class ServiceResponse
    {
        public string RequestUri { get; set; }

        public string ExternalError { get; set; }

        public HttpStatusCode HttpStatusCode { get; set; } = HttpStatusCode.OK;

        public object Result { get; set; }

        public ServiceResponse BuildSuccessResponse(object result, string requestUri = null)
        {
            return new ServiceResponse() { HttpStatusCode = HttpStatusCode.OK, Result = result, RequestUri = requestUri };
        }

        public ServiceResponse BuildErrorResponse(string error, string requestUri = null)
        {
            return new ServiceResponse() { HttpStatusCode = HttpStatusCode.InternalServerError, ExternalError = error, RequestUri = requestUri };
        }
    }

    public static class Response
    {
        public static ServiceResponse Build()
        {
            return new ServiceResponse();
        }
    }
}
