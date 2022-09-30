using System.Management.Automation.Subsystem.Prediction;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json;
using PPCLI.PowerShell.Predictor.Abstractions.Interfaces;
using PPCLI.PowerShell.Predictor.Abstractions.Model;

namespace PPCLI.PowerShell.Predictor.Services
{
    public class PPCLIPowerShellPredictorService : IPPCLIPowerShellPredictorService
    {
        private SuggestionsFile? _suggestionsFile;
        private List<Suggestion>? _allPredictiveSuggestions;
        private readonly HttpClient _client;
        private readonly string? _commandsFilePath;


        public PPCLIPowerShellPredictorService()
        {
            _commandsFilePath = PPCLIPowerShellPredictorConstants.CommandsFilePath; //Add modifications in future if needed
            _client = new HttpClient();
            RequestAllPredictiveCommands();
        }

        private void SetPredictiveSuggestions()
        {
            var lastUpdatedOn = _suggestionsFile?.LastUpdatedOn;
            _allPredictiveSuggestions = _suggestionsFile?.Suggestions;

            var today = DateTime.Now.ToString("dd MMMM yyyy");

            if (lastUpdatedOn != today)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"WARNING: Predictions displayed will be as of {lastUpdatedOn}. So, you might not see some examples being predicted. Press enter to continue.");
                Console.ResetColor();
            }
        }

        protected virtual void RequestAllPredictiveCommands()
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    _suggestionsFile = await _client.GetFromJsonAsync<SuggestionsFile>(_commandsFilePath);
                    SetPredictiveSuggestions();
                }
                catch (Exception)
                {
                    _allPredictiveSuggestions = null;
                }

                if (_allPredictiveSuggestions == null)
                {
                    try
                    {
                        var executableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                        var fileName = Path.Combine($"{executableLocation}{PPCLIPowerShellPredictorConstants.SuggestionsFileRelativePath}", PPCLIPowerShellPredictorConstants.SuggestionsFileName);
                        var jsonString = await File.ReadAllTextAsync(fileName);
                        _suggestionsFile = JsonSerializer.Deserialize<SuggestionsFile>(jsonString);
                        SetPredictiveSuggestions();
                    }
                    catch (Exception)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Write("Unable to load predictions. Press enter to continue.");
                        Console.ResetColor();
                        _allPredictiveSuggestions = null;
                    }
                }
            });
        }

        public virtual List<PredictiveSuggestion>? GetSuggestions(PredictionContext context)
        {
            var input = context.InputAst.Extent.Text;
            if (string.IsNullOrWhiteSpace(input))
            {
                return null;
            }

            //TODO: Decide how the source data should be structured and then add a logic to get filtered suggestions
            var filteredSuggestions = _allPredictiveSuggestions?.
                FindAll(pc => pc.Command.ToLower().StartsWith(input.ToLower())).
                OrderBy(pc => pc.Rank);

            var result = filteredSuggestions?.Select(fs => new PredictiveSuggestion(fs.Command)).ToList();

            return result;
        }
    }
}
