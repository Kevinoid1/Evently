﻿using Evently.Common.Application.Messaging;
using Evently.Common.Domain.Abstractions;
using Evently.Modules.Users.Application.Abstractions.Data;
using Evently.Modules.Users.Application.Abstractions.Identity;
using Evently.Modules.Users.Domain.Users;

namespace Evently.Modules.Users.Application.Users.RegisterUser;

internal sealed class RegisterAdminUserCommandHandler(IIdentityProviderService identityProviderService, IUserRepository userRepository, IUnitOfWork unitOfWork)
    : ICommandHandler<RegisterAdminUserCommand, Guid>
{
    public async Task<Result<Guid>> Handle(RegisterAdminUserCommand request, CancellationToken cancellationToken)
    {
        Result<string> result = await identityProviderService.RegisterUserAsync(new UserModel(request.Email, request.Password,
            request.FirstName, request.LastName), cancellationToken);

        if (result.IsFailure)
        {
            return Result.Failure<Guid>(result.Error);
        }
        
        var user = User.Create(request.Email, request.FirstName, request.LastName, result.Value, true);

        userRepository.Insert(user);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}