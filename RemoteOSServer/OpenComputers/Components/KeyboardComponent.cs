using RemoteOS.OpenComputers.Data;
using RemoteOS.Helpers;

namespace RemoteOS.OpenComputers.Components
{
    [Component("keyboard")]
    public class KeyboardComponent : Component
    {
        /// <summary>
        /// This event is sent when player pressed the key on the keyboard
        /// <para>Parameters:
        /// <br>char - character that got pressed</br>
        /// <br>LWJGLKeyCode - keycode representation of the pressed key</br>
        /// <br>string - player name</br>
        /// </para>
        /// </summary>
        public event Action<char, LWJGLKeyCode, string?>? KeyDown;
        /// <summary>
        /// This event is sent when player stopped pressing the key
        /// <para>Parameters:
        /// <br>char - character that got pressed</br>
        /// <br>LWJGLKeyCode - keycode representation of the pressed key</br>
        /// <br>string - player name</br>
        /// </para>
        /// </summary>
        public event Action<char, LWJGLKeyCode, string?>? KeyUp;
        /// <summary>
        /// This event is sent when player inserted his clipboard
        /// <para>Parameters:
        /// <br>string - clipboard text</br>
        /// <br>string - player name</br>
        /// </para>
        /// </summary>
        public event Action<string, string?>? Clipboard;
        public KeyboardComponent(Machine parent, Guid address) : base(parent, address)
        {
            parent.Listen("key_down", (parameters) => {
                if (parameters[0].Value == Address.ToString())
                    KeyDown?.Invoke((char)parameters[1], (LWJGLKeyCode)parameters[2].AsInt, parameters[3]);
            });
            parent.Listen("key_up", (parameters) => {
                if (parameters[0].Value == Address.ToString())
                    KeyUp?.Invoke((char)parameters[1], (LWJGLKeyCode)parameters[2].AsInt, parameters[3]);
            });
            parent.Listen("clipboard", (parameters) => {
                if (parameters[0].Value == Address.ToString())
                    Clipboard?.Invoke(parameters[1], parameters[2]);
            });
        }
    }
}
