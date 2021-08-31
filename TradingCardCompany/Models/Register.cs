using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TradingCardCompany.Models
{
    public partial class Register
    {
        public int UserID { get; set; }

        [Required(ErrorMessage = "UserName cannot be null.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password cannot be null.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "ConfirmPassword cannot be null.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and ConfirmPassword should be same")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string City { get; set; }
    }
}