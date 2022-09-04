using eVoucher_Entities.RequestModels;
using eVoucher_Entities.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVoucher_Repo.Evoucher
{
    public interface IEvoucherRepository
    {
        public newEvoucherResponse CreateNewEvoucher(NewEvoucherRequest _request);
        public newEvoucherResponse UpdateEVoucher(UpdateEvoucherRequest _request);
        public newEvoucherResponse UpdateStatus(UpdateStatusRequest _request);
    }
}
