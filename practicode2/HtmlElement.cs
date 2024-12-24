
using practicode2;
using System.Text.Json;
using System.Text.Json.Serialization; // עבור [JsonPropertyName]

public class HtmlElement
{
    public string Id { get; set; }
    public string Name { get; set; }
    public List<string> Attributes { get; set; }
    public List<string> Classes { get; set; }
    public string InnerHtml { get; set; }
    public HtmlElement Parent { get; set; }
    public List<HtmlElement> Children { get; set; }

    public HtmlElement()
    {
        Attributes = new List<string>();
        Classes = new List<string>();
        Children = new List<HtmlElement>();
    }
    public HtmlElement(string name, HtmlElement parent)
    {
        Name = name;
        Parent = parent;
        Attributes = new List<string>();
        Classes = new List<string>();
        Children = new List<HtmlElement>();
    }

    public static HtmlElement Deserialize(string json)
    {
        return JsonSerializer.Deserialize<HtmlElement>(json);
    }
 
    public IEnumerable<HtmlElement> Descendants()
    {
        Queue<HtmlElement> queue = new Queue<HtmlElement>();
        queue.Enqueue(this);

        while (queue.Count > 0)
        {
            HtmlElement current = queue.Dequeue();
            yield return current;

            foreach (var child in current.Children)
            {
                queue.Enqueue(child);
            }
        }
    }

    public IEnumerable<HtmlElement> Ancestors()
    {
        HtmlElement current = this;
        while (current.Parent != null)
        {
            current = current.Parent;
            yield return current;
        }
    }

    public static IEnumerable<HtmlElement> FilterElementsBySelector(HtmlElement root, Selector selector)
    {
        HashSet<HtmlElement> results = new HashSet<HtmlElement>();
        FilterElementsBySelectorRecursive(root, selector, results);
        return results;
 }

    
    public static void FilterElementsBySelectorRecursive(HtmlElement element, Selector selector, HashSet<HtmlElement> results)
    {
        if (element.MatchesSelector(selector))
        {
            results.Add(element);
        }

        foreach (var child in element.Children)
        {
            FilterElementsBySelectorRecursive(child, selector, results);
        }
    }
 




public bool MatchesSelector(Selector selector)
    {
        var uniqueMatches = new HashSet<string>();

        if (!string.IsNullOrWhiteSpace(selector.TagName) && !string.Equals(selector.TagName, this.Name, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(selector.Id) && !string.Equals(selector.Id, this.Id, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        foreach (var className in selector.Classes)
        {
            if (!this.Classes.Contains(className, StringComparer.OrdinalIgnoreCase))
            {
                return false;
            }
            uniqueMatches.Add(className);
        }

        return uniqueMatches.Count == selector.Classes.Count;
    }

}



