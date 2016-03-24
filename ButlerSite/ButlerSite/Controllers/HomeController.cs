using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ButlerSite.DataWarehouse;
using ButlerSite.Models;

namespace ButlerSite.Controllers
{
  public class HomeController : Controller
  {
    public ActionResult Index()
    {
      return View();
    }

    public ActionResult About()
    {
      ViewBag.Message = "Your application description page.";

      return View();
    }

    public ActionResult Contact()
    {
      ViewBag.Message = "Your contact page.";

      return View();
    }

    public ActionResult Search(string question)
    {
      var actionresult = default(ActionResult);

      if (!string.IsNullOrEmpty(question))
      {
        var information = new List<InformationViewModel>();

        var key = default(string);
        var subinformation = default(IEnumerable<object>);

        var warehouseclient = new DataWarehouseClient();
        var diggingresult = warehouseclient.DigInformation(question);
        foreach (var item in diggingresult)
        {
          var informationitem = new InformationViewModel();

          //  Bilder
          key = "image";
          subinformation = item.ContainsKey(key) ? item[key] : default(IEnumerable<object>);
          if (subinformation != null && subinformation.Any())
          {
            informationitem.Images = subinformation.Where(info => !string.IsNullOrEmpty(info.ToString())).Select(info => new Uri(info.ToString()));
          }

          //  Uri
          key = "uri";
          subinformation = item.ContainsKey(key) ? item[key] : default(IEnumerable<object>);
          if (subinformation != null && subinformation.Any())
          {
            informationitem.Uri = subinformation.Select(info => new Uri(info.ToString())).FirstOrDefault();
          }

          information.Add(informationitem);
        }

        actionresult = View(information);
      }
      else
      {
        ViewBag.Message = "Your search page.";

        actionresult = View();
      }

      return actionresult;
    }
  }
}