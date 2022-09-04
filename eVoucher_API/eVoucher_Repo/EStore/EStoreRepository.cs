using eVoucher_Entities.EntityModels;
using eVoucher_Entities.Models;
using eVoucher_Entities.RequestModels;
using eVoucher_Entities.ResponseModels;
using eVoucher_Repo.Helper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using RestSharp;
using QRCoder;
using MiniGuids;
using System.IO;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Components.Routing;

namespace eVoucher_Repo.EStore
{
    public class EStoreRepository : IEStoreRepository
    {
        private eVoucherContext db_Evoucher;
        private readonly IConfiguration configuration;

        public EStoreRepository(eVoucherContext _db_Evoucher, IConfiguration _configuration)
        {
            db_Evoucher = _db_Evoucher;
            configuration = _configuration;
        }

        public EStoreAccessTokenResponse GenerateToken(EStoreAccessTokenRequest _request)
        {
            EStoreAccessTokenResponse response = new EStoreAccessTokenResponse();
            if (configuration["EStoreClientID"] == _request.ClientID)
            {
                AccessTokenConfig ATConfig = new AccessTokenConfig
                {
                    Audience = configuration["Audience"],
                    Issuer = configuration["Issuer"],
                    PrivateKey = configuration["RSAPrivateKey"],
                    TokenExpiryMinute = Int32.Parse(configuration["TokenExpiryInMin"]),
                    RefreshTokenExpiryMinute = Int32.Parse(configuration["RefreshTokenExpiryInMin"]),
                    UserId = 0,
                    UserName = "EStoreUser"
                };
                GeneratedToken generatedToken = AccessTokenHelper.GenerateToken(ATConfig);
                if (String.IsNullOrEmpty(generatedToken.ErrorStatus))
                {
                    response.AccessToken = generatedToken.AccessToken;
                    response.AccessTokenExpireMinutes = Int32.Parse(configuration["TokenExpiryInMin"]);
                    response.RefreshToken = generatedToken.RefreshToken;
                    response.RefreshTokenExpireMinutes = Int32.Parse(configuration["RefreshTokenExpiryInMin"]);
                    saveAccessToken(generatedToken);
                }
                else
                {
                    response.StatusCode = generatedToken.statusCode;
                    response.ErrorType = "internal-error";
                    response.ErrorMessage = "Cannot generate access token.";
                }
            }
            else
            {
                response.StatusCode = 400;
                response.ErrorType = "bad-request";
                response.ErrorMessage = "Wrong Client ID";
            }
            return response;
        }

        private void saveAccessToken(GeneratedToken _request)
        {
            TblaccessToken at = new TblaccessToken();
            at.AccessToken = _request.AccessToken;
            at.AccessTokenExpiry = _request.AccessTokenExpiryDate;
            at.RefreshToken = _request.RefreshToken;
            at.RefreshTokenExpiry = _request.RefreshTokenExpiryDate;
            at.UserId = _request.UserID;
            db_Evoucher.TblaccessTokens.Add(at);
            db_Evoucher.SaveChanges();
        }

        public List<PaymentMethodListResponse> GetPaymentMethodList()
        {
            List<PaymentMethodListResponse> response = new List<PaymentMethodListResponse>();
            response = (from p in db_Evoucher.TblpaymentMethods
                        where p.Status == 1
                        select new PaymentMethodListResponse
                        {
                            payment_method = p.PaymentMethod,
                            discount_percentage = p.DiscountPercentage,
                            Status = p.Status == 1 ? "Active" : "Inactive"
                        }
                        ).ToList();
            return response;
        }

        public List<GetEVoucherListingResponse> GetVoucherList()
        {
            List<GetEVoucherListingResponse> response = new List<GetEVoucherListingResponse>();
            response = (from e in db_Evoucher.TblEvouchers
                        where e.Quantity > 0
                        select new GetEVoucherListingResponse
                        {
                            expirydate = e.ExpiryDate,
                            quantity = e.Quantity,
                            status = e.Status == Convert.ToSByte(true) ? "Active" : "Inactive",
                            price = e.Price,
                            title = e.Title,
                            amount = e.Amount,
                            voucher_no = e.VoucherNo
                        }
                        ).ToList();
            return response;
        }

