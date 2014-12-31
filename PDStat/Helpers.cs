using System;
using System.Collections.Generic;

namespace PDStat
{
	public static class Helpers
	{
		public const string PD1 = "Project Diva (1)";
		public const string PD2 = "Project Diva 2nd";
		public const string PDX = "Project Diva Extend";

		public const string PDDT  = "Project Diva DT";
		public const string PDDT2 = "Project Diva DT 2nd";
		public const string PDDTX = "Project Diva DT Extend";

		public const string PDFV  = "Project Diva f (Vita)";
		public const string PDFP  = "Project Diva F (PS3)";
		public const string PDF2V = "Project Diva f 2nd (Vita)";
		public const string PDF2P = "Project Diva F 2nd (PS3)";

		public static List<string> games = new List<string>() { PD1, PD2, PDX, PDDT, PDDT2, PDDTX, PDFV, PDFP, PDF2V, PDF2P };

		public static bool IsOfFFamily(string game)
		{
			return (game == PDFV || game == PDFP || game == PDF2V || game == PDF2P);
		}

		public static string GetBestName(Song s, ScoreStyle style)
		{
			if (!String.IsNullOrEmpty(s.LocalizedTitle) && style.Id >= 3)
			{
				return s.LocalizedTitle;
			}
			else if (!String.IsNullOrEmpty(s.ENTitle) && style.Id >= 2)
			{
				return s.ENTitle;
			}
			else if (!String.IsNullOrEmpty(s.RomajiTitle) && style.Id >= 1)
			{
				return s.RomajiTitle;
			}
			else
			{
				return s.JPTitle;
			}
		}
	}
}
