using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Panuon.WPF.Charts
{
    public abstract class SeriesBase
        : ChartDrawingControlBase, IChartArgument
    {
        #region Fields
        private ChartBase _chart;

        internal bool _isAnimationBeginCalled = false;

        private AnimationProgressObject _loadAnimationProgressObject;

        private bool _isRenderEverCalled;
        #endregion

        #region Ctor
        public SeriesBase()
        {
            ClipToBounds = true;
            Loaded += SeriesBase_Loaded;
        }
        #endregion

        #region Properties

        #region ShowValueLabels
        public bool ShowValueLabels
        {
            get { return (bool)GetValue(ShowValueLabelsProperty); }
            set { SetValue(ShowValueLabelsProperty, value); }
        }

        public static readonly DependencyProperty ShowValueLabelsProperty =
            DependencyProperty.Register("ShowValueLabels", typeof(bool), typeof(SeriesBase), new PropertyMetadata(false));
        #endregion

        #region InvertForeground
        public Brush InvertForeground
        {
            get { return (Brush)GetValue(InvertForegroundProperty); }
            set { SetValue(InvertForegroundProperty, value); }
        }

        public static readonly DependencyProperty InvertForegroundProperty =
            DependencyProperty.Register("InvertForeground", typeof(Brush), typeof(SeriesBase), new FrameworkPropertyMetadata(Brushes.White, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region ValueLabelStroke
        public Brush ValueLabelStroke
        {
            get { return (Brush)GetValue(ValueLabelStrokeProperty); }
            set { SetValue(ValueLabelStrokeProperty, value); }
        }

        public static readonly DependencyProperty ValueLabelStrokeProperty =
            DependencyProperty.Register("ValueLabelStroke", typeof(Brush), typeof(SeriesBase), new FrameworkPropertyMetadata(Brushes.White, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region ValueLabelStrokeThickness
        public double ValueLabelStrokeThickness
        {
            get { return (double)GetValue(ValueLabelStrokeThicknessProperty); }
            set { SetValue(ValueLabelStrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty ValueLabelStrokeThicknessProperty =
            DependencyProperty.Register("ValueLabelStrokeThickness", typeof(double), typeof(SeriesBase), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region ValueLabelPlacement
        public SeriesLabelPlacement ValueLabelPlacement
        {
            get { return (SeriesLabelPlacement)GetValue(ValueLabelPlacementProperty); }
            set { SetValue(ValueLabelPlacementProperty, value); }
        }

        public static readonly DependencyProperty ValueLabelPlacementProperty =
            DependencyProperty.Register("ValueLabelPlacement", typeof(SeriesLabelPlacement), typeof(SeriesBase), new FrameworkPropertyMetadata(SeriesLabelPlacement.Above, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #endregion

        #region Internal Properties

        #region Offset
        internal double Offset
        {
            get { return (double)GetValue(OffsetProperty); }
            set { SetValue(OffsetProperty, value); }
        }

        internal static readonly DependencyProperty OffsetProperty =
            DependencyProperty.Register("Offset", typeof(double), typeof(SeriesBase), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #endregion

        #region Methods
        protected FormattedText CreateFormattedText(
            string text,
            Brush foreground
        )
        {
            return new FormattedText(
                text,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(FontFamily, FontStyle, FontWeight, FontStretch),
                FontSize,
                foreground
#if NET452 || NET462 || NET472 || NET48
#else
                , VisualTreeHelper.GetDpi(this).PixelsPerDip
#endif
            )
            {
                TextAlignment = TextAlignment.Center
            };
        }

        public IEnumerable<SeriesLegendEntry> RetrieveLegendEntries()
        {
            return OnInternalRetrieveLegendEntries();
        }

        public void InvalidateLayout()
        {
            if (IsLoaded
                && (_loadAnimationProgressObject == null || _chart.AnimationMode == AnimationTriggerMode.Always))
            {
                _isAnimationBeginCalled = false;
                BeginLoadAnimation();
                return;
            }

            if (_loadAnimationProgressObject == null
                || !_isRenderEverCalled)
            {
                return;
            }

            _isAnimationBeginCalled = false;
            _loadAnimationProgressObject.BeginAnimation(AnimationProgressObject.ProgressProperty, null);
            _loadAnimationProgressObject.Progress = 1;
        }
        #endregion

        #region Internal Methods
        internal void OnAttached(ChartBase chart)
        {
            _chart = chart;
        }
        #endregion

        #region Overrides
        protected override void OnRender(DrawingContext context)
        {
            if (_chart == null
                || !_chart.IsCanvasReady()
                || _chart.ItemsSource == null)
            {
                return;
            }

            if (_chart.AnimationDuration is TimeSpan duration
                && duration.TotalMilliseconds > 0
                && _loadAnimationProgressObject == null)
            {
                return;
            }

            var progress = _loadAnimationProgressObject?.Progress ?? 1;

            _isRenderEverCalled = true;
            var drawingContext = _chart.CreateDrawingContext(context);
            if (_chart is CartesianChart cartesianChart
                && cartesianChart.CanvasWidth > cartesianChart.SliceWidth)
            {
                if (!cartesianChart.SwapXYAxes)
                {
                    drawingContext.PushTranslate(Offset, 0);
                }
                else
                {
                    drawingContext.PushTranslate(0, Offset);
                }
            }

            var canvasContext = _chart.GetCanvasContext();
            if (!_isAnimationBeginCalled
                || progress == 1)
            {
                OnInternalRenderBegin(
                    drawingContext,
                    canvasContext
                );
                _isAnimationBeginCalled = true;
            }

            OnInternalRendering(
                drawingContext: drawingContext,
                chartContext: canvasContext,
                animationProgress: Math.Max(0, Math.Min(1, (double)progress))
            );

            if (progress == 1)
            {
                OnInternalRenderCompleted(
                    drawingContext: drawingContext,
                    chartContext: canvasContext
                );
            }
        }
        #endregion

        #region Abstract Methods
        internal protected virtual void OnInternalRenderBegin(
            IDrawingContext drawingContext,
            IChartContext chartContext
        )
        {
        }

        /// <summary>
        /// Call this method during rendering chart.
        /// </summary>
        /// <param name="drawingContext">DrawingContext.</param>
        /// <param name="chartContext">ChartContext</param>
        /// <param name="animationProgress">Animation progress. From 0 to 1.</param>
        internal protected abstract void OnInternalRendering(
            IDrawingContext drawingContext,
            IChartContext chartContext,
            double animationProgress
        );

        internal protected virtual void OnInternalRenderCompleted(
            IDrawingContext drawingContext,
            IChartContext chartContext
        )
        {
        }

        internal protected abstract IEnumerable<SeriesLegendEntry> OnInternalRetrieveLegendEntries();
        #endregion

        #region Event Handlers
        private void SeriesBase_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= SeriesBase_Loaded;

            if (_chart.ItemsSource != null
                && _chart.AnimationDuration is TimeSpan duration
                && duration.TotalMilliseconds > 0
                && _loadAnimationProgressObject == null)
            {
                BeginLoadAnimation();
            }
        }

        private static void OnAnimationPercentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var series = (SeriesBase)d;
            series.InvalidateVisual();
        }

        private void BeginLoadAnimation()
        {
            _loadAnimationProgressObject = new AnimationProgressObject();
            _loadAnimationProgressObject.ProgressChanged += LoadAnimationProgressObject_ProgressChanged;

            _loadAnimationProgressObject.Progress = 0;
            if (_chart.AnimationDuration is TimeSpan duration
                && duration.TotalMilliseconds > 0)
            {
                var animation = new DoubleAnimation(0, 1, duration)
                {
                    EasingFunction = _chart.AnimationEasing.ToEasingFunction()
                };
                animation.Completed += delegate
                {
                    _loadAnimationProgressObject.Progress = 1;
                };

                _loadAnimationProgressObject.BeginAnimation(AnimationProgressObject.ProgressProperty, animation);
            }
            else
            {
                _loadAnimationProgressObject.Progress = 1;
            }
        }

        private void LoadAnimationProgressObject_ProgressChanged(object sender, EventArgs e)
        {
            base.InvalidateVisual();
        }
        #endregion

    }

}