using Panuon.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Samples
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView : Window
    {
        #region Fields
        private static readonly List<Type> _viewTypes;

        private int _themeFlag = 0;
        #endregion

        MainViewModel viewModel;

        #region Ctor
        static MainView()
        {
            _viewTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(x => x.IsPublic && typeof(FrameworkElement).IsAssignableFrom(x) && x.GetCustomAttribute<ExampleViewAttribute>() != null)
                .OrderBy(x => x.GetCustomAttribute<ExampleViewAttribute>().Index)
                .ToList();
        }

        public MainView()
        {
            InitializeComponent();

            viewModel = new MainViewModel();
            DataContext = viewModel;

            Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
            {
                InitExampleItems();
            }));
        }
        #endregion

        #region Functions
        private void InitExampleItems()
        {
            var items = _viewTypes
                .Select(x =>
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
                });
            LsbExamples.ItemsSource = items;
        }
        #endregion

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Delay(2000);

            viewModel.IncomeSourcesProportion = new object[]
            {
                new { Title = "已登记", Value = 50 },
                new { Title = "已总检", Value = 40 },
                new { Title = "已审核", Value = 50 },
                new { Title = "正在分检", Value = 20 }
            };
        }
    }

    class MainViewModel
        : NotifyPropertyChangedBase
    {
        public MainViewModel()
        {
            IncomeSourcesProportion = new object[]
            {
                new { Title = "医保收入3", Value = 900 },
                new { Title = "门诊收入2", Value = 100 }
            };
        }

        public IEnumerable<object> IncomeSourcesProportion { get => _IncomeSourcesProportion; set => Set(ref _IncomeSourcesProportion, value); }
        private IEnumerable<object> _IncomeSourcesProportion;
    }
}