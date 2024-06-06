using Microsoft.AspNetCore.Components.Forms;

namespace AI_Awards.Models
{
    public class AnalyzerFormModel
    {
        public const long MAX_FILE_SIZE_BYTES = 50 * 1024 * 1024;

        public string? Text { get; set; }
        public IBrowserFile? File { get; set; }

        public AnalyzerFormModel()
        {
            Text = null;
            File = null;
        }
    }
}
