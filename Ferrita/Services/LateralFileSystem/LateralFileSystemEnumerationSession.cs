namespace Ferrita.Services.LateralFileSystem
{
    internal sealed class LateralFileSystemEnumerationSession
    {
        public List<LateralFileSystemEntry> Entries { get; } = new();

        public string SearchExpression { get; set; } = "*";

        public int NextIndex { get; set; }
    }
}
