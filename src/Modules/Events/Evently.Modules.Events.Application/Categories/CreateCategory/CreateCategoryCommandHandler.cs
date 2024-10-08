﻿using Evently.Common.Application.Messaging;
using Evently.Common.Domain.Abstractions;
using Evently.Modules.Events.Application.Abstraction.Data;
using Evently.Modules.Events.Domain.Categories;

namespace Evently.Modules.Events.Application.Categories.CreateCategory;

internal sealed class CreateCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    : ICommandHandler<CreateCategoryCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = Category.Create(request.Name);

        categoryRepository.Insert(category);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return category.Id;
    }
}
