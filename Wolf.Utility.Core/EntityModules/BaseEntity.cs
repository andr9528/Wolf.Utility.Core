using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Wolf.Utility.Core.EntityModules.Core;
using Wolf.Utility.Core.Exceptions;

namespace Wolf.Utility.Core.EntityModules
{
    // Needs reworking into Dependency injection modules
    public abstract class BaseEntity
    {
        private List<IModule> modules;

        [NotMapped]
        protected List<IModule> Modules { get => modules; private set => modules = value; }

        public BaseEntity()
        {
            Modules = new List<IModule>();
        }

        protected void AddModule(IModule module)
        {
            Modules.Add(module);
        }

        /// <summary>
        /// Should be called from within a try-block, 
        /// as an incorrect amount of modules (to few or many)
        /// will cause an exception, stateing if too many or too few was found.
        /// </summary>
        /// <param name="type"> Module type to try and retrieve. 
        /// Casting to the implementation of that module is guarenteed safe after call.</param>
        /// <returns></returns>
        protected IModule GetModule(ModuleType type) 
        {
            var output = Modules.Select(x => x).Where(x => x.GetModuleType() == type).ToList();

            if (output.Count != 1) throw IncorrectCountException<IModule>.Constructor(1, output.Count, true, Modules);

            return output.ElementAt(0);
        }
    }
}

