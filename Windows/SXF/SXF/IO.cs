using System;
using System.IO;

namespace SXF
{
  public class IO
  {
    public static Document Load(Stream stream)
    {
      Document document = new Document();

      using (StreamReader reader = new StreamReader(stream))
      {
        if (reader.ReadLine() != Document.HeaderComment)
        {
          throw new Exception();
        }

        string line;
        while ((line = reader.ReadLine()) != null)
        {
          string[] tokens = line.Split(' ');

          if (tokens.Length == 0)
          {
            continue;
          }

          switch (tokens[0])
          {
            case Document.TypeComment:
              break;
            case Document.TypeVertex:
              document.Elements.Add(loadVertex(1, tokens));
              break;
            case Document.TypeEdge:
              document.Elements.Add(loadEdge(1, tokens));
              break;
            default:
              throw new Exception();
          }
        }
      }

      return document;
    }

    private static Vertex loadVertex(int index, string[] tokens)
    {
      return new Vertex(
          Double.Parse(tokens[index + 0]),
          Double.Parse(tokens[index + 1]),
          Double.Parse(tokens[index + 2]));
    }

    private static Edge loadEdge(int index, string[] tokens)
    {
      return new Edge(
          loadVertex(index + 0, tokens),
          loadVertex(index + 3, tokens));
    }

    public static void Save(Document document, Stream stream)
    {
      using (StreamWriter writer = new StreamWriter(stream))
      {
        writer.WriteLine(Document.HeaderComment);

        foreach (Element element in document.Elements)
        {
          Vertex vertex = element as Vertex;
          if (vertex != null)
          {
            saveVertex(vertex, writer);
            continue;
          }

          Edge edge = element as Edge;
          if (edge != null)
          {
            saveEdge(edge, writer);
            continue;
          }

          throw new Exception();
        }
      }
    }

    private static void saveVertex(Vertex vertex, StreamWriter writer)
    {
      writer.Write(Document.TypeVertex);
      writer.Write(' ');
      saveNakedVertex(vertex, writer);
      writer.WriteLine();
    }

    private static void saveNakedVertex(Vertex vertex, StreamWriter writer)
    {
      writer.Write(vertex.X);
      writer.Write(' ');
      writer.Write(vertex.Y);
      writer.Write(' ');
      writer.Write(vertex.Z);
    }

    private static void saveEdge(Edge edge, StreamWriter writer)
    {
      writer.Write(Document.TypeEdge);
      writer.Write(' ');
      saveNakedVertex(edge.V0, writer);
      writer.Write(' ');
      saveNakedVertex(edge.V1, writer);
      writer.WriteLine();
    }
  }
}
