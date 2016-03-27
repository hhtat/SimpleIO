using SXF;
using System;
using System.IO;

namespace SXF2SVG
{
  public class Program
  {
    public static void Main(string[] args)
    {
      Document document;

      using (FileStream stream = File.Open(args[0], FileMode.Open, FileAccess.Read))
      {
        document = IO.Load(stream);
      }

      double xMin = Double.MaxValue;
      double xMax = Double.MinValue;
      double yMin = Double.MaxValue;
      double yMax = Double.MinValue;

      document.Flatten();
      document.Optimize();

      foreach (Element element in document.Elements)
      {
        Vertex minBounds = element.MinBounds();
        Vertex maxBounds = element.MinBounds();

        if (minBounds.X < xMin) { xMin = minBounds.X; }
        if (minBounds.Y < yMin) { yMin = minBounds.Y; }
        if (maxBounds.X > xMax) { xMax = maxBounds.X; }
        if (maxBounds.Y > yMax) { yMax = maxBounds.Y; }
      }

      using (FileStream stream = File.Open(args[1], FileMode.Create, FileAccess.Write))
      using (StreamWriter writer = new StreamWriter(stream))
      {
        writer.Write("<svg xmlns='http://www.w3.org/2000/svg' width='");
        writer.Write(xMax - xMin);
        writer.Write("in' height='");
        writer.Write(yMax - yMin);
        writer.Write("in' viewBox='");
        writer.Write(xMin);
        writer.Write(' ');
        writer.Write(yMin);
        writer.Write(' ');
        writer.Write(xMax - xMin);
        writer.Write(' ');
        writer.Write(yMax - yMin);
        writer.WriteLine("'>");

        Vertex lastVertex = null;

        foreach (Element element in document.Elements)
        {
          Edge edge = element as Edge;

          if (edge == null)
          {
            continue;
          }

          Vertex start = edge.Start();
          Vertex end = edge.End();

          if (!start.Equals(lastVertex))
          {
            if (lastVertex != null)
            {
              writer.WriteLine("' stroke='black' stroke-width='0.001pt' fill='none'/>");
            }

            writer.Write("<path d='");
            writer.Write('M');
            writer.Write(start.X);
            writer.Write(',');
            writer.Write(start.Y);
          }

          writer.Write('L');
          writer.Write(end.X);
          writer.Write(',');
          writer.Write(end.Y);

          lastVertex = end;
        }

        if (lastVertex != null)
        {
          writer.WriteLine("' stroke='black' stroke-width='0.001pt' fill='none'/>");
        }

        writer.Write("</svg>");
      }
    }
  }
}
