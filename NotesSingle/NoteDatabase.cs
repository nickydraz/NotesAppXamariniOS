using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;
using System.IO;

namespace NotesSingle
{
    public static class NoteDatabase
	{
		public async static Task<List<Note>> GetNotesFromLocal()
		{
			var cmd = GetConnection();
			//cmd.DropTable<Note>();
			cmd.CreateTable<Note>();
			var Notes = new List<Note>();
			if (cmd.Table<Note>().Count() > 0)
			{

				foreach (Note note in cmd.Table<Note>())
				{
					Notes.Add(note);
				}

			}
			else {
				cmd.Insert(new Note { Title = "Note 1", Content = "Not updated." });
				Notes.Add(new Note { Title = "Note 1", Content = "Not updated." });
			}

			return Notes;
		}

		public async static Task<Note> GetNoteByIdFromLocal(long id)
		{
			var cmd = GetConnection();
			//cmd.DropTable<Note>();
			cmd.CreateTable<Note>();

			if (cmd.Table<Note>().Count() > 0)
			{
				return cmd.Get<Note>(id);
			}

			return null;
		}

		public async static Task UpdateNoteLocal(Note note)
		{
			var table = GetConnection();

			table.CreateTable<Note>();
			note.LastChanged = DateTime.Now;
			if (table.Table<Note>().Count() > 0)
			{
				//string update = "UPDATE Note SET Title = @Title, Content = @Content, LastModified = (datetime(CURRENT_TIMESTAMP, 'localtime')) WHERE _id = @Id;";
				//table.Execute(update, note.Title, note.Content, note.Id);
				////table.Update(note, typeof(Note));
				////table.Update(note.Id, typeof(Note));
				table.Update(note);
			}
		}

		public async static Task<Note> InsertNoteToLocal(string title, string content, int user)
		{
			var table = GetConnection();
			var note = new Note() { Title = title, Content = content, User = user };
			table.Insert(note);
			return note;
		}

		public async static Task<Note> InsertNoteToLocal(Note note)
		{
			var table = GetConnection();
			table.Insert(note);
			return note;
		}
		public static  List<Note> GetNotesFromDatabase()
		{
			try
			{
				var client = new RestClient("http://ndraz.com:/NotesApi/");
				var request = new RestRequest(Method.POST);
				request.AddHeader("postman-token", "38bf6ae3-a903-878e-6eca-24c47100201d");
				request.AddHeader("cache-control", "no-cache");
				request.AddHeader("content-type", "application/json");
				request.AddParameter("application/json", "{\n  \"action\": \"getall\"\n\n}", ParameterType.RequestBody);

				var response = client.Execute(request);
				//Console.WriteLine(response.Content);
				
				var start = response.Content.IndexOf('[');
				var end = response.Content.IndexOf(']');
				if (start < 0 || end < 0)
				{
					Console.WriteLine("RestSharp Error Message: " + response.ErrorException.Message);
					//Sometimes the failure is just a weird bug (supposedly in Mono) and not a problem with the call
					//Try again and see what happens
					response = client.Execute(request);
					start = response.Content.IndexOf('[');
					end = response.Content.IndexOf(']');
				}

				var result = response.Content.Substring(start, end - start + 1);
				var notes = JsonConvert.DeserializeObject<List<Note>>(result);
				return notes;
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error in Get All: " + ex.Message);
				return new List<Note>(); //return an empty list so the app doesn't crash
			}
		}

		public static Note GetNoteById(long id)
		{
			var client = new RestClient("http://ndraz.com/NotesApi/");
			var request = new RestRequest(Method.POST);
			request.AddHeader("postman-token", "25ed98cf-d91a-7c8f-660b-eb5152d417dc");
			request.AddHeader("cache-control", "no-cache");
			request.AddHeader("content-type", "application/json");
			request.AddParameter("application/json", "{\n\t\"action\": \"getid\",\n\t\"Id\": \"" + id + "\"\n}", ParameterType.RequestBody);
			var response = client.Execute(request);

			var start = response.Content.IndexOf('[');
			var end = response.Content.IndexOf(']');
			if (start < 0 || end < 0)
			{
				Console.WriteLine("RestSharp Error Message: " + response.ErrorException.Message);
				//Sometimes the failure is just a weird bug (supposedly in Mono) and not a problem with the call
				//Try again and see what happens
				response = client.Execute(request);
				start = response.Content.IndexOf('[');
				end = response.Content.IndexOf(']');
			}

			try
			{
				var result = response.Content.Substring(start, end - start + 1);
				var note = JsonConvert.DeserializeObject<List<Note>>(result);
				//Console.WriteLine(note[0].Id);
				return note[0];
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error in Get By ID: " + ex.Message);
				return null;
			}

		}

