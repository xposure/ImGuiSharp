namespace ImGui
{
    public enum ImMouseButton
    {
        None,
        LeftButton,
        RightButton
    }

    public enum ImEventType
    {
        MouseDown,
        MouseUp,
        MouseMove,
        MouseDrag,
        KeyDown,
        KeyRepeat,
        KeyUp,
        ScrollWheel,
        Repaint,
        Layout,
        Ignore,
        Used        
    }

    public class ImEvent
    {
        public bool IsAltDown { get; internal set; }
        public bool IsShiftDown { get; internal set; }
        public bool IsCapsLockDown { get; internal set; }
        public bool IsControlDown { get; internal set; }
        public bool IsKeyboardEvent { get; internal set; }
        public bool IsMouseEvent { get; internal set; }

        public char Character { get; internal set; }

        public int MouseClickCount { get; internal set; }
        public ImMouseButton MouseButton { get; internal set; }
        public ImVec2 MousePosition { get; internal set; }

        public float DeltaTime { get; internal set; }
        public ImEventType EventType { get; internal set; }

        public void Use()
        {
            EventType = ImEventType.Used;
        }

        //public ImEventType GetTypeForControl()
        //{

        //}
    }
}
