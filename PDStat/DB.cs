﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;

namespace PDStat
{
	public class Song
	{
		[Key]
		public int Id { get; set; }

		[StringLength(30)]
		public string Game { get; set; }

		[StringLength(150)]
		public string Title { get; set; }

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

	public enum ScoreStyle
	{
		Auto,
		[Description("English (F)")]
		EnglishF,
		[Description("English (F 2nd)")]
		EnglishF2,
		Japanese,
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

			return (Errors.ValidationErrors.Count > 0) ? Errors : base.ValidateEntity(entityEntry, items);
		}


		public DbSet<Game> Games { get; set; }
		public DbSet<Song> Songs { get; set; }
		public DbSet<Difficulty> Difficulties { get; set; }
		public DbSet<Rank> Ranks { get; set; }
		public DbSet<PdStat> PDStats { get; set; }
	}

	class DbInitializer
#if (DEBUG)
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
			#endregion

			string def = "Default";
			string tut = "Tutorial";
			string ip = "Ievan Polkka";

			int i = 0;
			string[] pd1songs = PDStat.Properties.Resources.PD1.Split('\n');
			foreach (string s in pd1songs)
			{
				context.Songs.Add(new Song() { Id = i++, Mode = def, Game = Helpers.PD1, Title = s.Trim() });
				context.Songs.Add(new Song() { Id = i++, Mode = def, Game = Helpers.PDDT, Title = s.Trim() });
			}

			context.Songs.Add(new Song() { Id = i++, Mode = tut, Game = Helpers.PD2, Title = ip });
			context.Songs.Add(new Song() { Id = i++, Mode = tut, Game = Helpers.PDDT2, Title = ip });
			string[] pd2songs = PDStat.Properties.Resources.PD2.Split('\n');
			foreach (string s in pd2songs)
			{
				context.Songs.Add(new Song() { Id = i++, Mode = def, Game = Helpers.PD2, Title = s.Trim() });
				context.Songs.Add(new Song() { Id = i++, Mode = def, Game = Helpers.PDDT2, Title = s.Trim() });
			}

			context.Songs.Add(new Song() { Id = i++, Mode = tut, Game = Helpers.PDX, Title = ip });
			context.Songs.Add(new Song() { Id = i++, Mode = tut, Game = Helpers.PDDTX, Title = ip });
			string[] pdXsongs = PDStat.Properties.Resources.PDX.Split('\n');
			foreach (string s in pdXsongs)
			{
				context.Songs.Add(new Song() { Id = i++, Mode = def, Game = Helpers.PDX, Title = s.Trim() });
				context.Songs.Add(new Song() { Id = i++, Mode = def, Game = Helpers.PDDTX, Title = s.Trim() });
			}

			context.Songs.Add(new Song() { Id = i++, Mode = tut, Game = Helpers.PDFV, Title = ip });
			context.Songs.Add(new Song() { Id = i++, Mode = tut, Game = Helpers.PDFP, Title = ip });
			string[] pdFsongs = PDStat.Properties.Resources.PDF.Split('\n');
			foreach (string s in pdFsongs)
			{
				context.Songs.Add(new Song() { Id = i++, Mode = def, Game = Helpers.PDFV, Title = s.Trim() });
				context.Songs.Add(new Song() { Id = i++, Mode = def, Game = Helpers.PDFP, Title = s.Trim() });
			}

			context.Songs.Add(new Song() { Id = i++, Mode = tut, Game = Helpers.PDF2V, Title = ip });
			context.Songs.Add(new Song() { Id = i++, Mode = tut, Game = Helpers.PDF2P, Title = ip });
			context.Songs.Add(new Song() { Id = i++, Mode = tut, Game = Helpers.PDF2V, Title = ip + " (Extreme)" });
			context.Songs.Add(new Song() { Id = i++, Mode = tut, Game = Helpers.PDF2P, Title = ip + " (Extreme)" });
			string[] pdF2songs = PDStat.Properties.Resources.PDF2.Split('\n');
			foreach (string s in pdF2songs)
			{
				context.Songs.Add(new Song() { Id = i++, Mode = def, Game = Helpers.PDF2V, Title = s.Trim() });
				context.Songs.Add(new Song() { Id = i++, Mode = def, Game = Helpers.PDF2P, Title = s.Trim() });
			}

			base.Seed(context);
		}
	}
}
