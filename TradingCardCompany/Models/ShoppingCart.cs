using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TradingCardCompany.Models
{
    public class ShoppingCart
    {
        public int CardId { get; set; }
        public string Cardname { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public string Total { get; set; }

        public string ImagePath { get; set; }

        public int Description { get; set; }

    }
}