using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Panuon.WPF.Charts.Controls.Internals
{
    internal class SeriesPresenter
        : FrameworkElement
    {
        #region Fields
        private SeriesPanel _seriesPanel;

        private ChartPanel _chartPanel;

        private bool _isAnimationCompleted;
        #endregion

        #region Ctor
        public SeriesPresenter(
            SeriesPanel seriesPanel,
            ChartPanel chartPanel
        )
        {
            _seriesPanel = seriesPanel;
            _chartPanel = chartPanel;
        }
        #endregion

        #region Properties
        public SeriesBase Series
        {
            get => _series;
            set
            {
                if (_series != null)
                {
                    _series.InternalInvalidRender -= Series_InternalInvalidRender;
                }
                if (value != null)
                {
                    value.InternalInvalidRender -= Series_InternalInvalidRender;
                    value.InternalInvalidRender += Series_InternalInvalidRender;
                }
                _series = value;
            }
        }
        private SeriesBase _series;
        #endregion

        #region Internal Properties

        #region AnimationPercent
        internal double? AnimationPercent
        {
            get { return (double?)GetValue(AnimationPercentProperty); }
            set { SetValue(AnimationPercentProperty, value); }
        }

        internal static readonly DependencyProperty AnimationPercentProperty =
            DependencyProperty.Register("AnimationPercent", typeof(double?), typeof(SeriesPresenter), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender, OnAnimationPercentChanged));
        #endregion

        #endregion

        #region Event Handlers
        private static void OnAnimationPercentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var presenter = (SeriesPresenter)d;
            presenter.InvalidateVisual();
        }
        #endregion

        #region Overrides
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            AnimationPercent = 0;
            if (_chartPanel.AnimationDuration is TimeSpan duration
                && duration.TotalMilliseconds > 0)
            {
                var animation = new DoubleAnimation(0, 1, duration)
                {
                    EasingFunction = CreateEasingFunction(_chartPanel.AnimationEasing)
                };
                animation.Completed += delegate
                {
                    AnimationPercent = 1;
                    _isAnimationCompleted = true;
                };
                BeginAnimation(AnimationPercentProperty, animation);
            }
            else
            {
                AnimationPercent = 1;
                _isAnimationCompleted = true;
            }
        }

        protected override void OnRender(DrawingContext context)
        {
            if (Series == null
                || _chartPanel.Coordinates == null
                || !_chartPanel.IsCanvasReady()
                || AnimationPercent == null)
            {
                return;
            }

            var drawingContext = _chartPanel.CreateDrawingContext(context);
            var chartContext = _chartPanel.GetCanvasContext();

            if(AnimationPercent == 0
                || _isAnimationCompleted)
            {
                Series.BeginRender(drawingContext, chartContext);
            }
            Series.Render(
                drawingContext: drawingContext,
                chartContext: chartContext,
                animationProgress: (double)AnimationPercent
            );
            if (AnimationPercent == 1)
            {
                Series.CompleteRender(drawingContext, chartContext);
            }
        }
        #endregion

        #region Event Handlers
        private void Series_InternalInvalidRender()
        {
            InvalidateVisual();
        }
        #endregion

        #region Functions

        #region CreateEasingFunction
        private static IEasingFunction CreateEasingFunction(AnimationEasing? animationEasing)
        {
            if (animationEasing == null)
            {
                return null;
            }
            switch (animationEasing)
            {
                case AnimationEasing.BackIn:
                    return new BackEase() { EasingMode = EasingMode.EaseIn };
                case AnimationEasing.BackOut:
                    return new BackEase() { EasingMode = EasingMode.EaseOut };
                case AnimationEasing.BackInOut:
                    return new BackEase() { EasingMode = EasingMode.EaseInOut };
                case AnimationEasing.BounceIn:
                    return new BounceEase() { EasingMode = EasingMode.EaseIn };
                case AnimationEasing.BounceOut:
                    return new BounceEase() { EasingMode = EasingMode.EaseOut };
                case AnimationEasing.BounceInOut:
                    return new BounceEase() { EasingMode = EasingMode.EaseInOut };
                case AnimationEasing.CircleIn:
                    return new CircleEase() { EasingMode = EasingMode.EaseIn };
                case AnimationEasing.CircleOut:
                    return new CircleEase() { EasingMode = EasingMode.EaseOut };
                case AnimationEasing.CircleInOut:
                    return new CircleEase() { EasingMode = EasingMode.EaseInOut };
                case AnimationEasing.CubicIn:
                    return new CubicEase() { EasingMode = EasingMode.EaseIn };
                case AnimationEasing.CubicOut:
                    return new CubicEase() { EasingMode = EasingMode.EaseOut };
                case AnimationEasing.CubicInOut:
                    return new CubicEase() { EasingMode = EasingMode.EaseInOut };
                case AnimationEasing.ElasticIn:
                    return new ElasticEase() { EasingMode = EasingMode.EaseIn };
                case AnimationEasing.ElasticOut:
                    return new ElasticEase() { EasingMode = EasingMode.EaseOut };
                case AnimationEasing.ElasticInOut:
                    return new ElasticEase() { EasingMode = EasingMode.EaseInOut };
                case AnimationEasing.ExponentialIn:
                    return new ExponentialEase() { EasingMode = EasingMode.EaseIn };
                case AnimationEasing.ExponentialOut:
                    return new ExponentialEase() { EasingMode = EasingMode.EaseOut };
                case AnimationEasing.ExponentialInOut:
                    return new ExponentialEase() { EasingMode = EasingMode.EaseInOut };
                case AnimationEasing.PowerIn:
                    return new PowerEase() { EasingMode = EasingMode.EaseIn };
                case AnimationEasing.PowerOut:
                    return new PowerEase() { EasingMode = EasingMode.EaseOut };
                case AnimationEasing.PowerInOut:
                    return new PowerEase() { EasingMode = EasingMode.EaseInOut };
                case AnimationEasing.QuadraticIn:
                    return new QuadraticEase() { EasingMode = EasingMode.EaseIn };
                case AnimationEasing.QuadraticOut:
                    return new QuadraticEase() { EasingMode = EasingMode.EaseOut };
                case AnimationEasing.QuadraticInOut:
                    return new QuadraticEase() { EasingMode = EasingMode.EaseInOut };
                case AnimationEasing.QuarticIn:
                    return new QuarticEase() { EasingMode = EasingMode.EaseIn };
                case AnimationEasing.QuarticOut:
                    return new QuarticEase() { EasingMode = EasingMode.EaseOut };
                case AnimationEasing.QuarticInOut:
                    return new QuarticEase() { EasingMode = EasingMode.EaseInOut };
                case AnimationEasing.QuinticIn:
                    return new QuarticEase() { EasingMode = EasingMode.EaseIn };
                case AnimationEasing.QuinticOut:
                    return new QuarticEase() { EasingMode = EasingMode.EaseOut };
                case AnimationEasing.QuinticInOut:
                    return new QuarticEase() { EasingMode = EasingMode.EaseInOut };
                case AnimationEasing.SineIn:
                    return new SineEase() { EasingMode = EasingMode.EaseIn };
                case AnimationEasing.SineOut:
                    return new SineEase() { EasingMode = EasingMode.EaseOut };
                case AnimationEasing.SineInOut:
                    return new SineEase() { EasingMode = EasingMode.EaseInOut };
            }
            return null;
        }
        #endregion
        #endregion
    }
}
