﻿using System.Management.Automation.Subsystem.Prediction;
using System.Reflection;
using System.Text.Json;
using PPCLI.PowerShell.Predictor.Abstractions.Interfaces;
using PPCLI.PowerShell.Predictor.Abstractions.Model;

namespace PPCLI.PowerShell.Predictor.Services
{
    public class PPCLIPowerShellPredictorService : IPPCLIPowerShellPredictorService
    {
        private List<Suggestion>? _allPredictiveSuggestions;

        public PPCLIPowerShellPredictorService()
        {
            RequestAllPredictiveCommands();
        }
        protected virtual void RequestAllPredictiveCommands()
        {
            //TODO: Decide if we need to make an http request here to get all the commands
            //TODO: if the http request fails then fallback to local JSON file?
            _ = Task.Run(async () =>
            {
                string executableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string fileName = Path.Combine($"{executableLocation}{PPCLIPowerShellPredictorConstants.SuggestionsFileRelativePath}", PPCLIPowerShellPredictorConstants.SuggestionsFileName);
                string jsonString = await File.ReadAllTextAsync(fileName);
                _allPredictiveSuggestions = JsonSerializer.Deserialize<List<Suggestion>>(jsonString)!;
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
