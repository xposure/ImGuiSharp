namespace ImGui
{
    // Stacked data for BeginGroup()/EndGroup()
    internal struct ImGuiGroupData
    {
        public ImVec2 BackupCursorPos;
        public ImVec2 BackupCursorMaxPos;
        public float BackupIndentX;
        public float BackupCurrentLineHeight;
        public float BackupCurrentLineTextBaseOffset;
        public float BackupLogLinePosY;
        public bool AdvanceCursor;
    }
}
