using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Windows.Controls;
using MongoDB.Bson;
using MongoDB.Driver;
using MessageBox = Microsoft.Windows.Controls.MessageBox;

namespace LogViewer
{
	public class LogViewModelEntry
	{
		public int N { get; set; }
		public DateTime Time { get; set; }
		public string Category { get; set; }
		public RichTextBox Message { get; set; }
	}

	public class LogViewModel
	{
		int page = 0;
		int onpage = 100;

		MongoServer mongoServer;
		MongoDatabase mongodb;

		public ICollectionView LogEntries { get; set; }
		public ICollectionView LogInstances { get; set; }
		public string LogCurrentInstance { get; private set; }
		public string Query { get; private set; }

		public LogViewModel()
		{
			mongoServer = MongoServer.Create("mongodb://localhost");
			mongodb = mongoServer.GetDatabase("log");

			List<string> logInstances = (from instanceName in mongodb.GetCollectionNames()
										 where !instanceName.StartsWith("system.")
										 select instanceName).ToList();

			LogInstances = CollectionViewSource.GetDefaultView(logInstances);

			if (logInstances.Count > 0)
			{
				LogCurrentInstance = logInstances.First();

				List<LogViewModelEntry> enties = new List<LogViewModelEntry>();// GetLogEntries(page);
				LogEntries = CollectionViewSource.GetDefaultView(enties);
			}
		}

		public void NextPage()
		{
			page++;
			if (GetCount() > page * onpage)
			{
				UpdateEntities();
			}
			else
				page--;

		}

		public void PreviousPage()
		{
			if (page > 0)
				page--;
			UpdateEntities();
		}

		public void SelectLogInstance(string instanceName)
		{
			page = 0;
			LogCurrentInstance = instanceName;
			UpdateEntities();
		}

		public void SetQuery(string query)
		{
			page = 0;
			Query = query;
			UpdateEntities();
		}
		
		void UpdateEntities()
		{
			try
			{
				using (var z = LogEntries.DeferRefresh())
				{
					((List<LogViewModelEntry>)LogEntries.SourceCollection).Clear();
					((List<LogViewModelEntry>)LogEntries.SourceCollection).AddRange(GetLogEntries());
				}
				LogEntries.Refresh();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}


		private long GetCount()
		{
			try
			{
				var result = GetLogEntitiesCoursor().Count();
				return result;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				return 0;
			}
		}
		
		MongoCursor<BsonDocument> GetLogEntitiesCoursor()
		{
			var logCollection = mongodb.GetCollection(LogCurrentInstance);

			MongoCursor<BsonDocument> entities;

			if (!string.IsNullOrEmpty(Query))
			{
				QueryDocument query;
				try
				{
					query = new QueryDocument(BsonDocument.Parse(Query));
				}
				catch(Exception ex)
				{
					query = new QueryDocument(BsonDocument.Parse("{}"));
					MessageBox.Show(ex.Message);
				}

				entities = logCollection.FindAs<BsonDocument>(query);
			}
			else
				entities = logCollection.FindAllAs<BsonDocument>();

			entities.Limit = onpage;
			entities.Skip = page * onpage;

			return entities;
		}

		List<LogViewModelEntry> GetLogEntries()
		{

			List<LogViewModelEntry> result = new List<LogViewModelEntry>();

			int n = page*onpage;
			foreach (var entry in GetLogEntitiesCoursor())
				result.Add(ParseAndAddLogEntry(entry, n++));

			return result;
		}

		private LogViewModelEntry ParseAndAddLogEntry(BsonDocument entry, int entryNumber)
		{
			DateTime time = entry.GetValue("time").AsDateTime;
			string category = entry.GetValue("category").AsString;
			string format = entry.GetValue("message").AsString;
			BsonArray args = entry.GetValue("args").AsBsonArray;

			Paragraph paragraph = new Paragraph();
			string current = "";
			int i = 0;
			int n = 0;

			bool arrayHack = (format.Count(x => x == '{') == 1 && args.Count > 1);

			while (i < format.Length)
			{
				if (format[i] != '{')
					current += format[i];
				else
				{
					paragraph.Inlines.Add(new Run(current));
					string token = "";
					i++;
					while (i < format.Length && format[i] != '}')
						token += format[i++];

					if (i < format.Length && format[i] == '}')
					{
						var run = new Run(token);
						run.Foreground = Brushes.MediumBlue;
						run.Cursor = Cursors.Hand;
						int localN = n;
						if (arrayHack)
							run.MouseDown += delegate { ShowObject(args); };
						else
							run.MouseDown += delegate { ShowObject(args[localN]); };
						paragraph.Inlines.Add(run);
					}

					n++;
					current = "";
				}
				i++;
			}
			if (current != "")
				paragraph.Inlines.Add(new Run(current));

			var flowDoc = new FlowDocument(paragraph);
			flowDoc.FontFamily = new FontFamily("lucida");
			var msgRtb = new RichTextBox(flowDoc);
			msgRtb.IsReadOnly = true;
			var result = new LogViewModelEntry()
			{
				N = entryNumber,
				Time = time,
				Category = category,
				Message = msgRtb,
			};

			return result;
		}

		private void ShowObject(BsonValue bsonValue)
		{
			var w = new ShowObjectWindow();
			//w.Show();
			w.ShowObject(bsonValue);
		}
	}
}
