using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data.Entity;

namespace PDStat
{
	/// <summary>
	/// Interaction logic for ManageEditSongs.xaml
	/// </summary>
	public partial class ManageEditSongs : Window
	{
		public ManageEditSongs()
		{
			InitializeComponent();
		}

		private void gamesBox_Loaded(object sender, RoutedEventArgs e)
		{
			gamesBox.ItemsSource = MainWindow.games;
			gamesBox.SelectedIndex = 0;
		}

		private void gamesBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			LoadEditSongs();
		}

		private void addEditButton_Click(object sender, RoutedEventArgs e)
		{
			if (editTitleBox.Text != null || editTitleBox.Text != String.Empty)
			{
				using (PDStatContext db = new PDStatContext())
				{
					int i = db.Songs.OrderByDescending(s => s.Id).Select(s => s.Id).First();
					db.Songs.Add(new Song() { Id = i++, Mode = "Edit", Game = gamesBox.SelectedItem.ToString(), Title = editTitleBox.Text});
					db.SaveChanges();
				}

				LoadEditSongs();
			}
		}

		private void removeEditSongButton_Click(object sender, RoutedEventArgs e)
		{
			using (PDStatContext db = new PDStatContext())
			{
				db.Songs.Remove(db.Songs.Where(s => s.Game == gamesBox.SelectedItem.ToString() && s.Mode == "Edit" && s.Title == existingEditSongs.SelectedItem.ToString()).First());
			}
			LoadEditSongs();
		}

		private void LoadEditSongs()
		{
			if (gamesBox.SelectedItem != null)
			{
				using (PDStatContext db = new PDStatContext())
				{
					List<string> editSongs = new List<string>();
					try
					{
						editSongs = db.Songs.Where(s => s.Game == gamesBox.SelectedItem.ToString() && s.Mode == "Edit").OrderBy(s => s.Id).Select(s => s.Title).ToList();
						existingEditSongs.ItemsSource = editSongs;
						existingEditSongs.IsEnabled = true;
						removeEditSongButton.IsEnabled = true;
					}
					catch (InvalidOperationException)
					{
						existingEditSongs.ItemsSource = null;
						existingEditSongs.IsEnabled = false;
						removeEditSongButton.IsEnabled = false;
					}
				}
			}
		}
	}
}
