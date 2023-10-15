using System;
using System.Collections.Generic;

namespace firstasp.Models;

public partial class CategoryTable
{
    public int Id { get; set; }

    public string ArticleCategory { get; set; } = null!;

    public virtual ICollection<ArticleTable> ArticleTables { get; set; } = new List<ArticleTable>();
}
