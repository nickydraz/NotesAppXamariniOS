using CoreGraphics;
using System.Threading;
using System.Threading.Tasks;
using UIKit;
using System;
using System.Collections.Generic;

namespace NotesSingle
{
    public class CustomViewController : UIViewController
	{
	    private UITableView _table;
	    private Thread _updateThread;
		public async override void ViewDidLoad()
		{
			base.ViewDidLoad();
			try
			{
				var width = View.Bounds.Width;
				var height = View.Bounds.Height;
				Title = "NDraz Notes";
				_table = new UITableView(new CGRect(0, 0, width, height)) { AutoresizingMask = UIViewAutoresizing.All };
				await CreateTableItems();
				Add(_table);

				SetToolbarItems(new[] {
			new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace) { Width = width - 50 },
				new UIBarButtonItem(UIBarButtonSystemItem.Add, (s,e) =>
				{ NavigationController.PushViewController(new CreateNoteViewController(), true); })}, false);

				NavigationController.ToolbarHidden = false;
				_updateThread = new Thread(UpdateList);
				_updateThread.Start();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		protected async Task CreateTableItems()
		{
			//var notes = new List<Note>();
			try
			{
				var source = new TableSource(await NoteDatabase.GetNotesFromLocal(), this);
				InvokeOnMainThread(() =>
				{
					_table.Source = source;
					_table.ReloadData();
				});
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}


		public async override void ViewWillAppear(bool animated)
		{
			try
			{
				base.ViewWillAppear(animated);

				await CreateTableItems();
				_isRunning = true;
				if (_updateThread.ThreadState != ThreadState.Running) _updateThread.Start();
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error in View Will Appear for Main View: " + ex.Message);
			}
		}

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);
			_isRunning = false;
			
		}


		private bool _isRunning = true;
		public async void UpdateList()
		{
			while (_isRunning)
			{
				try
				{
					Thread.Sleep(5000);
				 	await CreateTableItems();
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
				}
			}
		}
	}
}
