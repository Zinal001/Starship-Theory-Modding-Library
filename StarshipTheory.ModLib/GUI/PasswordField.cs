using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarshipTheory.ModLib.GUI
{
    /// <summary>
    /// A wrapper class for the GUILayout.Password Unity method
    /// <see>https://docs.unity3d.com/ScriptReference/GUILayout.PasswordField.html</see>
    /// </summary>
    public class PasswordField : GUIItem
    {
        /// <summary>
        /// Password to edit
        /// </summary>
        public String Password { get; set; }

        /// <summary>
        /// Character to mask the password with.
        /// </summary>
        public Char MaskChar { get; set; }

        /// <summary>
        /// Called whenever the password changes.
        /// </summary>
        public event TextFieldEventDelegate PasswordChanged;

        /// <summary>
        /// Creates a new PasswordField.
        /// </summary>
        /// <param name="password">Password to edit</param>
        /// <param name="maskChar">Character to mask the password with.</param>
        public PasswordField(String password = "", Char maskChar = '*')
        {
            this.Password = password;
            this.MaskChar = maskChar;
        }

        internal override void Draw()
        {
            if (this.Visible)
            {
                String newPass = UnityEngine.GUILayout.PasswordField(Password, MaskChar, Options);
                if (newPass != Password)
                    PasswordChanged?.Invoke(this);
                Password = newPass;
            }
        }

    }
}
