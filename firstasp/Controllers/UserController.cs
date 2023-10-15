using firstasp.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;
using System.Net;
using System.Security.Claims;
using System.IO;
using Microsoft.Extensions.Hosting;

namespace firstasp.Controllers
{
    public class UserController : Controller
    {
        string _path;
        private readonly PrjDbContext _prjDbContext;
        public UserController(PrjDbContext prjDbContext, IWebHostEnvironment hostEnvironment)
        {
            _path = $@"{hostEnvironment.WebRootPath}\images\User";
            _prjDbContext = prjDbContext;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Sign()
        {
            UserTable usermodel = new UserTable();
            return View(usermodel);
        }

        [HttpPost]
        public IActionResult Sign(UserTable usermodel)
        {
            if (ModelState.IsValid)
            {
                var users = _prjDbContext.UserTables;
                var search_id = users.Where(u => u.UserId == usermodel.UserId).FirstOrDefault();
                var search_email = users.Where(u => u.UserEmail == usermodel.UserEmail).FirstOrDefault();
                if (search_id != null)
                {
                    ViewBag.message = "帳號已存在";
                    return View();
                }
                else if (search_email != null)
                {
                    ViewBag.message = "該信箱已註冊";
                    return View();
                }
                else
                {
                    _prjDbContext.Add(usermodel);
                    _prjDbContext.SaveChanges();
                    string dirPath = _path + "\\" + usermodel.UserId;
                    Directory.CreateDirectory(dirPath);
                }
                return Redirect("/User/Login");
            }
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string login_id, string login_pwd)
        {
            var users = _prjDbContext.UserTables;
            var search_userid = users.Where(u => u.UserId == login_id).FirstOrDefault();
            if ( search_userid != null )
            {
                var search_user = users.Where(u => u.UserId == login_id && u.UserPwd == login_pwd).FirstOrDefault();
                if (search_user != null)
                {
                    IList<Claim> claims = new List<Claim> {
                        new Claim(ClaimTypes.Name,search_user.UserId),
                        new Claim(ClaimTypes.Role,search_user.UserRole),
                        };

                    //身份辨識，指定帳號
                    var claimsIndentity = new ClaimsIdentity(
                        claims,
                        CookieAuthenticationDefaults.AuthenticationScheme
                        );
                    var authProperties = new AuthenticationProperties { IsPersistent = true };

                    //登入進行身份識別
                    HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme, 
                        new ClaimsPrincipal(claimsIndentity), authProperties);
                    return Redirect("/Home");
                }
                else
                {
                    ViewBag.message = "密碼錯誤";
                    return View();
                }
            }
            else
            {
                ViewBag.message = "該帳號尚未註冊";
                return View();
            }
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            return Redirect("/User/Login");
        }

        public IActionResult Article_list()
        {
            return View();
        }

        public IActionResult Single_post(string poster, string title, string category)
        {
            var data = _prjDbContext.ArticleTables.Where(m => m.UserId == poster && m.ArticleTitle == title && m.ArticleCategory == category).FirstOrDefault();
            return View(data);
        }

        public IActionResult Article_Editer(string categary, string title)
        {
            var data = _prjDbContext.ArticleTables.Where(u => u.UserId == User.Identity.Name && u.ArticleTitle == title && u.ArticleCategory == categary).FirstOrDefault();
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Article_Editer(
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
            return RedirectToAction("Single_post", new { poster = edit_user, title = edit_title , category = edit_category});
        }

        public IActionResult Article_Deleter(string del_categary, string del_title, string del_photo)
        {
            _prjDbContext.ArticleTables.Where(u => u.UserId == User.Identity.Name && u.ArticleTitle == del_title && u.ArticleCategory == del_categary).ExecuteDelete();
            _prjDbContext.ReplyTables.Where(u => u.ArticleTitle == del_title && u.ArticleCategary == del_categary).ExecuteDelete();
            _prjDbContext.PhotoTables.Where(u => u.UserId == User.Identity.Name && u.PhotoName == del_photo).ExecuteDelete();

            return RedirectToAction("Article_list");
        }
        public IActionResult Contact()
        {
            return View();
        }
    }
}
