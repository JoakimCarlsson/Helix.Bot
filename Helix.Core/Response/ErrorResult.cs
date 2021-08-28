using Helix.Core.Abstractions;

namespace Helix.Core.Response
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