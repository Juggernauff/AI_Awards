using AI_Awards.Configurations;
using Microsoft.Extensions.Options;

namespace AI_Awards.Services.Implementation
{
    public class TextService : ITextService
    {
        private readonly IPythonService _pythonService;
        private readonly PythonScriptsConfiguration _configuration;

        public TextService(
            IPythonService pythonService,
            IOptions<PythonScriptsConfiguration> options)
        {
            _pythonService = pythonService;
            _configuration = options.Value;
        }

        public async Task<string?> TextAnalyze(string text)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException(nameof(text));

            return await _pythonService.RunScript(new string[] { _configuration.TextAnalyzeScriptPath, "--text", $"\"{text}\"" });
        }
    }
}
