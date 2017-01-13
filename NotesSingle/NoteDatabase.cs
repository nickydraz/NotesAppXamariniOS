using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Foundation;
using SQLite;
using RestSharp;

using Newtonsoft.Json;
using System.Threading.Tasks;

namespace NotesSingle
{
	public static class NoteDatabase
	{
		public static List<Note> GetNotesFromDatabase()
		{
			var client = new RestClient("http://ndraz.com/NotesApi/");
			var request = new RestRequest(Method.POST);
			request.AddHeader("postman-token", "38bf6ae3-a903-878e-6eca-24c47100201d");
			request.AddHeader("cache-control", "no-cache");
			request.AddHeader("content-type", "application/json");
			request.AddParameter("application/json", "{\n  \"action\": \"getall\"\n\n}", ParameterType.RequestBody);
			IRestResponse response = client.Execute(request);
			//Console.WriteLine(response.Content);



			var start = response.Content.IndexOf('[');
			var end = response.Content.IndexOf(']');
			var result = response.Content.Substring(start, end - start + 1);
			var notes = JsonConvert.DeserializeObject<List<Note>>(result);
			return notes;


			//var cmd = GetConnection();
			////cmd.DropTable<Note>();
			//cmd.CreateTable<Note>();
			//var Notes = new List<Note>();
			//if (cmd.Table<Note>().Count() > 0)
			//{

			//	foreach (Note note in cmd.Table<Note>())
			//	{
			//		Notes.Add(note);
			//	}

			//}
			//else {
			//	cmd.Insert(new Note { Title = "Note 1", Content = "Not updated." });
			//	Notes.Add(new Note { Title = "Note 1", Content = "Not updated." });
			//}

			//return Notes;
		}

		public static Note GetNoteById(long id)
		{
			var client = new RestClient("http://ndraz.com/NotesApi/");
			var request = new RestRequest(Method.POST);
			request.AddHeader("postman-token", "25ed98cf-d91a-7c8f-660b-eb5152d417dc");
			request.AddHeader("cache-control", "no-cache");
			request.AddHeader("content-type", "application/json");
			request.AddParameter("application/json", "{\n\t\"action\": \"getid\",\n\t\"Id\": \"" + id + "\"\n}", ParameterType.RequestBody);
			IRestResponse response = client.Execute(request);

			var start = response.Content.IndexOf('[');
			var end = response.Content.IndexOf(']');
			var result = response.Content.Substring(start, end - start + 1);
			var note = JsonConvert.DeserializeObject<List<Note>>(result);
			//Console.WriteLine(note[0].Id);
			return note[0];

			//var cmd = GetConnection();
			////cmd.DropTable<Note>();
			//cmd.CreateTable<Note>();

			//if (cmd.Table<Note>().Count() > 0)
			//{
			//	return cmd.Get<Note>(id);
			//}

			//return null;
		}

		public static void UpdateNote(Note note)
		{
			var id = note.Id;
			var title = note.Title;
			var content = note.Content;

			var client = new RestClient("http://ndraz.com/NotesApi/");
			var request = new RestRequest(Method.POST);
			request.AddHeader("postman-token", "f4b286c9-95ef-1aa9-3a55-c6447365b9b8");
			request.AddHeader("cache-control", "no-cache");
			request.AddHeader("content-type", "application/json");
			request.AddParameter("application/json", "{\n\t\"action\": \"update\",\n\t\"Id\": \"" + id + "\",\n\t\"Title\": \"" + title + "\",\n\t\"Content\": \"" + content + "\"\n}", ParameterType.RequestBody);
			client.Execute(request);


			//var table = GetConnection();
			
			//table.CreateTable<Note>();

			//if (table.Table<Note>().Count() > 0)
			//{
			//	string update = "UPDATE Note SET Title = @Title, Content = @Content, LastModified = (datetime(CURRENT_TIMESTAMP, 'localtime')) WHERE _id = @Id;";
			//	table.Execute(update, note.Title, note.Content, note.Id);
			//	//table.Update(note, typeof(Note));
			//	//table.Update(note.Id, typeof(Note));
			//}
		}

		public static Note InsertNote(string title, string content)
		{
			var client = new RestClient("http://ndraz.com/NotesApi/");
			var request = new RestRequest(Method.POST);
			request.AddHeader("postman-token", "5b086c62-b3f2-d68b-eeb4-660251ce9b6e");
			request.AddHeader("cache-control", "no-cache");
			request.AddHeader("content-type", "application/json");
			request.AddParameter("application/json", "{\n\t\"action\": \"add\",\n\t\"Title\": \"" + title + "\",\n\t\"Content\": \"" + content + "\",\n\t\"User\": \"1\"\n}", ParameterType.RequestBody);
			IRestResponse response = client.Execute(request);

			Dictionary<string, string> obj = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content);

			return new Note()
			{
				Id = Convert.ToInt64(obj["InsertedId"]),
				Title = title,
				Content = content
			};

		}

		private static SQLiteConnection GetConnection()
		{
			var sqliteFilename = "Notes.db";
#if __ANDROID__
// Just use whatever directory SpecialFolder.Personal returns
string libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); ;
#else
			// we need to put in /Library/ on iOS5.1+ to meet Apple's iCloud terms
			// (they don't want non-user-generated data in Documents)
			string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder
			string libraryPath = Path.Combine(documentsPath, "..", "Library"); // Library folder instead
#endif
			var path = Path.Combine(libraryPath, sqliteFilename);

			return new SQLiteConnection(path, false);
		}
	}
}
