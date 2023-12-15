using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Web;

namespace WebApplication.Models
{
    public class ConnModel
    {
        string sConn = ConfigurationManager.ConnectionStrings["SqliteConn"].ConnectionString;

        #region
        public bool SQLiteConnTest()
        {
            var connection = new SQLiteConnection(sConn);

            using (connection)
            {
                //開啟連結
                connection.Open();

                try
                {
                    // 查詢資料庫版本
                    string query = "SELECT SQLite_Version()";
                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        var result = command.ExecuteScalar();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        #endregion

        #region SQLiteCreatTable
        /// <summary>
        /// 建立資料表
        /// </summary>
        public void SQLiteCreatTable()
        {
            var connection = new SQLiteConnection(sConn);

            using (connection)
            {
                //開啟連線
                connection.Open();
                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 建立資料表
                        string sSQLCmd = "CREATE TABLE IF NOT EXISTS Team (TeamName TEXT, HomeCourt TEXT,League TEXT, PRIMARY KEY (\"TeamName\"))";
                        string sSQLCmd2 = "CREATE TABLE IF NOT EXISTS PlayerInfo (PlayerID INTEGER, PlayerName TEXT, Age INTEGER,Height INTEGER, Weight INTEGER,PRIMARY KEY (\"PlayerID\" AUTOINCREMENT))";
                        string sSQLCmd3 = "CREATE TABLE IF NOT EXISTS PlayerTeam (PlayerID INTEGER, TeamName TEXT, PRIMARY KEY (\"PlayerID\" AUTOINCREMENT))";
                        string[] sSQLCmds = { sSQLCmd, sSQLCmd2, sSQLCmd3 };

                        foreach (string sCmd in sSQLCmds)
                        {
                            using (SQLiteCommand command = new SQLiteCommand(sCmd, connection))
                            {
                                command.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                    }
                }
                connection.Close();
            }
        }
        #endregion

        #region SelectDatatable
        /// <summary>
        /// 資料表查詢
        /// </summary>
        /// <returns></returns>
        public DataTable SelectDatatable(string sPlayerName)
        {
            DataTable dt = new DataTable();

            try
            {
                var connection = new SQLiteConnection(sConn);

                using (connection)
                {
                    //開啟連線
                    connection.Open();

                    string sSQLCmd = "select A.PlayerName,A.Age,A.Height,A.Weight,B.TeamName,B.League from PlayerInfo as A ";
                    sSQLCmd += "left join PlayerTeam On A.PlayerID = PlayerTeam.PlayerID ";
                    sSQLCmd += "left join Team as B On PlayerTeam.TeamName=B.TeamName ";
                    if (sPlayerName != null && sPlayerName != "")
                    {
                        sSQLCmd += "where A.PlayerName = @PlayerName";
                    }
                    using (SQLiteCommand command = new SQLiteCommand(sSQLCmd, connection))
                    {
                        command.Parameters.AddWithValue("@PlayerName", sPlayerName);
                        using (SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(command))
                        {
                            dataAdapter.Fill(dt);
                        }
                    }

                    connection.Close();
                }
            }
            catch (Exception ex) { }
            return dt;
        }
        #endregion

        #region InsertData
        public int InsertData(string sPlayerName, string sTeamName, string sLeague, int iAge, int iHeight, int iWeight)
        {
            int iInsert = 0;

            var connection = new SQLiteConnection(sConn);

            using (connection)
            {
                //開啟連線
                connection.Open();
                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string insertQuery = "INSERT INTO PlayerInfo (PlayerName, Age, Height, Weight) VALUES (@PlayerName, @Age, @Height, @Weight)";

                        using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                        {
                            //參數化 防止SQL injection
                            command.Parameters.AddWithValue("@PlayerName", sPlayerName);
                            command.Parameters.AddWithValue("@Age", iAge);
                            command.Parameters.AddWithValue("@Height", iHeight);
                            command.Parameters.AddWithValue("@Weight", iWeight);

                            // 輸入資料庫
                            iInsert = command.ExecuteNonQuery();
                        }

                        insertQuery = "INSERT INTO PlayerTeam (TeamName) VALUES (@TeamName)";

                        using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                        {
                            //參數化 防止SQL injection
                            command.Parameters.AddWithValue("@TeamName", sTeamName);

                            // 輸入資料庫
                            iInsert = command.ExecuteNonQuery();
                        }
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                    }
                }
                connection.Close();
                return iInsert;
            }
        }
    }
    #endregion
}
