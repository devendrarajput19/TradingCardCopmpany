using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TradingCardCompany.Models
{
    public class PaymentInitiate
    {
        public string name { get; set; }
        public string email { get; set; }
        public string contactnumber { get; set; }
        public string address { get; set; }

        public int amount { get; set; }

    }
}