namespace AI_Awards.Services
{
    public interface IPythonService
    {
        public Task<string?> RunScript(string[] args);
    }
}
