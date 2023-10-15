using System;
using System.Collections.Generic;

namespace firstasp.Models;

public partial class ReplyTable
{
    public string? UserId { get; set; }

    public string? ArticleCategary { get; set; }

    public string? ArticleTitle { get; set; }

    public string ReplyContent { get; set; } = null!;

    public string ReplyDate { get; set; } = null!;

    public virtual ArticleTable? ArticleTitleNavigation { get; set; }

    public virtual UserTable? User { get; set; }
}
