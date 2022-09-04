using eVoucher_Entities.RequestModels;
using eVoucher_Entities.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVoucher_Repo.EStore
{
    public interface IEStoreRepository
    {
        public List<PaymentMethodListResponse> GetPaymentMethodList();
        public EStoreAccessTokenResponse GenerateToken(EStoreAccessTokenRequest _request);
        public List<GetEVoucherListingResponse> GetVoucherList();
        public GetEVoucherDetailResponse GetEVoucherDetail(EvoucherDetailRequest _request);
        public PromoCodeResponse CheckPromoCode(PromoCodeRequest _request);
        public void ScheduleGeneratePromoCode(GeneratePromoCodeRequest _requestData);
        public GeneratePromoCodeResponse GeneratePromoCode(GeneratePromoCodeRequest _request);
        public BuyEVoucherResponse BuyEvoucher(BuyEvoucherRequest _request);
        public List<GetPurchaseHistoryResponse> GetPurchaseHistory();
    }
}
