﻿using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Repositories;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.Commands.Items
{
    public record CreateItemsCommand(string ClientId, List<ItemDtoForClient> Items) : ICommand<bool>;
    public class CreateItemsCommandHandler(ClientRepository repository, IMapper mapper) : ICommandHandler<CreateItemsCommand, bool>
    {
        private readonly ClientRepository _repository = repository;
        private readonly IMapper _mapper = mapper;
        public async Task<BaseResponse<bool>> Handle(CreateItemsCommand request, CancellationToken cancellationToken)
        {
            if (!ObjectId.TryParse(request.ClientId, out var objectId))
                return new NotFoundResponse<bool>("Invalid client ID format");
            var client = await _repository.GetByIdAsync(objectId, cancellationToken);
            if (client == null || client.IsDeleted)
                return new NotFoundResponse<bool>("Client not found");
            await _repository.AddItems(objectId, _mapper.Map<List<Item>>(request.Items), cancellationToken);
            return new SuccessResponse<bool>(true);
        }
    }
}
