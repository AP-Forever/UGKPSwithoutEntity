using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UGKPSwithoutEntity.Models;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
using UGKPSwithoutEntity.Useful_Classes;
using UGKPSwithoutEntity.Attributes;
using PagedList;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Microsoft.Reporting.WebForms;
using System.Web.UI.WebControls;

namespace UGKPSwithoutEntity.Controllers
{
    public class InvestmentsController : Controller
    {
        GeneralFunctions functions = new GeneralFunctions();
        string _mainconnString = ConfigurationManager.ConnectionStrings["ugkpsConnectionString"].ConnectionString; //Connection string from web.config
        // GET: Investments
        [HttpGet]
        [CustomAuthorize]
        public ActionResult AddStock()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddStock(Stock stockModel)
        {
            User user = functions.GetUserDetails(Convert.ToInt32(System.Web.HttpContext.Current.Session["UserID"]));
            stockModel.Comment = stockModel.Comment == null ? "" : stockModel.Comment;
            if (ModelState.IsValid)
            {
                int _queryResult = functions.InsertStock(stockModel, user);
                if(_queryResult > 0)
                {
                    ModelState.Clear();
                    ViewBag.Message = "Success";
                }
                else
                {
                    ViewBag.Message = null;
                }
                
            }
            return View(stockModel);
        }

        [HttpGet]
        [CustomAuthorize]
        public async Task<ActionResult> Stocks(string sortBy, int? page)
        {
            //Sorting Implementation 18-04-2020
            #region sort Parameters Viewbag insert/update

            if (!string.IsNullOrEmpty(sortBy))
            {
                TempData["SortParameterName"] = sortBy;
            }
            else
            {
                TempData["SortParameterName"] = "Ticker";
            }

            ViewBag.SortByParameter = sortBy;

            #endregion

            DataTable dtblStocks = new DataTable();
            using (SqlConnection sqlcon = new SqlConnection(_mainconnString))
            {
                sqlcon.Open();
                SqlDataAdapter daStocks = new SqlDataAdapter("SELECT * FROM [dbo].[Stocks_Investment]", _mainconnString);
                daStocks.Fill(dtblStocks);
            }

            List<Stock> List = new List<Stock>();

            await Task.Run(()=>
            {
                for (int i = 0; i < dtblStocks.Rows.Count; i++)
                {
                    Stock stock = new Stock();
                    for (int j = 0; j < dtblStocks.Columns.Count; j++)
                    {
                        string property = dtblStocks.Columns[j].ColumnName.ToString();
                        //if ((property != "PWResetCode") && (property != "ActivationCode"))
                        //{
                        stock.GetType().GetProperty(property).SetValue(stock, dtblStocks.Rows[i][j]);
                        //}
                    }
                    List.Add(stock);
                }
            });

            //Sorting Implementation 18-04-2020
            #region Implement Sorting through OrderBy in List
            var sortByParameter = (string)TempData["SortParameterName"];

            if ( sortByParameter.IndexOf("Desc") >= 0)
            {
                sortByParameter = sortByParameter.Substring(0,sortByParameter.IndexOf("Desc"));
            }

            var propertyInfo = typeof(Stock).GetProperty(sortByParameter);

            if( ((string)TempData["SortParameterName"]).Contains("Desc"))
            {
                List = List.OrderByDescending(stock => propertyInfo.GetValue(stock, null)).ToList();
            }
            else
            {
                List = List.OrderBy(stock => propertyInfo.GetValue(stock, null)).ToList();
            }
            #endregion


            return View(List.ToPagedList(page ?? 1, 10));
        }

        [CustomAuthorize]
        public ActionResult UpdateStockRecordInDB(Stock stock)
        {
            if (ModelState.IsValid)
            {
               int _queryResult = functions.UpdateStockDetails(stock);
                if (_queryResult > 0)
                {
                    ModelState.Clear();
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }

            }
            return PartialView("_EditStock", stock);

        }

