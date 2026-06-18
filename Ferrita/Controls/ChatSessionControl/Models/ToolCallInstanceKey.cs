namespace Ferrita.Controls.ChatSessionControl.Models
{
    public readonly record struct ToolCallInstanceKey(
        int IterationNumber,
        int PartIndex,
        int ToolCallIndex)
    {
        public static ToolCallInstanceKey Create(
            int? iterationNumber,
            int? partIndex,
            int toolCallIndex)
        {
            return new ToolCallInstanceKey(
                iterationNumber ?? 0,
                partIndex ?? 0,
                toolCallIndex);
        }
    }
}
