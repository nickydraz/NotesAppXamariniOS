﻿using System;
using UIKit;
using Foundation;
using System.Net;
using System.IO;
using System.Threading;

namespace NotesSingle
{
	public class NoteDetailViewController: UIViewController
	{
		UITextView ContentView;
		public static Note CurrNote
		{
			get;
			set;
		}

		public CustomViewController Owner
		{
			get;
			set;
		}

		public NoteDetailViewController(Note note)
		{
			CurrNote = note;
		}


		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			View.BackgroundColor = new UIColor(214f, 214f,214f, 0.68f);
			ContentView = CreateContentView();
			this.View.Add(ContentView);

			Thread updateThread = new Thread(new ThreadStart(StayUpdated));
			updateThread.Start();
		}

		public UITextView CreateContentView()
		{
			UITextView text = new UITextView 
			{ 
				Editable = false, 
				Frame = new CoreGraphics.CGRect(10, 10, View.Bounds.Width - 20, View.Bounds.Height - 20) 
			};
			//Create the double tap to edit event handler and add it to the text view
			UITapGestureRecognizer dblTap = new UITapGestureRecognizer(HandleTouch);
			dblTap.NumberOfTapsRequired = 2;
			text.GestureRecognizers = new UIGestureRecognizer[] { dblTap };

			//Add style
			text.Layer.BorderWidth = 0.5f;
			text.Layer.BorderColor = new CoreGraphics.CGColor(255f, 255f, 255f);
			text.Layer.CornerRadius = 5.0f;
			//Set the text
			text.AttributedText = MarkdownToText(CurrNote.Content);

			return text;

		}

		public NSAttributedString MarkdownToText(string markdown)
		{
			NSError error = null;
			var ret = new NSAttributedString(CommonMark.CommonMarkConverter.Convert(markdown), new NSAttributedStringDocumentAttributes { DocumentType = NSDocumentType.HTML }, ref error);
			return ret;
		}

		public void HandleTouch()
		{
			this.NavigationController.PushViewController(new EditViewController(CurrNote), true);
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			//Refresh the note on the screen
			CurrNote = NoteDatabase.GetNoteById(CurrNote.Id);
			UpdateView();
			isrunning = true;
		}

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);
			isrunning = false;
		}

		bool isrunning = true;
		public void StayUpdated()
		{
			while (isrunning)
			{
				Thread.Sleep(5000);
				InvokeOnMainThread(() =>
				{
					CurrNote = NoteDatabase.GetNoteById(CurrNote.Id);
					UpdateView();
				});
			}
		}

		public void UpdateView()
		{
			Title = CurrNote.Title;
			ContentView.AttributedText = MarkdownToText(CurrNote.Content);
		}

	}


}
