using System;
using System.Collections.Generic;
using System.Text;
using Wolf.Utility.Main.Persistence.Core;

namespace Wolf.Utility.Main.Persistence.EntityFramework.Core
{
    public interface IHandler
    {
        void Save();

        T Find<T>(T predicate) where T : class, IEntity;

        ICollection<T> FindMultiple<T>(T predicate) where T : class, IEntity;

        /// <summary>
        /// Remember to call Save!
        /// </summary>
        bool Update<T>(T element) where T : class, IEntity;

        /// <summary>
        /// Remember to call Save!
        /// </summary>
        bool Delete<T>(T element) where T : class, IEntity;

        /// <summary>
        /// Remember to call Save!
        /// </summary>
        bool Add<T>(T element) where T : class, IEntity;

        /// <summary>
        /// Remember to call Save!
        /// </summary>
        string AddMultiple<T>(ICollection<T> elements) where T : class, IEntity;
    }
}
