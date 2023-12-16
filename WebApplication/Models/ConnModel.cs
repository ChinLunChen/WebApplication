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

        #region SQLiteConnTest
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
        /// 資料查詢
        /// </summary>
        /// <param name="sPlayerName">球員名稱</param>
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

                    string sSQLCmd = "select A.PlayerID,A.PlayerName,A.Age,A.Height,A.Weight,B.TeamName,B.League from PlayerInfo as A ";
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

        #region SelectDatatableByID
        /// <summary>
        /// 以ID資料查詢
        /// </summary>
        /// <param name="sPlayerID">球員編號</param>
        /// <returns></returns>
        public DataTable SelectDatatableByID(string sPlayerID)
        {
            DataTable dt = new DataTable();

            try
            {
                var connection = new SQLiteConnection(sConn);

                using (connection)
                {
                    //開啟連線
                    connection.Open();

                    string sSQLCmd = "select A.PlayerID,A.PlayerName,A.Age,A.Height,A.Weight,B.TeamName,B.League from PlayerInfo as A ";
                    sSQLCmd += "left join PlayerTeam On A.PlayerID = PlayerTeam.PlayerID ";
                    sSQLCmd += "left join Team as B On PlayerTeam.TeamName=B.TeamName ";
                    sSQLCmd += "where A.PlayerID = @PlayerID";
                    using (SQLiteCommand command = new SQLiteCommand(sSQLCmd, connection))
                    {
                        command.Parameters.AddWithValue("@PlayerID", sPlayerID);
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
        /// <summary>
        /// 輸入資料
        /// </summary>
        /// <param name="sPlayerName">球員名稱</param>
        /// <param name="sTeamName">隊名</param>
        /// <param name="sLeague">聯盟</param>
        /// <param name="iAge">年齡</param>
        /// <param name="iHeight">身高</param>
        /// <param name="iWeight">體重</param>
        /// <returns></returns>
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
        #endregion

        #region UpdateData
        /// <summary>
        /// 更新資料
        /// </summary>
        /// <param name="iPlayerID"></param>
        /// <param name="sPlayerName"></param>
        /// <param name="sTeamName"></param>
        /// <param name="iAge"></param>
        /// <param name="iHeight"></param>
        /// <param name="iWeight"></param>
        /// <returns></returns>
        public int UpdateData(int iPlayerID, string sPlayerName, string sTeamName, int iAge, int iHeight, int iWeight)
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
                        string UpdateQuery = "Update PlayerInfo SET PlayerName = @PlayerName, Age = @Age, Height = @Height, Weight = @Weight where PlayerID = @PlayerID";

                        using (SQLiteCommand command = new SQLiteCommand(UpdateQuery, connection))
                        {
                            //參數化 防止SQL injection
                            command.Parameters.AddWithValue("@PlayerID", iPlayerID);
                            command.Parameters.AddWithValue("@PlayerName", sPlayerName);
                            command.Parameters.AddWithValue("@Age", iAge);
                            command.Parameters.AddWithValue("@Height", iHeight);
                            command.Parameters.AddWithValue("@Weight", iWeight);

                            // 輸入資料庫
                            iInsert = command.ExecuteNonQuery();
                        }

                        UpdateQuery = "Update PlayerTeam SET TeamName =@TeamName where PlayerID = @PlayerID";

                        using (SQLiteCommand command = new SQLiteCommand(UpdateQuery, connection))
                        {
                            //參數化 防止SQL injection
                            command.Parameters.AddWithValue("@PlayerID", iPlayerID);
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
        #endregion

        #region DeleteData
        /// <summary>
        /// 刪除資料
        /// </summary>
        /// <param name="iPlayerID"></param>
        /// <returns></returns>
        public int DeleteData(int iPlayerID)
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
                        string DeleteQuery = "Delete From PlayerInfo where PlayerID = @PlayerID";

                        using (SQLiteCommand command = new SQLiteCommand(DeleteQuery, connection))
                        {
                            //參數化 防止SQL injection
                            command.Parameters.AddWithValue("@PlayerID", iPlayerID);

                            // 輸入資料庫
                            iInsert = command.ExecuteNonQuery();
                        }

                        DeleteQuery = "Delete From PlayerTeam where PlayerID = @PlayerID";

                        using (SQLiteCommand command = new SQLiteCommand(DeleteQuery, connection))
                        {
                            //參數化 防止SQL injection
                            command.Parameters.AddWithValue("@PlayerID", iPlayerID);

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
        #endregion
    }

}
