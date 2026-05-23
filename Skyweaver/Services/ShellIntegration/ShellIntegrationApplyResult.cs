namespace Skyweaver.Services.ShellIntegration
{
    public sealed class ShellIntegrationApplyResult
    {
        public bool Succeeded { get; init; }

        public bool IsRegistered { get; init; }

        public string ErrorMessage { get; init; } = string.Empty;

        public static ShellIntegrationApplyResult Success(bool isRegistered)
        {
            return new ShellIntegrationApplyResult
            {
                Succeeded = true,
                IsRegistered = isRegistered
            };
        }

        public static ShellIntegrationApplyResult Failure(string errorMessage, bool isRegistered)
        {
            return new ShellIntegrationApplyResult
            {
                Succeeded = false,
                IsRegistered = isRegistered,
                ErrorMessage = errorMessage
            };
        }
    }
}
