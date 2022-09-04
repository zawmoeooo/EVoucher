using eVoucher_Repo;
using eVoucher_Entities.EntityModels;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVoucher_Repo.UserRepo;
using eVoucher_Repo.Evoucher;
using eVoucher_Repo.EStore;

namespace eVoucher_Repo
{
    public class Repositories : IRepositories
    {
        private eVoucherContext db_Evoucher;
        private IConfiguration configuration;
        private IUserRepository _user;
        private IEvoucherRepository _evoucher;
        private IEStoreRepository _estore;
        public Repositories(eVoucherContext _db_Evoucher, IConfiguration _configuration)
        {
            db_Evoucher = _db_Evoucher;
            configuration = _configuration;
        }

        public IUserRepository User
        {
            get
            {
                if (_user == null)
                {
                    _user = new UserRepository(db_Evoucher, configuration);
                }
                return _user;
            }
        }

        public IEvoucherRepository Evoucher
        {
            get
            {
                if (_evoucher == null)
                {
                    _evoucher = new EvoucherRepository(db_Evoucher, configuration);
                }
                return _evoucher;
            }
        }

        public IEStoreRepository Estore
        {
            get
            {
                if (_estore == null)
                {
                    _estore = new EStoreRepository(db_Evoucher, configuration);
                }
                return _estore;
            }
        }
    }
}
