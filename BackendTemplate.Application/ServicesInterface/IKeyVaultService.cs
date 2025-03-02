namespace BackendTemplate.Application.ServicesInterface
{
    public interface IKeyVaultService
    {
        Task<string> GetSecretAsync(string secretName);
    }
}
