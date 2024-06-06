using System.Diagnostics;
using System.Text;

namespace AI_Awards.Services.Implementation
{
    public class PythonService : IPythonService
    {
        public async Task<string?> RunScript(string[] args)
        {
            if (args.Length == 0)
                throw new ArgumentException(nameof(args));

            var processStartInfo = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = string.Join(' ', args),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            };

            using var process = Process.Start(processStartInfo);

            if (process == null)
                throw new InvalidOperationException("Не удалось запустить процесс.");

            var output = await process.StandardOutput.ReadToEndAsync();
            var error = await process.StandardError.ReadToEndAsync();

            if (!string.IsNullOrEmpty(error))
                throw new Exception(error);

            process.WaitForExit();

            return output;
        }
    }
}
