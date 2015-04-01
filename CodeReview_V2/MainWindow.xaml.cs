using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CodeReview_V2.DataAccess;
using CodeReview_V2.ViewModel;

namespace CodeReview_V2
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private MainWindowViewModel viewModel;
		public MainWindow()
		{
			InitializeComponent();
			this.viewModel = new MainWindowViewModel();
			this.DataContext = this.viewModel;
			this.IncidentAssociations.ItemsSource = this.viewModel.IncidentDataGrid;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			this.viewModel.GetIncident(72382);
		}
	}
}
