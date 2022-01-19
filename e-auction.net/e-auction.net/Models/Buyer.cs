using e_auction.net.Scripts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace e_auction.net.Models
{
    public class Buyer
    {
        [JsonProperty(PropertyName = "id")]
        public string BidID { get; set; }

        [JsonProperty(PropertyName = "First Name")]
        [Required]
        [StringLength(maximumLength: 30, MinimumLength = 5, ErrorMessage = "Invalid First Name")]
        public string FirstName { get; set; }
        [JsonProperty(PropertyName = "Last Name")]
        [Required]
        [StringLength(maximumLength: 25, MinimumLength = 3, ErrorMessage = "Invalid Last Name")]
        public string LastName { get; set; }
        [JsonProperty(PropertyName = "Address")]
        public string Address { get; set; }
        [JsonProperty(PropertyName = "City")]
        public string City { get; set; }
        [JsonProperty(PropertyName = "State")]
        public string State { get; set; }
        [JsonProperty(PropertyName = "Pin")]
        public string Pin { get; set; }
        [JsonProperty(PropertyName = "Phone")]
        [Required]
        [DataType(DataType.PhoneNumber, ErrorMessage = "Valid phone number should be specified")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Phone number is not valid.")]
        public string Phone { get; set; }
        [JsonProperty(PropertyName = "eMail")]
        [Required]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Email is not valid.")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Valid email should be specified")]
        public string Email { get; set; }
        [JsonProperty(PropertyName = "Bid Amount")]
        [DataType(DataType.Currency, ErrorMessage = "Amount should be specified")]
        [Range(1, 1000000000000, ErrorMessage = "Amount should be specified")]
        public int BidAmount { get; set; }

        [JsonProperty(PropertyName = "Product ID")]
        [Required]
       // [CustomValidation(typeof(Buyer), "ValidateProdID")]
        public string ProdID { get; set; }

        public static ValidationResult ValidateProdID(string ProdID, ValidationContext validationContext)
        {
            Seller product = CosmosDbService<Seller>.GetCollectionItemAsync(ProdID).Result;
            if (product != null)
            {
                if (product.BidEndDate > DateTime.Now)
                {
                    if (product.bids == null )//|| product.bids.Where(p => p.Email == ).Count() == 0)
                    {
                        return ValidationResult.Success;
                    }
                    else
                    {
                        return new ValidationResult("Active bid placed by user already present");
                    }
                }
                else
                {
                    return new ValidationResult("Product bid date completed");
                }
            }
            else
            {
                return new ValidationResult("Invalid Prod ID");
            }
        }


    }
}