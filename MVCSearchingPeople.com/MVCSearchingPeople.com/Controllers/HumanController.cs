using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DomainModel.Abstract;
using DomainModel.People;
using MVCSearchingPeople.com.Models;

namespace MVCSearchingPeople.com.Controllers
{
    public class HumanController : Controller
    {
        private IHumanRepository repository;
        private ISearchNET service;
        public HumanController(IHumanRepository humanRepository, ISearchNET searchservice)
        {
            repository = humanRepository;
            service = searchservice;
        }
        public ActionResult List()
        {
            ViewBag.Resolution = (repository.Humans.Count * 430);
            ViewBag.Resolution2 = ((repository.Humans.Count * 430) / 2);
            if (repository.Humans.Count > 0)
                return View(repository.Humans);
            else return new RedirectResult("/");
        }

        [HttpGet]
        public ActionResult Start()
        {
            repository.Humans.Clear();
            return View();
        }

        public ActionResult StartNew(int opcode)
        {
              service.Search(repository, opcode);
              if (repository.Humans.Count > 0)
              {
                   return new RedirectResult("~/human/list");
              }
              else
              {
                  ViewData["noresult"] = "1";
                  return View();
              }
        }
        [HttpPost]
        public ActionResult Start(FakeHuman input)
        {
             if (ModelState.IsValid)
             {
                 service.GetFake(input);
                 service.Search(repository, 0);
                 if (repository.Humans.Count > 0)
                 {
                     return new RedirectResult("human/list");
                 }
                 else
                 {
                     ViewData["noresult"] = "1";
                     return View();
                 }
             }
             else return View();
             //return new RedirectResult("~/human/list");
        }
        public ActionResult GetImageFake(int id)
        {
            return File(repository.Humans.ElementAt(id).Image, "image/jpg");
        }
        public ActionResult MoreInformation(int id)
        {
            return View(repository.Humans.ElementAt(id));
        }
        /*public IEnumerable<string> GetTags(int id)
        {
            IEnumerable<string> rezult = null;
            double[] countcategory = null;
            List<string> category = new List<string>();
            if (repository.Humans[id].Tags.Count > 0)
            {;
                category = repository.Humans[id].Tags.Keys.ToList();
                countcategory = new double[category.Count];
                int j = 0;
                foreach(KeyValuePair<string, double> val in repository.Humans[id].Tags)
                {
                    countcategory[j] = val.Value;
                    j++;
                }
                double max = 0;
                foreach (double b in countcategory)
                {
                    if (b > max)
                        max = b;
                }

                int fontsize = 0;   //размер шрифта
                double part = 0;

                int m = 0;
                foreach (string i in category)
                {
                    //исходя из результата полученного из этой формулы получаем размер шрифта
                    part = (countcategory[m] / max) * 100;

                    if (part >= 98)
                    { fontsize = 24; }

                    else if (part >= 70)
                    { fontsize = 19; }

                    else if (part >= 50)
                    { fontsize = 15; }

                    else if (part >= 30)
                    { fontsize = 14; }

                    else if (part >= 10)
                    { fontsize = 8; }

                    else if (part < 10)
                    { fontsize = 6; }

                    //отображение облака с применением различных стилей
                    rezult = "<span style=\"color:#005296; font-size:" + fontsize.ToString() + "pt\">" + i.ToString() + " " + "</span>");
                    m++;
                }
            }
            return rezult;
        }*/
    }
}