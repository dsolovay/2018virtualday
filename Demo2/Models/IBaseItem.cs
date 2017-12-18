using System.Collections.Generic;

namespace Demo2.Models
{
    public interface IBaseItem
    {

        IEnumerable<IBaseItem> Children { get; set; }

        string Name { get; set; }

        string Url { get; set; }
    }
}