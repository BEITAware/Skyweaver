namespace Skyweaver.Controls.ChatSessionControl.Models
{
    public enum ChatMessagePartType
    {
        Text = 0,
        Code = 1,
        Status = 2,
        Placeholder = 3,
        Tool = 4,
        ToolCall = 5,
        ToolOutput = 6,
        StructuredXml = 7,
        Image = 8,
        Audio = 9,
        Video = 10,
        Document = 11,
        HostPreservedContent = 12,
        Reasoning = 13,
        TextAttachment = 14
    }
}
