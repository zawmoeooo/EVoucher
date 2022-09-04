using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace eVoucher_Entities.ResponseModels
{
    public sealed class Error
    {
        public string Code { get; }
        public string Message { get; }
        

        public Error(string code, string description)
        {
            Code = code;
            Message = description;
        }
    }

    public class ErrorDto
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }

}
