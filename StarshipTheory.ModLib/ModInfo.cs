using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarshipTheory.ModLib
{
    public class ModInfo
    {
        /// <summary>
        /// The unique name of the mod
        /// </summary>
        public String Name { get; internal set; }

        /// <summary>
        /// <para>The display name of the mod.</para>
        /// <para>This will be displayed in the mod window, for example.</para>
        /// </summary>
        public String DisplayName { get; internal set; }

        /// <summary>
        /// <para>The current version of the mod.</para>
        /// <para>Example: 1.0.0</para>
        /// </summary>
        public Version Version { get; internal set; }

        /// <summary>
        /// <para>The description of the mod.</para>
        /// </summary>
        public String Description { get; internal set; }
        
        /// <summary>
        /// The author(s) of the mod.
        /// </summary>
        public String Author { get; internal set; }

        /// <summary>
        /// The relative path to the mod's main .dll file
        /// </summary>
        public String EntryDLL { get; internal set; }

        /// <summary>
        /// <para>A list of required mods for this mod to be enabled.</para>
        /// </summary>
        public ModDependency[] Dependencies { get; internal set; }

        public ModInfo()
        {

        }
    }

    public class ModDependency
    {
        /// <summary>
        /// The unique name of the mod.
        /// </summary>
        public String Name { get; internal set; }

        /// <summary>
        /// The minimum supported version of the mod.
        /// </summary>
        public Version MinimumVersion { get; internal set; }

        /// <summary>
        /// The maximum supported version of the mod.
        /// </summary>
        public Version MaximumVersion { get; internal set; }

        public ModDependency()
        {

        }
    }
}
