using System.Threading;
using System.Threading.Tasks;
using UIKit;

namespace NotesSingle
{
    public class CreateNoteViewController: UIViewController
	{
		public static Note CurrNote { get; set; } = null;
		UITextView _title;
		UITextView _textview;

	    public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			Title = "Edit Note";
			View.BackgroundColor = UIColor.White;
			var h = View.Bounds.Height;
			var w = View.Bounds.Width;

			_title = new UITextView()
			{
				Frame = new CoreGraphics.CGRect(10, 50, w - 20, 100),
				BackgroundColor = UIColor.LightGray
			};

			_textview = new UITextView()
			{
				Frame = new CoreGraphics.CGRect(10, 150, w - 20, h - 100),
				
				BackgroundColor = new UIColor(240, 255, 255, 0)
			};

			Add(_title);
			Add(_textview);

			UIApplication.Notifications.ObserveWillTerminate(async (sender, e) =>
			{
				await SaveNote();
			});
			UIApplication.Notifications.ObserveDidEnterBackground(async (sender, e) =>
			{
				await SaveNote();
			});

			var saveThread = new Thread(SaveTimer);
			saveThread.Start();
		}

		public override async void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);
		    _isRunning = false;
			await SaveNote();
		}

		bool _isRunning = true;
		public async void SaveTimer()
		{
			while (_isRunning)
			{
				Thread.Sleep(5000);
				if (CurrNote != null)
				{
					InvokeOnMainThread(() =>
					{
						CurrNote.Content = _textview.Text;
					});

					}
                //Don't update on the UI thread, in case it takes longer
                await SaveNote();
				
			}
		}

	    public async Task SaveNote()
	    {
	        await Task.Run(() =>
	        {
	            InvokeOnMainThread(async () =>
	            {
	                if (CurrNote == null && _textview.Text != null)
	                {
	                    if (!_textview.Text.Equals(""))
	                    {
	                        string sTitle = _title.Text;
	                        string sContent = _textview.Text;
	                        CurrNote = await NoteDatabase.InsertNote(sTitle, sContent);
	                    }
	                }
	                else if (CurrNote != null)
	                {
	                    CurrNote.Title = _title.Text;
	                    CurrNote.Content = _textview.Text;
	                    await NoteDatabase.UpdateNote(CurrNote);
	                }
	            });
	        });
	    }


	}
}
