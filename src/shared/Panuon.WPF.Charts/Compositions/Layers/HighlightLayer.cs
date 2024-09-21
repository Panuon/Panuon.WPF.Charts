using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Animation;

namespace Panuon.WPF.Charts
{
    public class HighlightLayer
        : LayerBase
    {
        #region Fields
        private readonly Dictionary<SeriesBase, Dictionary<int, AnimationProgressObject>> _highlightProgressObjects =
            new Dictionary<SeriesBase, Dictionary<int, AnimationProgressObject>>();

        private readonly Dictionary<SeriesBase, int> _lastCoordinates = 
            new Dictionary<SeriesBase, int>();
        #endregion

        #region Ctor
        public HighlightLayer()
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
            DependencyProperty.Register("AnimationDuration", typeof(TimeSpan?), typeof(HighlightLayer));
        #endregion

        #region AnimationDuration
        public AnimationEasing AnimationEasing
        {
            get { return (AnimationEasing)GetValue(AnimationEasingProperty); }
            set { SetValue(AnimationEasingProperty, value); }
        }

        public static readonly DependencyProperty AnimationEasingProperty =
            DependencyProperty.Register("AnimationEasing", typeof(AnimationEasing), typeof(HighlightLayer));
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
            foreach (var seriesAnimationObjects in _highlightProgressObjects)
            {
                var series = seriesAnimationObjects.Key;
                series.Highlight(
                    drawingContext,
                    chartContext,
                    layerContext,
                    seriesAnimationObjects.Value.ToDictionary(
                        kv => chartContext.Coordinates.First(c => c.Index == kv.Key),
                        kv => (double)kv.Value.Progress
                    )
                );
            }
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