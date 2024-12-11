using System.Xml.Linq;

namespace ComicRack.Core;

public static class XDocumentParser
{
    public static XElement GetElement(this XDocument document, string elementName)
    {
        return document.Root?.Element(elementName) ?? null;
    }

    public static string GetRootElement(this XDocument document, string elementName)
    {
        return document.Root?.Element(elementName)?.Value ?? "";
    }

    public static int? GetRootElementAsInt(this XDocument document, string elementName)
    {
        var value = GetRootElement(document, elementName);
        return int.TryParse(value, out var year) ? year : null;
    }
}
