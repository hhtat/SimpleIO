using System;

namespace SXF
{
  public class Edge : Element
  {
    public Vertex V0 { get; private set; }
    public Vertex V1 { get; private set; }

    public Edge()
      : this(new Vertex(), new Vertex())
    {
    }

    public Edge(Edge e)
      : this(new Vertex(e.V0), new Vertex(e.V1))
    {
    }

    public Edge(Vertex v0, Vertex v1)
    {
      V0 = v0;
      V1 = v1;
    }

    public Vertex Start()
    {
      return V0;
    }

    public Vertex End()
    {
      return V1;
    }

    public Vertex MinBounds()
    {
      return new Vertex(Math.Min(V0.X, V1.X), Math.Min(V0.Y, V1.Y), Math.Min(V0.Z, V1.Z));
    }

    public Vertex MaxBounds()
    {
      return new Vertex(Math.Max(V0.X, V1.X), Math.Max(V0.Y, V1.Y), Math.Max(V0.Z, V1.Z));
    }

    public void Flatten()
    {
      V0.Flatten();
      V1.Flatten();
    }

    public Element Collapsed()
    {
      if (V0.Equals(V1))
      {
        return V0;
      }

      return this;
    }

    public void Reverse()
    {
      Vertex tmp = V0;
      V0 = V1;
      V1 = tmp;
    }

    public void Sort()
    {
      if (V0.CompareTo(V1) > 0)
      {
        Reverse();
      }
    }

    public override bool Equals(object obj)
    {
      Edge e = obj as Edge;

      if (e != null)
      {
        return V0.Equals(e.V0) && V1.Equals(e.V1);
      }

      return false;
    }

    public override int GetHashCode()
    {
      return
          V0.GetHashCode() ^
          V1.GetHashCode();
    }
  }
}
