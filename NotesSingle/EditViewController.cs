using System;
using System.Threading;
using System.Threading.Tasks;
using UIKit;
namespace NotesSingle
{
    public class EditViewController : UIViewController
	{
		public static Note CurrNote { get; set;}
	    private UITextView _textview;


		public EditViewController(Note note)
		{
			CurrNote = note;
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			Title = "Edit Note";
			View.BackgroundColor = UIColor.White;
			var h = View.Bounds.Height;
			var w = View.Bounds.Width;

		 	_textview = new UITextView()
			{
				Frame = new CoreGraphics.CGRect(10, 50, w - 20, h - 100),
				Text = CurrNote.Content,
				BackgroundColor = new UIColor(240, 255, 255, 0)

			};

			//saveBtn = UIButton.FromType(UIButtonType.RoundedRect);
			//saveBtn.Frame = new CoreGraphics.CGRect(10, View.Bounds.Bottom - 60, w - 20, 31.0f);
			//saveBtn.BackgroundColor = UIColor.White;
			//saveBtn.SetTitle("Save Changes", UIControlState.Normal);
			//saveBtn.Layer.CornerRadius = 5f;
			//saveBtn.TouchUpInside += (sender, e) =>
			//{
			//	CurrNote.Content = textview.Text;
			//	NoteDatabase.UpdateNote(CurrNote);
			//};

			Add(_textview);
			//Add(saveBtn);

			//UIApplication.Notifications.ObserveWillTerminate(async (sender, e) => {
			//	await UpdateNote();
			//});
			//UIApplication.Notifications.ObserveDidEnterBackground(async (sender, e) =>
			//{
			//	await UpdateNote();
			//});

			//var saveThread = new Thread(SaveTimer);
			//saveThread.Start();
		}

		public override async void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);

			_isRunning = false;
			await UpdateNote();
		}


	    private bool _isRunning = true;
		public async void SaveTimer()
		{
			while (_isRunning)
			{
				Thread.Sleep(5000);
				InvokeOnMainThread(() =>
				{
					CurrNote.Content = _textview.Text;
				});
 				
				//Don't update on the UI thread, in case it takes longer
				await NoteDatabase.UpdateNoteLocal(CurrNote);
			}
		}

		public async Task UpdateNote()
		{
			try
			{
				CurrNote.Content = _textview.Text;
				await NoteDatabase.UpdateNoteLocal(CurrNote);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}
	}
}
