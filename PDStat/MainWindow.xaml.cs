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
		List<string> games = new List<string>();
		List<string> songs;
		List<string> diff;
		int currentAttempt = 0;

		public MainWindow()
		{
			InitializeComponent();

			using (PDStatContext db = new PDStatContext()) { } //no-op for initializing
		}

		private void gamesBox_Loaded(object sender, RoutedEventArgs e)
		{
			using (PDStatContext db = new PDStatContext())
			{
				foreach (var g in db.Games.OrderBy(g => g.Id).ToList())
				{
					games.Add(g.Name);
				}
			}
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

				if (gamesBox.SelectedItem.ToString() == "Project Diva (1)" ||
					gamesBox.SelectedItem.ToString() == "Project Diva DT")
				{
					diff.Remove("Extreme");
				}
				foreach (var s in db.Songs.Where(g => g.Game == gamesBox.SelectedItem.ToString()).OrderBy(s => s.Id))
				{
					songs.Add(s.Title);
				}
			}
			diffBox.ItemsSource = diff;
			songBox.ItemsSource = songs;
			songBox.IsEnabled = true;

			if (styleBox.SelectedItem != null && styleBox.SelectedItem.ToString() == "Auto")
			{
				ChangeStyle();
			}
		}

		private void songBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			diffBox.ItemsSource = diff;
			if (diffBox.IsEnabled && diffBox.SelectedItem != null)
			{
				LoadBestAttempt();
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

			if (IsOfFFamily(gamesBox.SelectedItem.ToString()))
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

			LoadBestAttempt();
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
			ranks.Remove("Unfinished");
			rankBox.ItemsSource = ranks;
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

		private void attemptCounter_Loaded(object sender, RoutedEventArgs e)
		{
			IncrementAttempt(ref currentAttempt);
		}


		private void SubmitBtn_Click(object sender, RoutedEventArgs e)
		{
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
					PdStat stat = new PdStat(){
						Song = (from s in db.Songs where s.Game == gamesBox.SelectedItem.ToString() && s.Title == songBox.SelectedItem.ToString() select s.Id).First(),
						diff = (from d in db.Difficulties where d.Name == diffBox.SelectedItem.ToString() select d).First(),
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
						r = (from ra in db.Ranks where ra.Name == rankBox.SelectedItem.ToString() select ra).First(),
					};
					db.PDStats.Add(stat);
					
					try
					{
						db.SaveChanges();
						LoadBestAttempt();
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
		}

		private void ResetBtn_Click(object sender, RoutedEventArgs e)
		{
			using (PDStatContext db = new PDStatContext())
			{
				PdStat stat = new PdStat()
				{
					Song = (from s in db.Songs where s.Game == gamesBox.SelectedItem.ToString() && s.Title == songBox.SelectedItem.ToString() select s.Id).First(),
					diff = (from d in db.Difficulties where d.Name == diffBox.SelectedItem.ToString() select d).First(),
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
					r = (from ra in db.Ranks where ra.Name == "Unfinished" select ra).First(),
				};
				db.PDStats.Add(stat);
				db.SaveChanges();
			}

			IncrementAttempt(ref currentAttempt);
		}

		private void AwfulBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			int g;
			int c;
			if (AwfulBox.Text == "0" && BadBox.Text == "0" && SafeBox.Text == "0" &&
				Int32.TryParse(GoodBox.Text, out g) && g > 0 && Int32.TryParse(CoolBox.Text, out c) && c > 0)
			{
				if (IsOfFFamily(gamesBox.SelectedItem.ToString()))
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


		private void ChangeStyle()
		{
			string selection = styleBox.SelectedItem.ToString();
			if (selection == EnumHelper.GetEnumDescription(ScoreStyle.Auto))
			{
				string game = gamesBox.SelectedItem.ToString();
				if (game == "Project Diva f (Vita)" ||
					game == "Project Diva F (PS3)")
				{
					selection = EnumHelper.GetEnumDescription(ScoreStyle.EnglishF);
				}
				else if (game == "Project Diva f 2nd (Vita)" ||
						 game == "Project Diva F 2nd (PS3)")
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

		private void LoadBestAttempt()
		{
			using (PDStatContext db = new PDStatContext())
			{
				if (songBox.SelectedItem != null)
				{
					int song = (from s in db.Songs where s.Game == gamesBox.SelectedItem.ToString() && s.Title == songBox.SelectedItem.ToString() select s.Id).First();
					PdStat stat;
					try
					{
						stat = (from s in db.PDStats where s.Song == song && s.Difficulty == diffBox.SelectedItem.ToString() select s).OrderByDescending(s => s.Score).First();
					
						bestCool.Content = stat.Cool;
						bestGood.Content = stat.Good;
						bestSafe.Content = stat.Safe;
						bestBad.Content = stat.Bad;
						bestAwful.Content = stat.Awful;

						if (IsOfFFamily(gamesBox.SelectedItem.ToString()))
						{
							bestCT.Content = stat.ChanceTimeBonus ? "Clear" : "Not clear";
							bestTZ1.Content = stat.TechZoneBonus1 ? "Clear" : "Not clear";
							bestTZ2.Content = diffBox.SelectedItem.ToString() == "Easy" ? String.Empty : (stat.TechZoneBonus2 ? "Clear" : "Not clear");
						}
						else
						{
							bestCT.Content = String.Empty;
							bestTZ1.Content = String.Empty;
							bestTZ2.Content = String.Empty;
						}
						bestScore.Content = stat.Score;
						bestRank.Content = stat.Rank;

						currentAttempt = (from s in db.PDStats where s.Song == song && s.Difficulty == diffBox.SelectedItem.ToString() select s).OrderByDescending(s => s.Attempt).First().Attempt;
						IncrementAttempt(ref currentAttempt);
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
						IncrementAttempt(ref currentAttempt);
					}
				}
			}
		}

		private bool IsOfFFamily(string game)
		{
			return (game == "Project Diva f (Vita)" || game == "Project Diva F (PS3)" ||
					game == "Project Diva f 2nd (Vita)" || game == "Project Diva F 2nd (PS3)");
		}
	}
}
