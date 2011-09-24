using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LogViewer
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		LogViewModel _viewModel;

		public MainWindow()
		{
			InitializeComponent();
			_viewModel = new LogViewModel();
			DataContext = _viewModel;
		}

		private void NextPageClick(object sender, RoutedEventArgs e)
		{
			_viewModel.NextPage();
		}

		private void PreviousPageClick(object sender, RoutedEventArgs e)
		{
			_viewModel.PreviousPage();
		}

		private void loginstanceComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count > 0)
				_viewModel.SelectLogInstance((string)e.AddedItems[0]);
		}

		private void FilterClick(object sender, RoutedEventArgs e)
		{
			_viewModel.SetQuery(queryTextBox.Text);
		}

		private void queryTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
				_viewModel.SetQuery(queryTextBox.Text);
		}
	}
}
