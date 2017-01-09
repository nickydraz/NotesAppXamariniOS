using System;
using System.Collections.Generic;
using System.IO;
using Foundation;
using SQLite;

namespace NotesSingle
{
	public static class NoteDatabase
	{
		public static List<Note> GetNotesFromDatabase()
		{
			var cmd = GetConnection();
			//cmd.DropTable<Note>();
			cmd.CreateTable<Note> ();
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

		public static Note GetNoteById(int id)
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

		public static void UpdateNote(Note note)
		{
			var table = GetConnection();
			
			table.CreateTable<Note>();

			if (table.Table<Note>().Count() > 0)
			{
				string update = "UPDATE Note SET Title = @Title, Content = @Content, LastModified = (datetime(CURRENT_TIMESTAMP, 'localtime')) WHERE _id = @Id;";
				table.Execute(update, note.Title, note.Content, note.Id);
				//table.Update(note, typeof(Note));
				//table.Update(note.Id, typeof(Note));
			}
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
