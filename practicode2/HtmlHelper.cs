
using System;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;

public class HtmlHelper
{
    public string[] allHtmlTags { get; private set; }
    public string[] selfClosingTags { get; private set; }

    private static readonly HtmlHelper _instance = new HtmlHelper();

    public static HtmlHelper Instance => _instance;
    private HtmlHelper()
    {
        allHtmlTags = LoadTagsFromJson("AllTags.json");
        selfClosingTags = LoadTagsFromJson("SelfClosingTags.json");
    }

    private string[] LoadTagsFromJson(string jsonFilePath)
    {
        try
        {
            using (StreamReader reader = new StreamReader(jsonFilePath))
            {
                string jsonString = reader.ReadToEnd();
                return JsonSerializer.Deserialize<string[]>(jsonString);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading tags from JSON file '{jsonFilePath}': {ex.Message}");
            return null;
        }
    }


   
    public void PrintAllTags()
    {
        Console.WriteLine("All HTML Tags:");
        foreach (var tag in allHtmlTags)
        {
            Console.WriteLine(tag);
        }
    }

    public void PrintSelfClosingTags()
    {
        Console.WriteLine("Self Closing HTML Tags:");
        foreach (var tag in selfClosingTags)
        {
            Console.WriteLine(tag);
        }
    }

   
    public bool IsTagWithoutClosing(string tag)
    {
        return allHtmlTags.Contains(tag) && !selfClosingTags.Contains(tag);
    }


    public bool IsSelfClosingTag(string tag)
    {
        return selfClosingTags.Contains(tag);
    }


    public  HtmlElement BuildHtmlTree(List<string > htmlStrings)
    {
        HtmlElement rootElement = new HtmlElement("root", null);
        HtmlElement currentElement = rootElement;

        foreach (var line in htmlStrings)
        {
            var parts = line.Split(new char[] { ' ' }, 2);
            var tag = parts[0];

            if (tag.StartsWith("html/"))
            {
                break;
            }

            if (currentElement != null && tag.StartsWith("/"))
            {
                currentElement = currentElement.Parent;
            }
            else if (allHtmlTags.Contains(tag))
            {
                var newElement = new HtmlElement(tag, currentElement);
                newElement.Classes = new List<string>();
                newElement.Attributes = new List<string>();

                // Parse attributes
                var attributes = new Regex("<(.*?)>").Split(line).Skip(1).Where(s => s.Length > 0).ToList();
                foreach (var attribute in attributes)
                {
                    var parts1 = attribute.Split(new char[] { '=' }, 2);

                    // Check if there are two parts before accessing parts1[1]
                    if (parts1 != null && parts1.Length == 2)
                    {
                        var name = parts1[0].Trim();
                        var value = parts1[1].Trim();

                        if (name.Equals("class"))
                        {
                            newElement.Classes.Add(value);
                        }
                        else
                        {
                            newElement.Attributes.Add(name);
                        }
                    }
                }

                // Check if tag is self-closing
                if (tag.EndsWith("/") || selfClosingTags.Contains(tag) || line.EndsWith("/>"))
                {
                    newElement.InnerHtml = "";
                    if (currentElement != null)
                    {
                        if (currentElement.Children == null)
                        {
                            currentElement.Children = new List<HtmlElement>();
                        }
                        currentElement.Children.Add(newElement);
                    }
                }
                else
                {
                    newElement.InnerHtml = parts.Length > 1 ? parts[1] : "";

                    if (currentElement != null)
                    {
                        if (currentElement.Children == null)
                        {
                            currentElement.Children = new List<HtmlElement>();
                        }
                        currentElement.Children.Add(newElement);
                        currentElement = newElement;
                    }
                }
            }
            else
            {
                if (currentElement != null)
                {
                    currentElement.InnerHtml += line;
                }
            }
        }

        return rootElement;
    }
}
    
   








