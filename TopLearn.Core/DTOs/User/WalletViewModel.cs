using System;
using System.ComponentModel.DataAnnotations;

namespace TopLearn.Core.DTOs.User
{
    public class WalletChargeViewModel
    {
        [Display(Name = "مبلغ")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int Amount { get; set; }
    }

    public class WalletViewModel
    {
        public int Amount { get; set; }
        public int Type { get; set; }
        public string Description { get; set; }
        public bool IsPay { get; set; }
        public string FactorUrl { get; set; }
        public DateTime DateTime { get; set; }
    }
}
