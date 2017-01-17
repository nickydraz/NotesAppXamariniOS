using Foundation;
using System;
using System.Collections.Generic;
using System.Globalization;
using UIKit;

namespace NotesSingle
{
    public class TableSource : UITableViewSource
	{

		private List<Note> _tableItems;
		private string _cellIdentifier = "TableCell";
		CustomViewController _owner;

		public TableSource(List<Note> items, CustomViewController owner)
		{
			_tableItems = items;
			this._owner = owner;

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
			return _tableItems.Count;
		}

		/// <summary>
		/// Called when a row is touched
		/// </summary>
		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			//UIAlertController okAlertController = UIAlertController.Create(tableItems[indexPath.Row].Title, tableItems[indexPath.Row].Content, UIAlertControllerStyle.Alert);
			//okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
			var detailController = new NoteDetailViewController(NoteDatabase.GetNoteById(_tableItems[indexPath.Row].Id));
			_owner.NavigationController.PushViewController(detailController, true);

			tableView.DeselectRow(indexPath, true);
		}

		/// <summary>
		/// Called by the TableView to get the actual UITableViewCell to render for the particular row
		/// </summary>
		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			UITableViewCell cell = tableView.DequeueReusableCell(_cellIdentifier);
			string item = _tableItems[indexPath.Row].Title;

			//---- if there are no cells to reuse, create a new one
			if (cell == null)
			{ cell = new UITableViewCell(UITableViewCellStyle.Subtitle, _cellIdentifier); }

			cell.TextLabel.Text = item;
			cell.DetailTextLabel.Text = _tableItems[indexPath.Row].LastChanged.ToString(CultureInfo.InvariantCulture);

			return cell;
		}

		//		public override string TitleForHeader (UITableView tableView, nint section)
		//		{
		//			return " ";
		//		}

	}
}