using Evently.Common.Application.Messaging;

namespace Evently.Modules.Users.Application.Users.RegisterUser;

public sealed record RegisterUserCommand(string Email, string Password, string FirstName, string LastName)
    : ICommand<Guid>;

public sealed record RegisterAdminUserCommand(string Email, string Password, string FirstName, string LastName)
    : ICommand<Guid>;
