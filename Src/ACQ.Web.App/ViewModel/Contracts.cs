using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACQ.Web.App.ViewModel
{
    public class Contracts
    {
        public ContractDetails Contrct_Detail { get; set; }
        public StageDetail Stage_Detail { get; set; }
    }
   
    public partial class ContractDetails
    {
        public int Id { get; set; }
        public string ContractId { get; set; }
        public string Contract_Number { get; set; }
        public Nullable<System.DateTime> DateOfContractSigning { get; set; }
        public Nullable<System.DateTime> createdate { get; set; }
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
        public string TotalPaymentMade { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
    }
}