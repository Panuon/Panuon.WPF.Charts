﻿using Panuon.WPF.Charts.Controls.Internals;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Markup;

namespace Panuon.WPF.Charts
{
    [ContentProperty(nameof(Series))]
    public class RadialChart
        : ChartBase
    {
        #region Fields
        #endregion

        #region Ctor
        public RadialChart()
        {
            Series = new SeriesCollection<RadialSeriesBase>();

            _seriesPanel = new SeriesPanel(this);
            _children.Add(_seriesPanel);
        }
        #endregion

        #region Properties

        #region Series
        public SeriesCollection<RadialSeriesBase> Series
        {
            get { return (SeriesCollection<RadialSeriesBase>)GetValue(SeriesProperty); }
            set { SetValue(SeriesProperty, value); }
        }

        public static readonly DependencyProperty SeriesProperty =
            DependencyProperty.Register("Series", typeof(SeriesCollection<RadialSeriesBase>), typeof(RadialChart), new PropertyMetadata(null));
        #endregion

        #endregion

        #region Overrides
        public override IEnumerable<SeriesBase> GetSeries() => Series;
        #endregion
    }
}