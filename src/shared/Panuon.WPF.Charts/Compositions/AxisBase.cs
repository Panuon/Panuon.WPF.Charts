using Panuon.WPF.Charts.Controls.Internals;
using System.Windows;
using System.Windows.Media;

namespace Panuon.WPF.Charts
{
    public abstract class AxisBase
        : DependencyObject
    {
        #region RoutedEvent

        #endregion

        #region Properties

        #region Spacing
        public double Spacing
        {
            get { return (double)GetValue(SpacingProperty); }
            set { SetValue(SpacingProperty, value); }
        }

        public static readonly DependencyProperty SpacingProperty =
            AxisPresenterBase.SpacingProperty.AddOwner(typeof(AxisBase), new FrameworkPropertyMetadata(5d,
                FrameworkPropertyMetadataOptions.Inherits));
        #endregion

        #region LabelStyle
        public Style LabelStyle
        {
            get { return (Style)GetValue(LabelStyleProperty); }
            set { SetValue(LabelStyleProperty, value); }
        }

        public static readonly DependencyProperty LabelStyleProperty =
            DependencyProperty.Register("LabelStyle", typeof(Style), typeof(AxisBase), new PropertyMetadata(null));
        #endregion

        #region Foreground
        public Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        public static readonly DependencyProperty ForegroundProperty =
            AxisPresenterBase.ForegroundProperty.AddOwner(typeof(AxisBase), new FrameworkPropertyMetadata(Brushes.Black,
                FrameworkPropertyMetadataOptions.Inherits));
        #endregion

        #region StrokeThickness
        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty StrokeThicknessProperty =
            AxisPresenterBase.StrokeThicknessProperty.AddOwner(typeof(AxisBase), new FrameworkPropertyMetadata(1d,
                FrameworkPropertyMetadataOptions.Inherits));
        #endregion

        #region Stroke
        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        public static readonly DependencyProperty StrokeProperty =
            AxisPresenterBase.StrokeProperty.AddOwner(typeof(AxisBase), new FrameworkPropertyMetadata(Brushes.Black,
                FrameworkPropertyMetadataOptions.Inherits));
        #endregion

        #region FontFamily
        public FontFamily FontFamily
        {
            get { return (FontFamily)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        public static readonly DependencyProperty FontFamilyProperty =
            AxisPresenterBase.FontFamilyProperty.AddOwner(typeof(AxisBase), new FrameworkPropertyMetadata(SystemFonts.MessageFontFamily,
                FrameworkPropertyMetadataOptions.Inherits));
        #endregion

        #region FontStyle
        public FontStyle FontStyle
        {
            get { return (FontStyle)GetValue(FontStyleProperty); }
            set { SetValue(FontStyleProperty, value); }
        }

        public static readonly DependencyProperty FontStyleProperty =
            AxisPresenterBase.FontStyleProperty.AddOwner(typeof(AxisBase), new FrameworkPropertyMetadata(SystemFonts.MessageFontStyle,
                FrameworkPropertyMetadataOptions.Inherits));
        #endregion

        #region FontWeight
        public FontWeight FontWeight
        {
            get { return (FontWeight)GetValue(FontWeightProperty); }
            set { SetValue(FontWeightProperty, value); }
        }

        public static readonly DependencyProperty FontWeightProperty =
            AxisPresenterBase.FontWeightProperty.AddOwner(typeof(AxisBase), new FrameworkPropertyMetadata(SystemFonts.MessageFontWeight,
                FrameworkPropertyMetadataOptions.Inherits));
        #endregion

        #region FontStretch
        public FontStretch FontStretch
        {
            get { return (FontStretch)GetValue(FontStretchProperty); }
            set { SetValue(FontStretchProperty, value); }
        }

        public static readonly DependencyProperty FontStretchProperty =
            AxisPresenterBase.FontStretchProperty.AddOwner(typeof(AxisBase), new FrameworkPropertyMetadata(FontStretches.Normal,
                FrameworkPropertyMetadataOptions.Inherits));
        #endregion

        #region FontSize
        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        public static readonly DependencyProperty FontSizeProperty =
            AxisPresenterBase.FontSizeProperty.AddOwner(typeof(AxisBase), new FrameworkPropertyMetadata(SystemFonts.MessageFontSize,
                FrameworkPropertyMetadataOptions.Inherits));
        #endregion

        #region TicksSize
        public double TicksSize
        {
            get { return (double)GetValue(TicksSizeProperty); }
            set { SetValue(TicksSizeProperty, value); }
        }

        public static readonly DependencyProperty TicksSizeProperty =
            AxisPresenterBase.TicksSizeProperty.AddOwner(typeof(AxisBase), new FrameworkPropertyMetadata(3d,
                FrameworkPropertyMetadataOptions.Inherits));
        #endregion

        #region TicksBrush
        public Brush TicksBrush
        {
            get { return (Brush)GetValue(TicksBrushProperty); }
            set { SetValue(TicksBrushProperty, value); }
        }

        public static readonly DependencyProperty TicksBrushProperty =
            AxisPresenterBase.TicksBrushProperty.AddOwner(typeof(AxisBase), new FrameworkPropertyMetadata(Brushes.Black,
                FrameworkPropertyMetadataOptions.Inherits));
        #endregion

        #endregion

        #region Event Handlers
        #endregion

        #region Functions

        #endregion

    }
}
