using System;
using System.Collections.Generic;
using System.Linq;

namespace SXF
{
  public class Document
  {
    public const string TypeComment = "#";
    public const string TypeVertex = "vertex";
    public const string TypeEdge = "edge";

    public const string HeaderComment = TypeComment + " simple io v0";

    public List<Element> Elements { get; set; }

    public Document()
    {
      Elements = new List<Element>();
    }

    public void Flatten()
    {
      foreach (Element element in Elements)
      {
        element.Flatten();
      }
    }

    public void Collapse()
    {
      HashSet<Element> elementSet = new HashSet<Element>();

      foreach (Element element in Elements)
      {
        element.Sort();
        elementSet.Add(element.Collapsed());
      }

      Elements.Clear();
      Elements.AddRange(elementSet);
    }

    public void Optimize()
    {
      Collapse();

      Dictionary<Vertex, HashSet<Element>> vertexMapping = new Dictionary<Vertex, HashSet<Element>>();

      foreach (Element element in Elements)
      {
        AddElement(element, element.Start(), vertexMapping);
        AddElement(element, element.End(), vertexMapping);
      }

      List<Element> newElements = new List<Element>();
      Vertex currentVertex = new Vertex();

      while (vertexMapping.Count > 0)
      {
        Vertex nearestVertex = vertexMapping.Keys.First();
        double nearestDistance = nearestVertex.Distance(currentVertex);

        foreach (Vertex vertex in vertexMapping.Keys)
        {
          double distance = vertex.Distance(currentVertex);

          if (distance < nearestDistance)
          {
            nearestVertex = vertex;
            nearestDistance = distance;
          }
        }

        Element element = PeekElement(nearestVertex, vertexMapping);

        RemoveElement(element, element.Start(), vertexMapping);
        RemoveElement(element, element.End(), vertexMapping);

        if (nearestVertex.Equals(element.End()))
        {
          element.Reverse();
        }

        newElements.Add(element);

        currentVertex = element.End();
      }

      Elements.Clear();
      Elements.AddRange(newElements);
    }

    private void AddElement(Element element, Vertex vertex, Dictionary<Vertex, HashSet<Element>> vertexMapping)
    {
      HashSet<Element> elements;

      if (!vertexMapping.TryGetValue(vertex, out elements))
      {
        elements = new HashSet<Element>();
        vertexMapping.Add(vertex, elements);
      }

      elements.Add(element);
    }

    private Element PeekElement(Vertex vertex, Dictionary<Vertex, HashSet<Element>> vertexMapping)
    {
      HashSet<Element> elements;

      if (!vertexMapping.TryGetValue(vertex, out elements))
      {
        throw new Exception();
      }

      return elements.First();
    }

    private void RemoveElement(Element element, Vertex vertex, Dictionary<Vertex, HashSet<Element>> vertexMapping)
    {
      HashSet<Element> elements;

      if (vertexMapping.TryGetValue(vertex, out elements) && elements.Contains(element))
      {
        elements.Remove(element);

        if (elements.Count == 0)
        {
          vertexMapping.Remove(vertex);
        }
      }
    }
  }
}
