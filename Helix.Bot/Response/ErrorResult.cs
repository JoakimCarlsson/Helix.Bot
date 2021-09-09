using Helix.Bot.Abstractions;

namespace Helix.Bot.Response
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