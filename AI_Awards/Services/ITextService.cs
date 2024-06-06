namespace AI_Awards.Services
{
    public interface ITextService
    {
        public Task<string?> TextAnalyze(string text);
    }
}
