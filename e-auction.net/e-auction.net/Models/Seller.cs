using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace e_auction.net.Models
{
    public class Seller
    {
        //product properties
        [JsonProperty(PropertyName = "id")]
        public string ProdID { get; set; }

        [JsonProperty(PropertyName = "Product Name")]
        [Required]
        [StringLength(maximumLength:30, MinimumLength = 5,ErrorMessage = "Invalid Product Name")]
        public string ProductName { get; set; }

        [JsonProperty(PropertyName = "Short Description")]
        public string ShortDescription { get; set; }

        [JsonProperty(PropertyName = "Detail Description")]
        public string DetailDescription { get; set; }
     
        [JsonProperty(PropertyName = "Category")]
        // [DefaultValue("")]
        [CustomValidation(typeof(Seller), "ValidateCategory")]
        public string Category { get; set; }

        [JsonProperty(PropertyName = "Bid End Date")]
        [CustomValidation(typeof(Seller), "ValidateBidEndDate")]
        public DateTime BidEndDate { get; set; }

        [JsonProperty(PropertyName = "Starting Price")]
        [DataType(DataType.Currency, ErrorMessage = "Amount should be specified")]
        [Range(1,1000000000000, ErrorMessage = "Amount should be specified") ]
        public int StartingPrice { get; set; }

        //seller properties
        [JsonProperty(PropertyName = "First Name")]
        [Required]
        [StringLength(maximumLength: 30, MinimumLength = 5, ErrorMessage = "Invalid First Name")]
        public string FirstName { get; set; }
        [JsonProperty(PropertyName = "Last Name")]
        [Required]
        [StringLength(maximumLength: 25, MinimumLength = 3, ErrorMessage = "Invalid Last Name")]
        public string LastName { get; set; }
        [JsonProperty(PropertyName = "Address")]
        public string Address        { get; set; }
        [JsonProperty(PropertyName = "City")]
        public string City        { get; set; }
        [JsonProperty(PropertyName = "State")]
        public string State        { get; set; }
        [JsonProperty(PropertyName = "Pin")]
        public string Pin         { get; set; }
        [JsonProperty(PropertyName = "Phone")]
        [Required]
        [DataType(DataType.PhoneNumber, ErrorMessage = "Valid phone number should be specified")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Phone number is not valid.")]         
        public string Phone        { get; set; }
        [JsonProperty(PropertyName = "eMail")]
        [Required]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Email is not valid.")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Valid email should be specified")]
        public string Email { get; set; }
        public Buyer[] bids;

        public static ValidationResult ValidateBidEndDate(DateTime BidEndDate, ValidationContext validationContext)
        {
            if (BidEndDate < DateTime.Now)
            {
                return new ValidationResult("Bid End Date cannot be less than today");
            }

            return ValidationResult.Success;
        }

        public static ValidationResult ValidateCategory(string Category, ValidationContext validationContext)
        {
            if (Category.ToLower()=="painting"|| Category.ToLower() == "sculpture"|| Category.ToLower() == "ornament")
            {
                return ValidationResult.Success;            }

            return new ValidationResult("Category not Correct");
        }
    }

   
}