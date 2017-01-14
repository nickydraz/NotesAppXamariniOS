using CoreGraphics;
using System.Threading;
using System.Threading.Tasks;
using UIKit;

namespace NotesSingle
{
    public class CustomViewController : UIViewController
	{
	    private UITableView _table;
	    private Thread _updateThread;
		public override async void ViewDidLoad()
		{
			base.ViewDidLoad();
			var width = View.Bounds.Width;
			var height = View.Bounds.Height;
			Title = "NDraz Notes";
			_table = new UITableView(new CGRect(0, 0, width, height)) {AutoresizingMask = UIViewAutoresizing.All};
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

		protected async Task CreateTableItems()
		{
			var notes = await NoteDatabase.GetNotesFromDatabase();
			InvokeOnMainThread(() =>
			{
				_table.Source = new TableSource( notes, this);
			});
		}


		public override async void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			await CreateTableItems();
			_isRunning = true;
			if (!_updateThread.IsAlive) _updateThread.Start();
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
				Thread.Sleep(5000);
				await CreateTableItems();
			}
		}
	}
}
