namespace Skyweaver.Models.ChatSession
{
    public enum ChatSessionContentBlockKind
    {
        Text = 0,
        Code = 1,
        Status = 2,
        Placeholder = 3,
        StructuredXml = 4,
        ToolCall = 5,
        ToolOutput = 6,
        ToolReference = 7,
        Image = 8,
        Audio = 9,
        HostPreservedContent = 10,
        Reasoning = 11
    }
}
