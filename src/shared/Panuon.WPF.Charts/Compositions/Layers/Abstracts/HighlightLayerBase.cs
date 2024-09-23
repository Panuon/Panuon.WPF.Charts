using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Animation;

namespace Panuon.WPF.Charts
{
    public abstract class HighlightLayerBase
        : LayerBase
    {
        #region Fields
        private static readonly Dictionary<Type, Dictionary<Type, SeriesHighlightHandler>> _highlightHandlers
            = new Dictionary<Type, Dictionary<Type, SeriesHighlightHandler>>();

        private readonly Dictionary<SeriesBase, Dictionary<int, AnimationProgressObject>> _highlightProgressObjects =
            new Dictionary<SeriesBase, Dictionary<int, AnimationProgressObject>>();

        private readonly Dictionary<SeriesBase, int> _lastCoordinates =
            new Dictionary<SeriesBase, int>();
        #endregion

        #region Ctor
        public HighlightLayerBase()
        {
        }
        #endregion

        #region Properties

        #region AnimationDuration
        public TimeSpan? AnimationDuration
        {
            get { return (TimeSpan?)GetValue(AnimationDurationProperty); }
            set { SetValue(AnimationDurationProperty, value); }
        }

        public static readonly DependencyProperty AnimationDurationProperty =
            DependencyProperty.Register("AnimationDuration", typeof(TimeSpan?), typeof(HighlightLayerBase));
        #endregion

        #region AnimationEasing
        public AnimationEasing AnimationEasing
        {
            get { return (AnimationEasing)GetValue(AnimationEasingProperty); }
            set { SetValue(AnimationEasingProperty, value); }
        }

        public static readonly DependencyProperty AnimationEasingProperty =
            DependencyProperty.Register("AnimationEasing", typeof(AnimationEasing), typeof(HighlightLayerBase));
        #endregion

        #endregion

        #region Overrides
        protected override void OnMouseIn(
            IChartContext chartContext,
            ILayerContext layerContext
        )
        {
            var currentCoordinates = new List<int>();
            foreach (var series in chartContext.Series)
            {
                if (!_highlightProgressObjects.ContainsKey(series))
                {
                    _highlightProgressObjects.Add(series, new Dictionary<int, AnimationProgressObject>());
                }
                var highlightProgressObjects = _highlightProgressObjects[series];

                ICoordinate coordinate = null;
                if (layerContext.GetMousePosition() is Point position)
                {
                    coordinate = series.RetrieveCoordinate(
                        chartContext,
                        layerContext,
                        position
                    );
                    if (coordinate != null)
                    {
                        currentCoordinates.Add(coordinate.Index);

                        if (_lastCoordinates.ContainsKey(series)
                            && _lastCoordinates[series] == coordinate.Index)
                        {
                            continue;
                        }
                        _lastCoordinates[series] = coordinate.Index;

                        if (!highlightProgressObjects.ContainsKey(coordinate.Index))
                        {
                            highlightProgressObjects[coordinate.Index] = new AnimationProgressObject();
                        }
                        var @object = highlightProgressObjects[coordinate.Index];
                        @object.ProgressChanged -= AnimationProgressObject_ProgressChanged;
                        @object.ProgressChanged += AnimationProgressObject_ProgressChanged;

                        if (AnimationDuration is TimeSpan duration
                            && duration.TotalMilliseconds > 0)
                        {
                            var animation = new DoubleAnimation()
                            {
                                To = 1,
                                Duration = duration,
                                EasingFunction = AnimationUtil.CreateEasingFunction(AnimationEasing)
                            };

                            @object.BeginAnimation(AnimationProgressObject.ProgressProperty, animation);
                        }
                        else
                        {
                            @object.BeginAnimation(AnimationProgressObject.ProgressProperty, null);
                            @object.Progress = 1;
                        }

                    }
                }
            }

            foreach (var seriesObjects in _highlightProgressObjects)
            {
                var series = seriesObjects.Key;
                for (var i = seriesObjects.Value.Count - 1; i >= 0; i--)
                {
                    var coordinateObject = seriesObjects.Value.ElementAt(i);

                    if (!currentCoordinates.Contains(coordinateObject.Key))
                    {
                        var oldObject = coordinateObject.Value;
                        if (AnimationDuration is TimeSpan duration
                            && duration.TotalMilliseconds > 0)
                        {
                            var animation = new DoubleAnimation()
                            {
                                To = 0,
                                Duration = duration,
                                EasingFunction = AnimationUtil.CreateEasingFunction(AnimationEasing)
                            };
                            oldObject.BeginAnimation(AnimationProgressObject.ProgressProperty, animation);
                        }
                        else
                        {
                            oldObject.BeginAnimation(AnimationProgressObject.ProgressProperty, null);
                            oldObject.Progress = 0;
                        }
                    }
                    else
                    {

                    }
                }
            }
        }

        protected override void OnMouseOut(IChartContext chartContext,
            ILayerContext layerContext)
        {
            foreach (var seriesAnimationObjects in _highlightProgressObjects)
            {
                var series = seriesAnimationObjects.Key;
                var progressObjects = seriesAnimationObjects.Value;
                for (var i = progressObjects.Count - 1; i >= 0; i--)
                {
                    var coordinateObject = progressObjects.ElementAt(i);
                    var oldObject = coordinateObject.Value;
                    if (AnimationDuration is TimeSpan duration
                        && duration.TotalMilliseconds > 0)
                    {
                        var animation = new DoubleAnimation()
                        {
                            To = 0,
                            Duration = duration,
                            EasingFunction = AnimationUtil.CreateEasingFunction(AnimationEasing)
                        };
                        oldObject.BeginAnimation(AnimationProgressObject.ProgressProperty, animation);
                    }
                    else
                    {
                        oldObject.BeginAnimation(AnimationProgressObject.ProgressProperty, null);
                        oldObject.Progress = 0;
                    }
                }
            }

            _lastCoordinates.Clear();
        }


        protected override void OnRender(
            IDrawingContext drawingContext,
            IChartContext chartContext,
            ILayerContext layerContext
        )
        {
            foreach (var seriesAnimationObjects in GetHighlightProgress())
            {
                var series = seriesAnimationObjects.Key;

                var layerType = GetType();
                if (_highlightHandlers.ContainsKey(layerType))
                {
                    if (_highlightHandlers[GetType()].ContainsKey(series.GetType()))
                    {
                        var handler = _highlightHandlers[GetType()][series.GetType()];

                        handler.Invoke(
                            layer: this,
                            series: series,
                            drawingContext,
                            chartContext,
                            layerContext,
                            seriesAnimationObjects.Value
                        );
                    }
                }
            }
        }
        #endregion

        #region Methods
        protected Dictionary<SeriesBase, Dictionary<int, double>> GetHighlightProgress()
        {
            return _highlightProgressObjects.ToDictionary(
                sd => sd.Key,
                sd => sd.Value.ToDictionary(
                        ip => ip.Key,
                        ip => ip.Value.Progress
                    )
            );
        }

        protected static void RegistHighlightHandler<TLayer, TSeries, TChartContext>(SeriesHighlightHandler<TLayer, TSeries, TChartContext> handler)
            where TLayer : HighlightLayerBase
            where TSeries : SeriesBase
            where TChartContext : IChartContext
        {
            if (!_highlightHandlers.ContainsKey(typeof(TLayer)))
            {
                _highlightHandlers.Add(typeof(TLayer), new Dictionary<Type, SeriesHighlightHandler>());
            }
            var highlightHandlers = _highlightHandlers[typeof(TLayer)];

            var newHandler = new SeriesHighlightHandler((layer, series, drawingContext, chartContext, layerContext, coordinatesProgress) =>
            {
                handler.Invoke((TLayer)layer, (TSeries)series, drawingContext, (TChartContext)chartContext, layerContext, coordinatesProgress);
            });
            highlightHandlers.Add(typeof(TSeries), newHandler);
        }
        #endregion

        #region Event Handlers
        private void AnimationProgressObject_ProgressChanged(object sender, EventArgs e)
        {
            InvalidateVisual();
        }
        #endregion
    }
}