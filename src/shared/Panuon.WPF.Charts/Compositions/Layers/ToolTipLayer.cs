using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Panuon.WPF.Charts
{
    public class ToolTipLayer
        : LayerBase
    {
        #region Fields
        private Label _label;
        #endregion

        #region Ctor
        public ToolTipLayer()
        {
            //SetCurrentValue(ContentTemplateProperty, (DataTemplate)Application.Current.FindResource(TooltipContentTemplateKey));
            _label = new Label()
            {
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                Background = Brushes.White,
                BorderBrush = Brushes.LightGray,
                BorderThickness = new Thickness(2),
                Padding = new Thickness(10, 5, 10, 5),
                Visibility = Visibility == ToolTipVisibility.Visible ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed
            };
            _label.SetBinding(
                Label.ContentTemplateProperty,
                new Binding()
                {
                    Path = new PropertyPath(ContentTemplateProperty),
                    Source = this
                }
            );
            AddChild(_label);
        }
        #endregion

        #region Properties
        public static ComponentResourceKey TooltipContentTemplateKey { get; } =
            new ComponentResourceKey(typeof(ToolTipLayer), nameof(TooltipContentTemplateKey));

        #region ContentTemplate
        public DataTemplate ContentTemplate
        {
            get { return (DataTemplate)GetValue(ContentTemplateProperty); }
            set { SetValue(ContentTemplateProperty, value); }
        }

        public static readonly DependencyProperty ContentTemplateProperty =
            DependencyProperty.Register("ContentTemplate", typeof(DataTemplate), typeof(ToolTipLayer));
        #endregion

        #region Visibility
        public new ToolTipVisibility Visibility
        {
            get { return (ToolTipVisibility)GetValue(VisibilityProperty); }
            set { SetValue(VisibilityProperty, value); }
        }

        public new static readonly DependencyProperty VisibilityProperty =
            DependencyProperty.Register("Visibility", typeof(ToolTipVisibility), typeof(ToolTipLayer), new PropertyMetadata(ToolTipVisibility.VisibleOnHover));
        #endregion

        #region Placement
        public ToolTipPlacement Placement
        {
            get { return (ToolTipPlacement)GetValue(PlacementProperty); }
            set { SetValue(PlacementProperty, value); }
        }

        public static readonly DependencyProperty PlacementProperty =
            DependencyProperty.Register("Placement", typeof(ToolTipPlacement), typeof(ToolTipLayer), new PropertyMetadata(ToolTipPlacement.Fixed, OnInvalidRenderPropertyChanged));
        #endregion

        #region Position
        public ToolTipPosition Position
        {
            get { return (ToolTipPosition)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position", typeof(ToolTipPosition), typeof(ToolTipLayer), new PropertyMetadata(ToolTipPosition.TopLeft, OnInvalidRenderPropertyChanged));
        #endregion

        #region OffsetX
        public double OffsetX
        {
            get { return (double)GetValue(OffsetXProperty); }
            set { SetValue(OffsetXProperty, value); }
        }

        public static readonly DependencyProperty OffsetXProperty =
            DependencyProperty.Register("OffsetX", typeof(double), typeof(ToolTipLayer), new PropertyMetadata(0d, OnInvalidRenderPropertyChanged));
        #endregion

        #region OffsetY
        public double OffsetY
        {
            get { return (double)GetValue(OffsetYProperty); }
            set { SetValue(OffsetYProperty, value); }
        }

        public static readonly DependencyProperty OffsetYProperty =
            DependencyProperty.Register("OffsetY", typeof(double), typeof(ToolTipLayer), new PropertyMetadata(0d, OnInvalidRenderPropertyChanged));
        #endregion

        #endregion

        #region Overrides
        protected override void OnMouseIn(IChartContext chartContext)
        {
            _label.Content = null;
            _label.Visibility = System.Windows.Visibility.Visible;
            InvalidateVisual();
        }

        protected override void OnMouseOut(IChartContext chartContext)
        {
            _label.Content = null;
            _label.Visibility = System.Windows.Visibility.Collapsed;
            InvalidateVisual();
        }

        protected override void OnRender(
            IDrawingContext drawingContext,
            IChartContext chartContext
        )
        {
            if (chartContext.GetMousePosition(MouseRelativeTarget.Layer) is Point position)
            {
                var coordinate = chartContext.RetrieveCoordinate(position);
                if (coordinate != null)
                {
                    _label.Content = coordinate.GetSource();
                }

                if (_label.Content != null)
                {
                    _label.Visibility = System.Windows.Visibility.Visible;
                    UpdateLabelPosition(position);
                }
                else
                {
                    _label.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }
        #endregion

        #region Functions
        private void UpdateLabelPosition(Point mousePosition)
        {
            var offsetX = 0d;
            var offsetY = 0d;

            _label.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            switch (Position)
            {
                case ToolTipPosition.Left:
                    offsetX = Placement == ToolTipPlacement.FollowMouse ? mousePosition.X - _label.DesiredSize.Width : 0;
                    offsetY = Placement == ToolTipPlacement.FollowMouse ? mousePosition.Y - _label.DesiredSize.Height / 2 : 0;
                    break;
                case ToolTipPosition.TopLeft:
                    offsetX = Placement == ToolTipPlacement.FollowMouse ? mousePosition.X - _label.DesiredSize.Width : 0;
                    offsetY = Placement == ToolTipPlacement.FollowMouse ? mousePosition.Y - _label.DesiredSize.Height : 0;
                    break;
                case ToolTipPosition.Top:
                    offsetX = Placement == ToolTipPlacement.FollowMouse ? mousePosition.X - _label.DesiredSize.Width / 2 : 0;
                    offsetY = Placement == ToolTipPlacement.FollowMouse ? mousePosition.Y - _label.DesiredSize.Height : 0;
                    break;
                case ToolTipPosition.TopRight:
                    offsetX = Placement == ToolTipPlacement.FollowMouse ? mousePosition.X : 0;
                    offsetY = Placement == ToolTipPlacement.FollowMouse ? mousePosition.Y - _label.DesiredSize.Height : 0;
                    break;
                case ToolTipPosition.Right:
                    offsetX = Placement == ToolTipPlacement.FollowMouse ? mousePosition.X : 0;
                    offsetY = Placement == ToolTipPlacement.FollowMouse ? mousePosition.Y - _label.DesiredSize.Height / 2 : 0;
                    break;
                case ToolTipPosition.BottomRight:
                    offsetX = Placement == ToolTipPlacement.FollowMouse ? mousePosition.X : 0;
                    offsetY = Placement == ToolTipPlacement.FollowMouse ? mousePosition.Y : 0;
                    break;
                case ToolTipPosition.Bottom:
                    offsetX = Placement == ToolTipPlacement.FollowMouse ? mousePosition.X - _label.DesiredSize.Width / 2 : 0;
                    offsetY = Placement == ToolTipPlacement.FollowMouse ? mousePosition.Y : 0;
                    break;
                case ToolTipPosition.BottomLeft:
                    offsetX = Placement == ToolTipPlacement.FollowMouse ? mousePosition.X - _label.DesiredSize.Width : 0;
                    offsetY = Placement == ToolTipPlacement.FollowMouse ? mousePosition.Y : 0;
                    break;
            }

            _label.Margin = new Thickness(offsetX + OffsetX, offsetY + OffsetY, 0, 0);
        }
        #endregion
    }
}