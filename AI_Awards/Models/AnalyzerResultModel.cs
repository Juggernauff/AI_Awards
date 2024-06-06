namespace AI_Awards.Models
{
    public class AnalyzerResultModel
    {
        public string? Text { get; set; }
        public bool WaitingAnswer { get; set; }

        public AnalyzerResultModel()
        {
            Text = null;
            WaitingAnswer = false;
        }

        public AnalyzerResultModel(string text)
        {
            Text = text;
            WaitingAnswer = false;
        }
    }
}
