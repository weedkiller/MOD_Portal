using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ACQ.Web.App.ViewModel
{
    public class Contracts
    {
        public ContractDetails Contrct_Detail { get; set; }
        public List<StageDetail> Stage_Detail { get; set; }
        public string Message { get; set; }
    }

    public partial class ContractDetails
    {
        public int Id { get; set; }
        public string ContractId { get; set; }
        public string Contract_Number { get; set; }
        public Nullable<System.DateTime> DateOfContractSigning { get; set; }
        public Nullable<System.DateTime> createdate { get; set; }
        public string Descriptions { get; set; }
        public string Category { get; set; }
        public Nullable<System.DateTime> EffectiveDate { get; set; }
        public Nullable<System.DateTime> ABGDate { get; set; }
        public Nullable<int> PWBGPercentage { get; set; }
        public Nullable<System.DateTime> PWBGDate { get; set; }
        public string Incoterms { get; set; }
        public string Warranty { get; set; }
        public decimal? ContractValue { get; set; }
        public string FEContent { get; set; }
        public string TaxesAndDuties { get; set; }
        public Nullable<System.DateTime> FinalDeliveryDate { get; set; }
        public string GracePeriod { get; set; }
        public string Services { get; set; }
    }
    public partial class StageDetail
    {
        public int Id { get; set; }
        public Nullable<int> ContractmasterId { get; set; }
        public Nullable<int> StageNumber { get; set; }
        public string stageDescription { get; set; }
        public Nullable<System.DateTime> StageStartdate { get; set; }
        public Nullable<System.DateTime> StageCompletionDate { get; set; }
        public Nullable<int> PercentOfContractValue { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<System.DateTime> DueDateOfPayment { get; set; }
        public string Conditions { get; set; }
        public Nullable<System.DateTime> RevisedDateOfpayment { get; set; }
        public string ReasonsForSlippage { get; set; }
        public Nullable<System.DateTime> ActualDateOfPayment { get; set; }
        public decimal? TotalPaymentMade { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string FullorPartPaymentMade { get; set; }
        public string ContractId { get; set; }
        public Nullable<decimal> ExpendMadeTill31March { get; set; }
        public string PaymentId { get; set; }
    }
    public class ImportExcel
    {
        [Required(ErrorMessage = "Please select file")]
        [FileExt(Allow = ".xls,.xlsx", ErrorMessage = "Only excel file")]
        public HttpPostedFileBase file { get; set; }
    }

    public class FileExt : ValidationAttribute
    {
        public string Allow;
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string extension = ((System.Web.HttpPostedFileBase)value).FileName.Split('.')[1];
                if (Allow.Contains(extension))
                    return ValidationResult.Success;
                else
                    return new ValidationResult(ErrorMessage);
            }
            else
                return ValidationResult.Success;
        }
    }
}