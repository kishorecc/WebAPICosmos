using e_auction.net.Models;
using e_auction.net.Scripts;
using e_auction.net.Service;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace e_auction.net.Controllers
{
    public class BuyerController : ApiController
    {
        // GET api/values
        [HttpGet]
        [ActionName("Get-Bids")]
        public IEnumerable<string> GetBids()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet]
        [ActionName("Show-Bids")]
        public string ShowBids(int prodID)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        [ActionName("place-bid")]
        public async Task<HttpResponseMessage> PlaceBidsAsync(string prodID, [FromBody] Buyer value)
        {
            //value.ProdID = prodID;
            if (ModelState.IsValid)
            {
                ItemResponse<Seller> dbObject = await CosmosDbService<Seller>.GetCollectionItemAsync(prodID);

                if (dbObject.Resource != null)
                {
                    if (dbObject.Resource.BidEndDate > DateTime.Now)
                    {
                        if (dbObject.Resource.bids == null || dbObject.Resource.bids.Where(p => p.Email == value.Email).Count() == 0)
                        {
                            value.BidID = Guid.NewGuid().ToString();
                            if (dbObject.Resource.bids == null)
                            {
                                dbObject.Resource.bids = new Buyer[] { value };
                            }
                            else
                            {
                                dbObject.Resource.bids = dbObject.Resource.bids.Concat(new Buyer[] { value }).ToArray();
                            }
                            // await CosmosDbService<Seller>.ReplaceItemAsync(prodID, dbObject);
                            var serializedData = JsonConvert.SerializeObject(value);

                            await QueueService.InsertMessageAsync(serializedData);
                            QueueService.InsertRabbitMQMessageAsync(serializedData);
                        }
                        else
                        {
                            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, new HttpError("Product has bid placed by user already"));
                        }
                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, new HttpError("Product bid date completed"));
                    }
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, new HttpError("Invalid Prod ID"));
                }

                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }


        // PUT api/values/5
        [HttpPost]
        [ActionName("update-bid")]
        public async Task<HttpResponseMessage> UpdateBidAsync(string prodID, int amount, string emailID)
        {
            ItemResponse<Seller> dbObject = await CosmosDbService<Seller>.GetCollectionItemAsync(prodID);
            if (dbObject.Resource != null)
            {
                if (dbObject.Resource.bids != null && dbObject.Resource.bids.Where(p => p.Email == emailID).Count() != 0)
                {
                    dbObject.Resource.bids.Where(p => p.Email == emailID).FirstOrDefault().BidAmount = amount;
                    await CosmosDbService<Seller>.ReplaceItemAsync(prodID, dbObject);
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, new HttpError("Bid not present"));
                }
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, new HttpError("Product not present"));
            }
        }

        // DELETE api/values/5
        [HttpDelete]
        [ActionName("Delete-bid")]
        public void DeleteBid(int id)
        {
        }
    }
}
