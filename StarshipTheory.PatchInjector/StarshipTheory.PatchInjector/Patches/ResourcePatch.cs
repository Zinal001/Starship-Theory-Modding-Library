using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace StarshipTheory.PatchInjector.Patches
{
    class ResourcePatch : BasePatch
    {
        public override string Name => "Resourch Patch";

        public override void Inject()
        {
            TypeDefinition CostsDef = ModLibModule.GetTypeByName("Costs");
            MethodDefinition CostsGetResourceRequirementsMethod = CostsDef.GetMethodByName("GetResourceRequirements");

            TypeDefinition ManagerDef = GameModule.GetTypeByName("ManagerResources");
            MethodDefinition ManagerGetResourceRequirementsMethod = ManagerDef.GetMethodByName("getResourcesRequirement");

            ManagerGetResourceRequirementsMethod.Body.Instructions.Clear();
            ManagerGetResourceRequirementsMethod.Body.GetILProcessor().InjectInstructionsToEnd(new Instruction[] {
                Instruction.Create(OpCodes.Ldarg_1),
                Instruction.Create(OpCodes.Ldarg_2),
                Instruction.Create(OpCodes.Callvirt, GameModule.Import(CostsGetResourceRequirementsMethod)),
                Instruction.Create(OpCodes.Ret)
            });
        }
    }
}
