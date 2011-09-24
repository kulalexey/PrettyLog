using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LogViewer
{
	/// <summary>
	/// Interaction logic for LogEntryControl.xaml
	/// </summary>
	public partial class LogEntryControl : UserControl
	{
		public static readonly DependencyProperty DocumentProperty =
			DependencyProperty.Register("Document"
										, typeof(RichTextBox)
										, typeof(LogEntryControl)
										, new FrameworkPropertyMetadata(null
																		, new PropertyChangedCallback(OnDocumentChanged)
																		)
										);

		RichTextBox _document;
		public RichTextBox Document
		{
			set
			{
				_document = value;
				Content = value;
			}
		}

		public LogEntryControl()
		{
			InitializeComponent();
		}

		private static void OnDocumentChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			if (args.NewValue != null)
			{
				var logEntryControl = (LogEntryControl)obj;
				var newValue = (RichTextBox)args.NewValue;
				logEntryControl.Document = newValue;
			}
		}
	}
}
