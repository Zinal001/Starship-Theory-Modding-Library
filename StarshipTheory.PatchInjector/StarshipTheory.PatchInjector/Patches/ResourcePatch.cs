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
            MethodDefinition CostsCtor = CostsDef.GetMethodByName(".ctor");
            MethodDefinition CostsGetResourceRequirementsMethod = CostsDef.GetMethodByName("GetResourceRequirements");

            GameModule.Import(ModLibModule.GetTypeByName("EntityCost"));

            TypeDefinition ManagerDef = GameModule.GetTypeByName("ManagerResources");
            MethodDefinition ManagerGetResourceRequirementsMethod = ManagerDef.GetMethodByName("getResourcesRequirement");
            FieldDefinition CostsFieldDef = new FieldDefinition("Costs", FieldAttributes.Public | FieldAttributes.Static | FieldAttributes.InitOnly, GameModule.Import(CostsDef));
            ManagerDef.Fields.Add(CostsFieldDef);

            MethodDefinition ManagerCtor = ManagerDef.GetMethodByName(".ctor");
            ManagerCtor.Body.GetILProcessor().InjectInstructionsToEnd(new Instruction[] {
                Instruction.Create(OpCodes.Newobj, GameModule.Import(CostsCtor)),
                Instruction.Create(OpCodes.Stsfld, GameModule.Import(CostsFieldDef))
            });

            MethodDefinition logMethod = Injector.GetModuleZ("UnityEngine.dll").GetTypeByName("Debug").GetMethodByName("Log", 1);

            TypeDefinition stringType = Injector.GetModuleZ("mscorlib.dll").GetTypeByName("String");
            MethodDefinition isNullOrEmptyMethod = stringType.GetMethodByName("IsNullOrEmpty");
            MethodDefinition concatMethod = stringType.GetMethodByName("Concat", 4, GameModule.Import(stringType));

            ManagerGetResourceRequirementsMethod.Body.Instructions.Clear();
            ManagerGetResourceRequirementsMethod.Body.GetILProcessor().InjectInstructionsToEnd(new Instruction[] {
                Instruction.Create(OpCodes.Ldsfld, CostsFieldDef),
                Instruction.Create(OpCodes.Ldarg_1),
                Instruction.Create(OpCodes.Ldarg_2),
                Instruction.Create(OpCodes.Callvirt, GameModule.Import(CostsGetResourceRequirementsMethod)),
                Instruction.Create(OpCodes.Ret)
            });

            /*ManagerGetResourceRequirementsMethod.Body.Instructions.Clear();

            ManagerGetResourceRequirementsMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_1));
            ManagerGetResourceRequirementsMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_2));
            ManagerGetResourceRequirementsMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Callvirt, GameModule.Import(CostsGetResourceRequirementsMethod)));
            ManagerGetResourceRequirementsMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));*/
        }
    }
}
