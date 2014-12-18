namespace PDStat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initail : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Difficulties",
                c => new
                    {
                        Name = c.String(nullable: false, maxLength: 10),
                        Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Name);
            
            CreateTable(
                "dbo.Games",
                c => new
                    {
                        Name = c.String(nullable: false, maxLength: 30),
                        Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Name);
            
            CreateTable(
                "dbo.PdStats",
                c => new
                    {
                        Song = c.Int(nullable: false),
                        Difficulty = c.String(nullable: false, maxLength: 10),
                        Attempt = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        BestCombo = c.Short(nullable: false),
                        Cool = c.Short(nullable: false),
                        Good = c.Short(nullable: false),
                        Safe = c.Short(nullable: false),
                        Bad = c.Short(nullable: false),
                        Awful = c.Short(nullable: false),
                        Score = c.Int(nullable: false),
                        Rank = c.String(maxLength: 15),
                        ChanceTimeBonus = c.Boolean(nullable: false),
                        TechZoneBonus1 = c.Boolean(nullable: false),
                        TechZoneBonus2 = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.Song, t.Difficulty, t.Attempt })
                .ForeignKey("dbo.Difficulties", t => t.Difficulty, cascadeDelete: true)
                .ForeignKey("dbo.Ranks", t => t.Rank)
                .ForeignKey("dbo.Songs", t => t.Song, cascadeDelete: true)
                .Index(t => t.Song)
                .Index(t => t.Difficulty)
                .Index(t => t.Rank);
            
            CreateTable(
                "dbo.Ranks",
                c => new
                    {
                        Name = c.String(nullable: false, maxLength: 15),
                        Id = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.Name);
            
            CreateTable(
                "dbo.Songs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Game = c.String(maxLength: 30),
                        JPTitle = c.String(maxLength: 150),
                        RomajiTitle = c.String(maxLength: 150),
                        ENTitle = c.String(maxLength: 150),
                        LocalizedTitle = c.String(maxLength: 150),
                        Mode = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Games", t => t.Game)
                .Index(t => t.Game);
            
            CreateTable(
                "dbo.ScoreStyles",
                c => new
                    {
                        StyleName = c.String(nullable: false, maxLength: 4000),
                        Id = c.Int(nullable: false),
                        CoolStyle = c.String(maxLength: 4000),
                        GoodStyle = c.String(maxLength: 4000),
                        SafeStyle = c.String(maxLength: 4000),
                        BadStyle = c.String(maxLength: 4000),
                        AwfulStyle = c.String(maxLength: 4000),
                        PRank = c.String(maxLength: 4000),
                        ERank = c.String(maxLength: 4000),
                        GRank = c.String(maxLength: 4000),
                        SRank = c.String(maxLength: 4000),
                        LRank = c.String(maxLength: 4000),
                        FRank = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.StyleName);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PdStats", "Song", "dbo.Songs");
            DropForeignKey("dbo.Songs", "Game", "dbo.Games");
            DropForeignKey("dbo.PdStats", "Rank", "dbo.Ranks");
            DropForeignKey("dbo.PdStats", "Difficulty", "dbo.Difficulties");
            DropIndex("dbo.Songs", new[] { "Game" });
            DropIndex("dbo.PdStats", new[] { "Rank" });
            DropIndex("dbo.PdStats", new[] { "Difficulty" });
            DropIndex("dbo.PdStats", new[] { "Song" });
            DropTable("dbo.ScoreStyles");
            DropTable("dbo.Songs");
            DropTable("dbo.Ranks");
            DropTable("dbo.PdStats");
            DropTable("dbo.Games");
            DropTable("dbo.Difficulties");
        }
    }
}
