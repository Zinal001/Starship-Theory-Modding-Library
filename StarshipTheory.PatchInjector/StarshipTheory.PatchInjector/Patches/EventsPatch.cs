using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace StarshipTheory.PatchInjector.Patches
{
    class EventsPatch : BasePatch
    {
        public override string Name => "Events Patch";

        public override void Inject()
        {
            OnTilesChangedEvent();
        }

        private void OnTilesChangedEvent()
        {
            FieldDefinition tileInfoToBecomeField = GameModule.GetTypeByName("TileInfo").GetFieldByName("toBecome");
            MethodDefinition onTileChangedMethod = ModLibModule.GetTypeByName("TileEvents").GetMethodByName("__OnTileChanged");

            InjectionHelper.Instance.OverrideFieldWithProperty(tileInfoToBecomeField, onTileChangedMethod);

        }
    }
}
