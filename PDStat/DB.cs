﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using Newtonsoft.Json;

namespace PDStat
{
	public class Song
	{
		[Key]
		public int Id { get; set; }

		[StringLength(30)]
		public string Game { get; set; }

        [Required]
		[StringLength(150)]
		public string JPTitle { get; set; }

		[StringLength(150)]
		public string RomajiTitle { get; set; }

		[StringLength(150)]
		public string ENTitle { get; set; }

		[StringLength(150)]
		public string LocalizedTitle { get; set; }

		[ForeignKey("Game")]
		public virtual Game g { get; set; }

		public string Mode { get; set; }
	}

	public class PdStat
	{
		[Key]
		[Column(Order = 1)]
		public int Song { get; set; }

		[Key]
		[Column(Order = 2)]
		public string Difficulty { get; set; }

		[Key]
		[Column(Order = 3)]
		public int Attempt { get; set; }

		[ForeignKey("Song")]
		public virtual Song s { get; set; }

		[ForeignKey("Difficulty")]
		public virtual Difficulty diff { get; set; }

		public DateTime Date { get; set; }

		public short BestCombo { get; set; }
		public short Cool { get; set; }
		public short Good { get; set; } //Fine
		public short Safe { get; set; }
		public short Bad { get; set; } //Sad
		public short Awful { get; set; } //Miss, Worst
		public int Score { get; set; }

		public string Rank { get; set; }

		[ForeignKey("Rank")]
		public virtual Rank r { get; set; }

		public bool ChanceTimeBonus { get; set; }
		public bool TechZoneBonus1 { get; set; }
		public bool TechZoneBonus2 { get; set; }
	}

	public class Difficulty
	{
		public int Id { get; set; }

		[Key]
		[StringLength(10)]
		public string Name { get; set; }
	}

	public class Rank
	{
		public byte Id { get; set; }

		[Key]
		[StringLength(15)]
		public string Name { get; set; }
	}

	public class Game
	{
		public int Id { get; set; }

		[Key]
		[StringLength(30)]
		public string Name { get; set; }
		//[Description("Project Diva 1")]
		//ProjectDiva1,
		//[Description("Project Diva 2nd")]
		//ProjectDiva2,
		//[Description("Project Diva Extend")]
		//ProjectDivaX,
		//[Description("Project Diva DT")]
		//ProjectDivaDT,
		//[Description("Project Diva DT 2nd")]
		//ProjectDivaDT2,
		//[Description("Project Diva DT Extend")]
		//ProjectDivaDTX,
		//[Description("Project Diva f (Vita)")]
		//ProjectDivaFVita,
		//[Description("Project Diva F (PS3)")]
		//ProjectDivaFPS3,
		//[Description("Project Diva f 2nd (Vita)")]
		//ProjectDivaF2Vita,
		//[Description("Project Diva F 2nd (PS3)")]
		//ProjectDivaF2PS3,
	}

	public class ScoreStyle
	{
		public int Id { get; set; }

		[Key]
		public string StyleName { get; set; }

		public string CoolStyle { get; set; }
		public string GoodStyle { get; set; }
		public string SafeStyle { get; set; }
		public string BadStyle { get; set; }
		public string AwfulStyle { get; set; }
		public string PRank { get; set; }
		public string ERank { get; set; }
		public string GRank{ get; set; }
		public string SRank{ get; set; }
		public string LRank { get; set; }
		public string FRank { get; set; }
	}

	public class PDStatContext : DbContext
	{
		static PDStatContext()
		{
			// Database initialize
			Database.SetInitializer<PDStatContext>(new DbInitializer());
			using (PDStatContext db = new PDStatContext())
				db.Database.Initialize(false);
		}

		protected override DbEntityValidationResult ValidateEntity(DbEntityEntry entityEntry, IDictionary<Object, Object> items)
		{
			DbEntityValidationResult Errors = new DbEntityValidationResult(entityEntry, new List<DbValidationError>());

			if (entityEntry.Entity is PdStat)
			{
				PdStat stat = (PdStat)entityEntry.Entity;
				DbValidationError dve;

				if (stat.Rank == "Perfect")
				{
					if (stat.Safe != 0 || stat.Bad != 0 || stat.Awful != 0 || (!(stat.Good > 0) || !(stat.Cool > 0)))
					{
						dve = new DbValidationError("Rank", "Perfect requires 0 SAFE/BAD/MISS, and more than 0 COOL/GOOD");
						Errors.ValidationErrors.Add(dve);
					}

					if (Helpers.IsOfFFamily(stat.s.Game) && (!stat.ChanceTimeBonus || !stat.TechZoneBonus1 || (stat.Difficulty != "Easy" && !stat.TechZoneBonus2)))
					{
						dve = new DbValidationError("Rank", "Perfect in F/F2nd requires Chance Time and Tech Zone bonusses to be checked");
						Errors.ValidationErrors.Add(dve);
					}
				}
			}

			if (entityEntry.Entity is Song)
			{
				Song song = (Song)entityEntry.Entity;
				DbValidationError dve;

				if (song.Mode == "Edit")
				{
					List<Song> ls = new List<Song>();
					using (PDStatContext db = new PDStatContext())
					{
						try 
						{	        
							ls = (from s in db.Songs where s.Game == song.Game && s.JPTitle == song.JPTitle select s).ToList();
						}
						catch (InvalidOperationException)
						{
						}
					}

					if (ls.Count > 0)
					{
						dve = new DbValidationError("Song title","An Edit song with that name already exists for that game. Please enter a different name.");
						Errors.ValidationErrors.Add(dve);
					}
				}
			}

			return (Errors.ValidationErrors.Count > 0) ? Errors : base.ValidateEntity(entityEntry, items);
		}


