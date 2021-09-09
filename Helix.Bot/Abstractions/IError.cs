namespace Helix.Bot.Abstractions
{
    public interface IError
    {
        string ErrorMessage { get; }
        IError InnerError { get; }
    }
}