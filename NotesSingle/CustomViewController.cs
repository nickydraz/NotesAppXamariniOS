using System;
using CoreGraphics;
using UIKit;
using System.Collections.Generic;
using System.Threading;

namespace NotesSingle
{
	public class CustomViewController : UIViewController
	{
		UITableView table;
		Thread updateThread;
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

			this.SetToolbarItems(new UIBarButtonItem[] {
		new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace) { Width = width - 50 }
		, new UIBarButtonItem(UIBarButtonSystemItem.Add, (s,e) => {
		this.NavigationController.PushViewController(new CreateNoteViewController(), true);

	})
}, false);

			this.NavigationController.ToolbarHidden = false;
			//updateThread = new Thread(new System.Threading.ThreadStart(UpdateList));
			//updateThread.Start();
			
		}

		protected async void CreateTableItems()
		{
			//List<Note> notes = await NoteDatabase.GetNotesFromDatabase();
			//InvokeOnMainThread(() =>
			//{
				table.Source = new TableSource(NoteDatabase.GetNotesFromDatabase(), this);
			//});
		}


		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			CreateTableItems();
			//isRunning = true;
			//if (!updateThread.IsAlive) updateThread.Start();
		}

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);
			isRunning = false;
			
		}

		
		bool isRunning = true;
		public void UpdateList()
		{
			while (isRunning)
			{
				Thread.Sleep(5000);
				CreateTableItems();
			}
		}
	}
}
