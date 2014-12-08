using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace PDStat
{
	public class Song
	{
		public byte Id { get; set; }

		[Key]
		[StringLength(150)]
		public string Title { get; set; }
	}

	public class Pd1Song : Song { }
	public class Pd2Song : Song { }
	public class PdXSong : Song { }
	public class PdFSong : Song { }
	public class PdF2Song : Song { }

	public class PdStat
	{
		[Key]
		[Column(Order = 2)]
		public uint Attempt { get; set; }

		[Key]
		[Column(Order = 3)]
		public string Difficulty { get; set; }

		[ForeignKey("Difficulty")]
		public Difficulty diff { get; set; }

		public DateTime Date { get; set; }
		public short Cool { get; set; }
		public short Good { get; set; } //Fine
		public short Safe { get; set; }
		public short Bad { get; set; } //Sad
		public short Awful { get; set; } //Miss, Worst
		public int Score { get; set; }

		public string Rank { get; set; }

		[ForeignKey("Rank")]
		public Rank r { get; set; }
	}
	public class PdfStat : PdStat
	{
		public bool ChanceTimeBonus { get; set; }
		public bool TechZoneBonus1 { get; set; }
		public bool TechZoneBonus2 { get; set; }
	}
	
	public class Pd1SongStat : PdStat
	{
		
		[Key]
		[Column(Order = 1)]
		[StringLength(150)]
		public string Title { get; set; }

		[ForeignKey("Title")]
		public Pd1Song song { get; set; }
	}

	public class Pd2SongStat : PdStat
	{
		[Key]
		[Column(Order = 1)]
		[StringLength(150)]
		public string Title { get; set; }

		[ForeignKey("Title")]
		public Pd2Song song { get; set; }
	}

	public class PdXSongStat : PdStat
	{
		[Key]
		[Column(Order = 1)]
		[StringLength(150)]
		public string Title { get; set; }

		[ForeignKey("Title")]
		public PdXSong song { get; set; }
	}

	public class PdDTSongStat : PdStat
	{
		[Key]
		[Column(Order = 1)]
		[StringLength(150)]
		public string Title { get; set; }

		[ForeignKey("Title")]
		public Pd1Song song { get; set; }
	}

	public class PdDT2SongStat : PdStat
	{
		[Key]
		[Column(Order = 1)]
		[StringLength(150)]
		public string Title { get; set; }

		[ForeignKey("Title")]
		public Pd2Song song { get; set; }
	}

	public class PdDTXSongStat : PdStat
	{
		[Key]
		[Column(Order = 1)]
		[StringLength(150)]
		public string Title { get; set; }

		[ForeignKey("Title")]
		public PdXSong song { get; set; }
	}

	public class PdFVitaSongStat : PdfStat
	{
		[Key]
		[Column(Order = 1)]
		[StringLength(150)]
		public string Title { get; set; }

		[ForeignKey("Title")]
		public PdFSong song { get; set; }
	}

	public class PdF2VitaSongStat : PdfStat
	{
		[Key]
		[Column(Order = 1)]
		[StringLength(150)]
		public string Title { get; set; }

		[ForeignKey("Title")]
		public PdF2Song song { get; set; }
	}

	public class PdFPS3SongStat : PdfStat
	{
		[Key]
		[Column(Order = 1)]
		[StringLength(150)]
		public string Title { get; set; }

		[ForeignKey("Title")]
		public PdFSong song { get; set; }
	}

	public class PdF2PS3SongStat : PdfStat
	{
		[Key]
		[Column(Order = 1)]
		[StringLength(150)]
		public string Title { get; set; }

		[ForeignKey("Title")]
		public PdF2Song song { get; set; }
	}

	public class Difficulty
	{
		public byte Id { get; set; }

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

	public enum Game
	{
		[Description("Project Diva 1")]
		ProjectDiva1,
		[Description("Project Diva 2nd")]
		ProjectDiva2,
		[Description("Project Diva Extend")]
		ProjectDivaX,
		[Description("Project Diva DT")]
		ProjectDivaDT,
		[Description("Project Diva DT 2nd")]
		ProjectDivaDT2,
		[Description("Project Diva DT Extend")]
		ProjectDivaDTX,
		[Description("Project Diva f (Vita)")]
		ProjectDivaFVita,
		[Description("Project Diva F (PS3)")]
		ProjectDivaFPS3,
		[Description("Project Diva f 2nd (Vita)")]
		ProjectDivaF2Vita,
		[Description("Project Diva F 2nd (PS3)")]
		ProjectDivaF2PS3,
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

		
		public DbSet<Difficulty> Difficulties { get; set; }
		public DbSet<Rank> Ranks { get; set; }
		public DbSet<Pd1Song> Pd1Songs { get; set; }
		public DbSet<Pd2Song> Pd2Songs { get; set; }
		public DbSet<PdXSong> PdXSongs { get; set; }
		public DbSet<PdFSong> PdFSongs { get; set; }
		public DbSet<PdF2Song> PdF2Songs { get; set; }

		[Description("Project Diva 1")]
		public DbSet<Pd1SongStat> ProjectDiva1 { get; set; }

		[Description("Project Diva 2nd")]
		public DbSet<Pd2SongStat> ProjectDiva2 { get; set; }
		
		[Description("Project Diva Extend")]
		public DbSet<PdXSongStat> ProjectDivaX { get; set; }

		[Description("Project Diva DT")]
		public DbSet<PdDTSongStat> ProjectDivaDT { get; set; }

		[Description("Project Diva DT 2nd")]
		public DbSet<PdDT2SongStat> ProjectDivaDT2 { get; set; }

		[Description("Project Diva DT Extend")]
		public DbSet<PdDTXSongStat> ProjectDivaDTX { get; set; }

		[Description("Project Diva f (Vita)")]
		public DbSet<PdFVitaSongStat> ProjectDivaFVita { get; set; }

		[Description("Project Diva f 2nd (Vita)")]
		public DbSet<PdF2VitaSongStat> ProjectDivaF2Vita { get; set; }

		[Description("Project Diva F (PS3)")]
		public DbSet<PdFPS3SongStat> ProjectDivaFPS3 { get; set; }

		[Description("Project Diva F 2nd (PS3)")]
		public DbSet<PdF2PS3SongStat> ProjectDivaF2PS3 { get; set; }
	}

	class DbInitializer : DropCreateDatabaseAlways<PDStatContext>
	{
		protected override void Seed(PDStatContext context)
		{
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

			string[] pd1songs = PDStat.Properties.Resources.PD1.Split('\n');
			for (byte i = 0; i < pd1songs.Length; i++)
			{
				context.Pd1Songs.Add(new Pd1Song() { Id = i, Title = pd1songs[i].Trim() });
			}
			
			string[] pd2songs = PDStat.Properties.Resources.PD2.Split('\n');
			for (byte i = 0; i < pd2songs.Length; i++)
			{
				context.Pd2Songs.Add(new Pd2Song() { Id = i, Title = pd2songs[i].Trim() });
			}

			string[] pdXsongs = PDStat.Properties.Resources.PDX.Split('\n');
			for (byte i = 0; i < pdXsongs.Length; i++)
			{
				context.PdXSongs.Add(new PdXSong() { Id = i, Title = pdXsongs[i].Trim() });
			}

			string[] pdFsongs = PDStat.Properties.Resources.PDF.Split('\n');
			for (byte i = 0; i < pdFsongs.Length; i++)
			{
				context.PdFSongs.Add(new PdFSong() { Id = i, Title = pdFsongs[i].Trim() });
			}

			string[] pdF2songs = PDStat.Properties.Resources.PDF2.Split('\n');
			for (byte i = 0; i < pdF2songs.Length; i++)
			{
				context.PdF2Songs.Add(new PdF2Song() { Id = i, Title = pdF2songs[i].Trim() });
			}

			base.Seed(context);
		}
	}
}
