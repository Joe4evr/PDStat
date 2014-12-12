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
using System.Data.Entity.Validation;

namespace PDStat
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public static List<string> games = new List<string>();
		List<string> songs;
		List<string> diff;
		int currentAttempt = 0;

		public MainWindow()
		{
			InitializeComponent();

			using (PDStatContext db = new PDStatContext()) { } //no-op for initializing
		}

		private async void gamesBox_Loaded(object sender, RoutedEventArgs e)
		{
			using (PDStatContext db = new PDStatContext())
			{
				foreach (var g in await db.Games.OrderBy(g => g.Id).ToListAsync())
				{
					games.Add(g.Name);
				}
			}
			gamesBox.ItemsSource = games;
			gamesBox.SelectedIndex = 0;
		}

		private async void gamesBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			await GamesBoxChangedAsync();
		}

		private async void diffBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			await DiffBoxChangedAsync();
		}

		private async void songBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			await LoadBestAttemptAsync();
		}

		private async void rankBox_Loaded(object sender, RoutedEventArgs e)
		{
			List<string> ranks = new List<string>();
			using (PDStatContext db = new PDStatContext())
			{
				foreach (var r in await db.Ranks.OrderBy(r => r.Id).ToListAsync())
				{
					ranks.Add(r.Name);
				}
			}
			ranks.Remove("Unfinished");
			rankBox.ItemsSource = ranks;
		}

		private async void styleBox_Loaded(object sender, RoutedEventArgs e)
		{
			List<string> styles = new List<string>();
			using (PDStatContext db = new PDStatContext())
			{
				styles = await db.ScoreStyle.OrderBy(s => s.Id).Select(s => s.StyleName).ToListAsync();
			}
			styles.Insert(0, "Auto");
			styleBox.ItemsSource = styles;
			styleBox.SelectedIndex = 0;
		}

		private async void styleBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			await ChangeStyle();
		}

		private void attemptCounter_Loaded(object sender, RoutedEventArgs e)
		{
			IncrementAttempt(ref currentAttempt);
		}


		private async void SubmitBtn_Click(object sender, RoutedEventArgs e)
		{
			await SubmitScoreAsync();
		}

		private async void ResetBtn_Click(object sender, RoutedEventArgs e)
		{
			using (PDStatContext db = new PDStatContext())
			{
				db.PDStats.Add(new PdStat()
				{
					s = await (from s in db.Songs where s.Game == gamesBox.SelectedItem.ToString() && s.Title == songBox.SelectedItem.ToString() select s).FirstAsync(),
					diff = await (from d in db.Difficulties where d.Name == diffBox.SelectedItem.ToString() select d).FirstAsync(),
					Attempt = currentAttempt,
					Date = DateTime.Today.Date,
					Cool = 0,
					Good = 0,
					Safe = 0,
					Bad = 0,
					Awful = 0,
					ChanceTimeBonus = false,
					TechZoneBonus1 = false,
					TechZoneBonus2 = false,
					Score = 0,
					r = await (from ra in db.Ranks where ra.Name == "Unfinished" select ra).FirstAsync(),
				});
				await db.SaveChangesAsync();
			}

			IncrementAttempt(ref currentAttempt);
		}

		private async void manageEdits_Click(object sender, RoutedEventArgs e)
		{
			ManageEditSongs mes = new ManageEditSongs();
			mes.ShowDialog();
			await ReloadSongs();
		}

		private void AwfulBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			int g;
			int c; //yet another subject for C# 6 refactoring
			if (AwfulBox.Text == "0" && BadBox.Text == "0" && SafeBox.Text == "0" &&
				Int32.TryParse(GoodBox.Text, out g) && g > 0 && Int32.TryParse(CoolBox.Text, out c) && c > 0)
			{
				//maxCombo.Text = GoodBox.Text + CoolBox.Text;
				if (Helpers.IsOfFFamily(gamesBox.SelectedItem.ToString()))
				{
					CTChk.IsChecked = true;
					TZ1Chk.IsChecked = true;
					TZ2Chk.IsChecked = true;
				}
				rankBox.SelectedItem = "Perfect";
			}
		}

		private void debugLabel_Loaded(object sender, RoutedEventArgs e)
		{
#if (DEBUG)
			debugLabel.Content = "Debug";
#endif
		}


		private void IncrementAttempt(ref int attempt)
		{
			attemptCounter.Content = String.Format("#{0}", attempt+=1);
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

		private async Task GamesBoxChangedAsync()
		{
			diff = new List<string>();
			songs = new List<string>();

			using (PDStatContext db = new PDStatContext())
			{
				foreach (var d in await db.Difficulties.OrderBy(d => d.Id).ToListAsync())
				{
					diff.Add(d.Name);
				}
				foreach (var s in await db.Songs.OrderBy(s => s.Id).Where(s => s.Game == gamesBox.SelectedItem.ToString()).Select(s => s.Title).ToListAsync())
				{
					songs.Add(s);
				}
			}

			if (gamesBox.SelectedItem.ToString() == Helpers.PD1 || gamesBox.SelectedItem.ToString() == Helpers.PDDT)
			{
				diff.Remove("Extreme");
				diff.Remove("Tutorial");
			}

			if (styleBox.SelectedItem != null && styleBox.SelectedItem.ToString() == "Auto")
			{
				await ChangeStyle();
			}

			diffBox.ItemsSource = diff;
			diffBox.IsEnabled = true;
			await LoadBestAttemptAsync();
		}

		private async Task DiffBoxChangedAsync()
		{
			CTChk.IsChecked = false;
			TZ1Chk.IsChecked = false;
			TZ2Chk.IsChecked = false;

			if (Helpers.IsOfFFamily(gamesBox.SelectedItem.ToString()))
			{
				CTChk.IsEnabled = true;
				TZ1Chk.IsEnabled = true;
				TZ2Chk.IsEnabled = !(diffBox.SelectedItem.ToString() == "Easy");
			}
			else
			{
				CTChk.IsEnabled = false;
				TZ1Chk.IsEnabled = false;
				TZ2Chk.IsEnabled = false;
			}

			await ReloadSongs();
			songBox.IsEnabled = true;
			await LoadBestAttemptAsync();
		}

		private async Task LoadBestAttemptAsync()
		{
			using (PDStatContext db = new PDStatContext())
			{
				if (gamesBox.SelectedItem != null && diffBox.SelectedItem != null && songBox.SelectedItem != null)
				{
					int song = await (from s in db.Songs where s.Game == gamesBox.SelectedItem.ToString() && s.Title == songBox.SelectedItem.ToString() select s.Id).FirstAsync();
					try
					{
						PdStat stat = await (from s in db.PDStats where s.Song == song && s.Difficulty == diffBox.SelectedItem.ToString() select s).OrderByDescending(s => s.Score).FirstAsync();
					
						bestCool.Content = stat.Cool;
						bestGood.Content = stat.Good;
						bestSafe.Content = stat.Safe;
						bestBad.Content = stat.Bad;
						bestAwful.Content = stat.Awful;

						if (Helpers.IsOfFFamily(stat.s.Game))
						{
							bestCT.Content = stat.ChanceTimeBonus ? "Clear" : "Not clear";
							bestTZ1.Content = stat.TechZoneBonus1 ? "Clear" : "Not clear";
							bestTZ2.Content = stat.Difficulty == "Easy" ? String.Empty : (stat.TechZoneBonus2 ? "Clear" : "Not clear");
						}
						else
						{
							bestCT.Content = String.Empty;
							bestTZ1.Content = String.Empty;
							bestTZ2.Content = String.Empty;
						}
						bestScore.Content = stat.Score;
						bestRank.Content = stat.Rank;

						PdStat s2 = await (from s in db.PDStats where s.Song == song && s.Difficulty == stat.Difficulty select s).OrderByDescending(s => s.Attempt).FirstAsync();
						currentAttempt = s2.Attempt;
					}
					catch (InvalidOperationException)
					{
						bestCool.Content = String.Empty;
						bestGood.Content = String.Empty;
						bestSafe.Content = String.Empty;
						bestBad.Content = String.Empty;
						bestAwful.Content = String.Empty;

						bestCT.Content = String.Empty;
						bestTZ1.Content = String.Empty;
						bestTZ2.Content = String.Empty;

						bestScore.Content = String.Empty;
						bestRank.Content = String.Empty;

						currentAttempt = 0;
					}

					IncrementAttempt(ref currentAttempt);
					CoolBox.Focus();
				}
			}
		}

		private async Task SubmitScoreAsync()
		{
			SubmitBtn.IsEnabled = false;
			short cools;
			short goods;
			short safes;
			short bads;
			short awfuls;
			int score; //This can be nicely refactored only once C# 6 is out =(
			if (Int16.TryParse(CoolBox.Text, out cools) && Int16.TryParse(GoodBox.Text, out goods) &&
				Int16.TryParse(SafeBox.Text, out safes) && Int16.TryParse(BadBox.Text, out bads) &&
				Int16.TryParse(AwfulBox.Text, out awfuls) && Int32.TryParse(ScoreBox.Text, out score))
			{
				using (PDStatContext db = new PDStatContext())
				{
					PdStat stat = db.PDStats.Add(new PdStat()
					{
						s = await (from s in db.Songs where s.Game == gamesBox.SelectedItem.ToString() && s.Title == songBox.SelectedItem.ToString() select s).FirstAsync(),
						diff = await (from d in db.Difficulties where d.Name == diffBox.SelectedItem.ToString() select d).FirstAsync(),
						Attempt = currentAttempt,
						Date = DateTime.Today.Date,
						Cool = cools,
						Good = goods,
						Safe = safes,
						Bad = bads,
						Awful = awfuls,
						ChanceTimeBonus = CTChk.IsChecked ?? false,
						TechZoneBonus1 = TZ1Chk.IsChecked ?? false,
						TechZoneBonus2 = TZ2Chk.IsChecked ?? false,
						Score = score,
						r = await (from ra in db.Ranks where ra.Name == rankBox.SelectedItem.ToString() select ra).FirstAsync(),
					});

					try
					{
						await db.SaveChangesAsync();
						await LoadBestAttemptAsync();
						ClearScores();
					}
					catch (DbEntityValidationException)
					{
						List<DbEntityValidationResult> err = db.GetValidationErrors().ToList();
						MessageBoxResult d = System.Windows.MessageBoxResult.None;
						do
						{
							d = System.Windows.MessageBox.Show(err.First().ValidationErrors.First().ErrorMessage, "Invalid score submitted", MessageBoxButton.OK);
						} while (d != System.Windows.MessageBoxResult.OK);
					}
				}
			}
			SubmitBtn.IsEnabled = true;
		}

		private async Task ReloadSongs()
		{
			songs = new List<string>();

			string mode = (diffBox.SelectedItem != null && (diffBox.SelectedItem.ToString() == "Tutorial" || diffBox.SelectedItem.ToString() == "Edit")) ? diffBox.SelectedItem.ToString() : "Default";

			using (PDStatContext db = new PDStatContext())
			{
				foreach (var s in await db.Songs.Where(g => g.Game == gamesBox.SelectedItem.ToString() && g.Mode == mode).OrderBy(s => s.Id).ToListAsync())
				{
					songs.Add(s.Title);
				}
			}

			songBox.ItemsSource = songs;
		}

		private async Task ChangeStyle()
		{
			string selection = styleBox.SelectedItem.ToString();
			if (selection == "Auto")
			{
				switch (gamesBox.SelectedItem.ToString())
				{
					case Helpers.PDFV:
					case Helpers.PDFP:
						selection = "English (F)";
						break;
					case Helpers.PDF2V:
					case Helpers.PDF2P:
						selection = "English (F 2nd)";
						break;
					default:
						selection = "Japanese";
						break;
				}
			}
			
			
			ScoreStyle ss;
			using (PDStatContext db = new PDStatContext())
			{
				ss = await (from s in db.ScoreStyle where s.StyleName == selection select s).FirstAsync();
			}
			coolLabel.Content = ss.CoolStyle;
			goodLabel.Content = ss.GoodStyle;
			safeLabel.Content = ss.SafeStyle;
			badLabel.Content = ss.BadStyle;
			awfulLabel.Content = ss.AwfulStyle;
			
			List<string> ranks = new List<string>();
			ranks.Add(ss.FRank);
			ranks.Add(ss.LRank);
			ranks.Add(ss.SRank);
			ranks.Add(ss.GRank);
			ranks.Add(ss.ERank);
			ranks.Add(ss.PRank);
			rankBox.ItemsSource = ranks;
		}
	}
}
