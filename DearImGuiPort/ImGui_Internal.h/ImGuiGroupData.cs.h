// Stacked data for BeginGroup()/EndGroup()
struct ImGuiGroupData
{
	ImVec2      BackupCursorPos;
	ImVec2      BackupCursorMaxPos;
	float       BackupIndentX;
	float       BackupCurrentLineHeight;
	float       BackupCurrentLineTextBaseOffset;
	float       BackupLogLinePosY;
	bool        AdvanceCursor;
};