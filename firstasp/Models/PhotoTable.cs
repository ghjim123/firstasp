using System;
using System.Collections.Generic;

namespace firstasp.Models;

public partial class PhotoTable
{
    public string UserId { get; set; } = null!;

    public string PhotoName { get; set; } = null!;

    public virtual ICollection<ArticleTable> ArticleTables { get; set; } = new List<ArticleTable>();

    public virtual UserTable User { get; set; } = null!;
}
