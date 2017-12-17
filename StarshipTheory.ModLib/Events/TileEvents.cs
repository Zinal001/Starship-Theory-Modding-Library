using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarshipTheory.ModLib.Events
{
    public static class TileEvents
    {
        public class TileEventArgs : System.EventArgs
        {
            public TileInfo Tile { get; private set; }
            public String FieldName { get; private set; }
            public Object NewValue { get; private set; }

            public TileEventArgs(TileInfo tile, String fieldName, Object newValue)
            {
                this.Tile = tile;
                this.FieldName = fieldName;
                this.NewValue = newValue;
            }

            public T GetValue<T>()
            {
                return (T)NewValue;
            }
        }

        public static event EventHandler<TileEventArgs> TileChanged;


        public static void __OnTileChanged(TileInfo tile, String fileName, Object newValue)
        {
            TileChanged?.Invoke(null, new TileEventArgs(tile, fileName, newValue));
        }
    }
}
