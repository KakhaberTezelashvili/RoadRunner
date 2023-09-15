using System.Text;

namespace TDOC.WebComponents.Utilities;

public class DomUtilities
{
    public static string ShowHideCssClass(bool display) => display ? "d-flex" : "d-none";

    public static string CalcAvailableHeight(int height, string viewportUnits = "vh") => $"calc(100{viewportUnits} - {height}px)";

    public static string CalcAvailableWidth(int width, string viewportUnits = "vh") => $"calc(100{viewportUnits} - {width}px)";
   
    public static string CalcSumOfDimensions(int[] values)
    {
        var expressionBuilder = new StringBuilder("calc(");
        foreach (int value in values)
        {
            expressionBuilder.Append($"{value}px + ");
        }
        expressionBuilder.Remove(expressionBuilder.Length - 2, 2);
        expressionBuilder.Append(")");
        return expressionBuilder.ToString();
    }
}