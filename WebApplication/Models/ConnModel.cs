using System;
using System.Collections.Generic;
using System.Configuration;
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
                        string sSQLCmd = "CREATE TABLE IF NOT EXISTS Team (TeamName TEXT, HomeCourt TEXT, PRIMARY KEY (\"TeamName\"))";
                        string sSQLCmd2 = "CREATE TABLE IF NOT EXISTS Player (ID INTEGER, PlayerName TEXT, Age INTEGER, PRIMARY KEY (\"ID\"))";
                        string[] sSQLCmds = { sSQLCmd, sSQLCmd2 };

                        foreach(string sCmd in sSQLCmds)
                        {
                            using (var cmd = new SQLiteCommand(sCmd, connection))
                            {
                                cmd.ExecuteNonQuery();
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
            #endregion
        }
    }
}