using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Wolf.Utility.Core.Bases;

namespace Wolf.Utility.Core.Persistence.Core
{
    public abstract class ChangedEntity : BasePropertyChanged, IEntity
    {
        protected int id = default;
        public int Id 
        {
            get { return id; }
            set 
            { 
                SetProperty(ref id, value, nameof(Id));
            } 
        }

        protected byte[] version = default;
        public byte[] Version
        {
            get { return version; }
            set
            {
                SetProperty(ref version, value, nameof(Version));
            }
        }

        protected DateTime createdDate = default;
        public DateTime CreatedDate
        {
            get { return createdDate; }
            set
            {
                SetProperty(ref createdDate, value, nameof(CreatedDate));
            }
        }

        protected DateTime updatedDate = default;
        public DateTime UpdatedDate
        {
            get { return updatedDate; }
            set
            {
                SetProperty(ref updatedDate, value, nameof(UpdatedDate));
            }
        }        
    }
}
