using PrettyLog;

namespace Test
{
	public class TestObject
	{
		public int N { get; set; }
	}

	class Program
	{
		static void Main(string[] args)
		{
			var log = new Logger("log");
			var log2 = new Logger("log2");

			for (int i = 0; i < 100; i++)
			{
				var obj = new TestObject() { N = i };
				log.Write(LogCategory.Info, "logged {object} N\n\nas as as aaaaaaaaaaaaaaaaaaaaaaaaaa ssssssssssssssss dddddddddddddddddddddddd fffffffffff" + i.ToString(), obj);
			}

			for (int i = 0; i < 10; i++)
			{
				var obj = new TestObject() { N = i };
				log2.Write(LogCategory.Info, "{object}" + i.ToString(), obj);
			}
		}
	}
}
