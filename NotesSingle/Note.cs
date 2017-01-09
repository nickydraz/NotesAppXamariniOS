using System;
using SQLite;

namespace NotesSingle
{
	public class Note
	{
		[PrimaryKey, AutoIncrement, Column("_id")]
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

		public DateTime LastModified { get; set; } = DateTime.Now;
	}
}
