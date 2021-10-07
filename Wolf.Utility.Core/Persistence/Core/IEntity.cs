using System;
using System.Collections.Generic;
using System.Text;

namespace Wolf.Utility.Core.Persistence.Core
{
    public interface IEntity
    {
        int Id { get; set; }
        byte[] Version { get; set; }
        DateTime CreatedDate { get; set; }
        DateTime UpdatedDate { get; set; }
    }
}
