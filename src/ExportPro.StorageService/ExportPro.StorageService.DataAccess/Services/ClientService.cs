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
    public ClientService(ClientRepository clientRepository, IInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
        _clientRepository = clientRepository;
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
        //List<InvoiceResponse> res = client.Select(x => new InvoiceResponse()
        //{
        //    Id = x.Id.ToString(),
        //    InvoiceNumber = x.InvoiceNumber,
        //    DueDate = x.DueDate,
        //    Amount = x.Amount,
        //    Currency = x.Currency,
        //    PaymentStatus = x.PaymentStatus,
        //    BankAccountNumber = x.BankAccountNumber,
        //    ClientId = x.ClientId,
        //    ItemIds = x.ItemIds
        //}).ToList();
        //foreach (var i in client.InvoiceIds)
        //{
        //    foreach (var j in res)
        //    {
        //        if (j.Id == i)
        //        {

        //        }
        //    }
        //}
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
            invoices =invoiceResponses,
        };
        return fullClientResponse;
    }
    public Task<FullClientResponse> GetAllFullClients(string clientid)
    {
        throw new NotImplementedException();
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