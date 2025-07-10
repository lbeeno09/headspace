using headspace.Models.Common;

namespace headspace.Models
{
    public partial class ScreenplayModel : ModelBase
    {
        private string? _content;
        public string? Content
        {
            get => _content;
            set => SetPropertyAndMarkDirty(ref _content, value);
        }

        public override string FilePathPrefix => "screenplay";
    }
}
