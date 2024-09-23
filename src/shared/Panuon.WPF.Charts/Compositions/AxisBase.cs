using Panuon.WPF.Charts.Controls.Internals;
using Panuon.WPF.Charts.Implements;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Panuon.WPF.Charts
{
    public abstract class AxisBase
        : FrameworkElement
    {
        #region Fields
        protected CartesianChart _chart;

        #endregion

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
            DependencyProperty.Register("Spacing", typeof(double), typeof(AxisBase), new PropertyMetadata(5d));
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
            DependencyProperty.Register("Foreground", typeof(Brush), typeof(AxisBase), new PropertyMetadata(Brushes.Black));
        #endregion

        #region StrokeThickness
        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(AxisBase), new PropertyMetadata(1d));
        #endregion

        #region Stroke
        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(AxisBase), new PropertyMetadata(Brushes.Black));
        #endregion

        #region FontFamily
        public FontFamily FontFamily
        {
            get { return (FontFamily)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        public static readonly DependencyProperty FontFamilyProperty =
            DependencyProperty.Register("FontFamily", typeof(FontFamily), typeof(AxisBase), new PropertyMetadata(SystemFonts.MessageFontFamily));
        #endregion

        #region FontStyle
        public FontStyle FontStyle
        {
            get { return (FontStyle)GetValue(FontStyleProperty); }
            set { SetValue(FontStyleProperty, value); }
        }

        public static readonly DependencyProperty FontStyleProperty =
            DependencyProperty.Register("FontStyle", typeof(FontStyle), typeof(AxisBase), new PropertyMetadata(SystemFonts.MessageFontStyle));
        #endregion

        #region FontWeight
        public FontWeight FontWeight
        {
            get { return (FontWeight)GetValue(FontWeightProperty); }
            set { SetValue(FontWeightProperty, value); }
        }

        public static readonly DependencyProperty FontWeightProperty =
            DependencyProperty.Register("FontWeight", typeof(FontWeight), typeof(AxisBase), new PropertyMetadata(SystemFonts.MessageFontWeight));
        #endregion

        #region FontStretch
        public FontStretch FontStretch
        {
            get { return (FontStretch)GetValue(FontStretchProperty); }
            set { SetValue(FontStretchProperty, value); }
        }

        public static readonly DependencyProperty FontStretchProperty =
            DependencyProperty.Register("FontStretch", typeof(FontStretch), typeof(AxisBase), new PropertyMetadata(FontStretches.Normal));
        #endregion

        #region FontSize
        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        public static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register("FontSize", typeof(double), typeof(AxisBase), new PropertyMetadata(SystemFonts.MessageFontSize));
        #endregion

        #region TicksSize
        public double TicksSize
        {
            get { return (double)GetValue(TicksSizeProperty); }
            set { SetValue(TicksSizeProperty, value); }
        }

        public static readonly DependencyProperty TicksSizeProperty =
            DependencyProperty.Register("TicksSize", typeof(double), typeof(AxisBase), new PropertyMetadata(3d));
        #endregion

        #region TicksBrush
        public Brush TicksBrush
        {
            get { return (Brush)GetValue(TicksBrushProperty); }
            set { SetValue(TicksBrushProperty, value); }
        }

        public static readonly DependencyProperty TicksBrushProperty =
            DependencyProperty.Register("TicksBrush", typeof(Brush), typeof(AxisBase), new PropertyMetadata(Brushes.Black));
        #endregion

        #endregion

        #region Internal Methods
        internal void OnAttached(CartesianChart chart)
        {
            _chart = chart;
        }
        #endregion

        #region Event Handlers
        #endregion

        #region Functions

        #endregion

    }
}
