namespace Helix.Core.Abstractions
{
    public interface IError
    {
        string ErrorMessage { get; }
        IError InnerError { get; }
    }
}