using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportPro.Common.Shared.Library
{
    public class ValidationFailedResponse : BaseResponse
    {
        public Dictionary<string, string[]> Errors { get; set; }

        public ValidationFailedResponse()
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationFailedResponse(Dictionary<string, string[]> errors)
        {
            ApiState = System.Net.HttpStatusCode.UnprocessableEntity;
            IsSuccess = false;
            Errors = errors;
        }
    }
}
