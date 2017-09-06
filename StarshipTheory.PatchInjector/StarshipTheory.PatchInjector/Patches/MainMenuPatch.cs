using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarshipTheory.PatchInjector.Patches
{
    class MainMenuPatch : BasePatch
    {
        public override string Name => "Main Menu Patch";

        public override void Inject()
        {
            TypeDefinition ModLoaderDef = ModLibModule.GetTypeByName("ModLoader");
            TypeDefinition ManagerDef = GameModule.GetTypeByName("ManagerOptions");
            
            ChangeGameTitle();
        }

        private void ChangeGameTitle()
        {
            TypeDefinition ManagerMenuDef = GameModule.GetTypeByName("ManagerMenu");
            MethodDefinition Method = ManagerMenuDef.GetMethodByName("createMainMenuBackground");

            foreach (Instruction I in Method.Body.Instructions)
            {
                if (I.OpCode == OpCodes.Ldstr && I.Operand.ToString() == "STARSHIP  THEORY")
                {
                    I.Operand = "STARSHIP  THEORY - MODDED";
                    break;
                }
            }
        }
    }
}
