using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PDStat
{
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
}
