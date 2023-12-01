using Panuon.WPF.Charts.Implements;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Panuon.WPF.Charts.Controls.Internals
{
    internal class SeriesPanel
        : FrameworkElement
    {
        #region Fields
        private ChartPanel _chartPanel;

        private UIElementCollection _children;

        private readonly List<SeriesPresenter> _seriesPresenters = 
            new List<SeriesPresenter>();

        private IList<ICoordinate> _coordinates;
        #endregion

        #region Ctor
        internal SeriesPanel(ChartPanel chartPanel)
        {
            _chartPanel = chartPanel;
            _chartPanel.Series.CollectionChanged += ChartPanelSeries_CollectionChanged;

            _children = new UIElementCollection(this, this);
        }
        #endregion

        #region Properties
        internal IEnumerable<CoordinateImpl> Coordinates { get; set; }

        internal double MinValue { get; set; }

        internal double MaxValue { get; set; }

        #endregion

        #region Overrides
        protected override int VisualChildrenCount => _children.Count;

        protected override Visual GetVisualChild(int index) => _children[index];

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach(SeriesPresenter child in _children)
            {
                child.Measure(availableSize);
            }
            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (SeriesPresenter child in _children)
            {
                child.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
            }
            return base.ArrangeOverride(finalSize);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {

        }

        #endregion

        #region Methods
        public new void InvalidateVisual()
        {
            foreach(SeriesPresenter child in _children)
            {
                child.InvalidateVisual();
            }
        }
        #endregion

        #region Event Handlers
        private void ChartPanelSeries_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (SeriesBase series in e.OldItems)
                {
                    var presenter = _seriesPresenters.FirstOrDefault(x => x.Series == series);
                    if (presenter != null)
                    {
                        _children.Remove(presenter);
                        _seriesPresenters.Remove(presenter);
                    }
                }
            }
            if (e.NewItems != null)
            {
                foreach (SeriesBase series in e.NewItems)
                {
                    var presenter = _seriesPresenters.FirstOrDefault(x => x.Series == series);
                    if (presenter == null)
                    {
                        presenter = new SeriesPresenter(this, _chartPanel)
                        {
                            Series = series
                        };
                        _children.Add(presenter);
                        _seriesPresenters.Add(presenter);
                    }
                }
            }
        }
        #endregion

        #region Functions
        #endregion
    }
}
