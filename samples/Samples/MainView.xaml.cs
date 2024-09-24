using Panuon.WPF;
using Panuon.WPF.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

namespace Samples
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView : WindowX
    {
        #region Fields
        private static readonly List<Type> _cartesianViewTypes;
        private static readonly List<Type> _radialViewTypes;

        private int _themeFlag = 0;
        #endregion

        #region Ctor
        static MainView()
        {
            _cartesianViewTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(x => x.IsPublic
                    && typeof(FrameworkElement).IsAssignableFrom(x)
                    && x.GetCustomAttribute<ExampleViewAttribute>() != null
                    && x.GetCustomAttribute<ExampleViewAttribute>().Type == "Cartesian")
                .OrderBy(x => x.GetCustomAttribute<ExampleViewAttribute>().Index)
                .ToList();

            _radialViewTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(x => x.IsPublic
                    && typeof(FrameworkElement).IsAssignableFrom(x)
                    && x.GetCustomAttribute<ExampleViewAttribute>() != null
                    && x.GetCustomAttribute<ExampleViewAttribute>().Type == "Radial")
                .OrderBy(x => x.GetCustomAttribute<ExampleViewAttribute>().Index)
                .ToList();
        }

        public MainView()
        {
            InitializeComponent();

            Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
            {
                InitExampleItems();
            }));
        }
        #endregion

        #region Functions
        private void InitExampleItems()
        {
            var createItems = (IEnumerable<Type> types) =>
            {
                return types.Select(x =>
                {
                     var viewAttribute = x.GetCustomAttribute<ExampleViewAttribute>();
                     var view = (FrameworkElement)Activator.CreateInstance(x);
                     return new ExampleItem()
                     {
                         DisplayName = viewAttribute.DisplayName,
                         ViewType = x,
                         ViewPath = $"Samples/Views/Examples/{x.Name}",
                         PreviewView = view,
                     };
                })
                .ToList();
            };
            var cartesianItems = createItems(_cartesianViewTypes);
            LsbCartesianExamples.ItemsSource = cartesianItems;
            UpdateCartesianChartViewAnimation();
            GenerateCartesianChartDataset();

            var radialItems = createItems(_radialViewTypes);
            LsbRadialExamples.ItemsSource = radialItems;
        }
        #endregion

        #region Event Handlers

        private void CartesianAnimationDurationSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!IsLoaded)
            {
                return;
            }
            UpdateCartesianChartViewAnimation();
        }

        private void CartesianAnimationEasingComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }
            UpdateCartesianChartViewAnimation();
        }

        private void CartesianGenerateButton_Click(object sender, RoutedEventArgs e)
        {
            GenerateCartesianChartDataset();
        }
        #endregion

        #region Functions
        private void UpdateCartesianChartViewAnimation()
        {
            var animationEasing = (AnimationEasing)CartesianAnimationEasingComboBox.SelectedValue;
            var animationDuration = TimeSpan.FromSeconds(CartesianAnimationDurationSlider.Value);

            foreach (ExampleItem item in LsbCartesianExamples.ItemsSource)
            {
                (item.PreviewView as ICartesianChartView).SetAnimation(
                    animationEasing,
                    animationDuration
                );
            }
        }

        private void GenerateCartesianChartDataset()
        {
            foreach (ExampleItem item in LsbCartesianExamples.ItemsSource)
            {
                (item.PreviewView as ICartesianChartView).Generate();
            }
        }
        #endregion

    }
}