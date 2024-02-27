using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class EFModel
    {
        public string sPlayerName { get; set; }
        public string sTeamName { get; set; }
        public string sLeague { get; set; }
        public int iAge { get; set; }
        public int iHeight { get; set; }
        public int iWeight { get; set; }
    }

    [Table("PlayerInfo")]
    public class PlayerInfo
    {
        [Key]
        public int PlayerID { get; set; }
        public string PlayerName { get; set; }
        public int Age { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }

    }

    [Table("PlayerTeam")]
    public class PlayerTeam
    {
        [Key]
        public int PlayerID { get; set; }
        public string TeamName { get; set; }

    }

    [Table("Team")]
    public class Team
    {
        [Key]
        public string TeamName { get; set; }
        public string HomeCourt { get; set; }
        public string League { get; set; }
    }

    public class PlayerContext : DbContext
    {
        public DbSet<PlayerInfo> PlayerInfo { get; set; }

        public DbSet<PlayerTeam> PlayerTeam { get; set; }

        public DbSet<Team> Team { get; set; }
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    // 配置數據庫連接字符串
        //    optionsBuilder.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SchoolDB;Integrated Security=True");
        //}

        public PlayerContext() : base("name=SqliteConn")
        {
        }
    }
}