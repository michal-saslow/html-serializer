

using practicode2;


using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.RegularExpressions;


namespace Practicode2
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            // טעינת ה-HTML של האתר
            var url = "https://www.example.net/";
            var html = await Load(url);

            var cleanhtml = new Regex("\\s").Replace(html, "");
            var htmlLines = new Regex("<(.*?)>").Split(cleanhtml).Where(s => s.Length > 0);
          
            ;
            Console.WriteLine(htmlLines.ToList().Count());
            // יצירת סלקטור עבור הקלאס ".ga-nav"
            var selector = Selector.ConvertQueryStringToSelector("p");
           
            // בניית עץ אלמנטים
           
            var rootElement = HtmlHelper.Instance.BuildHtmlTree(htmlLines.ToList());
           
            // מציאת כל האלמנטים העונים לקלאס ".ga-nav"
            var elements = HtmlElement.FilterElementsBySelector(rootElement, selector);

            // הדפסת עץ האלמנטים
            foreach (var element in elements)
            {
              
                    Console.WriteLine(element.Name);
                    foreach (var child in element.Children)
                    {
                        Console.WriteLine($"<{element.Name} class=\"{string.Join(" ", element.Classes)}\">{element.InnerHtml}</{element.Name}>");
                    }
                
            }
        }

        static async Task<string> Load(string url)
        {
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(url);
            var html = await response.Content.ReadAsStringAsync();
            return html;
        }



    }
    }