        public JsonResult GetStockDetailsByID(int StockID)
        {
            Stock stock= functions.GetStockDetails(StockID);
            string value = string.Empty;
            value = JsonConvert.SerializeObject(stock, Formatting.Indented);
            return Json(value, JsonRequestBehavior.AllowGet);
        }

        
        public JsonResult DeleteStockRecord(int StockID)
        {
            int _queryResult = functions.RemoveStock(StockID);
            if (_queryResult > 0)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateUsersInvestmentAccessRecord(int UserID)
        {
            InvestmentAccess investAccessUser = functions.GetUsersInvestmentAccessDetails(UserID);
            investAccessUser.HasAcceptedDisclaimer = true;
            investAccessUser.DateAccepted = DateTime.Now;
            int _queryResult = functions.UpdateInvestmentAccessRecord(investAccessUser);
            if (_queryResult > 0)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RefreshStocks()
        {
            DataTable dtblStocks = new DataTable();
            using (SqlConnection sqlcon = new SqlConnection(_mainconnString))
            {
                sqlcon.Open();
                SqlDataAdapter daStocks = new SqlDataAdapter("SELECT * FROM [dbo].[Stocks_Investment]", _mainconnString);
                daStocks.Fill(dtblStocks);
            }

            List<Stock> List = new List<Stock>();

            for (int i = 0; i < dtblStocks.Rows.Count; i++)
            {
                Stock stock = new Stock();
                for (int j = 0; j < dtblStocks.Columns.Count; j++)
                {
                    string property = dtblStocks.Columns[j].ColumnName.ToString();
                    stock.GetType().GetProperty(property).SetValue(stock, dtblStocks.Rows[i][j]);
                }
                List.Add(stock);
            }

            bool success = false;
            string url;
            string finalTicker="";
            string errored_stocks="";                    
                
            foreach (Stock _stock in List)
            {
                try
                {
                    using (var httpClient = new HttpClient())
                    {
                        url = "https://financialmodelingprep.com/api/v3/quote/";
                        switch (_stock.Exchange)
                        {
                            case "TSX":
                                finalTicker = _stock.Ticker + ".TO";
                                break;
                            default:
                                finalTicker = _stock.Ticker;
                                break;
                        }
                        url = url + finalTicker + "?apikey=50dbbf39738ef7b394b5491fe5b53439";
                        var response = httpClient.GetStringAsync(new Uri(url)).Result;
                        if(response != null)
                        {
                            var stockArray = JArray.Parse(response);
                            if (stockArray != null && stockArray.HasValues)
                            {
                                var stockObject = JObject.Parse(stockArray.First.ToString());
                                if (!String.IsNullOrEmpty(stockObject["price"].ToString())) _stock.Price = (Double)stockObject["price"];
                                if (!String.IsNullOrEmpty(stockObject["yearHigh"].ToString())) _stock.Price_High = (Double)stockObject["yearHigh"];
                                if (!String.IsNullOrEmpty(stockObject["dayHigh"].ToString())) _stock.Day_Price_High = (Double)stockObject["dayHigh"];
                                if (!String.IsNullOrEmpty(stockObject["yearLow"].ToString())) _stock.Price_Low = (Double)stockObject["yearLow"];
                                if (!String.IsNullOrEmpty(stockObject["dayLow"].ToString())) _stock.Day_Price_Low = (Double)stockObject["dayLow"];
                                if (!String.IsNullOrEmpty(stockObject["eps"].ToString())) _stock.EPS = (Double)stockObject["eps"];
                                if (!String.IsNullOrEmpty(stockObject["pe"].ToString())) _stock.P_E_Ratio = (Double)stockObject["pe"];
                                _stock.LastUpdated = (DateTimeOffset.FromUnixTimeSeconds((long)stockObject["timestamp"])).UtcDateTime.ToLocalTime();

                                int _queryResult = functions.UpdateStockDetails(_stock);
                                success = (_queryResult <= 0) ? false : true;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    errored_stocks += finalTicker + ", ";
                }
                finally
                {
                    
                }
            }
            if (errored_stocks != "")
            {
                TempData["message"] = "Following stocks data did not refresh: <br/>" +
                                errored_stocks;
            }
            else
            {
                TempData["message"] = "New Data fetched. <br/>" + "Following stocks data did not refresh: <br/>" +
                                errored_stocks;
            }
            

            if (success)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
            
        }

        [HttpGet]
        [CustomAuthorize]
        public async Task<ActionResult> Transactions(string sortBy, int? page)
        {
            //Sorting Implementation 18-04-2020
            #region sort Parameters Viewbag insert/update

            if (!string.IsNullOrEmpty(sortBy))
            {
                TempData["SortParameterName"] = sortBy;
            }
            else
            {
                TempData["SortParameterName"] = "Ticker";
            }

            ViewBag.SortByParameter = sortBy;

            #endregion

            DataTable dtblTransactions = new DataTable();
            using (SqlConnection sqlcon = new SqlConnection(_mainconnString))
            {
                sqlcon.Open();
                SqlDataAdapter daTransactions = new SqlDataAdapter("SELECT * FROM [dbo].[Stocks_Transactions] WHERE UserID = " + Convert.ToInt32(System.Web.HttpContext.Current.Session["UserID"]) + " Order By Ticker", _mainconnString);
                daTransactions.Fill(dtblTransactions);
            }

            List<Transactions> List = new List<Transactions>();
            bool NewStock = true;

            await Task.Run(() =>
            {
                Transactions allCurrentStockTransactions = new Transactions();
                allCurrentStockTransactions.transactions = new List<Transaction>();
                for (int i = 0; i < dtblTransactions.Rows.Count; i++)
                {
                    Transaction trans = new Transaction();
                    for (int j = 0; j < dtblTransactions.Columns.Count; j++)
                    {
                        string property = dtblTransactions.Columns[j].ColumnName.ToString();
                        trans.GetType().GetProperty(property).SetValue(trans, dtblTransactions.Rows[i][j]);
                    }

                    if (i > 0)
                    {
                        if ((trans.Ticker == (string)dtblTransactions.Rows[i-1][2]) && (trans.Exchange == (string)dtblTransactions.Rows[i-1][3]) )
                        {
                            NewStock = false;
                        }
                        else
                        {
                            NewStock = true;
                        }
                    }
                    if(NewStock)
                    {
                        if (i > 0)
                        {
                            Stock prevStock = functions.GetStockDetailsByTicker((string)dtblTransactions.Rows[i - 1][2], (string)dtblTransactions.Rows[i - 1][3]);
                            allCurrentStockTransactions.AveragePrice = allCurrentStockTransactions.TotalPrice / allCurrentStockTransactions.TotalQuantity;
                            allCurrentStockTransactions.ChangeInCurrency = prevStock.Price - allCurrentStockTransactions.AveragePrice;
                            allCurrentStockTransactions.ChangeInPercentage = (prevStock.Price - allCurrentStockTransactions.AveragePrice) / allCurrentStockTransactions.AveragePrice;
                            allCurrentStockTransactions.SI_ID = prevStock.SI_ID;
                            List.Add(allCurrentStockTransactions);
                        }
                        allCurrentStockTransactions = new Transactions();
                        allCurrentStockTransactions.transactions = new List<Transaction>();
                        allCurrentStockTransactions.Ticker = trans.Ticker;
                        allCurrentStockTransactions.Exchange = trans.Exchange;
                        allCurrentStockTransactions.CompanyName = trans.CompanyName;
                    }
                    else
                    {

                    }
                    allCurrentStockTransactions.TotalPrice += (trans.Price * trans.Quantity);
                    allCurrentStockTransactions.TotalQuantity += trans.Quantity;
                    allCurrentStockTransactions.UserID = trans.UserID;
                    allCurrentStockTransactions.transactions.Add(trans);
                    if (i == dtblTransactions.Rows.Count - 1)
                    {
                        Stock prevStock = functions.GetStockDetailsByTicker((string)dtblTransactions.Rows[i - 1][2], (string)dtblTransactions.Rows[i - 1][3]);
                        allCurrentStockTransactions.AveragePrice = allCurrentStockTransactions.TotalPrice / allCurrentStockTransactions.TotalQuantity;
                        allCurrentStockTransactions.ChangeInCurrency = prevStock.Price - allCurrentStockTransactions.AveragePrice;
                        allCurrentStockTransactions.ChangeInPercentage = (prevStock.Price - allCurrentStockTransactions.AveragePrice) / allCurrentStockTransactions.AveragePrice;
                        allCurrentStockTransactions.SI_ID = prevStock.SI_ID;
                        List.Add(allCurrentStockTransactions);
                    }
                }
            });

            //Sorting Implementation 18-04-2020
            #region Implement Sorting through OrderBy in List
            var sortByParameter = (string)TempData["SortParameterName"];

            if (sortByParameter.IndexOf("Desc") >= 0)
            {
                sortByParameter = sortByParameter.Substring(0, sortByParameter.IndexOf("Desc"));
            }

            var propertyInfo = typeof(Transactions).GetProperty(sortByParameter);

            if (((string)TempData["SortParameterName"]).Contains("Desc"))
            {
                List = List.OrderByDescending(trans => propertyInfo.GetValue(trans, null)).ToList();
            }
            else
            {
                List = List.OrderBy(trans => propertyInfo.GetValue(trans, null)).ToList();
            }
            #endregion


            return View(List.ToPagedList(page ?? 1, 10));
        }
 
        [HttpGet]
        [CustomAuthorize]
        public ActionResult AddTransaction()
        {
            return View();
        }


        public JsonResult SaveStockTransactionRecordInDB(Transaction transModel)
        {
            User user = functions.GetUserDetails(Convert.ToInt32(System.Web.HttpContext.Current.Session["UserID"]));
            if (ModelState.IsValid)
            {
                if(transModel.Stock_Transaction_ID < 0)
                {
                    transModel.Quantity = 0 - transModel.Quantity;
                    transModel.Stock_Transaction_ID = 0 - transModel.Stock_Transaction_ID;
                }
                int _queryResult = functions.InsertTransaction(transModel);
                if (_queryResult > 0)
                {
                    ViewBag.Message = "Success";
                    ModelState.Clear();
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    ViewBag.Message = null;
                    return Json(false, JsonRequestBehavior.AllowGet);
                }

            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteStockTransactionRecord(int Stock_Transaction_ID)
        {
            int _queryResult = functions.RemoveStockTransactionRecord(Stock_Transaction_ID);
            if (_queryResult > 0)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStockTransactionByID(int Stock_Transaction_ID)
        {
            Transaction trans= functions.GetStockTransactionByID(Stock_Transaction_ID);
            string value = string.Empty;
            value = JsonConvert.SerializeObject(trans, Formatting.Indented);
            return Json(value, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [CustomAuthorize]
        public ActionResult Holdings()
        {
            string ssrsUrl = ConfigurationManager.AppSettings["SSRSReportsUrl"].ToString();
            ReportViewer viewer = new ReportViewer();
            viewer.ProcessingMode = ProcessingMode.Remote;
            viewer.SizeToReportContent = true;
            viewer.AsyncRendering = false;
            viewer.Width = Unit.Percentage(100);
            viewer.Height = Unit.Percentage(100);
            viewer.ServerReport.ReportServerUrl = new Uri(ssrsUrl);
            viewer.ServerReport.ReportPath = "/Report1";
            ViewBag.ReportViewer = viewer;

            return View();
        }

    }
}