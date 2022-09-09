using FluentValidation.Results;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace AstroOdysseyCore
{
    public class ActionCommandResponse
    {
        public string RequestUri
        {
            get;
            set;
        }

        //public string ExternalError
        //{
        //    get;
        //    set;
        //}

        public ValidationResult Errors
        {
            get;
            set;
        }

        public IEnumerable<string> ErrorMessages
        {
            get
            {
                if (Errors is not null)
                {
                    foreach (ValidationFailure error in Errors.Errors)
                    {
                        yield return error.ErrorMessage;
                    }
                }

                //if (!string.IsNullOrWhiteSpace(ExternalError))
                //{
                //    yield return ExternalError;
                //}
            }
        }

        public int StatusCode
        {
            get
            {
                if (Errors is not null)
                {
                    return !Errors.IsValid ? 1 : 0;
                }

                //if (!string.IsNullOrWhiteSpace(ExternalError))
                //{
                //    return 1;
                //}

                return ErrorMessages is not null && ErrorMessages.Any() ? 1 : 0;
            }
        }

        public HttpStatusCode HttpStatusCode => StatusCode == 0 ? HttpStatusCode.OK : HttpStatusCode.InternalServerError;

        public ActionCommandResponse()
        {
            Errors = new ValidationResult();
        }

        public ActionCommandResponse(ValidationResult result)
        {
            Errors = result;
        }

        public ActionCommandResponse SetError(string propertyName, string errorMessage)
        {
            var item = new ValidationFailure(propertyName, errorMessage);
            Errors.Errors.Add(item);
            return this;
        }

        public string ToJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public object Result { get; set; }

        public string Message { get; set; }

        public ActionCommandResponse WithMessage(string message)
        {
            Message = message;
            return this;
        }

        public ActionCommandResponse WithResult(object result)
        {
            Result = result;
            return this;
        }

        public ActionCommandResponse WithErrors(string[] errorMessages)
        {
            if (errorMessages is null) return this;

            foreach (var err in errorMessages)
            {
                SetError("", err);
            }
            return this;
        }
    }

    public static class Response
    {
        public static ActionCommandResponse Build()
        {
            return new ActionCommandResponse();
        }
    }
}
