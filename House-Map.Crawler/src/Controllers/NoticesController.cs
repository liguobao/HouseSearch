using Microsoft.AspNetCore.Mvc;
using HouseMap.Dao;
using HouseMap.Common;
using System.Linq;
using HouseMap.Dao.DBEntity;
using System;

namespace HouseMap.Crawler.Controllers
{
    public class NoticesController : Controller
    {



        private HouseMapContext _context;


        public NoticesController(HouseMapContext context)
        {
            _context = context;
        }

        public IActionResult Index(string source)
        {
            var notices = _context.Notices.ToList();
            return View(notices);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return View(null);
            }
            var notice = _context.Notices.FirstOrDefault(n => n.Id == id);
            return View(notice);
        }

        public IActionResult Save(int id, string content, string date)
        {
            Notice notice;
            if (id != 0)
            {
                notice = _context.Notices.FirstOrDefault(n => n.Id == id);
            }
            else
            {
                notice = new Notice();
                notice.DataCreateTime = DateTime.Now;
                _context.Notices.Add(notice);
            }
            notice.Content = content;
            notice.EndShowDate = string.IsNullOrEmpty(date) ? DateTime.Now.AddDays(7) : DateTime.Parse(date);
            _context.SaveChanges();
            return Json(new { success = true });
        }

    }
}