        public GetEVoucherDetailResponse GetEVoucherDetail(EvoucherDetailRequest _request)
        {
            //GetEVoucherDetailResponse response = new GetEVoucherDetailResponse();
            var response = ( from e in db_Evoucher.TblEvouchers
                        where e.VoucherNo == _request.evoucher_no
                        select new GetEVoucherDetailResponse
                        {
                            voucher_No = e.VoucherNo,
                            title = e.Title,
                            description = string.IsNullOrEmpty(e.Description) ? "" : e.Description,
                            expiry_date = e.ExpiryDate.HasValue ? e.ExpiryDate.Value : DateTime.MaxValue,
                            image = string.IsNullOrEmpty(e.ImagePath) ? "" : configuration["EvoucherURL"] + e.ImagePath,
                            amount = e.Amount,
                            payment_method = string.IsNullOrEmpty(e.PaymentMethod) ? "" : e.PaymentMethod,
                            price = e.Price,
                            discount = e.Discount.HasValue ? e.Discount.Value : 0,
                            quantity = e.Quantity,
                            max_limit = e.MaxLimit,
                            buy_type = e.BuyType,
                            gift_per_user_limit = e.GiftPerUserLimit,
                            status = e.Status == Convert.ToSByte(true) ? "Active" : "Inactive"
                        }
                        ).FirstOrDefault();
            return response;
        }

        public PromoCodeResponse CheckPromoCode(PromoCodeRequest _request)
        {
            var response = (from p in db_Evoucher.TblpromoCodes
                            where p.OwnerPhone == _request.phone_no && p.ExpiryDate > DateTime.Now
                            && p.PromoCode == _request.promo_code
                            select new PromoCodeResponse
                            {
                                status = p.Status == Convert.ToSByte(true) ? "Active" : "Inactive",
                                promo_code = p.PromoCode
                            }
                            ).FirstOrDefault();
            return response;
        }

        public static class BuyerType
        {
            public static string EVOUCHER_BUY_TYPE_ONLYME = "Only me usage";
            public static string EVOUCHER_BUY_TYPE_GIFT = "Gift to others";
        }

