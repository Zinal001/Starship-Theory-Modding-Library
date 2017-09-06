using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace StarshipTheory.ModInjector.Patches
{
    class GUIPatch : BasePatch
    {
        public override string Name => "GUI Patch";

        public override void Inject()
        {
            TypeDefinition ManagerDef = GameModule.GetTypeByName("ManagerOptions");

            TypeDefinition ModGUIDef = ModLibModule.GetTypeByName("ModGUI");
            MethodDefinition CreateModGUIMethod = ModGUIDef.GetMethodByName("__Create");
            MethodDefinition ManagerStartMethod = ManagerDef.GetMethodByName("Start");

            ManagerStartMethod.Body.GetILProcessor().InjectInstructionsToEnd(new Instruction[] {
                Instruction.Create(OpCodes.Callvirt, GameModule.Import(CreateModGUIMethod)),
            });

        }
    }
}
