using Kingspeak.Admin.Models;
using Kingspeak.Admin.Service;
using Kingsun.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Kingspeak.AdminController
{
    public class ApplicationController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetAppTokenList(int pageindex, int pagesize, string sortName, string sortOrder, string SearchKey, int? SearchType)
        {
            PageParams<Tb_AppToken> param = new PageParams<Tb_AppToken>();
            ApplicationService service = new ApplicationService();
            param.PageSize = pagesize;
            param.PageIndex = param.GetPageIndex(pageindex, pagesize);

            if (SearchType.HasValue && !string.IsNullOrEmpty(SearchKey))
            {
                param.Wheres = new List<System.Linq.Expressions.Expression<Func<Tb_AppToken, bool>>>();
                switch (SearchType)
                {
                    case 1:
                        param.Wheres.Add(it => it.ID.ToString() == SearchKey);
                        break;
                    case 2:
                        param.Wheres.Add(it => it.AppName.Contains(SearchKey));
                        break;
                    case 3:
                        param.Wheres.Add(it => it.AppToken == SearchKey);
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
            List<Tb_AppToken> list = service.GetPageList<Tb_AppToken>(param, ref totalCount);
            object obj = new { total = totalCount, rows = list };
            return Json(obj);
        }

        public JsonResult ChangeState(int ID, int State)
        {
            ApplicationService service = new ApplicationService();
            if (service.Update<Tb_AppToken>(new Tb_AppToken { State = State, ID = ID }, it => it.ID, it => it.State))
            {
                return Json(KingResponse.GetResponse(""));
            }
            else
            {
                return Json(KingResponse.GetErrorResponse("修改失败"));
            }
        }
    }
}
