using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Repositories;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.Commands.Items;

public record CreateItemCommand(string Name,
    string Description, 
    double Price, 
    string CustomerId, 
    string InvoiceId, 
    string ClientId): ICommand<string>;

public class CreateItemCommandHandler(ItemRepository repository, CustomerRepository customerRepository) : ICommandHandler<CreateItemCommand, string>
{
    private readonly ItemRepository _repository = repository;
    private readonly CustomerRepository _customerRepository = customerRepository;
    public async Task<BaseResponse<string>> Handle(CreateItemCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(new ObjectId(request.CustomerId), cancellationToken);
        if(customer == null)
        {
            return new NotFoundResponse<string>("Customer not found");
        }
        var item = new Models.Models.Item
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            CustomerId = request.CustomerId,
            InvoiceId = request.InvoiceId,
            ClientId = request.ClientId
        };
        await _repository.AddOneAsync(item, cancellationToken);
        return new SuccessResponse<string>(item.Id.ToString());
    }
}



