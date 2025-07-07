using headspace.Models.Common;

namespace headspace.Models
{
    public partial class DocumentModel : ModelBase
    {
        private string? _content;
        public string? Content
        {
            get => _content;
            set => SetPropertyAndMarkDirty(ref _content, value);
        }
    }
}
