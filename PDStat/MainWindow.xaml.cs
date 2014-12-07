using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.Entity;

namespace PDStat
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		List<string> games = new List<string>();
		List<string> songs;
		List<string> diff;
		//string connectionString;
		//string statDB = System.Configuration.ConfigurationManager.ConnectionStrings["statDB"].ConnectionString;
		//int currentAttempt = 0;

		public MainWindow()
		{
			InitializeComponent();

			using (PDStatContext db = new PDStatContext()) { } //no-op
		}

		private void gamesBox_Loaded(object sender, RoutedEventArgs e)
		{
			EnumHelper.EnumToList<Game>().ForEach(delegate(Game g)
			{
				games.Add(EnumHelper.GetEnumDescription(g));
			});
			gamesBox.ItemsSource = games;
			gamesBox.SelectedIndex = 0;
		}

		private void gamesBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			diff = new List<string>();
			songs = new List<string>();

			using (PDStatContext db = new PDStatContext())
			{
				foreach (var d in db.Difficulties.OrderBy(d => d.Id).ToList())
				{
					diff.Add(d.Name);
				}

				if (gamesBox.SelectedItem.ToString() == EnumHelper.GetEnumDescription(Game.ProjectDiva1) ||
					gamesBox.SelectedItem.ToString() == EnumHelper.GetEnumDescription(Game.ProjectDivaDT))
				{
					diff.Remove("Extreme");
					foreach (var s in db.Pd1Songs.OrderBy(s => s.Id))
					{
						songs.Add(s.Title);
					}
				}
				else if (gamesBox.SelectedItem.ToString() == EnumHelper.GetEnumDescription(Game.ProjectDiva2) ||
						 gamesBox.SelectedItem.ToString() == EnumHelper.GetEnumDescription(Game.ProjectDivaDT2))
				{
					foreach (var s in db.Pd2Songs.OrderBy(s => s.Id))
					{
						songs.Add(s.Title);
					}
				}
				else if (gamesBox.SelectedItem.ToString() == EnumHelper.GetEnumDescription(Game.ProjectDivaX) ||
						 gamesBox.SelectedItem.ToString() == EnumHelper.GetEnumDescription(Game.ProjectDivaDTX))
				{
					foreach (var s in db.PdXSongs.OrderBy(s => s.Id))
					{
						songs.Add(s.Title);
					}
				}
				else if (gamesBox.SelectedItem.ToString() == EnumHelper.GetEnumDescription(Game.ProjectDivaFVita) ||
						 gamesBox.SelectedItem.ToString() == EnumHelper.GetEnumDescription(Game.ProjectDivaFPS3))
				{
					foreach (var s in db.PdFSongs.OrderBy(s => s.Id))
					{
						songs.Add(s.Title);
					}
				}
				else if (gamesBox.SelectedItem.ToString() == EnumHelper.GetEnumDescription(Game.ProjectDivaF2Vita) ||
						 gamesBox.SelectedItem.ToString() == EnumHelper.GetEnumDescription(Game.ProjectDivaF2PS3))
				{
					foreach (var s in db.PdF2Songs.OrderBy(s => s.Id))
					{
						songs.Add(s.Title);
					}
				}
			}
			diffBox.ItemsSource = diff;
			songBox.ItemsSource = songs;
			songBox.IsEnabled = true;

			//if (styleBox.SelectedItem.ToString() == "Auto")
			//{
			//	ChangeStyle();
			//}
		}

		private void songBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			diffBox.ItemsSource = diff;
			if (diffBox.IsEnabled)
			{
				//loadBestAttempt();
			}
			else
			{
				diffBox.IsEnabled = true;
			}
		}

		private void diffBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			CTChk.IsChecked = false;
			TZ1Chk.IsChecked = false;
			TZ2Chk.IsChecked = false;

			if (gamesBox.SelectedItem.ToString() != EnumHelper.GetEnumDescription(Game.ProjectDivaFPS3) ||
				gamesBox.SelectedItem.ToString() != EnumHelper.GetEnumDescription(Game.ProjectDivaFVita) ||
				gamesBox.SelectedItem.ToString() != EnumHelper.GetEnumDescription(Game.ProjectDivaF2PS3) ||
				gamesBox.SelectedItem.ToString() != EnumHelper.GetEnumDescription(Game.ProjectDivaF2Vita))
			{
				CTChk.IsEnabled = false;
				TZ1Chk.IsEnabled = false;
				TZ2Chk.IsEnabled = false;
			}
			else
			{
				CTChk.IsEnabled = true;
				TZ1Chk.IsEnabled = true;
				//TZ2Chk.IsEnabled = !(diffBox.SelectedItem.ToString() == Difficulty.Easy.ToString());
			}

			//loadBestAttempt();
		}

		private void rankBox_Loaded(object sender, RoutedEventArgs e)
		{
			List<string> ranks = new List<string>();
			using (PDStatContext db = new PDStatContext())
			{
				foreach (var r in db.Ranks.OrderBy(r => r.Id).ToList())
				{
					ranks.Add(r.Name);
				}
			}
			rankBox.ItemsSource = ranks;
		}

		private void SubmitBtn_Click(object sender, RoutedEventArgs e)
		{
			short cools;
			short goods;
			short safes;
			short bads;
			short awfuls;
			int score;
			if (Int16.TryParse(CoolBox.Text, out cools) && Int16.TryParse(GoodBox.Text, out goods) &&
				Int16.TryParse(SafeBox.Text, out safes) && Int16.TryParse(BadBox.Text, out bads) &&
				Int16.TryParse(AwfulBox.Text, out awfuls) && Int32.TryParse(ScoreBox.Text, out score))
			{
				//InsertStat(gamesBox.SelectedItem.ToString(), songBox.SelectedItem.ToString(), diffBox.SelectedItem.ToString(),
							//currentAttempt, DateTime.Today, cools, goods, safes, bads, awfuls,
							//CTChk.IsChecked, TZ1Chk.IsChecked, TZ2Chk.IsChecked, score, RankBox.SelectedItem.ToString());

				ClearScores();

				//IncrementAttempt(ref currentAttempt);
			}
		}

		private void ResetBtn_Click(object sender, RoutedEventArgs e)
		{
			//InsertStat(gamesBox.SelectedItem.ToString(), songBox.SelectedItem.ToString(), diffBox.SelectedItem.ToString(),
						//currentAttempt, DateTime.Today, 0, 0, 0, 0, 0, false, false, false, 0, "Unfinished");

			//IncrementAttempt(ref currentAttempt);
		}

		private void styleBox_Loaded(object sender, RoutedEventArgs e)
		{
			var styles = new List<string>();
			EnumHelper.EnumToList<ScoreStyle>().ForEach(delegate (ScoreStyle s)
			{
				styles.Add(EnumHelper.GetEnumDescription(s));
			});
			styleBox.ItemsSource = styles;
			//TODO: Set to last setting/user preference
			//for now, default will be Auto
			styleBox.SelectedIndex = 0;
			
		}

		private void styleBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ChangeStyle();
		}



		private void ChangeStyle()
		{
			string selection = styleBox.SelectedItem.ToString();
			if (selection == EnumHelper.GetEnumDescription(ScoreStyle.Auto))
			{
				string game = gamesBox.SelectedItem.ToString();
				if (game == EnumHelper.GetEnumDescription(Game.ProjectDivaFVita) ||
					game == EnumHelper.GetEnumDescription(Game.ProjectDivaFPS3))
				{
					selection = EnumHelper.GetEnumDescription(ScoreStyle.EnglishF);
				}
				else if (game == EnumHelper.GetEnumDescription(Game.ProjectDivaF2Vita) ||
						 game == EnumHelper.GetEnumDescription(Game.ProjectDivaF2PS3))
				{
					selection = EnumHelper.GetEnumDescription(ScoreStyle.EnglishF2);
				}
				else
				{
					selection = EnumHelper.GetEnumDescription(ScoreStyle.Japanese);
				}
			}
			
			if (selection == EnumHelper.GetEnumDescription(ScoreStyle.EnglishF))
			{
				goodLabel.Content = "GOOD";
				badLabel.Content = "BAD";
				awfulLabel.Content = "AWFUL";
					

			}
			else if (selection == EnumHelper.GetEnumDescription(ScoreStyle.EnglishF2))
			{
				goodLabel.Content = "GOOD";
				badLabel.Content = "BAD";
				awfulLabel.Content = "MISS";
			}
			else //Japanese
			{
				goodLabel.Content = "FINE";
				badLabel.Content = "SAD";
				awfulLabel.Content = "WORST";
		 
			}
		}

		private void IncrementAttempt(ref int currentAttempt)
		{
			attemptCounter.Content = String.Format("#{0}", currentAttempt+=1);
		}

		private void ClearScores()
		{
			CoolBox.Text = String.Empty;
			GoodBox.Text = String.Empty;
			SafeBox.Text = String.Empty;
			BadBox.Text = String.Empty;
			AwfulBox.Text = String.Empty;

			CTChk.IsChecked = false;
			TZ1Chk.IsChecked = false;
			TZ2Chk.IsChecked = false;

			ScoreBox.Text = String.Empty;
		}
	}
}
