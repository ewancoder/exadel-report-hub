using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ExportPro.Common.Models.MongoDB
{
   public interface IModel
    {
        ObjectId Id { get; set; }
    }
}
