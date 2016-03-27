using System;

namespace SXF
{
  public class Vertex : Element, IComparable<Vertex>
  {
    public double X { get; private set; }
    public double Y { get; private set; }
    public double Z { get; private set; }

    public Vertex()
      : this(0.0, 0.0, 0.0)
    {
    }

    public Vertex(Vertex v)
      : this(v.X, v.Y, v.Z)
    {
    }

    public Vertex(double x, double y, double z)
    {
      X = x;
      Y = y;
      Z = z;
    }

    public double Distance(Vertex vertex)
    {
      double dX = vertex.X - X;
      double dY = vertex.Y - Y;
      double dZ = vertex.Z - Z;

      return Math.Sqrt(dX * dX + dY * dY + dZ * dZ);
    }

    public Vertex Start()
    {
      return this;
    }

    public Vertex End()
    {
      return this;
    }

    public Vertex MinBounds()
    {
      return this;
    }

    public Vertex MaxBounds()
    {
      return this;
    }

    public void Flatten()
    {
      Z = 0.0;
    }

    public Element Collapsed()
    {
      return this;
    }

    public void Reverse()
    {
    }

    public void Sort()
    {
    }

    public override bool Equals(object obj)
    {
      Vertex v = obj as Vertex;

      if (v != null)
      {
        return this.CompareTo(v) == 0;
      }

      return false;
    }

    public override int GetHashCode()
    {
      return
          X.GetHashCode() ^
          Y.GetHashCode() ^
          Z.GetHashCode();
    }

    public int CompareTo(Vertex other)
    {
      if (Z > other.Z) { return 1; }
      if (Z < other.Z) { return -1; }

      if (Y > other.Y) { return 1; }
      if (Y < other.Y) { return -1; }

      if (X > other.X) { return 1; }
      if (X < other.X) { return -1; }

      return 0;
    }
  }
}
