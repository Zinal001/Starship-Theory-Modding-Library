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
            TypeDefinition ManagerDef = GameModule.GetTypeByName("ManagerOptions");
            
            ChangeGameTitle();
            NewPanelOpened();
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

        private void NewPanelOpened()
        {
            TypeDefinition MenuEventsDef = ModLibModule.GetTypeByName("MenuEvents");
            MethodDefinition OnNewGamePanelOpened = MenuEventsDef.GetMethodByName("__OnNewGamePanelOpened");

            TypeDefinition ManagerMenuDef = GameModule.GetTypeByName("ManagerMenu");
            MethodDefinition Method = ManagerMenuDef.GetMethodByName("createNewGamePanel");

            FieldDefinition rightMenuFieldDef = ManagerMenuDef.GetFieldByName("mainMenuRight");

            Instruction[] instructions = new Instruction[] {
                Instruction.Create(OpCodes.Ldarg_0),
                Instruction.Create(OpCodes.Ldfld, GameModule.Import(rightMenuFieldDef)),
                Instruction.Create(OpCodes.Callvirt, GameModule.Import(OnNewGamePanelOpened))
            };

            Method.Body.GetILProcessor().InjectInstructionsToEnd(instructions);
        }
    }
}
