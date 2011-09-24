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
using System.Windows.Shapes;
using MongoDB.Bson;

namespace LogViewer
{
	/// <summary>
	/// Interaction logic for ShowObjectWindow.xaml
	/// </summary>
	public partial class ShowObjectWindow : Window
	{
		public ShowObjectWindow()
		{
			InitializeComponent();
		}

		public void ShowObject(BsonValue bsonValue)
		{
			var tree = new TreeView();
			

			var item = new TreeViewItem();
			tree.Items.Add(item);
			WriteObject(item, bsonValue);
			item.ExpandSubtree();
			this.Content = tree;
			Show();
			Focus();
			this.Topmost= true;
		}

		private void WriteObject(TreeViewItem root, BsonValue bsonValue)
		{
			if (bsonValue.IsBsonDocument)
			{
				var document = bsonValue.AsBsonDocument;

				foreach (var e in document.Elements)
				{
					var child = new TreeViewItem();
					if (e.Value.IsBsonArray || e.Value.IsBsonDocument)
					{
						child.Header = e.Name;
						WriteObject(child, e.Value);
					}
					else
						child.Header = e.Name + "=" + GetValue(e.Value);

					root.Items.Add(child);
				}
				//WriteObject(bsonValue.
			}
			else if (bsonValue.IsBsonArray)
			{
				var array = bsonValue.AsBsonArray;

				for (int i = 0; i < array.Count; i++)
				{
					var value = array[i];
					var child = new TreeViewItem();
					if (value.IsBsonArray || value.IsBsonDocument)
					{
						child.Header = "[" + i.ToString() + "]";
						WriteObject(child, value);
					}
					else
						child.Header = "[" + i.ToString() + "]" + "=" + GetValue(value);

					root.Items.Add(child);
				}
			}
			else
			{
				var child = new TreeViewItem();
				child.Header = bsonValue.AsString;
				root.Items.Add(child);
			}
		}

		public string GetValue(BsonValue value)
		{
			if (value.IsBsonNull)
				return "null";
			else
				return value.ToString();
		}
	}
}
