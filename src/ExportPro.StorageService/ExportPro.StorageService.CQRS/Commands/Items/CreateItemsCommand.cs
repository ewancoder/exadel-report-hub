using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Repositories;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;

namespace ExportPro.StorageService.CQRS.Commands.Items
{
    public record CreateItemsCommand(List<ItemDTO> Items) : ICommand<bool>;
    public class CreateItemsCommandHandler(ItemRepository repository, IMapper mapper) : ICommandHandler<CreateItemsCommand, bool>
    {
        private readonly ItemRepository _repository = repository;
        private readonly IMapper _mapper = mapper;
        public async Task<BaseResponse<bool>> Handle(CreateItemsCommand request, CancellationToken cancellationToken)
        {
             await _repository.AddManyAsync(_mapper.Map<List<Item>>(request.Items), cancellationToken);
             return new SuccessResponse<bool>(true);
        }
    }
}
