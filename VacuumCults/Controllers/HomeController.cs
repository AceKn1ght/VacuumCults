using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using VacuumCults.Models;
using HtmlAgilityPack;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Net;
using System.Text;
using System.IO;

namespace VacuumCults.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument document = web.Load("https://cults3d.com/en/users/yarrowpop/downloads");

            HtmlNode[] nodes = document.DocumentNode.SelectNodes("//a").ToArray();
            foreach (HtmlNode item in nodes)
            {
                Console.WriteLine(item.InnerHtml);
            }

            //string url = "https://en.wikipedia.org/wiki/List_of_programmers";
            //var response = CallUrl(url).Result;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult SearchURL(SearchViewModel svm)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument document = web.Load(svm.searchURL);

            HtmlNode[] nodes = document.DocumentNode.SelectNodes("//a[contains(@class, 'tbox-thumb drawer-contents')]").ToArray();

            List<string> websLinks = new List<string>();

            foreach (HtmlNode item in nodes)
            {
                HtmlAttribute hrefAtt = item.Attributes["href"];
                websLinks.Add(hrefAtt.Value);
                Console.WriteLine(hrefAtt.Value);
            }

            WriteToCsv(websLinks);

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private static async Task<string> CallUrl(string fullUrl)
        {
            HttpClient client = new HttpClient();
            var response = await client.GetStringAsync(fullUrl);
            return response;
        }

        private List<string> ParseHtml(string html)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var programmerLinks = htmlDoc.DocumentNode.Descendants("li")
                .Where(node => !node.GetAttributeValue("class", "").Contains("tocsection"))
                .ToList();

            List<string> wikiLink = new List<string>();

            foreach (var link in programmerLinks)
            {
                if (link.FirstChild.Attributes.Count > 0) wikiLink.Add("https://en.wikipedia.org/" + link.FirstChild.Attributes[0].Value);
            }

            return wikiLink;
        }

        private void WriteToCsv(List<string> links)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var link in links)
            {
                sb.AppendLine(link);
            }

            System.IO.File.WriteAllText("links.csv", sb.ToString());
        }


    }
}