using System.Windows.Controls;
using System.Windows.Media;

public class ChartElementBase
    : Control
{
    #region Ctor
    public ChartElementBase()
    {
        CacheMode = new BitmapCache()
        {
            SnapsToDevicePixels = true,
        };
    }
    #endregion
}