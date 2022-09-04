using eVoucher_Entities.EntityModels;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVoucher_Entities.ResponseModels;
using eVoucher_Entities.RequestModels;
using static System.Net.Mime.MediaTypeNames;
using eVoucher_Entities.Models;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Security.Cryptography;

namespace eVoucher_Repo.Evoucher
{
    public class EvoucherRepository : IEvoucherRepository
    {
        private eVoucherContext db_Evoucher;
        private readonly IConfiguration configuration;

        public EvoucherRepository(eVoucherContext _db_Evoucher, IConfiguration _configuration)
        {
            db_Evoucher = _db_Evoucher;
            configuration = _configuration;
        }

        public newEvoucherResponse CreateNewEvoucher(NewEvoucherRequest _request)
        {
            newEvoucherResponse response = new newEvoucherResponse();
            try
            {
                SaveImage img = new SaveImage();
                if (_request.base64image != "")
                {
                    img = SaveImage(_request.base64image);
                }
                if (!String.IsNullOrEmpty(img.ErrorMessage))
                {
                    response.StatusCode = 500;
                    response.ErrorType = "internal-error";
                    response.ErrorMessage = img.ErrorMessage;
                    return response;
                }

                var isValidPaymentMethod = (from p in db_Evoucher.TblpaymentMethods
                                            where p.PaymentMethod == _request.payment_method
                                            select true).FirstOrDefault();
                if (!isValidPaymentMethod)
                {
                    response.StatusCode = 400;
                    response.ErrorType = "bad-request";
                    response.ErrorMessage = "Invalid Payment Method.";
                    return response;
                }

                var vls = (from v in db_Evoucher.TblEvouchers
                           select new
                           {
                               v.Id
                           }
                          ).ToList();

                int maxNo = 1;
                if (vls != null && vls.Count > 0)
                {
                    maxNo = vls.Max(x => x.Id);
                    maxNo++;
                }

                TblEvoucher tblEvoucher = new TblEvoucher
                {
                    Id = maxNo,
                    VoucherNo = "V-" + maxNo.ToString().PadLeft(5, '0'),
                    Title = _request.title,
                    Description = _request.description,
                    ExpiryDate = _request.expiry_date,
                    ImagePath = img.imagePath,
                    Amount = _request.amount,
                    PaymentMethod = _request.payment_method,
                    Price = _request.price,
                    Discount = _request.discount,
                    Quantity = _request.quantity,
                    MaxLimit = _request.max_limit,
                    BuyType = _request.buy_type,
                    GiftPerUserLimit = _request.gift_per_user_limit,
                    Status = _request.status.ToLower() == "active" ? Convert.ToSByte(true) : Convert.ToSByte(false)
                };
                db_Evoucher.TblEvouchers.Add(tblEvoucher);
                db_Evoucher.SaveChanges();
                response.Evoucher_No = tblEvoucher.VoucherNo;
                return response;
            }
            catch(Exception ex)
            {
                response.StatusCode = 500;
                response.ErrorType = "internal-error";
                response.ErrorMessage = ex.Message;
                return response;
            }
        }

