using eVoucher_Entities.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVoucher_Entities.Models
{
    public class SaveImage : ResponseBase
    {
        public string imagePath { get; set; }
    }
}
