using System.Diagnostics;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.DataAccess.Repositories;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.Mapping;
using ExportPro.StorageService.SDK.Responses;
using MongoDB.Bson;
using MongoDB.Driver;
using SharpCompress.Readers.Rar;

namespace ExportPro.StorageService.DataAccess.Services;

public class ClientService:IClientService
{
    private readonly ClientRepository _clientRepository;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly CustomerRepository _customerRepository;
    public ClientService(ClientRepository clientRepository, IInvoiceRepository invoiceRepository,CustomerRepository customerRepository)
    {
        _invoiceRepository = invoiceRepository;
        _clientRepository = clientRepository;
        _customerRepository = customerRepository;
    }

    public async Task<List<ClientResponse>> GetClientsService()
    {
        var clients = await _clientRepository.GetClients();
        var clientresponse = clients.Where(x=>x.IsDeleted==false).Select(x => ClientToClientResponse.ClientToClientReponse(x)).ToList();
        return clientresponse;
    }
    public async Task<ClientResponse> AddClientFromClientDto(ClientDto clientDto)
    {
        Client client = new()
        {
            Name = clientDto.Name,
            Description = clientDto.Description
        };
        await _clientRepository.AddOneAsync(client, CancellationToken.None);
        return ClientToClientResponse.ClientToClientReponse(client);
    }

    public async Task<ClientResponse> GetClientByIdIncludingSoftDeleted(ObjectId ClientId)
    {
        var client =await _clientRepository.GetOneAsync(x=>x.Id==ClientId,CancellationToken.None);
        if (client == null) return null;
        var clientresponse= ClientToClientResponse.ClientToClientReponse(client) ;
        return clientresponse;
    }

    public async Task<ClientResponse> AddItemIds(string Clientid, List<string> ItemIds)
    {
        var client = await _clientRepository.GetOneAsync(x => x.Id.ToString() == Clientid,CancellationToken.None);
        foreach (var id in ItemIds)
        {
            if (!client.ItemIds.Contains(id))
                client.ItemIds.Add(id);
        }
        client.UpdatedAt = DateTime.UtcNow;
        await _clientRepository.UpdateOneAsync(client, CancellationToken.None);
        return ClientToClientResponse.ClientToClientReponse(client);
    }
    public async Task<ClientResponse> AddInvoiceIds(string Clientid, List<string> InvoiceIds)
    {
        var client = await _clientRepository.GetOneAsync(x => x.Id.ToString() == Clientid,CancellationToken.None);
        foreach (var id in InvoiceIds)
        {
            if (!client.InvoiceIds.Contains(id))
                client.InvoiceIds.Add(id);
        }
        client.UpdatedAt = DateTime.UtcNow;
        await _clientRepository.UpdateOneAsync(client, CancellationToken.None);
        return ClientToClientResponse.ClientToClientReponse(client);
    }

    public async Task<ClientResponse> AddCustomerIds(string Clientid, List<string> customerids)
    {
        var client = await _clientRepository.GetOneAsync(x => x.Id.ToString() == Clientid,CancellationToken.None);
        foreach (var id in customerids)
        {
            if (!client.CustomerIds.Contains(id))
                client.InvoiceIds.Add(id);
        }
        client.UpdatedAt = DateTime.UtcNow;
        await _clientRepository.UpdateOneAsync(client, CancellationToken.None);
        return ClientToClientResponse.ClientToClientReponse(client);
    }

