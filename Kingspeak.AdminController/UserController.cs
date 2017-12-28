﻿using Kingspeak.Admin.Models;
using Kingspeak.Admin.Service;
using Kingsun.Core.Utils;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Kingspeak.AdminController
{
    public class UserController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.AppList = GetAppTokenList();
            return View();
        }

        public ActionResult Administrator()
        {
            return View();
        }

        public JsonResult GetUserList(int pageindex, int pagesize, string sortName, string sortOrder, string SearchKey, int? SearchType, int? Source)
        {
            PageParams<Tb_UserInfo> param = new PageParams<Tb_UserInfo>();
            UserService service = new UserService();
            param.PageSize = pagesize;
            param.PageIndex = param.GetPageIndex(pageindex, pagesize);

            if (SearchType.HasValue && !string.IsNullOrEmpty(SearchKey))
            {
                param.Wheres = new List<System.Linq.Expressions.Expression<Func<Tb_UserInfo, bool>>>();
                switch (SearchType)
                {
                    case 1:
                        param.Wheres.Add(it => it.UserName.Contains(SearchKey));
                        break;
                    case 2:
                        param.Wheres.Add(it => it.RealName.Contains(SearchKey));
                        break;
                    case 3:
                        param.Wheres.Add(it => it.UserId.ToString() == SearchKey);
                        break;
                }
            }
            if (Source.HasValue && Source.Value != 0)
            {
                param.Wheres.Add(it => it.ResourceID == Source);
            }
            if (!string.IsNullOrEmpty(sortName))
            {
                param.StrOrderColumns = sortName + " " + sortOrder;
            }
            else
            {
                param.OrderColumns = it => it.CreateTime;
            }
            int totalCount = 0;
            List<Tb_UserInfo> list = service.GetPageList<Tb_UserInfo>(param, ref totalCount);
            object obj = new { total = totalCount, rows = list };
            return Json(obj);
        }


        public JsonResult GetAdminList(int pageindex, int pagesize, string sortName, string sortOrder, string SearchKey, int? SearchType)
        {
            PageParams<Tb_Admin_UserInfo> param = new PageParams<Tb_Admin_UserInfo>();
            UserService service = new UserService();
            param.PageSize = pagesize;
            param.PageIndex = param.GetPageIndex(pageindex, pagesize);
            if (SearchType.HasValue && !string.IsNullOrEmpty(SearchKey))
            {
                param.Wheres = new List<System.Linq.Expressions.Expression<Func<Tb_Admin_UserInfo, bool>>>();
                switch (SearchType)
                {
                    case 1:
                        param.Wheres.Add(it => it.UserName.Contains(SearchKey));
                        break;
                    case 2:
                        param.Wheres.Add(it => it.TrueName.Contains(SearchKey));
                        break;
                    case 3:
                        param.Wheres.Add(it => it.UserID.ToString() == SearchKey);
                        break;
                }
            }



            if (!string.IsNullOrEmpty(sortName))
            {
                param.StrOrderColumns = sortName + " " + sortOrder;
            }
            else
            {
                param.OrderColumns = it => it.CreateDate;
            }
            int totalCount = 0;
            List<Tb_Admin_UserInfo> list = service.GetPageList<Tb_Admin_UserInfo>(param, ref totalCount);
            object obj = new { total = totalCount, rows = list };
            return Json(obj);
        }

        private List<string> GetAllResource()
        {
            return null;
        }


        /// <summary>
        /// 导出用户列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public FileResult ExportUser()
        {
            UserService service = new UserService();
            HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();//创建工作簿
            string tmpTitle = "用户列表" + DateTime.Now.ToString("yyyy-MM-dd");
            List<Tb_UserInfo> list = service.GetAll<Tb_UserInfo>();
            CreateSheet(list.OrderByDescending(x => x.AddTime).ToList(), book, tmpTitle + " ", 0, list.Count);

            MemoryStream ms = new MemoryStream();
            book.Write(ms);
            string UserAgent = System.Web.HttpContext.Current.Request.ServerVariables["http_user_agent"].ToLower();
            if (UserAgent.IndexOf("firefox") == -1)
            {
                tmpTitle = HttpUtility.UrlEncode(tmpTitle, System.Text.Encoding.UTF8).Replace("+", "%20").Replace("%27", "'");
            }
            else
            {
                tmpTitle = "=?UTF-8?B?" + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(tmpTitle)) + "?=";
            }
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/excel", tmpTitle + ".xls");
        }

        private void CreateSheet(IList<Tb_UserInfo> list, HSSFWorkbook book, string tmpTitle, int startIndex, int endIndex)
        {
            ISheet sheet = book.CreateSheet(tmpTitle);//创建一个名为 taskTitle 的表
            IRow headerrow = sheet.CreateRow(0);//创建一行，此行为第一行           
            ICellStyle style = book.CreateCellStyle();//创建表格样式
            style.Alignment = HorizontalAlignment.Center;//水平对齐方式
            style.VerticalAlignment = VerticalAlignment.Center;//垂直对齐方式

            //给 sheet 添加第一行的头部标题         
            headerrow.CreateCell(0).SetCellValue("用户编号");
            headerrow.CreateCell(1).SetCellValue("用户名称");
            headerrow.CreateCell(2).SetCellValue("来源");
            headerrow.CreateCell(3).SetCellValue("创建时间");
            headerrow.CreateCell(4).SetCellValue("MOD用户编号");
            headerrow.CreateCell(5).SetCellValue("用户类型");
            headerrow.CreateCell(6).SetCellValue("真实姓名");
            headerrow.CreateCell(7).SetCellValue("性别");
            headerrow.CreateCell(8).SetCellValue("添加时间");
            headerrow.CreateCell(9).SetCellValue("年级");
            headerrow.CreateCell(10).SetCellValue("状态");
            headerrow.CreateCell(11).SetCellValue("来源系统用户编号");



            for (int i = startIndex; i < endIndex; i++)
            {
                if (list[i] != null)
                {
                    Tb_UserInfo toinfo = list[i];
                    IRow row = sheet.CreateRow(i + 1);      //新创建一行
                    ICell cell = row.CreateCell(0);         //在新创建的一行中创建单元格
                    cell.CellStyle = style;                 //设置单元格格式
                    row.CreateCell(0).SetCellValue(toinfo.UserId);
                    row.CreateCell(1).SetCellValue(toinfo.UserName);
                    row.CreateCell(2).SetCellValue(toinfo.Resource);
                    row.CreateCell(3).SetCellValue(toinfo.CreateTime.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                    row.CreateCell(4).SetCellValue(toinfo.UserIdMod);
                    row.CreateCell(5).SetCellValue(toinfo.UserType == 1 ? "教师" : "学生");
                    row.CreateCell(6).SetCellValue(toinfo.RealName);
                    row.CreateCell(7).SetCellValue(toinfo.Sex);
                    row.CreateCell(8).SetCellValue(toinfo.AddTime.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                    row.CreateCell(9).SetCellValue(toinfo.Grade);
                    row.CreateCell(10).SetCellValue(toinfo.Status == 2 ? "拒绝登录" : "正常");
                    row.CreateCell(11).SetCellValue(toinfo.YUid.ToString());

                }
            }
        }

        private List<Tb_AppToken> GetAppTokenList()
        {
            UserService service = new UserService();
            return service.GetList<Tb_AppToken>(it => it.State == 0);
        }
    }
}
