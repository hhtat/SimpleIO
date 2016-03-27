using SXF;
using System.IO;

namespace SXFOptimize
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

      document.Optimize();

      using (FileStream stream = File.Open(args[1], FileMode.Create, FileAccess.Write))
      {
        IO.Save(document, stream);
      }
    }
  }
}
