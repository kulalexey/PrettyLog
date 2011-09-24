using System;
using MongoDB.Bson;
using MongoDB.Driver;

namespace PrettyLog
{
	public class LogCategory
	{
		public const string Trace = "Trace";
		public const string Info = "Info";
		public const string Warning = "Warning";
		public const string Error = "Error";
		public const string Fatal = "Fatal";
	}

	public class Logger
	{
		MongoServer mongoServer;
		MongoDatabase mongodb;
		MongoCollection logCollection;

		public Logger(string logInstance)
		{
			mongoServer = MongoServer.Create("mongodb://localhost");
			mongodb = mongoServer.GetDatabase("log");
			logCollection = mongodb.GetCollection(logInstance);
		}

		public void Write(string category, string message, params object[] args)
		{
			var logEntry = new BsonDocument();

			logEntry.Add("time", DateTime.Now);
			logEntry.Add("category", category);
			logEntry.Add("message", message);

			BsonArray argDocs = new BsonArray(args.Length);
			for (int i = 0; i < args.Length; i++)
				argDocs.Add(args[i].ToBsonDocument());

			logEntry.Add("args", argDocs);

			logCollection.Insert(logEntry);
		}
	}
}
