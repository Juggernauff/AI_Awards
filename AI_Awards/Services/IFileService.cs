using Microsoft.AspNetCore.Components.Forms;

namespace AI_Awards.Services
{
    public interface IFileService
    {
        public Task<string?> FileAnalyze(IBrowserFile file);
    }
}
