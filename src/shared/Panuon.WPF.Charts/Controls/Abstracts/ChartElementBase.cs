using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

public class ChartElementBase
    : Control
{
    #region Ctor
    public ChartElementBase(double renderAtScale = 1)
    {
        CacheMode = new BitmapCache(renderAtScale)
        {
            SnapsToDevicePixels = true,
        };
    }
    #endregion

    #region Properties
    /// <summary>
    /// Render quality (0-100). Default is 100.
    /// </summary>
    public double RenderQuality
    {
        get { return (double)GetValue(RenderQualityProperty); }
        set { SetValue(RenderQualityProperty, value); }
    }

    public static readonly DependencyProperty RenderQualityProperty =
        DependencyProperty.Register("RenderQuality", typeof(double), typeof(ChartElementBase), new PropertyMetadata(100d, OnRenderQualityChanged));
    #endregion

    #region Event Handlers
    private static void OnRenderQualityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var element = (ChartElementBase)d;
        var quality = Math.Max(0, Math.Min(100, (double)e.NewValue));
        element.CacheMode = new BitmapCache(quality / 100d);
    }
    #endregion

}