using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Windows;
using HtmlAgilityPack;

namespace TestProfKholod
{
    class ParseSite
    {

        public int SearchTag(string tag, string url)
        {
            
            string page = GetHtmlPageText(url);
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(page);
            HtmlNodeCollection nodes = htmlDoc.DocumentNode.SelectNodes($"//{tag}");
            if (nodes is null)
                return 0;
            return nodes.Count;
            
        }

        public string GetHtmlPageText(string url)
        {
            string txt = String.Empty;
            WebRequest req = WebRequest.Create(url);
            WebResponse resp = req.GetResponse();
            using (Stream stream = resp.GetResponseStream())
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    txt = sr.ReadToEnd();
                }
            }

            return txt;
        }
    }
}
