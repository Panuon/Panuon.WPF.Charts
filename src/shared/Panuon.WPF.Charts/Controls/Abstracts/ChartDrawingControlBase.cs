using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Panuon.WPF.Charts
{
    public abstract class ChartDrawingControlBase
        : Control
    {
        #region Ctor
        public ChartDrawingControlBase()
        {
            //CacheMode = new BitmapCache()
            //{
            //    SnapsToDevicePixels = true,
            //};

        }
        #endregion

        #region Protected Methods
        protected FormattedText CreateFormattedText(
            string text,
            Brush foreground = null,
            int? maxLineCount = null,
            double? maxTextWidth = null
        )
        {
            var formattedText = new FormattedText(text,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(FontFamily, FontStyle, FontWeight, FontStretch),
                FontSize,
                foreground ?? Foreground
#if NET452
#else
                , VisualTreeHelper.GetDpi(this).PixelsPerDip
#endif
            )
            {
                Trimming = TextTrimming.CharacterEllipsis
            };
            if (maxLineCount != null)
            {
                formattedText.MaxLineCount = (int)maxLineCount;
            }
            if (maxTextWidth != null)
            {
                formattedText.MaxTextWidth = (double)maxTextWidth;
            }
            return formattedText;
        }
        #endregion
    }
}