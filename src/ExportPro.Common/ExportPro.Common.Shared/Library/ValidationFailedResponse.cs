using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportPro.Common.Shared.Library
{
    class ValidationFailedResponse: BaseResponse
    {
        public ValidationFailedResponse()
        {
            ApiState = System.Net.HttpStatusCode.UnprocessableEntity;
            IsSuccess = false;
        }
    }
}
