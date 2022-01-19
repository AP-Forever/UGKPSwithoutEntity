using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using UGKPSwithoutEntity.Attributes;

namespace UGKPSwithoutEntity.Models
{
    public class Stock
    {
        [Key]
        public int SI_ID { get; set; }

        public int UserID { get; set; }
        public string UserName { get; set; }
               
        [Display(Name = "Ticker/Symbol")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Ticker is required.")]
        public string Ticker { get; set; }

        [Display(Name = "Stock Exchange")]
        [Required(AllowEmptyStrings =false, ErrorMessage ="Stock Exchange selection is required.")]
        public string Exchange{ get; set; }

        [Display(Name = "Company Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Company Name is required.")]
        public string CompanyName { get; set; }

        [Display(Name = "LastUpdated")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:DD-MM-YYYY}")]
        public DateTime LastUpdated { get; set; }

        [Display(Name = "Price")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Price is required.")]
        [DataType(DataType.Currency)]
        public double Price { get; set; }

        [Display(Name = "52 Week Lowest Price")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Lowest Price is required.")]
        [LessThan("Price", ErrorMessage = "Lowest Price cannot be greater than Price.")]
        [DataType(DataType.Currency)]
        public double Price_Low { get; set; }

        [Display(Name = "52 Week Highest Price")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Highest Price is required.")]
        [GreaterThan("Price", ErrorMessage = "Highest Price cannot be lower than Price.")]
        [DataType(DataType.Currency)]
        public double Price_High { get; set; }

        [Display(Name = "Daily Lowest Price")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Lowest Price is required.")]
        [LessThan("Price", ErrorMessage = "Daily Lowest Price cannot be greater than Price.")]
        [DataType(DataType.Currency)]
        public double Day_Price_Low { get; set; }

        [Display(Name = "Daily Highest Price")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Highest Price is required.")]
        [GreaterThan("Price", ErrorMessage = "Daily Highest Price cannot be lower than Price.")]
        [DataType(DataType.Currency)]
        public double Day_Price_High { get; set; }

        [Display(Name = "Price to Earning (P_E) Ratio")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Price to Earning ratio is required.")]
        public double P_E_Ratio { get; set; }

        [Display(Name = "Earnings Per Share (EPS)")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Earnings Per Share is required.")]
        [DataType(DataType.Currency)]
        public double EPS { get; set; }

        [Display(Name = "Dividend Per Share ($)")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Dividend Per Share($) is required.")]
        [Range(0, Double.MaxValue)]
        [DataType(DataType.Currency)]
        public double DividendPerShare { get; set; }

        [Display(Name = "Dividend (%)")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Dividend(%) is required.")]
        [Range(0, Double.MaxValue)]
        public double DividendPercent { get; set; }

        [Display(Name = "Dividend Yield (%)")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Dividend Yield(%) is required.")]
        [Range(0, Double.MaxValue)]
        public double DividendYieldPercent { get; set; }

        [Display(Name = "Comment")]
        [DataType(DataType.MultilineText)]
        public string Comment { get; set; }

        [Display(Name = "Created Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:DD-MM-YYYY}")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Modified Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:DD-MM-YYYY}")]
        public DateTime ModifiedDate { get; set; }

        [Display(Name = "Sector")]
        public string Sector { get; set; }
    }

    public class ListOfStoks
    {
        public List<Stock> StocksList { get; set; }
    }

    public enum StockExchangeOptions
    {
        NASDAQ,
        NYSE,
        TSX,
        BSE,
        NSE
    }

    public class Transaction
    {
        [Key]
        public int Stock_Transaction_ID { get; set; }
        public int UserID { get; set; }

        [Display(Name = "Ticker/Symbol")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Ticker is required.")]
        public string Ticker { get; set; }

        [Display(Name = "Stock Exchange")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Stock Exchange selection is required.")]
        public string Exchange { get; set; }

        [Display(Name = "Company Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Company Name is required.")]
        public string CompanyName { get; set; }

        [Display(Name = "Price")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Price is required.")]
        [DataType(DataType.Currency)]
        public double Price { get; set; }

        [Display(Name = "Quantity")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Quantity is required.")]
        public int Quantity { get; set; }

        [Display(Name = "LastUpdated")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:DD-MM-YYYY}")]
        public DateTime LastUpdated { get; set; }

        [Display(Name = "CreatedDate")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:DD-MM-YYYY}")]
        public DateTime CreatedDate { get; set; }
    }

    public class Transactions : Transaction
    {
        public List<Transaction> transactions { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        public double AveragePrice { get; set; }
        public int TotalQuantity { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        public double ChangeInPercentage { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        public double ChangeInCurrency { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        public double TotalPrice { get; set; }
        public int SI_ID { get; set; }

    }

    
}