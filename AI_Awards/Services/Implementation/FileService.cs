using AI_Awards.Configurations;
using AI_Awards.Models;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Options;
using static System.Net.Mime.MediaTypeNames;

namespace AI_Awards.Services.Implementation
{
    public class FileService : IFileService
    {
        private readonly IPythonService _pythonService;
        private readonly PythonScriptsConfiguration _configuration;

        public FileService(
            IPythonService pythonService,
            IOptions<PythonScriptsConfiguration> options)
        {
            _pythonService = pythonService;
            _configuration = options.Value;
        }

        public async Task<string?> FileAnalyze(IBrowserFile file)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            var fileName = file.Name;
            var fileExtension = Path.GetExtension(fileName);

            switch(fileExtension)
            {
                case ".txt":
                    return await TextFileAnalyze(file);
                case ".mp3":
                    return await SoundFileAnalize(file, fileName, fileExtension);
                default:
                    throw new ArgumentException("Недопустимый формат файла.");
            }
        }

        private async Task<string?> TextFileAnalyze(IBrowserFile file)
        {
            var text = string.Empty;

            using (var stream = file.OpenReadStream(AnalyzerFormModel.MAX_FILE_SIZE_BYTES))
            {
                using (var reader = new StreamReader(stream))
                {
                    text = await reader.ReadToEndAsync();
                    return await _pythonService.RunScript(new string[] { _configuration.TextAnalyzeScriptPath, "--text", $"\"{text}\"" });
                }
            }
        }

        private async Task<string?> SoundFileAnalize(IBrowserFile file, string fileName, string fileExtension)
        {
            var guid = Guid.NewGuid();
            var currentDate = DateTime.Now;
            var directoryPath = Path.Combine("./Sounds", currentDate.Year.ToString(), currentDate.Month.ToString(), currentDate.Day.ToString());
            var newFileName = $"{Path.GetFileNameWithoutExtension(fileName)}-{guid}{fileExtension}";
            var filePath = Path.Combine(directoryPath, newFileName);

            Directory.CreateDirectory(directoryPath);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.OpenReadStream(AnalyzerFormModel.MAX_FILE_SIZE_BYTES).CopyToAsync(stream);
            }

            var text =  await _pythonService.RunScript(new string[] { _configuration.FileAnalyzeScriptPath, $"\"{filePath}\"" });
            return await _pythonService.RunScript(new string[] { _configuration.TextAnalyzeScriptPath, "--text", $"\"{text}\"" });
        }
    }
}
