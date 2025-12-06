using System.Diagnostics;
using Eto.Drawing;
using Eto.Forms;

namespace AdventOfCode.Tools.Visualization;
public static class FormStyle
{
    private static bool _styleRegistered = false;
    internal const string GridStyle = "AStarGrid";
    internal const string PanelStyle = "PanelStyle";
    internal const string PathStyle = "PathStyle";

    public static void LoadStyles(){
        if(_styleRegistered){
            Debug.WriteLine("WARN: Styles already registered");
            return;
        }
        Eto.Style.Add<TableLayout>(GridStyle, handler =>
        {
            handler.BackgroundColor = Color.FromArgb(0, 0, 0);
        });
        Eto.Style.Add<Layout>(PanelStyle, handler =>
        {
            handler.BackgroundColor = Color.FromArgb(255, 255, 255);
        });
        Eto.Style.Add<Layout>(PathStyle, handler =>
        {
            handler.BackgroundColor = Color.FromArgb(0, 255, 0);
        });

        _styleRegistered = true;
    }
}