		public DbSet<Game> Games { get; set; }
		public DbSet<Song> Songs { get; set; }
		public DbSet<Difficulty> Difficulties { get; set; }
		public DbSet<Rank> Ranks { get; set; }
		public DbSet<PdStat> PDStats { get; set; }
		public DbSet<ScoreStyle> ScoreStyle { get; set; }
	}

	class DbInitializer
#if DEBUG
 : DropCreateDatabaseAlways<PDStatContext>
#else
 : CreateDatabaseIfNotExists<PDStatContext>
#endif
	{
		protected override void Seed(PDStatContext context)
		{
			#region String literal entries
			context.Difficulties.Add(new Difficulty() { Id = 0, Name = "Easy" });
			context.Difficulties.Add(new Difficulty() { Id = 1, Name = "Normal" });
			context.Difficulties.Add(new Difficulty() { Id = 2, Name = "Hard" });
			context.Difficulties.Add(new Difficulty() { Id = 3, Name = "Extreme" });
			context.Difficulties.Add(new Difficulty() { Id = 4, Name = "Edit" });
			context.Difficulties.Add(new Difficulty() { Id = 5, Name = "Tutorial" });

			context.Ranks.Add(new Rank() { Id = 6, Name = "Perfect" });
			context.Ranks.Add(new Rank() { Id = 5, Name = "Excellent" });
			context.Ranks.Add(new Rank() { Id = 4, Name = "Great" });
			context.Ranks.Add(new Rank() { Id = 3, Name = "Standard" });
			context.Ranks.Add(new Rank() { Id = 2, Name = "So Close" });
			context.Ranks.Add(new Rank() { Id = 1, Name = "DROPxOUT" });
			context.Ranks.Add(new Rank() { Id = 0, Name = "Unfinished" });

			context.Games.Add(new Game() { Id = 0, Name = Helpers.PD1 });
			context.Games.Add(new Game() { Id = 1, Name = Helpers.PD2 });
			context.Games.Add(new Game() { Id = 2, Name = Helpers.PDX });

			context.Games.Add(new Game() { Id = 3, Name = Helpers.PDDT });
			context.Games.Add(new Game() { Id = 4, Name = Helpers.PDDT2 });
			context.Games.Add(new Game() { Id = 5, Name = Helpers.PDDTX });

			context.Games.Add(new Game() { Id = 6, Name = Helpers.PDFV });
			context.Games.Add(new Game() { Id = 7, Name = Helpers.PDFP });
			context.Games.Add(new Game() { Id = 8, Name = Helpers.PDF2V });
			context.Games.Add(new Game() { Id = 9, Name = Helpers.PDF2P });

			context.ScoreStyle.Add(new ScoreStyle() {
				Id = 0, StyleName = "Japanese", CoolStyle = "COOL", GoodStyle = "FINE", SafeStyle = "SAFE",
				BadStyle = "SAD", AwfulStyle = "WORST", PRank = "Perfect", ERank = "Excellent", GRank = "Great",
				SRank = "Standard", LRank = "Cheap", FRank = "MISSxTAKE"
			});
			context.ScoreStyle.Add(new ScoreStyle()
			{
				Id = 1, StyleName = "Romaji", CoolStyle = "COOL", GoodStyle = "FINE", SafeStyle = "SAFE",
				BadStyle = "SAD", AwfulStyle = "WORST", PRank = "Perfect", ERank = "Excellent", GRank = "Great",
				SRank = "Standard", LRank = "Cheap", FRank = "MISSxTAKE"
			});
			context.ScoreStyle.Add(new ScoreStyle()
			{
				Id = 2, StyleName = "English", CoolStyle = "COOL", GoodStyle = "FINE", SafeStyle = "SAFE",
				BadStyle = "SAD", AwfulStyle = "WORST", PRank = "Perfect", ERank = "Excellent", GRank = "Great",
				SRank = "Standard", LRank = "Cheap", FRank = "MISSxTAKE"
			});
			context.ScoreStyle.Add(new ScoreStyle() { 
				Id = 3, StyleName = "Localized (F)", CoolStyle = "COOL", GoodStyle = "GOOD", SafeStyle = "SAFE",
				BadStyle = "BAD", AwfulStyle = "AWFUL", PRank = "Perfect", ERank = "Excellent", GRank = "Great",
				SRank = "Standard", LRank = "Lousy", FRank = "DROPxOUT"
			});
			context.ScoreStyle.Add(new ScoreStyle() {
				Id = 4, StyleName = "Localized (F 2nd)", CoolStyle = "COOL", GoodStyle = "GOOD", SafeStyle = "SAFE",
				BadStyle = "BAD", AwfulStyle = "MISS", PRank = "Perfect", ERank = "Excellent", GRank = "Great",
				SRank = "Standard", LRank = "So Close", FRank = "DROPxOUT"
			});
			#endregion

			int i = 0;

			#region PD1
            List<Song> pd1songs = JsonConvert.DeserializeObject<List<Song>>(PDStat.Properties.Resources.PD1Songs);
			foreach (var song in pd1songs)
			{
                context.Songs.Add(new Song
                {
                    Id = i++,
                    Mode = song.Mode,
					Game = Helpers.PD1,
					JPTitle = song.JPTitle,
					RomajiTitle = song.RomajiTitle,
					ENTitle = song.ENTitle
				});
                context.Songs.Add(new Song
                {
                    Id = i++,
                    Mode = song.Mode,
                    Game = Helpers.PDDT,
                    JPTitle = song.JPTitle,
                    RomajiTitle = song.RomajiTitle,
                    ENTitle = song.ENTitle
				});
			}
			#endregion

            #region PD2
            List<Song> pd2songs = JsonConvert.DeserializeObject<List<Song>>(PDStat.Properties.Resources.PD2Songs);
            foreach (var song in pd2songs)
			{
                context.Songs.Add(new Song
                {
                    Id = i++,
                    Mode = song.Mode,
                    Game = Helpers.PD2,
                    JPTitle = song.JPTitle,
                    RomajiTitle = song.RomajiTitle,
                    ENTitle = song.ENTitle
				});
                context.Songs.Add(new Song
                {
                    Id = i++,
                    Mode = song.Mode,
                    Game = Helpers.PDDT2,
                    JPTitle = song.JPTitle,
                    RomajiTitle = song.RomajiTitle,
                    ENTitle = song.ENTitle
				});
			}
			#endregion

            #region PDX
            List<Song> pdXsongs = JsonConvert.DeserializeObject<List<Song>>(PDStat.Properties.Resources.PDXSongs);
            foreach (var song in pdXsongs)
			{
                context.Songs.Add(new Song
                {
                    Id = i++,
                    Mode = song.Mode,
                    Game = Helpers.PDX,
                    JPTitle = song.JPTitle,
                    RomajiTitle = song.RomajiTitle,
                    ENTitle = song.ENTitle
				});
                context.Songs.Add(new Song
                {
                    Id = i++,
                    Mode = song.Mode,
                    Game = Helpers.PDDTX,
                    JPTitle = song.JPTitle,
                    RomajiTitle = song.RomajiTitle,
                    ENTitle = song.ENTitle
				});
			}
			#endregion

            #region PDF
            List<Song> pdFsongs = JsonConvert.DeserializeObject<List<Song>>(PDStat.Properties.Resources.PDFSongs);
            foreach (var song in pdFsongs)
			{
                context.Songs.Add(new Song
                {
                    Id = i++,
                    Mode = song.Mode,
					Game = Helpers.PDFV,
                    JPTitle = song.JPTitle,
                    RomajiTitle = song.RomajiTitle,
                    ENTitle = song.ENTitle,
					LocalizedTitle = song.LocalizedTitle
				});
                context.Songs.Add(new Song
                {
                    Id = i++,
                    Mode = song.Mode,
					Game = Helpers.PDFP,
                    JPTitle = song.JPTitle,
                    RomajiTitle = song.RomajiTitle,
                    ENTitle = song.ENTitle,
                    LocalizedTitle = song.LocalizedTitle
				});
			}
			#endregion

            #region PDF2
            List<Song> pdF2songs = JsonConvert.DeserializeObject<List<Song>>(PDStat.Properties.Resources.PDF2Songs);
            foreach (var song in pdF2songs)
			{
                context.Songs.Add(new Song
                {
                    Id = i++,
                    Mode = song.Mode,
					Game = Helpers.PDF2V,
                    JPTitle = song.JPTitle,
                    RomajiTitle = song.RomajiTitle,
                    ENTitle = song.ENTitle,
                    LocalizedTitle = song.LocalizedTitle
				});
                context.Songs.Add(new Song
                {
                    Id = i++,
                    Mode = song.Mode,
					Game = Helpers.PDF2P,
                    JPTitle = song.JPTitle,
                    RomajiTitle = song.RomajiTitle,
                    ENTitle = song.ENTitle,
                    LocalizedTitle = song.LocalizedTitle
				});
			}
			#endregion

			base.Seed(context);
		}
	}
}
