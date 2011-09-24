using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LogViewer
{
	/// <summary>
	/// Interaction logic for Test.xaml
	/// </summary>
	public partial class Test : Window
	{
		//public SomeModel model2 = new SomeModel();


		public Test()
		{
			InitializeComponent();
			DataContext = new SomeModel();
		}
	}

	public class LogEntry
	{
		public DateTime Time { get; set; }
		public FlowDocument Message { get; set; }
	}

	public class SomeModel
	{
		public ICollectionView LogEntries 
		{
			get;
			private set;
		}

		public SomeModel()
		{
			var _logEntries = new List<LogEntry>()
			{
				new LogEntry { Time = DateTime.Now, Message = new FlowDocument(new Paragraph(new Run("asd"))) },
				new LogEntry { Time = DateTime.Now, Message = new FlowDocument(new Paragraph(new Run("xxx"))) },
			};

			LogEntries = CollectionViewSource.GetDefaultView(_logEntries);
		}
	}
}
