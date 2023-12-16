using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class HomeController : Controller
    {
        ConnModel ConnModel = new ConnModel();

        public ActionResult Query()
        {
            bool bConnTest = ConnModel.SQLiteConnTest();
            if (bConnTest)
                ConnModel.SQLiteCreatTable();

            List<PlayerTeamModel> modelList = new List<PlayerTeamModel>();
            DataTable dt;
            dt = ConnModel.SelectDatatable(null);
            modelList = GetModelList(dt);
            return View(modelList);
        }

        public ActionResult Edit()
        {
            PlayerTeamModel model = new PlayerTeamModel();
            
            return View(model);
        }

        [HttpPost]
        public ActionResult Query(string txtSearch, List<PlayerTeamModel> model, string btnSubmit)
        {
            List<PlayerTeamModel> modelList = new List<PlayerTeamModel>();
            DataTable dt;
            switch (btnSubmit)
            {
                case "Save":
                    string sPlayerName = model.Last().PlayerName;
                    string sTeamName = model.Last().TeamName;
                    string sLeague = model.Last().League;
                    int iAge = model.Last().Age;
                    int iHeight = model.Last().Height;
                    int iWeight = model.Last().Weight;
                    int iInsert = ConnModel.InsertData(sPlayerName, sTeamName, sLeague, iAge, iHeight, iWeight);
                    //輸入完成後重新查詢
                    if (iInsert != 0)
                    {
                        dt = ConnModel.SelectDatatable(null);
                        modelList = GetModelList(dt);
                    }
                    break;
                case "Search":
                    dt = ConnModel.SelectDatatable(txtSearch);
                    modelList = GetModelList(dt);
                    break;
                default:
                    break;
            }
            return View(modelList);
        }

        [HttpPost]
        public ActionResult Edit(PlayerTeamModel model, string txtSearchPlayerID, string btnSubmit)
        {
            PlayerTeamModel nModel = new PlayerTeamModel();
            DataTable dt;
            switch (btnSubmit)
            {
                case "Search":
                    dt = ConnModel.SelectDatatableByID(txtSearchPlayerID);
                    if (dt.Rows.Count == 0)
                    {
                        ViewBag.AlertMsg = "請輸入正確球員編號";
                        return View(nModel);
                    }
                    nModel.PlayerID = Convert.ToInt32(dt.Rows[0]["PlayerID"]);
                    nModel.PlayerName = dt.Rows[0]["PlayerName"].ToString();
                    nModel.TeamName = dt.Rows[0]["TeamName"].ToString();
                    nModel.League = dt.Rows[0]["League"].ToString();
                    nModel.Age = Convert.ToInt32(dt.Rows[0]["Age"]);
                    nModel.Height = Convert.ToInt32(dt.Rows[0]["Height"]);
                    nModel.Weight = Convert.ToInt32(dt.Rows[0]["Weight"]);
                    break;
                case "Edit":
                    int iPlayerID = model.PlayerID;
                    string sPlayerName = model.PlayerName;
                    string sTeamName = model.TeamName;
                    int iAge = model.Age;
                    int iHeight = model.Height;
                    int iWeight = model.Weight;
                    int iInsert = ConnModel.UpdateData(iPlayerID, sPlayerName, sTeamName, iAge, iHeight, iWeight);
                    if (iInsert != 0)
                    {
                        ViewBag.AlertMsg = "修改完成";
                        return View(nModel);
                    }
                    else
                    {
                        ViewBag.AlertMsg = "修改失敗";
                        return View(nModel);
                    }
                case "Delete":
                    iPlayerID = model.PlayerID;
                     iInsert = ConnModel.DeleteData(iPlayerID);
                    if (iInsert != 0)
                    {
                        ViewBag.AlertMsg = "刪除完成";
                        return View(nModel);
                    }
                    else
                    {
                        ViewBag.AlertMsg = "刪除失敗";
                        return View(nModel);
                    }
                default:
                    break;
            }
            return View(nModel);
        }

        public List<PlayerTeamModel> GetModelList(DataTable dt)
        {
            List<PlayerTeamModel> modelList = new List<PlayerTeamModel>();

            foreach (DataRow rows in dt.Rows)
            {
                PlayerTeamModel Model = new PlayerTeamModel
                {
                    PlayerID = Convert.ToInt32(rows["PlayerID"]),
                    PlayerName = rows["PlayerName"].ToString(),
                    TeamName = rows["TeamName"].ToString(),
                    League = rows["League"].ToString(),
                    Age = Convert.ToInt32(rows["Age"]),
                    Height = Convert.ToInt32(rows["Height"]),
                    Weight = Convert.ToInt32(rows["Weight"]),
                };
                modelList.Add(Model);
            }
            return modelList;
        }
    }
}