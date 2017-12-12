using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarshipTheory.PatchInjector.Patches
{
    class AccessPatch : BasePatch
    {
        public override string Name => "Access Patch";

        public override bool ShouldInject()
        {
            return true;
        }

        public override void Inject()
        {
            InjectionHelper.Instance.SetupReferences(GameModule);

            GameModule.GetTypes().ToList().ForEach(t => {
                if (!t.IsNested)
                {
                    t.IsPublic = true;
                    InjectionHelper.Instance.CreateModMethods(t);
                }
                else
                    t.IsNestedPublic = true;
            });

            GameModule.GetTypes().ToList().ForEach(t => {
                t.Methods.ToList().ForEach(m => {
                    if (m.IsPrivate)
                        m.IsFamily = true;
                });
            });

            GameModule.GetTypes().ToList().ForEach(t => {
                t.Fields.ToList().ForEach(f => {
                    if (f.IsPrivate)
                    {
                        f.IsFamily = true;
                        //InjectionHelper.Instance.CreateSetterGetterFor(f, t, f.IsStatic);
                    }
                });
            });
        }
    }
}
