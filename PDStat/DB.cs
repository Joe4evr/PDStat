using System;
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

					if (HelperMethods.IsOfFFamily(stat.s.Game) && (!stat.ChanceTimeBonus || !stat.TechZoneBonus1 || (stat.Difficulty != "Easy" && !stat.TechZoneBonus2)))
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

			context.Games.Add(new Game() { Id = 0, Name = "Project Diva (1)" });
			context.Games.Add(new Game() { Id = 1, Name = "Project Diva 2nd" });
			context.Games.Add(new Game() { Id = 2, Name = "Project Diva Extend" });

			context.Games.Add(new Game() { Id = 3, Name = "Project Diva DT" });
			context.Games.Add(new Game() { Id = 4, Name = "Project Diva DT 2nd" });
			context.Games.Add(new Game() { Id = 5, Name = "Project Diva DT Extend" });

			context.Games.Add(new Game() { Id = 6, Name = "Project Diva f (Vita)" });
			context.Games.Add(new Game() { Id = 7, Name = "Project Diva F (PS3)" });
			context.Games.Add(new Game() { Id = 8, Name = "Project Diva f 2nd (Vita)" });
			context.Games.Add(new Game() { Id = 9, Name = "Project Diva F 2nd (PS3)" });
			#endregion

			int i = 0;
			string[] pd1songs = PDStat.Properties.Resources.PD1.Split('\n');
			foreach (string s in pd1songs)
			{
				context.Songs.Add(new Song() { Id = i++, Game = "Project Diva (1)", Title = s.Trim() });
				context.Songs.Add(new Song() { Id = i++, Game = "Project Diva DT", Title = s.Trim() });
			}
			
			string[] pd2songs = PDStat.Properties.Resources.PD2.Split('\n');
			foreach (string s in pd2songs)
			{
				context.Songs.Add(new Song() { Id = i++, Game = "Project Diva 2nd", Title = s.Trim() });
				context.Songs.Add(new Song() { Id = i++, Game = "Project Diva DT 2nd ", Title = s.Trim() });
			}

			string[] pdXsongs = PDStat.Properties.Resources.PDX.Split('\n');
			foreach (string s in pdXsongs)
			{
				context.Songs.Add(new Song() { Id = i++, Game = "Project Diva Extend", Title = s.Trim() });
				context.Songs.Add(new Song() { Id = i++, Game = "Project Diva DT Extend", Title = s.Trim() });
			}

			string[] pdFsongs = PDStat.Properties.Resources.PDF.Split('\n');
			foreach (string s in pdFsongs)
			{
				context.Songs.Add(new Song() { Id = i++, Game = "Project Diva f (Vita)", Title = s.Trim() });
				context.Songs.Add(new Song() { Id = i++, Game = "Project Diva F (PS3)", Title = s.Trim() });
			}

			string[] pdF2songs = PDStat.Properties.Resources.PDF2.Split('\n');
			foreach (string s in pdF2songs)
			{
				context.Songs.Add(new Song() { Id = i++, Game = "Project Diva f 2nd (Vita)", Title = s.Trim() });
				context.Songs.Add(new Song() { Id = i++, Game = "Project Diva F 2nd (PS3)", Title = s.Trim() });
			}

			base.Seed(context);
		}
	}
}
