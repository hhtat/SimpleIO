using SVG2SXF.SVG;
using SXF;
using System.IO;
using System.Xml.Linq;

namespace SVG2SXF
{
  public class Program
  {
    public static void Main(string[] args)
    {
      XDocument xDocument = XDocument.Load(args[0]);

      Document document = new Document();

      LoadElement(xDocument.Root, document);

      document.Optimize();

      using (FileStream stream = File.Open(args[1], FileMode.Create, FileAccess.Write))
      {
        IO.Save(document, stream);
      }
    }

    private static void LoadElement(XElement element, Document document)
    {
      switch (element.Name.ToString())
      {
        case @"{http://www.w3.org/2000/svg}path":
          LoadPathElement(element, document);
          break;
        default:
          LoadDescendants(element, document);
          break;
      }
    }

    private static void LoadDescendants(XElement element, Document document)
    {
      foreach (XElement descendant in element.Descendants())
      {
        LoadElement(descendant, document);
      }
    }

    private static void LoadPathElement(XElement element, Document document)
    {
      LoadPathData(element.Attribute("d").Value, document);
      LoadDescendants(element, document);
    }

    private static void LoadPathData(string d, Document document)
    {
      PathData pathData = new PathData(d);

      foreach (SVG2SXF.SVG.PathData.Line line in pathData.Lines)
      {
        document.Elements.Add(new Edge(new Vertex(line.X0, line.Y0, 0.0), new Vertex(line.X1, line.Y1, 0.0)));
      }
    }
  }
}
