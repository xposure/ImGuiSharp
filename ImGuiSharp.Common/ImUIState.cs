namespace ImGui
{
    struct ImUIState
    {
        public int mousex;
        public int mousey;
        public bool mousedown;

        public int hotitem;
        public int activeitem;

        public int kbditem;
        public bool keyenterdown;
        public bool keyenterpress;
        public bool keytabpressed;
        public bool keyshiftdown;
        public bool keyuppressed;
        public bool keydownpressed;
        public bool keybackspacepressed;

        public string inputchar;
        //public int keyentered;
        //public int keymod;

        public int lastwidget;
    }
}