        public newEvoucherResponse UpdateEVoucher(UpdateEvoucherRequest _request)
        {
            newEvoucherResponse response = new newEvoucherResponse();
            try { 
                SaveImage img = new SaveImage();
                if (_request.base64image != "")
                {
                    img = SaveImage(_request.base64image);
                }
                if (!String.IsNullOrEmpty(img.ErrorMessage))
                {
                    response.StatusCode = 500;
                    response.ErrorMessage = img.ErrorMessage;
                    return response;
                }

                var isValidPaymentMethod = (from p in db_Evoucher.TblpaymentMethods
                                            where p.PaymentMethod == _request.payment_method
                                            select true).FirstOrDefault();
                if (!isValidPaymentMethod)
                {
                    response.StatusCode = 400;
                    response.ErrorMessage = "Invalid Payment Method.";
                    return response;
                }

                var EVourcher = (from v in db_Evoucher.TblEvouchers
                                 where v.VoucherNo == _request.voucher_No
                                 select v
                                   ).FirstOrDefault();
                if (EVourcher != null)
                {
                    EVourcher.Title = _request.title;
                    EVourcher.Description = _request.description;
                    EVourcher.ExpiryDate = _request.expiry_date;
                    EVourcher.ImagePath = img.imagePath;
                    EVourcher.Amount = _request.amount;
                    EVourcher.PaymentMethod = _request.payment_method;
                    EVourcher.Price = _request.price;
                    EVourcher.Discount = _request.discount;
                    EVourcher.Quantity = _request.quantity;
                    EVourcher.MaxLimit = _request.max_limit;
                    EVourcher.BuyType = _request.buy_type;
                    EVourcher.GiftPerUserLimit = _request.gift_per_user_limit;
                    EVourcher.Status = _request.status.ToLower() == "active" ? Convert.ToSByte(true) : Convert.ToSByte(false);
                    db_Evoucher.SaveChanges();
                    response.Evoucher_No = EVourcher.VoucherNo;
                    return response;
                }
                else
                {
                    response.StatusCode = 404;
                    response.ErrorType = "record-not-found";
                    response.ErrorMessage = "No Voucher Found.";
                    return response;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.ErrorType = "internal-error";
                response.ErrorMessage = ex.Message;
                return response;
            }
        }

        public newEvoucherResponse UpdateStatus(UpdateStatusRequest _request)
        {
            newEvoucherResponse response = new newEvoucherResponse();
            try
            {
                var EVourcher = (from v in db_Evoucher.TblEvouchers
                                 where v.VoucherNo == _request.evoucher_no
                                 select v
                                   ).FirstOrDefault();
                if (EVourcher != null)
                {
                    EVourcher.Status = _request.status.ToLower() == "active" ? Convert.ToSByte(true) : Convert.ToSByte(false);
                    db_Evoucher.SaveChanges();
                    response.Evoucher_No = EVourcher.VoucherNo;
                    return response;
                }
                else
                {
                    response.StatusCode = 404;
                    response.ErrorType = "record-not-found";
                    response.ErrorMessage = "No Voucher Found.";
                    return response;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.ErrorType = "internal-error";
                response.ErrorMessage = ex.Message;
                return response;
            }
        }

        private SaveImage SaveImage(string Base64Image)
        {
            SaveImage si = new SaveImage();
            try
            {                
                if (Base64Image != "")
                {
                    if (IsBase64(Base64Image))
                    {
                        String path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images\\");
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        var imageName = string.Format(@"{0}", Guid.NewGuid()) + ".jpg";
                        string imgPath = Path.Combine(path, imageName);
                        var imageBytes = Convert.FromBase64String(Base64Image);
                        var imagefile = new FileStream(imgPath, FileMode.Create);
                        imagefile.Write(imageBytes, 0, imageBytes.Length);
                        imagefile.Flush();
                        si.imagePath = Path.Combine("Images", imageName);
                        return si;
                    }
                    else
                    {
                        si.ErrorMessage = "Image is invalid Bse64 string.";
                        return si;
                    }
                }
                else
                {
                    si.ErrorMessage = "Base64 string is empty.";
                    return si;
                }
            }
            catch (Exception ex)
            {
                si.ErrorMessage = ex.Message;
                return si;
            }
        }

        public bool IsBase64(string base64String)
        {

            if (string.IsNullOrEmpty(base64String) || base64String.Length % 4 != 0
            || base64String.Contains(" ") || base64String.Contains("\t") || base64String.Contains("\r") || base64String.Contains("\n"))
                return false;
            try
            {
                Convert.FromBase64String(base64String);
                return true;
            }
            catch
            {

            }
            return false;
        }
    }
}
