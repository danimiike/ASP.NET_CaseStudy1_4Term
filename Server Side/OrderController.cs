using Casestudy.DAL;
using Casestudy.DAL.DAO;
using Casestudy.DAL.DomainClasses;
using Casestudy.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Casestudy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        AppDbContext _ctx;
        public OrderController(AppDbContext context) // injected here
        {
            _ctx = context;
        }
        [HttpPost]
        [Produces("application/json")]
        public async Task<ActionResult<string>> Index(OrderHelper helper)
        {
            bool isbackorder = false;
            string retVal = "";
            try
            {
                CustomerDAO uDao = new CustomerDAO(_ctx);
                Customer orderOwner = await uDao.GetByEmail(helper.email);
                OrderDAO tDao = new OrderDAO(_ctx);
                int orderId = await tDao.AddOrder(orderOwner.Id, helper.selections);

                foreach (OrderSelectionHelper obj in helper.selections)
                {
                    if (obj.Qty > obj.item.QtyOnHand)
                    {
                        isbackorder = true;
                        break;
                    }
                }

                if (orderId > 0)
                {
                    retVal = "Order " + orderId + " created!";
                    if (isbackorder)
                        retVal += " Goods backordered";
                }
                else
                {
                    retVal = "Order not saved";
                }
            }
            catch (Exception ex)
            {
                retVal = "Order not saved " + ex.Message;
            }
            return retVal;
        }
    }
}

