using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace StarshipTheory.PatchInjector.Patches
{
    class ModLoaderPatch : BasePatch
    {
        public override string Name => "Mod Loader Patch";

        public override void Inject()
        {
            TypeDefinition ModLoaderDef = ModLibModule.GetTypeByName("ModLoader");
            TypeDefinition ManagerDef = GameModule.GetTypeByName("ManagerOptions");
            TypeDefinition GameEventsDef = ModLibModule.GetTypeByName("GameEvents");

            InjectModLoaderField(ModLoaderDef, ManagerDef);
            InjectModLoaderLoadMods(ModLoaderDef, ManagerDef);
            InjectModLoaderGameStarted(GameEventsDef, ManagerDef);
            InjectModLoaderGameLoaded(GameEventsDef, ManagerDef);
            InjectModLoaderGameSaved(GameEventsDef, ManagerDef);
        }

        private void InjectModLoaderField(TypeDefinition ModLoaderDef, TypeDefinition ManagerDef)
        {
            MethodDefinition ModLoaderCtor = ModLoaderDef.GetMethodByName(".ctor");
            
            MethodDefinition ManagerCctor = ManagerDef.GetMethodByName(".cctor");

            FieldDefinition ModLoaderFieldDef = new FieldDefinition("ModLoader", FieldAttributes.Public | FieldAttributes.Static | FieldAttributes.InitOnly, GameModule.Import(ModLoaderDef));
            ManagerDef.Fields.Add(ModLoaderFieldDef);

            List<Instruction> Instructions = new List<Instruction>()
            {
                Instruction.Create(OpCodes.Newobj, GameModule.Import(ModLoaderCtor)),
                Instruction.Create(OpCodes.Stsfld, ModLoaderFieldDef)
            };

            ManagerCctor.Body.GetILProcessor().InjectInstructionsToEnd(Instructions);
        }

        private void InjectModLoaderLoadMods(TypeDefinition ModLoaderDef, TypeDefinition ManagerDef)
        {
            FieldDefinition ModLoaderFieldDef = ManagerDef.GetFieldByName("ModLoader");
            MethodDefinition ModLoaderLoadModsMethod = ModLoaderDef.GetMethodByName("__LoadMods");
            MethodDefinition ManagerStartMethod = ManagerDef.GetMethodByName("Start");

            List<Instruction> Instructions = new List<Instruction>() {
                Instruction.Create(OpCodes.Ldsfld, GameModule.Import(ModLoaderFieldDef)),
                Instruction.Create(OpCodes.Callvirt, GameModule.Import(ModLoaderLoadModsMethod))
            };

            ManagerStartMethod.Body.GetILProcessor().InjectInstructionsToEnd(Instructions);
        }

        private void InjectModLoaderGameStarted(TypeDefinition gameEventsDef, TypeDefinition managerDef)
        {
            MethodDefinition GameStartedMethod = gameEventsDef.GetMethodByName("__OnGameStarted");
            MethodDefinition ManagerLoadDefaultPlayerShipMethod = managerDef.GetMethodByName("loadDefaultPlayerShip");

            Instruction loadText = ManagerLoadDefaultPlayerShipMethod.Body.Instructions.Where(i => i.OpCode == OpCodes.Ldstr && i.Operand.ToString() == "loading default from script").FirstOrDefault();
            Instruction beforeText = loadText.Previous;

            Instruction endText = beforeText.Operand as Instruction;

            List<Instruction> Instructions = new List<Instruction>() {
                Instruction.Create(OpCodes.Callvirt, GameModule.Import(GameStartedMethod))
            };

            ManagerLoadDefaultPlayerShipMethod.Body.GetILProcessor().InsertBefore(endText, Instructions);
        }

        private void InjectModLoaderGameLoaded(TypeDefinition gameEventsDef, TypeDefinition managerDef)
        {
            MethodDefinition GameLoadedMethod = gameEventsDef.GetMethodByName("__OnGameLoaded");
            MethodDefinition ManagerExecuteLoadGameFromSlotMethod = managerDef.GetMethodByName("loadGameFromSlot");

            List<Instruction> Instructions = new List<Instruction>() {
                Instruction.Create(OpCodes.Ldarg_1),
                Instruction.Create(OpCodes.Callvirt, GameModule.Import(GameLoadedMethod))
            };

            ManagerExecuteLoadGameFromSlotMethod.Body.GetILProcessor().InjectInstructionsToEnd(Instructions);
        }

        private void InjectModLoaderGameSaved(TypeDefinition gameEventsDef, TypeDefinition managerDef)
        {
            MethodDefinition GameSavedMethod = gameEventsDef.GetMethodByName("__OnGameSaved");
            MethodDefinition ManagerSaveGameIntoSlotMethod = managerDef.GetMethodByName("saveGameIntoSlot");

            List<Instruction> Instructions = new List<Instruction>() {
                Instruction.Create(OpCodes.Ldarg_1),
                Instruction.Create(OpCodes.Callvirt, GameModule.Import(GameSavedMethod))
            };

            ManagerSaveGameIntoSlotMethod.Body.GetILProcessor().InjectInstructionsToEnd(Instructions);
        }

    }
}
