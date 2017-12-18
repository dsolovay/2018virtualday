using System.Collections.Generic;

namespace Demo2.Models
{
    public interface ISampleItem
    {
        IEnumerable<ISampleItem> Children { get; set; }
    }
}