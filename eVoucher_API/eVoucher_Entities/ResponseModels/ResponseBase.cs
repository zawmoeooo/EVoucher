using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVoucher_Entities.ResponseModels
{
    public class ResponseBase
    {
        public ResponseBase()
        {
            StatusCode = 200;
        }

        public ResponseBase(int statusCode, string errorType, string errorMessage)
        {
            StatusCode = statusCode;
            ErrorType = errorType;
            ErrorMessage = errorMessage;
        }

        public int StatusCode;
        public string ErrorType;
        public string ErrorMessage;

        public Error GetError()
        {
            return new Error(ErrorType, ErrorMessage);
        }
    }
}
