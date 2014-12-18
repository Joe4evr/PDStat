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
using System.Data.Entity.Validation;

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
			if (!String.IsNullOrEmpty(editTitleBox.Text))
			{
				using (PDStatContext db = new PDStatContext())
				{
					int i = db.Songs.OrderByDescending(s => s.Id).Select(s => s.Id).First();
					Game G = db.Games.Where(s => s.Name == gamesBox.SelectedItem.ToString()).Select(s => s).First();
					db.Songs.Add(new Song() { Id = i++, Mode = "Edit", g = G, JPTitle = editTitleBox.Text});

					try
					{
						db.SaveChanges();
					}
					catch (DbEntityValidationException)
					{
						List<DbEntityValidationResult> err = db.GetValidationErrors().ToList();
						MessageBoxResult d = System.Windows.MessageBoxResult.None;
						do
						{
							d = System.Windows.MessageBox.Show(err.First().ValidationErrors.First().ErrorMessage, "Invalid song submitted", MessageBoxButton.OK);
						} while (d != System.Windows.MessageBoxResult.OK);
					}
				}

				LoadEditSongs();
			}
		}

		private void removeEditSongButton_Click(object sender, RoutedEventArgs e)
		{
			using (PDStatContext db = new PDStatContext())
			{
				db.Songs.Remove(db.Songs.Where(s => s.Game == gamesBox.SelectedItem.ToString() && s.Mode == "Edit" && s.JPTitle == existingEditSongs.SelectedItem.ToString()).First());
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
						editSongs = db.Songs.Where(s => s.Game == gamesBox.SelectedItem.ToString() && s.Mode == "Edit").OrderBy(s => s.Id).Select(s => s.JPTitle).ToList();
						if (editSongs.Count > 0)
						{
							existingEditSongs.ItemsSource = editSongs;
							existingEditSongs.IsEnabled = true;
							removeEditSongButton.IsEnabled = true;
						}
						else
						{
							existingEditSongs.IsEnabled = false;
							removeEditSongButton.IsEnabled = false;
						}
					}
					catch (InvalidOperationException)
					{
						existingEditSongs.IsEnabled = false;
						removeEditSongButton.IsEnabled = false;
					}
				}
			}
		}
	}
}
