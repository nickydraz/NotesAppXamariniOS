using System;
using SQLite;

namespace NotesSingle
{
	public class Note
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set;}


		public string Title
		{
			get;
			set;
		} = "New Note";

		public string Content
		{
			get;
			set;
		} = "";

		public DateTime LastChanged{ get; set; } = DateTime.Now;

		public int User { get; set;}
	}
}
