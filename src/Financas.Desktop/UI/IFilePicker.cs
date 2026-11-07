namespace Financas.Desktop.UI
{
    /// <summary>Os métodos devolvem <c>null</c> quando o usuário cancela.</summary>
    public interface IFilePicker
    {
        Task<string?> PickFolderAsync();
        Task<string?> PickBackupFileAsync();
    }
}
