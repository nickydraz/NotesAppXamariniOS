using System;
using CoreGraphics;
using UIKit;
using System.Collections.Generic;

namespace NotesSingle
{
	public class CustomViewController : UIViewController
	{
		UITableView table;
		List<Note> Notes;
		public CustomViewController()
		{
			Notes = new List<Note>();

			for (int i = 0; i < 10; i++)
			{
				Notes.Add(new Note { Title = "Note " + i, Content = @"# CommonMark.NET

Implementation of [CommonMark] [1] specification (passes tests from version 0.27) in C# for converting Markdown documents to HTML.

The current version of the library is also [available on NuGet] [nuget].

## Usage

To convert Markdown data in a stream or file:
```C#" });
			}
		}
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			var width = View.Bounds.Width;
			var height = View.Bounds.Height;
			this.Title = "NDraz Notes";
			table = new UITableView(new CGRect(0, 0, width, height));
			table.AutoresizingMask = UIViewAutoresizing.All;
			CreateTableItems();
			Add(table);
		}

		protected void CreateTableItems()
		{
			table.Source = new TableSource(NoteDatabase.GetNotesFromDatabase(), this);
		}

		public override bool PrefersStatusBarHidden()
		{
			return true;
		}

	}
}
