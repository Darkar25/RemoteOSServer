using RemoteOS.OpenComputers.Data;

namespace RemoteOS.OpenComputers.Components
{
    [Component("keyboard")]
    public class KeyboardComponent : Component
    {
        public event Action<char, LWJGLKeyCode, string?>? KeyDown;
        public event Action<char, LWJGLKeyCode, string?>? KeyUp;
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
