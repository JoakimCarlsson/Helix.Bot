using Helix.Services.Abstractions;

namespace Helix.Services.Services
{
    public class ErrorResult : IError
    {
        public string ErrorMessage { get; }
        public IError InnerError { get; set; }

        public ErrorResult(string errorMessage, IError innerError = default)
        {
            ErrorMessage = errorMessage;
            InnerError = innerError;
        }
    }
}