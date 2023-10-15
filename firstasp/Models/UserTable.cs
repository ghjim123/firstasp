using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace firstasp.Models;

public partial class UserTable
{
    [Display(Name = "註冊帳號")]
    [Required(ErrorMessage = "請填入對應資料")]
    public string UserId { get; set; } = null!;

    [Display(Name = "設定密碼")]
    [Required(ErrorMessage = "請填入對應資料")]
    public string UserPwd { get; set; } = null!;

    [Display(Name = "使用者信箱")]
    [Required(ErrorMessage = "請填入對應資料")]
    [EmailAddress(ErrorMessage="信箱格式錯誤")]
    public string UserEmail { get; set; } = null!;

    public string UserRole { get; set; } = "member";

    public virtual ICollection<ArticleTable> ArticleTables { get; set; } = new List<ArticleTable>();

    public virtual ICollection<PhotoTable> PhotoTables { get; set; } = new List<PhotoTable>();
}
