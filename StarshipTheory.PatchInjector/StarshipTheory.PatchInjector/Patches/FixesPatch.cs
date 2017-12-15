using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarshipTheory.PatchInjector.Patches
{
    class FixesPatch : BasePatch
    {
        public override string Name => "Bug Fixes Patch";

        public override void Inject()
        {
            AI_anyPartOnFire_Fix();
            Structures_resetShipDataCoroutine_Fix();
            ManagerOptions_flyEnemyOutOfSceneCR_Fix();
        }

        /// <summary>
        /// Fixes a bug where crewmembers tries to find a tile outside the tile map (Position -444, -444).
        /// </summary>
        private void AI_anyPartOnFire_Fix()
        {
            TypeDefinition AIDef = GameModule.GetTypeByName("AI");
            MethodDefinition anyPartOnFireDef = AIDef.GetMethodByName("anyPartOnFire");
            
            ILProcessor il = anyPartOnFireDef.Body.GetILProcessor();

            Instruction[] ending = new Instruction[] {
                Instruction.Create(OpCodes.Ldc_I4_0),
                Instruction.Create(OpCodes.Ret)
            };

            il.Append(ending[0]);
            il.Append(ending[1]);

            Instruction[] beginning = new Instruction[] {
                Instruction.Create(OpCodes.Ldarg_1),
                Instruction.Create(OpCodes.Ldarg_2),
                Instruction.Create(OpCodes.Add),
                Instruction.Create(OpCodes.Ldc_I4, -888),
                Instruction.Create(OpCodes.Ceq),
                Instruction.Create(OpCodes.Brtrue, ending[0])
            };

            Instruction last = il.Body.Instructions[0];
            for(int i = beginning.Length - 1; i >= 0; i--)
            {
                il.InsertBefore(last, beginning[i]);
                last = beginning[i];
            }
        }


        /// <summary>
        /// Fixes the graphical bug where the enemy ship is "spawned" beneath the player ship after it is destroyed.
        /// </summary>
        private void Structures_resetShipDataCoroutine_Fix()
        {
            ModuleDefinition unityModule = Injector.GetModuleZ("UnityEngine.dll");

            TypeDefinition structuresDef = GameModule.GetTypeByName("Structures");
            TypeDefinition resetShipDataCoroutine = structuresDef.NestedTypes.Where(t => t.Name.Contains("resetShipDataCoroutine")).FirstOrDefault();
            
            MethodDefinition toggleShipVisual = structuresDef.GetMethodByName("toggleShipVisual");

            MethodDefinition moveNextMethod = resetShipDataCoroutine.GetMethodByName("MoveNext");
            FieldDefinition f_thisField = resetShipDataCoroutine.GetFieldByName("<>f__this");
            MethodDefinition gameObjectGetMethod = unityModule.GetTypeByName("Component").GetPropertyByName("gameObject").GetMethod;



            Instruction[] instructions = new Instruction[] {
                Instruction.Create(OpCodes.Ldarg_0),
                Instruction.Create(OpCodes.Ldfld, GameModule.Import(f_thisField)),
                Instruction.Create(OpCodes.Ldc_I4_0),
                Instruction.Create(OpCodes.Callvirt, GameModule.Import(toggleShipVisual))
            };

            ILProcessor processor = moveNextMethod.Body.GetILProcessor();

            Instruction lastInstruction = moveNextMethod.Body.Instructions.Where(i => i.OpCode == OpCodes.Callvirt && ((MethodReference)i.Operand).Name.Contains("set_position")).FirstOrDefault();
            foreach (Instruction ins in instructions)
            {
                processor.InsertAfter(lastInstruction, ins);
                lastInstruction = ins;
            }
        }

        /// <summary>
        /// Fixes the graphical bug where the enemy ship is "spawned" beneath the player ship after it is destroyed.
        /// </summary>
        private void ManagerOptions_flyEnemyOutOfSceneCR_Fix()
        {
            TypeDefinition managerOptions = GameModule.GetTypeByName("ManagerOptions");
            TypeDefinition flyEnemyOutOfSceneCR = managerOptions.NestedTypes.Where(t => t.Name.Contains("flyEnemyOutOfSceneCR")).FirstOrDefault();

            TypeDefinition bugFixes = ModLibModule.GetTypeByName("__BugFixes");
            MethodDefinition toggleShipVisual = bugFixes.GetMethodByName("ToggleShipVisual");

            MethodDefinition moveNextMethod = flyEnemyOutOfSceneCR.GetMethodByName("MoveNext");
            FieldDefinition f_thisField = flyEnemyOutOfSceneCR.GetFieldByName("<>f__this");
            FieldDefinition enemyShipField = f_thisField.FieldType.Resolve().GetFieldByName("EnemyShip");

            Instruction[] instructions = new Instruction[] {
                Instruction.Create(OpCodes.Ldarg_0),
                Instruction.Create(OpCodes.Ldfld, GameModule.Import(f_thisField)),
                Instruction.Create(OpCodes.Ldfld, GameModule.Import(enemyShipField)),
                Instruction.Create(OpCodes.Ldc_I4_0),
                Instruction.Create(OpCodes.Callvirt, GameModule.Import(toggleShipVisual))
            };

            ILProcessor processor = moveNextMethod.Body.GetILProcessor();

            Instruction lastInstruction = moveNextMethod.Body.Instructions.Where(i => i.OpCode == OpCodes.Callvirt && ((MethodReference)i.Operand).Name.Contains("playSoundEnemyWarpOut")).FirstOrDefault();
            foreach(Instruction ins in instructions)
            {
                processor.InsertAfter(lastInstruction, ins);
                lastInstruction = ins;
            }
        }


    }
}
