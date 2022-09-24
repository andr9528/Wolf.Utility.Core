using System;
using System.Collections.Generic;
using System.Text;

namespace Wolf.Utility.Core.Persistence.Core
{
    public interface IEntity : ISearchableEntity
    {
        byte[] Version { get; set; }
        DateTime CreatedDate { get; set; }
        DateTime UpdatedDate { get; set; }
    }
}
