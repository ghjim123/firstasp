using firstasp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.CodeAnalysis.Elfie.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Hosting;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace firstasp.Controllers
{
    public class CategoryController : Controller
    {
        string _path;
        private readonly PrjDbContext _prjDbContext;
        public CategoryController(PrjDbContext prjDbContext, IWebHostEnvironment hostEnvironment)
        {
            _prjDbContext = prjDbContext;
            _path = $@"{hostEnvironment.WebRootPath}\images\User";
        }
        public IActionResult Index(string key)
        {
            var article_data =
                from datas in _prjDbContext.ArticleTables
                where datas.ArticleCategory == key
                orderby datas.ArticleDate descending
                select datas;
            
            ViewBag.key = key;
            return View(article_data);
        }

        public IActionResult Single_post(string poster ,string title ,string category)
        {
            var data = _prjDbContext.ArticleTables.Where(m => m.UserId == poster && m.ArticleTitle == title && m.ArticleCategory == category).FirstOrDefault();
            var owner = User.Identity.Name;
            if(owner == poster)
            {
                ViewBag.edit = true;
            }
            else
            {
                ViewBag.edit = false;
            }
            return View(data);
        }

        public IActionResult SearchList()
        {
            return View();
        }

        public IActionResult Poster(string key)
        {
            ViewBag.article_categary = key;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Poster( 
            IFormFile formFile, string Category_key, string user_id, string article_title, string article_content)
        {
            var date = DateTime.Now.ToString(("yyyy/MM/dd HH:mm:ss"));
            var data = new ArticleTable();
            data.UserId = user_id;
            data.ArticleCategory = Category_key;
            data.ArticleTitle = article_title;
            data.ArticleDate = date;
            data.ArticleContent = article_content.Replace("\n","<br>");
            if (formFile != null)
            {
                if (formFile.Length > 0)
                {
                    string saveFile = $@"{_path}\{user_id}\{formFile.FileName}";
                    using (var stream = new FileStream(saveFile, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                    var img = new PhotoTable();
                    img.UserId = user_id;
                    img.PhotoName = formFile.FileName;
                    data.PhotoName = formFile.FileName;
                    _prjDbContext.Add(img);
                    _prjDbContext.SaveChanges();
                }
            }
            _prjDbContext.Add(data);
            _prjDbContext.SaveChanges();
            return RedirectToAction("Index", new { key = Category_key });
        }

        public IActionResult Editer(string categary, string title)
        {
            var data = _prjDbContext.ArticleTables.Where(u=> u.UserId == User.Identity.Name && u.ArticleTitle== title && u.ArticleCategory == categary).FirstOrDefault();
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Editer(
            IFormFile formFile, string edit_category, string edit_user, string edit_title, string edit_oldtitle, string edit_content)
        {
            _prjDbContext.ArticleTables
                .Where(u => u.UserId == edit_user && u.ArticleTitle == edit_oldtitle && u.ArticleCategory == edit_category)
                .ExecuteUpdate(setters => setters
                .SetProperty(b => b.ArticleTitle, edit_title)
                .SetProperty(b => b.ArticleContent, edit_content));
            if (formFile != null)
            {
                if (formFile.Length > 0)
                {
                    var oldfilename = _prjDbContext.ArticleTables.Where(u => u.UserId == edit_user && u.ArticleTitle == edit_oldtitle && u.ArticleCategory == edit_category).FirstOrDefault();
                    
                    string filename = $"{Guid.NewGuid().ToString()}.jpg";
                    string saveFile = $@"{_path}\{edit_user}\{filename}";
                    using (var stream = new FileStream(saveFile, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                    var img = new PhotoTable();
                    img.UserId = edit_user;
                    img.PhotoName = filename;
                    _prjDbContext.Add(img);
                    _prjDbContext.SaveChanges();
                    _prjDbContext.ArticleTables
                       .Where(u => u.UserId == edit_user && u.ArticleTitle == edit_oldtitle && u.ArticleCategory == edit_category)
                       .ExecuteUpdate(setters => setters
                       .SetProperty(b => b.PhotoName, filename));

                    if (oldfilename != null)
                    {
                        if (oldfilename.PhotoName != null)
                        {
                            string oldfilepath = $@"{_path}\{edit_user}\{oldfilename.PhotoName}";
                            System.IO.File.Delete(oldfilepath);
                            _prjDbContext.PhotoTables.Where(m => m.PhotoName == oldfilename.PhotoName).ExecuteDelete();
                        }
                    }
                }
            }
            var reply_title = _prjDbContext.ReplyTables.FirstOrDefault(e => e.ArticleTitle == edit_oldtitle && e.UserId == edit_user);
            if (reply_title != null) 
            {
                _prjDbContext.ArticleTables
                   .Where(u => u.UserId == edit_user && u.ArticleTitle == edit_oldtitle && u.ArticleCategory == edit_category)
                   .ExecuteUpdate(setters => setters
                   .SetProperty(b => b.ArticleTitle, edit_title));
            }
            return RedirectToAction("Single_post", new { poster = edit_user, title = edit_title, category=edit_category});
        }

        public IActionResult Deleter(string del_categary, string del_title, string del_photo)
        {
            _prjDbContext.ArticleTables.Where(u => u.UserId == User.Identity.Name && u.ArticleTitle == del_title && u.ArticleCategory == del_categary).ExecuteDelete();
            _prjDbContext.ReplyTables.Where(u => u.ArticleTitle == del_title && u.ArticleCategary == del_categary).ExecuteDelete();
            _prjDbContext.PhotoTables.Where(u => u.UserId == User.Identity.Name && u.PhotoName == del_photo).ExecuteDelete();

            return RedirectToAction("Index", new { key = del_categary });
        }

    }
}
