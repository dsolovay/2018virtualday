using System;
using System.Collections.Generic;
using Sitecore.Data.Query;

namespace Demo2.Models
{
    public interface IBaseItem
    {
        Guid Id { get; set; }

        IEnumerable<IBaseItem> Children { get; set; }

        string Name { get; set; }

        string Url { get; set; }
    }
}