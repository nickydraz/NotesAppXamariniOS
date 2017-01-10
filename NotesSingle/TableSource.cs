using System;
using System.Collections.Generic;
using System.IO;
using Foundation;
using UIKit;

namespace NotesSingle
{
	public class TableSource : UITableViewSource
	{

		protected List<Note> tableItems;
		protected string cellIdentifier = "TableCell";
		CustomViewController owner;

		public TableSource(List<Note> items, CustomViewController owner)
		{
			tableItems = items;
			this.owner = owner;

		}

		/// <summary>
		/// Called by the TableView to determine how many sections(groups) there are.
		/// </summary>
		public override nint NumberOfSections(UITableView tableView)
		{
			return 1;
		}

		/// <summary>
		/// Called by the TableView to determine how many cells to create for that particular section.
		/// </summary>
		public override nint RowsInSection(UITableView tableview, nint section)
		{
			return tableItems.Count;
		}

		/// <summary>
		/// Called when a row is touched
		/// </summary>
		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			//UIAlertController okAlertController = UIAlertController.Create(tableItems[indexPath.Row].Title, tableItems[indexPath.Row].Content, UIAlertControllerStyle.Alert);
			//okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
			NoteDetailViewController detailController = new NoteDetailViewController(NoteDatabase.GetNoteById(tableItems[indexPath.Row].Id));
			owner.NavigationController.PushViewController(detailController, true);

			tableView.DeselectRow(indexPath, true);
		}

		/// <summary>
		/// Called by the TableView to get the actual UITableViewCell to render for the particular row
		/// </summary>
		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			UITableViewCell cell = tableView.DequeueReusableCell(cellIdentifier);
			string item = tableItems[indexPath.Row].Title;

			//---- if there are no cells to reuse, create a new one
			if (cell == null)
			{ cell = new UITableViewCell(UITableViewCellStyle.Subtitle, cellIdentifier); }

			cell.TextLabel.Text = item;
			cell.DetailTextLabel.Text = tableItems[indexPath.Row].LastChanged.ToString();

			return cell;
		}

		//		public override string TitleForHeader (UITableView tableView, nint section)
		//		{
		//			return " ";
		//		}

	}
}