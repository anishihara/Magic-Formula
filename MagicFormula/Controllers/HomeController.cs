using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using HTMLParser;
using MagicFormula.Models;

namespace MagicFormula.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";
            //WebClient wc = new WebClient();
            string pageData;
            //pageData = wc.DownloadString("http://www.fundamentus.com.br/resultado.php");
            PostSubmitter post = new PostSubmitter();
            post.Url = "http://www.fundamentus.com.br/resultado.php";
            post.PostItems.Add("pl_min", "1");
            post.PostItems.Add("pl_max", "20");
            post.PostItems.Add("roe_min", "0");
            post.PostItems.Add("pvp_min", "0");
            post.PostItems.Add("pvp_max", "10");
            post.PostItems.Add("liqcorr_min", "1");
            post.PostItems.Add("margemebit_min", "0");
            post.PostItems.Add("tx_cresc_rec_min", "0.08");
            post.PostItems.Add("liq_min", "100000");
            pageData = post.Post();
           
            List<Stock> stocks = new List<Stock>();
            HTMLSearchResult searcher = new HTMLSearchResult();
            HTMLSearchResult result;
            
            int i = 0;
            for (i = 0; i < 20; i++ )
            {
                result = searcher.GetTagData(pageData, "html", 1).GetTagData("body").
                            GetTagData("table").GetTagData("tr", i + 2).GetTagData("td").GetTagData("span").GetTagData("a");
                Stock stock = new Stock
                {
                    Name = result.TAGData
                };
                result = searcher.GetTagData(pageData, "html", 1).GetTagData("body").
                            GetTagData("table").GetTagData("tr", i + 2).GetTagData("td", 3);
                stock.PL = Convert.ToDouble(result.TAGData);
                result = searcher.GetTagData(pageData, "html", 1).GetTagData("body").
                            GetTagData("table").GetTagData("tr", i + 2).GetTagData("td", 16);
                stock.ROE = Convert.ToDouble(result.TAGData.Split('%').First());
                if (stock.PL > 1 && stock.PL < 20 && stock.ROE > 0)
                    stocks.Add(stock);
               
            }
            stocks = stocks.OrderBy(a => a.PL).ToList();
            i = 1;
            foreach (var stock in stocks)
            {
                stock.PLPosition = i;
                i++;
            }
            stocks = stocks.OrderByDescending(a => a.ROE).ToList();
            i = 1;
            foreach (var stock in stocks)
            {
                stock.ROEPosition = i;
                stock.Position = stock.PLPosition + stock.ROEPosition;
                i++;
            }

            
            return View(stocks.OrderBy(a=>a.Position).ToList());
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
