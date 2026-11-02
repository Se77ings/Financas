namespace Financeiro.Application.Models.EntryModels
{
    public class RenameEntriesRequest
    {
        public List<Guid> PayableIds { get; set; } = new();
        public List<Guid> ReceivableIds { get; set; } = new();
        public string Description { get; set; }
    }
}
