using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data.SqlTypes;
using System.Web;
using System.Web.Mvc;
using TradingCardCompany.Models;

namespace TradingCardCompany.Controllers
{
    [Authorize]
    public class ShoppingController : Controller
    {
        TradingCardCompanyEntities cardcompany = new TradingCardCompanyEntities();
        
        List<ShoppingCart> listofShoppingcart;

         List<OrderDetail> listofOrderDetail = new List<OrderDetail>();

        public ActionResult CreateCard()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateCard(CardDetail card, HttpPostedFileBase frontimage, HttpPostedFileBase backimage)
        {
            string frontpath = UploadImage(frontimage);
            string backpath = UploadImage(backimage);

            using (cardcompany = new TradingCardCompanyEntities())
            {
                card.FrontImage = frontpath;
                card.BackImage = backpath;
                card.InsertedDate = DateTime.Now;
                cardcompany.CardDetails.Add(card);
                cardcompany.SaveChanges();
                ModelState.Clear();

                ViewBag.Message = "New Card Details Inserted Successfully...";
            }
            return View();
        }

        public string UploadImage(HttpPostedFileBase imgfile)
        {
            string path = "-1";

            if (imgfile != null && imgfile.ContentLength > 0)
            {
                string extension = Path.GetExtension(imgfile.FileName);
                if (extension.ToLower().Equals(".jpg") || extension.ToLower().Equals(".jpeg") || extension.ToLower().Equals(".png"))
                {
                    try
                    {
                        path = Path.Combine(Server.MapPath("~/Content/Upload/") + Path.GetFileName(imgfile.FileName));
                        imgfile.SaveAs(path);
                        path = "~/Content/Upload/" + Path.GetFileName(imgfile.FileName);
                    }
                    catch (Exception ex)
                    {
                        path = "-1";
                    }
                }
                else
                {
                    Response.Write("<script>alert('Only jpg, jpeg and png images are allowed....'); </script>");
                }
            }
            else
            {
                Response.Write("<script>alert('Please select a file'); </script>");
                path = "-1";
            }
            return path;
        }

        //////// END OF NEW CARD DETAILS ////////

        public ActionResult ViewList()
        {
            using (cardcompany = new TradingCardCompanyEntities())
            {                 
                return View(cardcompany.CardDetails.ToList());
            }
             Session["Viewlist"] = cardcompany.Registers.ToList();
        }

        
        public ActionResult AddtoCart(int cardid)
        {
            List<ShoppingCart> listofShoppingcart = new List<ShoppingCart>();

            ShoppingCart objshoppingcart = new ShoppingCart();
            CardDetail objitem = cardcompany.CardDetails.Single(model => model.CardID == cardid);
            
            if(Session["CartCounter"] != null)
            {
                listofShoppingcart = Session["CartItem"] as List<ShoppingCart>;
            }

            if(listofShoppingcart.Any(model => model.CardId == cardid))
            {
                objshoppingcart = listofShoppingcart.Single(model => model.CardId == cardid);
                objshoppingcart.Quantity = objshoppingcart.Quantity + 1;
                objshoppingcart.Total = (objshoppingcart.Quantity * Convert.ToDecimal(objshoppingcart.Price)).ToString();
            }
            else
            {
                objshoppingcart.CardId = cardid;
                objshoppingcart.ImagePath = objitem.FrontImage;
                objshoppingcart.Cardname = objitem.CardName;
                objshoppingcart.Quantity = 1;
                objshoppingcart.Total = objitem.Price;
                objshoppingcart.Price = Convert.ToDecimal(objitem.Price);
                listofShoppingcart.Add(objshoppingcart);
            }
            Session["CartCounter"] = listofShoppingcart.Count;
            Session["CartItem"] = listofShoppingcart;

            return RedirectToAction("ViewList");
        }

        public ActionResult ViewCart()
        {
            listofShoppingcart = Session["CartItem"] as List<ShoppingCart>;
            return View(listofShoppingcart);
        }


