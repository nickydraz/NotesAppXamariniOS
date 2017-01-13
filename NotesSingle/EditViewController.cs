using System;
using System.Threading;
using System.Threading.Tasks;
using UIKit;
namespace NotesSingle
{
	public class EditViewController : UIViewController
	{
		public static Note CurrNote { get; set;}
		UITextView textview;


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

		 	textview = new UITextView()
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

			Add(textview);
			//Add(saveBtn);

			UIApplication.Notifications.ObserveWillTerminate((sender, e) => {
				UpdateNote();
			});
			UIApplication.Notifications.ObserveDidEnterBackground((sender, e) =>
			{
				UpdateNote();
			});

			var saveThread = new Thread(new System.Threading.ThreadStart(SaveTimer));
			saveThread.Start();
		}

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);

			isRunning = false;

			UpdateNote();

		}

		
		bool isRunning = true;
		public void SaveTimer()
		{
			while (isRunning)
			{
				Thread.Sleep(5000);
				InvokeOnMainThread(() =>
				{
					CurrNote.Content = textview.Text;
				});
 				
				//Don't update on the UI thread, in case it takes longer
				NoteDatabase.UpdateNote(CurrNote);
			}
		}

		public void UpdateNote()
		{
			CurrNote.Content = textview.Text;
			NoteDatabase.UpdateNote(CurrNote);
		}
	}
}