        public BuyEVoucherResponse BuyEvoucher(BuyEvoucherRequest _request)
        {
            BuyEVoucherResponse response = new BuyEVoucherResponse();
            var isValidPaymentMethod = (from p in db_Evoucher.TblpaymentMethods
                                        where p.PaymentMethod == _request.PaymentMethod
                                        select true).FirstOrDefault();
            string errorMsg = "";
            if (!isValidPaymentMethod)
            {
                errorMsg = errorMsg + "\r\nInvalid payment method";
            }
            using (var dbContextTransaction = db_Evoucher.Database.BeginTransaction())
            {
                try
                {
                    var tblEvoucher = (from v in db_Evoucher.TblEvouchers
                                        where v.VoucherNo == _request.VoucherNo
                                        select v
                                ).FirstOrDefault();
                    if (tblEvoucher == null)
                    {
                        errorMsg = errorMsg + "\r\nInvalid voucher no";
                    }
                    else
                    {
                        if (tblEvoucher.ExpiryDate < DateTime.Now && tblEvoucher.Status != Convert.ToSByte(true))
                        {
                            errorMsg = errorMsg + "\r\nVouher has been expired or inactive.";
                        }
                        else if (tblEvoucher.Quantity <= 0)
                        {
                            errorMsg = errorMsg + "\r\nOut of stock.";
                        }
                        else if (tblEvoucher.Quantity < _request.Quantity)
                        {
                            errorMsg = errorMsg + "\r\nOrder quantity only available for " + tblEvoucher.Quantity.ToString();                            
                        }
                        else
                        {
                            var previousOrderList = (from p in db_Evoucher.Tblpurchases
                                                        where p.VoucherNo == _request.VoucherNo
                                                        && p.BuyerPhone == _request.BuyerPhone
                                                        select new
                                                        {
                                                            p.BuyType,
                                                            p.Quantity
                                                        }
                                                    ).ToList();

                            if (previousOrderList == null || previousOrderList.Count <= 0)
                            {
                                if (_request.BuyType == BuyerType.EVOUCHER_BUY_TYPE_ONLYME
                                    && _request.Quantity > tblEvoucher.MaxLimit
                                    )
                                {
                                    errorMsg = errorMsg + "\r\nReach Limitted Quantity,You can't buy anymore.";
                                }
                                else if (_request.Quantity > tblEvoucher.GiftPerUserLimit)
                                {
                                    errorMsg = errorMsg + "\r\nReach Limitted Gift Quantity,You can't buy anymore.";
                                }
                            }
                            else
                            {
                                var buyGroup = previousOrderList.GroupBy(x => x.BuyType)
                                                        .Select(x => new
                                                        {
                                                            BuyType = x.First().BuyType,
                                                            Quantity = x.Sum(x => x.Quantity)
                                                        }).ToList();
                                var OwnUsageQuantity = buyGroup.Where(x => x.BuyType == BuyerType.EVOUCHER_BUY_TYPE_ONLYME).Select(x => x.Quantity).FirstOrDefault();
                                var GiftUsageQuantity = buyGroup.Where(x => x.BuyType == BuyerType.EVOUCHER_BUY_TYPE_GIFT).Select(x => x.Quantity).FirstOrDefault();
                                var totalUsage = OwnUsageQuantity + GiftUsageQuantity;

                                if (_request.Quantity + totalUsage > tblEvoucher.MaxLimit)
                                {
                                    if (totalUsage > tblEvoucher.MaxLimit)
                                        errorMsg = errorMsg + "\r\nReach Limitted Quantity,You can buy anymore.";
                                    else
                                        errorMsg = $"{errorMsg}\r\nReach Limitted Quantity,You can buy only {tblEvoucher.MaxLimit - totalUsage} voucher.";

                                }
                                else if (_request.BuyType == BuyerType.EVOUCHER_BUY_TYPE_ONLYME
                                    && _request.Quantity + OwnUsageQuantity > tblEvoucher.MaxLimit
                                    )
                                {
                                    if (OwnUsageQuantity > tblEvoucher.MaxLimit)
                                        errorMsg = errorMsg + "\r\nYou can't buy anymore because of limitted quantity";
                                    else
                                        errorMsg = $"{errorMsg}\r\nYou can buy only {tblEvoucher.MaxLimit - OwnUsageQuantity} voucher.";
                                }
                                else if (_request.Quantity + GiftUsageQuantity > tblEvoucher.GiftPerUserLimit)
                                {
                                    if (GiftUsageQuantity > tblEvoucher.GiftPerUserLimit)
                                        errorMsg = $"{errorMsg}\r\nGift Usage Reach Limitted Quantity,You can't buy anymore.";
                                    else
                                        errorMsg = $"{errorMsg}\r\nGift Usage Reach Limitted Quantity,You can buy only {tblEvoucher.MaxLimit - GiftUsageQuantity} voucher.";
                                }

                            }

                            if (errorMsg == "")
                            {
                                var UpdatetblEvoucher = (from v in db_Evoucher.TblEvouchers
                                                            where v.VoucherNo == _request.VoucherNo
                                                            select v
                                                            ).FirstOrDefault();
                                UpdatetblEvoucher.Quantity = UpdatetblEvoucher.Quantity - _request.Quantity;
                                decimal totalPrice = UpdatetblEvoucher.Price;
                                short sellingDiscount;
                                if (_request.PaymentMethod == UpdatetblEvoucher.PaymentMethod
                                    && UpdatetblEvoucher.Price != null)
                                {
                                    var discountAmount = totalPrice * (decimal)((UpdatetblEvoucher.Discount ?? 0) / 100.0);
                                    totalPrice = totalPrice - discountAmount;
                                    if (totalPrice < 0)
                                        totalPrice = 0;
                                    sellingDiscount = UpdatetblEvoucher.Discount.HasValue ? (short)UpdatetblEvoucher.Discount.Value : (short)0;
                                }
                                else
                                {
                                    sellingDiscount = 0;
                                }

                                var plist = (from v in db_Evoucher.Tblpurchases
                                                    select new
                                                    {
                                                        v.Id
                                                    }
                                    ).ToList();

                                int maxNo = 1;
                                if (plist != null && plist.Count > 0)
                                {
                                    maxNo = plist.Max(x => x.Id);
                                    maxNo++;
                                }

                                Tblpurchase order = new Tblpurchase
                                {
                                    Id = maxNo,
                                    PurchaseId = "P-" + maxNo.ToString().PadLeft(6, '0'),
                                    BuyerName = _request.BuyerName,
                                    BuyerPhone = _request.BuyerPhone,
                                    BuyType = _request.BuyType,
                                    PurchaseDate = DateTime.Now,
                                    PaymentMethod = _request.PaymentMethod,
                                    Discount = sellingDiscount,
                                    Quantity = _request.Quantity,
                                    Status = (short)1,
                                    Total = totalPrice,
                                    Price = UpdatetblEvoucher.Price,
                                    ExpiryDate = UpdatetblEvoucher.ExpiryDate,
                                    Amount = UpdatetblEvoucher.Amount,
                                    VoucherNo = UpdatetblEvoucher.VoucherNo,
                                };

                                db_Evoucher.Tblpurchases.Add(order);
                                db_Evoucher.SaveChanges();

                                dbContextTransaction.Commit();
                                response.purchase_id = order.PurchaseId;
                                response.IsPurchaseSuccess = true;
                            }
                            else
                            {
                                response.ErrorMessage = errorMsg;
                                response.StatusCode = 400;
                                response.ErrorType = "bad-request";
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    response.StatusCode = 500;
                    response.ErrorType = "internal-error";
                    response.ErrorMessage = e.Message;

                    dbContextTransaction.Rollback();
                }
            }            

            return response;
        }

        public string ValidatePaymentMethod(BuyEVoucherRequest _request)
        {
            string returnmsg = "";

            var isValidPaymentMethod = (from p in db_Evoucher.TblpaymentMethods
                                        where p.PaymentMethod == _request.PaymentMethod
                                        select true).FirstOrDefault();
            if (!isValidPaymentMethod)
            {
                    returnmsg = "Invalid Payment Method.";
            }

            return returnmsg;
        }

        public void ScheduleGeneratePromoCode(GeneratePromoCodeRequest _requestData)
        {
            var serviceURL = configuration["PromoCodeServiceURL"];
            var client = new RestClient(serviceURL);
            var request = new RestRequest(serviceURL + "api/PromoCode/GeneratePromoCode");
            request.Method = Method.Post;
            request.AddJsonBody(_requestData);
            var resp = client.Execute(request);

        }

        public GeneratePromoCodeResponse GeneratePromoCode(GeneratePromoCodeRequest _request)
        {
            GeneratePromoCodeResponse response = new GeneratePromoCodeResponse();
            var order = (from o in db_Evoucher.Tblpurchases
                         where o.PurchaseId == _request.Purchase_id
                         select o
                             ).FirstOrDefault();
            if (order != null)
            {
                for (int i = 0; i < order.Quantity; i++)
                {
                    bool isUnique = false;
                    string promoCode = GeneatePromo();
                    string qrCodePath;
                    do
                    {
                        isUnique = !(from gp in db_Evoucher.TblpromoCodes
                                     where gp.PromoCode == promoCode
                                     select true
                                   ).FirstOrDefault();
                    } while (!isUnique);

                    qrCodePath = GenerateQRCode(promoCode);

                    if (qrCodePath != "")
                    {
                        TblpromoCode tblpromo = new TblpromoCode
                        {
                            ExpiryDate = order.ExpiryDate.HasValue ? order.ExpiryDate.Value : DateTime.MaxValue,
                            OwnerName = order.BuyerName,
                            OwnerPhone = order.BuyerPhone,
                            PromoCode = promoCode,
                            purchase_id = order.PurchaseId,
                            QrImage = qrCodePath,
                            Status = Convert.ToSByte(true),
                            VoucherAmount = Convert.ToDouble(order.Amount),
                            VoucherNo = order.VoucherNo
                        };

                        db_Evoucher.TblpromoCodes.Add(tblpromo);

                    }
                }
            }
            else
            {
                response.StatusCode = 404;
                response.ErrorType = "not-found";
                response.ErrorMessage = "Record Not Found";
            }
            db_Evoucher.SaveChanges();
            response.PromoCodeGenerated = true;
            return response;
        }

        public static string GenerateQRCode(string contentString)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(contentString, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            string saveFilePath = "";

            using (var qrCodeImage = qrCode.GetGraphic(20))
            {
                String path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images\\");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                var imageName = string.Format(@"{0}", Guid.NewGuid()) + ".jpg";
                string imgPath = Path.Combine(path, imageName);
                saveFilePath = Path.Combine("Images", imageName);
                var fileName = Guid.NewGuid().ToString() + ".jpg";
                qrCodeImage.Save(imgPath, ImageFormat.Png);
            }
            return saveFilePath;
        }

        public static string GeneatePromo()
        {
            var miniGuid = MiniGuid.NewGuid();

            //implicit conversions (and vice-versa too)
            string someString = miniGuid;
            Guid someGuid = miniGuid;

            //explicit stringifying and parsing
            var str = miniGuid.ToString();
            var sameMiniGuid = str.Substring(0, 5);
            var intGuid = codeFromCoupon(str);
            var PromoCode = sameMiniGuid + intGuid.ToString().Substring(0, 6);
            Random num = new Random();

            PromoCode = new string(PromoCode.ToCharArray().
                            OrderBy(s => (num.Next(2) % 2) == 0).ToArray());
            return PromoCode;
        }

        const string ALPHABET = "BG4FO6LEWV3TC7P8YI7UDX8SM0QKzw2emckd1j5euZHNfysaoeuiA";
        public static uint codeFromCoupon(string coupon)
        {
            uint n = 0;
            for (int i = 0; i < 6; ++i)
                n = n | (((uint)ALPHABET.IndexOf(coupon[i])) << (5 * i));
            return n;
        }

        public List<GetPurchaseHistoryResponse> GetPurchaseHistory()
        {
            var response = (from p in db_Evoucher.TblpromoCodes
                            join o in db_Evoucher.Tblpurchases
                            on p.purchase_id equals o.PurchaseId
                            select new GetPurchaseHistoryResponse
                            {
                                Purchase_Date = o.PurchaseDate,
                                PromoCode = p.PromoCode,
                                QR_Image_Path = p.QrImage,
                                status = o.Status == (short)1 ? "Un-used" : "Used"
                            }).ToList();
            return response;
        }
    }
}
