using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TradingCardCompany.Controllers;

namespace TradingCardCompany.Tests.Controllers
{
    [TestClass]
    public class ShoppingControllerTest
    {
        TradingCardCompanyEntities context = new TradingCardCompanyEntities();

        ShoppingController controller = new ShoppingController();

        public void TestViewList()
        {
            ViewResult result = controller.ViewList() as ViewResult;
            // Assert
            Assert.IsNotNull(result);
        }

        public void TestViewCart()
        {
            ViewResult result = controller.ViewCart() as ViewResult;
            // Assert
            Assert.IsNotNull(result);
        }

        public void TestOrderHistory()
        {
            ViewResult result = controller.OrderHistory() as ViewResult;
            // Assert
            Assert.IsNotNull(result);
        }

    }
}
