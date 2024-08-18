namespace Evently.Modules.Events.Domain.Events;

public class Result
{
    private Result(bool isSucess, Error error)
    {
        IsSucess = isSucess;
        Error = error;
    }

    public bool IsSucess { get; }
    public bool IsFailure => !IsSucess;
    public Error Error { get; }
}
