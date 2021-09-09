namespace Helix.Services.Abstractions
{
    public interface IError
    {
        string ErrorMessage { get; }
        IError InnerError { get; }
    }
}