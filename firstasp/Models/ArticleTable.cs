using System;
using System.Collections.Generic;

namespace firstasp.Models;

public partial class ArticleTable
{
    public string? UserId { get; set; }

    public string? ArticleCategory { get; set; }

    public string ArticleTitle { get; set; } = null!;

    public string ArticleDate { get; set; } = null!;

    public string? ArticleContent { get; set; }

    public string? PhotoName { get; set; }

    public virtual CategoryTable? ArticleCategoryNavigation { get; set; }

    public virtual PhotoTable? PhotoNameNavigation { get; set; }

    public virtual UserTable? User { get; set; }
}
