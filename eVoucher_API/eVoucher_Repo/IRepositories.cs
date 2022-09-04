using eVoucher_Repo.EStore;
using eVoucher_Repo.Evoucher;
using eVoucher_Repo.UserRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVoucher_Repo
{
    public interface IRepositories
    {
        IUserRepository User { get; }
        IEvoucherRepository Evoucher { get; }

        IEStoreRepository Estore { get; }
    }
}
