using System;
using System.Collections.Generic;
using System.Text;

namespace SVG2SXF.SVG
{
  public class PathData
  {
    public class Line
    {
      public readonly double X0;
      public readonly double Y0;
      public readonly double X1;
      public readonly double Y1;

      public Line(double x0, double y0, double x1, double y1)
      {
        X0 = x0;
        Y0 = y0;
        X1 = x1;
        Y1 = y1;
      }
    }

    private enum CharacterType
    {
      Delimiter,
      TokenStart,
      TokenStartEnd,
      Token,
    }

    private enum CommandMode
    {
      None,
      LineTo,
      HLineTo,
      VLineTo,
    }

    private string d;
    private int i;

    private List<Line> lines = new List<Line>();
    public IReadOnlyCollection<Line> Lines;

    public PathData(string d)
    {
      this.d = d;
      i = 0;

      Lines = lines.AsReadOnly();

      loadData();
    }

    private void loadData()
    {
      CommandMode mode = CommandMode.None;
      bool relative = false;

      double subpathX = 0.0;
      double subpathY = 0.0;

      double commandX = 0.0;
      double commandY = 0.0;

      double currentX = 0.0;
      double currentY = 0.0;

      while (true)
      {
        string token = nextToken();

        if (token == null)
        {
          break;
        }

        double x = 0.0;
        double y = 0.0;

        switch (token)
        {
          case "M":
          case "m":
            mode = CommandMode.LineTo;
            relative = Char.IsLower(token[0]);
            token = null;
            commandX = currentX = subpathX = double.Parse(nextToken()) + (relative ? currentX : 0.0); ;
            commandY = currentY = subpathY = double.Parse(nextToken()) + (relative ? currentY : 0.0);
            continue;
          case "Z":
          case "z":
            token = null;
            x = subpathX;
            y = subpathY;
            break;
          case "L":
          case "l":
            mode = CommandMode.LineTo;
            relative = Char.IsLower(token[0]);
            token = null;
            commandX = x = double.Parse(nextToken()) + (relative ? currentX : 0.0);
            commandY = y = double.Parse(nextToken()) + (relative ? currentY : 0.0);
            break;
          case "H":
          case "h":
            mode = CommandMode.HLineTo;
            relative = Char.IsLower(token[0]);
            token = null;
            commandX = x = double.Parse(nextToken()) + (relative ? currentX : 0.0);
            commandY = y = currentY;
            break;
          case "V":
          case "v":
            mode = CommandMode.VLineTo;
            relative = Char.IsLower(token[0]);
            token = null;
            commandX = x = currentX;
            commandY = y = double.Parse(nextToken()) + (relative ? currentY : 0.0);
            break;
        }

        if (token != null)
        {
          switch (mode)
          {
            case CommandMode.LineTo:
              x = double.Parse(token) + (relative ? currentX : 0.0);
              y = double.Parse(nextToken()) + (relative ? currentY : 0.0);
              break;
            case CommandMode.HLineTo:
              x = double.Parse(token) + (relative ? currentX : 0.0);
              y = currentY;
              break;
            case CommandMode.VLineTo:
              x = currentX;
              y = double.Parse(token) + (relative ? currentY : 0.0);
              break;
            default:
              throw new Exception();
          }
        }

        lines.Add(new Line(currentX, currentY, x, y));

        currentX = x;
        currentY = y;
      }
    }

    private string nextToken()
    {
      StringBuilder sb = new StringBuilder();

      while (i < d.Length)
      {
        CharacterType charType;

        switch (d[i])
        {
          case ' ':
          case '\t':
          case '\n':
          case '\r':
          case ',':
            charType = CharacterType.Delimiter;
            break;
          case 'M':
          case 'm':
          case 'Z':
          case 'z':
          case 'L':
          case 'l':
          case 'H':
          case 'h':
          case 'V':
          case 'v':
            charType = CharacterType.TokenStartEnd;
            break;
          case '-':
            charType = CharacterType.TokenStart;
            break;
          case '0':
          case '1':
          case '2':
          case '3':
          case '4':
          case '5':
          case '6':
          case '7':
          case '8':
          case '9':
          case '.':
            charType = CharacterType.Token;
            break;
          default:
            throw new Exception();
        }

        switch (charType)
        {
          case CharacterType.Delimiter:
            i++;
            if (sb.Length > 0)
            {
              return sb.ToString();
            }
            break;
          case CharacterType.TokenStartEnd:
            if (sb.Length > 0)
            {
              return sb.ToString();
            }
            sb.Append(d[i++]);
            if (sb.Length > 0)
            {
              return sb.ToString();
            }
            break;
          case CharacterType.TokenStart:
            if (sb.Length > 0)
            {
              return sb.ToString();
            }
            sb.Append(d[i++]);
            break;
          case CharacterType.Token:
            sb.Append(d[i++]);
            break;
        }
      }

      if (sb.Length > 0)
      {
        return sb.ToString();
      }

      return null;
    }
  }
}
