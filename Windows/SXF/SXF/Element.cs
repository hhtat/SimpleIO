namespace SXF
{
  public interface Element
  {
    Vertex Start();

    Vertex End();

    Vertex MinBounds();

    Vertex MaxBounds();

    void Flatten();

    Element Collapsed();

    void Reverse();

    void Sort();
  }
}