        [HttpPost]    
        public ActionResult AddOrder(Checkout checkout)
        {
            int OrderId = 0;
            listofShoppingcart = Session["CartItem"] as List<ShoppingCart>;

            var register = cardcompany.Registers.ToList();

            int userID = (from Registers in register
                          where Registers.UserName == User.Identity.Name
                          select Registers.UserID).FirstOrDefault();

            OrderDetail objorderdetails = new OrderDetail();

            Order orderobj = new Order()
            {
                OrderDate = DateTime.Now,
                OrderNumber = Convert.ToInt32(String.Format("{0:ddmmyy}", DateTime.Now))
            };
            cardcompany.Orders.Add(orderobj);
            cardcompany.SaveChanges();
            OrderId = orderobj.OrderID;

            foreach(var item in listofShoppingcart)
            {
              
                objorderdetails.Total = item.Total;
                objorderdetails.CardId = item.CardId;
                objorderdetails.CardName = item.Cardname;
                objorderdetails.UserId = userID;
                objorderdetails.OrderID = OrderId;
                objorderdetails.Quantity = item.Quantity.ToString();
                objorderdetails.Price = item.Price.ToString();
                cardcompany.OrderDetails.Add(objorderdetails);
                cardcompany.SaveChanges();
            }

            Session["OrderDetails"] = cardcompany.OrderDetails;

            TempData["orderdetailsId"] = objorderdetails.OrderDetailId;


            Session["CartItem"] = null;
            Session["CartCounter"] = null;
          //  return RedirectToAction("Index", "Payment");
            return RedirectToAction("Checkout");
        }

        public ActionResult Checkout()
        {
            return View();
        }

        //Post : Complete 

        [HttpPost]
        public ActionResult ProcessOrder(Checkout checkout)
        {
            int orderDetailsId;

            if (TempData.ContainsKey("orderDetailsId"))
            {
                orderDetailsId = (int)TempData["orderDetailsId"];
               
                Checkout objCheckout = new Checkout();
                var register = cardcompany.Registers.ToList();

                int userID = (from Registers in register
                           where Registers.UserName == User.Identity.Name
                           select Registers.UserID).FirstOrDefault();


                checkout.UserID = userID;
                checkout.OrderDetailId = orderDetailsId;
                
                cardcompany.Checkouts.Add(checkout);
                cardcompany.SaveChanges();
               
            }
            ModelState.Clear();

            ViewBag.Message = "Order Created Successfully...";

            return RedirectToAction("OrderHistory");
        }


        // Get : Order History Details placed by the user
        public ActionResult OrderHistory()
        {
            var register = cardcompany.Registers.ToList();

            int userID = (from Registers in register
                          where Registers.UserName == User.Identity.Name
                          select Registers.UserID).FirstOrDefault();

            var objlistOrderhistory = from records in cardcompany.OrderDetails
                                      where records.UserId == userID
                                      select records;

            Session["CartItem"] = null;
            Session["CartCounter"] = null;

            return View(objlistOrderhistory);

        }




        //[HttpPost]

        //public ActionResult FlipImage(int cardId, string frontimage, string backimage)
        //{
        //    var backimg = "";
        //    //List<CardDetail> listofShoppingcart = new List<CardDetail>();

        //    var listofShoppingcart = cardcompany.CardDetails.ToList();

        //  //  ShoppingCart objshoppingcart = new ShoppingCart();

        //  CardDetail objitem = cardcompany.CardDetails.Single(model => model.CardID == cardId);

        //    var frontimg = cardcompany.CardDetails.Single(model => model.FrontImage == frontimage);
        //     backimg = cardcompany.CardDetails.Single(model => model.BackImage == backimage).ToString();

        //    if (objitem.CardID == listofShoppingcart[0].CardID)
        //    {
        //        objitem.BackImage = backimg;
        //    }

        //   // return View(objitem.BackImage);

        //    return PartialView("FlipImage");
        //}






    }
}