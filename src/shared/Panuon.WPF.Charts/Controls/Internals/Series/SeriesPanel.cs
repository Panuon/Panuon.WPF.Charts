﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Panuon.WPF.Charts.Controls.Internals
{
    internal class SeriesPanel
        : FrameworkElement
    {
        #region Fields
        private ChartBase _chart;

        private UIElementCollection _children;

        private IList<ICoordinate> _coordinates;
        #endregion

        #region Ctor
        internal SeriesPanel(ChartBase chart)
        {
            _chart = chart;

            _children = new UIElementCollection(this, this);
        }
        #endregion

        #region Overrides
        protected override int VisualChildrenCount => _children.Count;

        protected override Visual GetVisualChild(int index) => _children[index];

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach(SeriesBase child in _children)
            {
                child.Measure(availableSize);
            }
            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (SeriesBase child in _children)
            {
                child.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));

                child.InvalidateLayout();
                child.InvalidateVisual();
            }
            return base.ArrangeOverride(finalSize);
        }
        #endregion

        #region Methods
        public void ResetSeries()
        {
            _children.Clear();
            foreach(var series in _chart.GetSeries())
            {
                _children.Add(series);
            }
        }
        #endregion

        #region Functions
        private IEnumerable<SeriesBase> GetSeries()
        {
            if (_chart is CartesianChart cartesianChart)
            {
                return cartesianChart.Series;
            }
            else if (_chart is RadialChart radialChart)
            {
                return radialChart.Series;
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        #endregion
    }
}
