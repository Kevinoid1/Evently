namespace Evently.Modules.Events.Domain.Abstractions;

public record Error
{
    public string Code { get; }
    public string Description { get; }
    public ErrorType Type { get; }

   
    
    protected Error(string code, string description, ErrorType type)
    {
        Code = code;
        Description = description;
        Type = type;
    }
        
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);
    public static readonly Error NullValue = new("General.Null", "Null value was provided", ErrorType.Failure);
    public static Error Failure(string code = "General.Failure", string description = "A failure occured.") => new(code, description, ErrorType.Failure);
    public static Error Conflict(string code = "General.Conflict", string description = "A conflict error occured.") => new(code, description, ErrorType.Conflict);
    public static Error Problem(string code = "General.Problem", string description = "A problem error has occured.") => new(code, description, ErrorType.Problem);
    public static Error NotFound(string code = "General.NotFound", string description = "A 'Not Found' error has occured.") => new(code, description, ErrorType.NotFound);
    public static Error Validation(string code = "General.Validation", string description = "A validation error has occured.") => new(code, description, ErrorType.Validation);
    public static Error UnExpected(string code = "General.Unexpected", string description = "An unexpected error has occured.") => new(code, description, ErrorType.UnExpected);
    public static Error Unauthorized(string code = "General.Unauthorized", string description = "An unauthorized error has occured.") => new(code, description, ErrorType.Unauthorized);
    public static Error Forbidden(string code = "General.Forbidden", string description = "A forbidden error has occured.") => new(code, description, ErrorType.Forbidden);
        
}

public enum ErrorType
{
    Failure = 0,
    Validation = 1,
    Problem = 2,
    NotFound = 3,
    Conflict = 4,
    UnExpected = 5,
    Unauthorized = 6,
    Forbidden = 7
}
