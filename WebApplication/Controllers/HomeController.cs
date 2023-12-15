using System;
using System.Collections.Generic;
using System.Data;
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

        public ActionResult Insert()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Edit()
        {
            ViewBag.Message = "Your contact page.";

            return View();
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

        public List<PlayerTeamModel> GetModelList(DataTable dt)
        {
            List<PlayerTeamModel> modelList = new List<PlayerTeamModel>();

            foreach (DataRow rows in dt.Rows)
            {
                PlayerTeamModel Model = new PlayerTeamModel
                {
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