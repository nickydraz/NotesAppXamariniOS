using System;
using System.Threading;
using UIKit;

namespace NotesSingle
{
	public class CreateNoteViewController: UIViewController
	{
		public static Note CurrNote { get; set; } = null;
		UITextView title;
		UITextView textview;

		public CreateNoteViewController()
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			Title = "Edit Note";
			View.BackgroundColor = UIColor.White;
			var h = View.Bounds.Height;
			var w = View.Bounds.Width;

			title = new UITextView()
			{
				Frame = new CoreGraphics.CGRect(10, 50, w - 20, 100),
				BackgroundColor = UIColor.LightGray
			};

			textview = new UITextView()
			{
				Frame = new CoreGraphics.CGRect(10, 150, w - 20, h - 100),
				
				BackgroundColor = new UIColor(240, 255, 255, 0)
			};

			Add(title);
			Add(textview);

			UIApplication.Notifications.ObserveWillTerminate((sender, e) =>
			{
				SaveNote();
			});
			UIApplication.Notifications.ObserveDidEnterBackground((sender, e) =>
			{
				SaveNote();
			});

			var saveThread = new Thread(new System.Threading.ThreadStart(SaveTimer));
			saveThread.Start();
		}

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);
			SaveNote();
		}

		bool isRunning = true;
		public void SaveTimer()
		{
			while (isRunning)
			{
				Thread.Sleep(5000);
				if (CurrNote != null)
				{
					InvokeOnMainThread(() =>
					{
						CurrNote.Content = textview.Text;
					});

					}
				//Don't update on the UI thread, in case it takes longer
				SaveNote();
				
			}
		}

		public void SaveNote()
		{
			InvokeOnMainThread(() =>
			{
				if (CurrNote == null && textview.Text != null)
				{
					if (!textview.Text.Equals(""))
					{
						string sTitle = title.Text;
						string sContent = textview.Text;
						CurrNote = NoteDatabase.InsertNote(sTitle, sContent);
					}
				}
				else if (CurrNote != null) {
					CurrNote.Title = title.Text;
					CurrNote.Content = textview.Text;
					NoteDatabase.UpdateNote(CurrNote);
				}
			});


		}


	}
}
