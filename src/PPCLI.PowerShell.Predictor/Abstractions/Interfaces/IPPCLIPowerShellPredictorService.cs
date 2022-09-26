using System.Management.Automation.Subsystem.Prediction;

namespace PPCLI.PowerShell.Predictor.Abstractions.Interfaces
{
    public interface IPPCLIPowerShellPredictorService
    {
        public List<PredictiveSuggestion>? GetSuggestions(PredictionContext context);
    }
}