		public static async Task UpdateNote(Note note)
		{
			var id = note.Id;
			var title = note.Title;
			var content = note.Content;

			var client = new RestClient("http://ndraz.com/NotesApi/");
			var request = new RestRequest(Method.POST);
			request.AddHeader("postman-token", "64c0f8d8-9b02-4e78-e052-bdc88799aae5");
			request.AddHeader("cache-control", "no-cache");
			request.AddHeader("content-type", "application/json");
			request.AddParameter("application/json", "{\n\t\"action\": \"update\",\n\t\"Id\": \"" + id + "\",\n\t\"Title\": \"" + EscapeForJson(title) + "\",\n\t\"Content\": \"" + EscapeForJson(content) + "\"\n}", ParameterType.RequestBody);
			await client.ExecuteTaskAsync(request);

			//Console.WriteLine(response.Content);
		}

		public static async Task<Note> InsertNote(string title, string content)
		{
			var client = new RestClient("http://ndraz.com/NotesApi/");
			var request = new RestRequest(Method.POST);
			request.AddHeader("postman-token", "5b086c62-b3f2-d68b-eeb4-660251ce9b6e");
			request.AddHeader("cache-control", "no-cache");
			request.AddHeader("content-type", "application/json");
			request.AddParameter("application/json", "{\n\t\"action\": \"add\",\n\t\"Title\": \"" + EscapeForJson(title) + "\",\n\t\"Content\": \"" + EscapeForJson(content) + "\",\n\t\"User\": \"1\"\n}", ParameterType.RequestBody);
			var response = await client.ExecuteTaskAsync(request);

			if (response.Content == null || response.Content.Equals(""))
			{
				response = await client.ExecuteTaskAsync(request);
			}

			var obj = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content);

			return new Note()
			{
				Id = Convert.ToInt32(obj["InsertedId"]),
				Title = title,
				Content = content
			};

		}

		public static async Task<Note> InsertNote(Note note)
		{
			var client = new RestClient("http://ndraz.com/NotesApi/");
			var request = new RestRequest(Method.POST);
			request.AddHeader("postman-token", "5b086c62-b3f2-d68b-eeb4-660251ce9b6e");
			request.AddHeader("cache-control", "no-cache");
			request.AddHeader("content-type", "application/json");
			request.AddParameter("application/json", "{\n\t\"action\": \"add\",\n\t\"Title\": \"" + EscapeForJson(note.Title) + "\",\n\t\"Content\": \"" + EscapeForJson(note.Content) + "\",\n\t\"User\": \"" + EscapeForJson(note.User.ToString()) + "\"\n}", ParameterType.RequestBody);
			var response = await client.ExecuteTaskAsync(request);

			if (response.Content == null || response.Content.Equals(""))
			{
				response = await client.ExecuteTaskAsync(request);
			}

			var obj = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content);

			return new Note()
			{
				Id = Convert.ToInt32(obj["InsertedId"]),
				Title = note.Title,
				Content = note.Content
			};

		}

		public async static void SyncDatabases()
		{
			List<Note> remoteNotes = GetNotesFromDatabase();

			List<Note> localNotes = await GetNotesFromLocal();

			List<Note> updatedNotes = new List<Note>();

			foreach (Note note in remoteNotes)
			{
				var foundInLocal = localNotes.Find((obj) => obj.Id == note.Id);
				if (foundInLocal != null)
				{
					if (foundInLocal.LastChanged < note.LastChanged)
					{
						await UpdateNoteLocal(note);
					}
					else {
						await UpdateNote(foundInLocal);
					}
				}
				else {
					await InsertNoteToLocal(note);
				}
			}

			foreach (Note note in localNotes)
			{
				var foudnInRemote = remoteNotes.Find((obj) => obj.Id == note.Id);
				if (foudnInRemote != null)
				{
					if (foudnInRemote.LastChanged > note.LastChanged)
					{
						await UpdateNoteLocal(note);
					}
					else {
						await UpdateNote(foudnInRemote);
					}
				}
				else {
					await InsertNote(note);
				}
			}

		}

		public static string EscapeForJson(string input)
		{
			return input.Replace("\n", "\\n").Replace("\t", "\\t").Replace("\b", "\\b").Replace("\f", "\\f").Replace("\r", "\\r");
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
