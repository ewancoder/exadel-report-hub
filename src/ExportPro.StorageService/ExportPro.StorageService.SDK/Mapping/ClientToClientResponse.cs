using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.Responses;
using MongoDB.Bson;

namespace ExportPro.StorageService.SDK.Mapping
{
    public static class ClientToClientResponse
    {
        public static ClientResponse ClientToClientReponse(Client client)
        {
            ClientResponse clientResponse = new();
            clientResponse.Id = client.Id.ToString();
            clientResponse.Name = client.Name;
            clientResponse.Description = client.Description;
            clientResponse.CreatedAt = client.CreatedAt;
            clientResponse.UpdatedAt = client.UpdatedAt;
            clientResponse.IsDeleted = client.IsDeleted;
            clientResponse.InvoiceIds = client.InvoiceIds;
            clientResponse.CustomerIds = client.CustomerIds;
            clientResponse.ItemIds = client.ItemIds;
            return clientResponse;
        }

        public static Client ClientResponseToClient(ClientResponse clientResponse)
        {   
            Client client = new();
            client.Id = ObjectId.Parse(clientResponse.Id);
            client.Name = clientResponse.Name;  
            client.Description = clientResponse.Description;
            client.CreatedAt = clientResponse.CreatedAt;
            client.UpdatedAt = clientResponse.UpdatedAt;
            client.IsDeleted = clientResponse.IsDeleted;
            client.InvoiceIds = clientResponse.InvoiceIds;
            client.CustomerIds = clientResponse.CustomerIds;
            client.ItemIds = clientResponse.ItemIds;
            return client;
        }
    }
}
