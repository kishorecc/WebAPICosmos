using e_auction.net.Models;
using e_auction.net.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;


namespace e_auction.net.Controllers
{
    public class SellerController : ApiController
    {
        // GET api/values
        [HttpGet]
        [ActionName("Get-Products")]
        public async Task<IEnumerable<Seller>> GetProducts()
        {
            return await CosmosDbService<Seller>.GetItemsAsync();
        }

        // GET api/values/5
        [HttpGet]
        [ActionName("Show-Bids")]
        public async Task<IEnumerable<Seller>> ShowBids(string prodID)
        {
            IEnumerable<Seller> unsortedSeller= await CosmosDbService<Seller>.GetItemsAsync(prodID); //.Result.OrderBy(p => p.bids.OrderBy(q => q.BidAmount)).AsEnumerable<Seller>();
            return unsortedSeller.OrderBy(p => p.bids.OrderBy(q => q.BidAmount));
        }

        // POST api/values
        [HttpPost]
        [ActionName("Add-Product")]
        public async Task<HttpResponseMessage> AddProduct([FromBody] Seller value)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    value.ProdID = Guid.NewGuid().ToString();
                    await CosmosDbService<Seller>.CreateItemAsync(value);
                    return new HttpResponseMessage(HttpStatusCode.OK);
                }
                catch (Exception ex)
                {
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                }

                // return new HttpResponseMessage(HttpStatusCode.OK);
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // PUT api/values/5
        public void Put(int id, [FromBody] Seller value)
        {
            Seller n = value;
        }

        // DELETE api/values/5
        [HttpDelete]
        [ActionName("Delete-Product")]
        public async Task<IHttpActionResult> DeleteProduct(string prodID)
        {
            Seller product = CosmosDbService<Seller>.GetItemsAsync(prodID).Result.FirstOrDefault();
            if (product != null)
            {
                if (product.BidEndDate > DateTime.Now)
                {
                    return new System.Web.Http.Results.ResponseMessageResult(
                  Request.CreateErrorResponse(
                      (HttpStatusCode)422,
                      new HttpError("Bid End Date is completed, product cannot be deleted")));

                }

                if (product.bids != null)
                {
                    return new System.Web.Http.Results.ResponseMessageResult(
                  Request.CreateErrorResponse(
                      (HttpStatusCode)422,
                      new HttpError("Product has valid bids, so it cannot be deleted")));

                }

                //return await CosmosDbService<Seller>.GetItemsAsync(prodID);
                return new System.Web.Http.Results.ResponseMessageResult(Request.CreateResponse(HttpStatusCode.OK, await CosmosDbService<Seller>.DeleteItemAsync(prodID)));
            }
            else
            {
                return new System.Web.Http.Results.ResponseMessageResult(
                  Request.CreateErrorResponse(
                      HttpStatusCode.NotFound,
                      new HttpError("Product not found")));
            }
        }
    }
}
