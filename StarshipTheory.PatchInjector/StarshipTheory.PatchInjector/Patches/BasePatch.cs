using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace StarshipTheory.PatchInjector.Patches
{
    abstract class BasePatch
    {
        public abstract String Name { get; }

        public static PatchInjector Injector { get; internal set; }

        public static ModuleDefinition GameModule { get; internal set; }
        public static ModuleDefinition ModLibModule { get; internal set; }

        public BasePatch()
        {

        }

        public virtual bool ShouldInject()
        {
            return true;
        }

        public abstract void Inject();
    }
}