    public async Task<ClientResponse> GetClientById(string Clientid)
    {
        var client =await _clientRepository.GetOneAsync(x=>x.Id.ToString()==Clientid && x.IsDeleted==false, CancellationToken.None);
        if (client == null) return null;
        var clientresponse= ClientToClientResponse.ClientToClientReponse(client) ;
        return clientresponse;
    }
    public async Task<FullClientResponse> GetFullClient(string clientid)
    {
        var client = await GetClientById(clientid);
        var invoices = await _invoiceRepository.GetAllAsync(CancellationToken.None);
        var invoiceResponses = invoices.Select(x => new InvoiceResponse()
        {
            Id = x.Id.ToString(),
            InvoiceNumber = x.InvoiceNumber,
            DueDate = x.DueDate,
            Amount = x.Amount,
            Currency = x.Currency,
            PaymentStatus = x.PaymentStatus,
            BankAccountNumber = x.BankAccountNumber,
            ClientId = x.ClientId,
            ItemIds = x.ItemIds
        }).ToList();
        List<InvoiceResponse> invoicesresponse = new();
        var customers  = await _customerRepository.GetAllAsync(CancellationToken.None);
        var customerresponse = customers.Select(x => new CustomerResponse()
        {
            Id = x.Id.ToString(),
            Name = x.Name,
            Country= x.Country,
            Email=x.Email,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt,
            IsDeleted = x.IsDeleted
        }).ToList();
        if(client.CustomerIds==null) client.CustomerIds = new List<string>();
        if(client.InvoiceIds == null) client.InvoiceIds = new List<string>();
        List<CustomerResponse> customerresponses = new();
        foreach (var i in client.CustomerIds)
        {
            foreach (var j in customerresponse)
            {
                if (j.Id == i)
                {
                    customerresponses.Add(j);
                }
            }
        }
        foreach (var i in client.InvoiceIds)
        {
            foreach (var j in invoiceResponses)
            {
                if (j.Id == i)
                {
                    invoicesresponse.Add(j);
                }
            }
        }
        FullClientResponse fullClientResponse = new()
        {
            Id = client.Id,
            Name = client.Name,
            Description = client.Description,
            CreatedAt = client.CreatedAt,
            UpdatedAt = client.UpdatedAt,
            IsDeleted = client.IsDeleted,
            invoices = invoiceResponses,
            customers = customerresponses,
            items = new List<Item>()
        };
        return fullClientResponse;
    }
    public async Task<List<FullClientResponse>> GetAllFullClients()
    {
        var clients = await GetClientsService();
        var invoices = await _invoiceRepository.GetAllAsync(CancellationToken.None);
        var customers = await _customerRepository.GetAllAsync(CancellationToken.None);
       List<FullClientResponse> fullclients = new();
        foreach (var i in clients)
        {
            var fullclient = new FullClientResponse
            {
                Id = i.Id,
                Name = i.Name,
                Description = i.Description,
                CreatedAt = i.CreatedAt,
                UpdatedAt = i.UpdatedAt,
                IsDeleted = i.IsDeleted,
                invoices = invoices.Where(x => i.InvoiceIds.Contains(x.Id.ToString())).Select(x => new InvoiceResponse()
                {
                    Id = x.Id.ToString(),
                    InvoiceNumber = x.InvoiceNumber,
                    DueDate = x.DueDate,
                    Amount = x.Amount,
                    Currency = x.Currency,
                    PaymentStatus = x.PaymentStatus,
                    BankAccountNumber = x.BankAccountNumber,
                    ClientId = x.ClientId,
                    ItemIds = x.ItemIds
                }).ToList(),
                customers = customers.Where(x=>i.InvoiceIds.Contains(x.Id.ToString())).Select(x => new CustomerResponse()
                {
                    Id = x.Id.ToString(),
                    Name = x.Name,
                    Country = x.Country,
                    Email = x.Email,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    IsDeleted = x.IsDeleted
                }).ToList(),
                items = new List<Item>()
            };
            fullclients.Add(fullclient);
        }
        return fullclients;
    }

    public async Task<List<ClientResponse>> GetAllCLientsIncludingSoftDeleted()
    {
        var clients =await _clientRepository.GetClients();
        var clientresponses=clients.Select(x => ClientToClientResponse.ClientToClientReponse(x)).ToList();
        return clientresponses;
    }

    public async Task<ClientResponse> UpdateClient(ClientUpdateDto clientUpdateDto,string clientid)
    {
     
     Client client = await _clientRepository.GetOneAsync(x=>x.Id.ToString()==clientid, CancellationToken.None);
        if (clientUpdateDto.Name != null)  client.Name=clientUpdateDto.Name;
        if (clientUpdateDto.Description != null) client.Description = clientUpdateDto.Description;
        client.IsDeleted =clientUpdateDto.IsDeleted;
        client.UpdatedAt=DateTime.UtcNow;
        await _clientRepository.UpdateOneAsync(client, CancellationToken.None);
        var after = ClientToClientResponse.ClientToClientReponse(client);
        return after;
    }

    public async Task<string> SoftDeleteClient(ObjectId Clientid)
    {
        await _clientRepository.SoftDeleteAsync(Clientid, CancellationToken.None);
        return "Soft Deleted";
    }

    public async Task<string> DeleteClient(ObjectId Clientid)
    {
        await _clientRepository.DeleteAsync(Clientid,CancellationToken.None);
        return "Deleted";
    }

}   