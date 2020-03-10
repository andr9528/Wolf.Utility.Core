using System;
using System.Collections.Generic;
using System.Text;

namespace Wolf.Utility.Main.Persistence.Core
{
    public interface IEntity
    {
        int Id { get; set; }
        //byte[] Version { get; set; }
    }
}
