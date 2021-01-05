using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Wolf.Utility.Core.Persistence.Core;

namespace Wolf.Utility.Core.Persistence.EntityFramework.Core
{
    public interface IHandler
    {
        Task Save();

        Task<T> Find<T>(T predicate) where T : class, IEntity;

        Task<IEnumerable<T>> FindMultiple<T>(T predicate) where T : class, IEntity;
        
        Task<bool> Update<T>(T element, bool autoSave = true) where T : class, IEntity;
        
        Task<bool> Delete<T>(T element, bool autoSave = true) where T : class, IEntity;

        Task<bool> Add<T>(T element, bool autoSave = true) where T : class, IEntity;

        Task<string> AddMultiple<T>(ICollection<T> elements) where T : class, IEntity;
    }
}
