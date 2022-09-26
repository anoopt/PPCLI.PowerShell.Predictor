using System.Management.Automation;
using System.Management.Automation.Subsystem;

namespace PPCLI.PowerShell.Predictor
{
    /// <summary>
    /// Register the predictor on module loading and unregister it on module un-loading.
    /// </summary>
    public class Init : IModuleAssemblyInitializer, IModuleAssemblyCleanup
    {
        private const string Identifier = "AAE9205F-3D7D-11ED-9F60-5905AA0E278C";

        /// <summary>
        /// Gets called when assembly is loaded.
        /// </summary>
        public void OnImport()
        {
            var predictor = new PPCLIPowerShellPredictor(Identifier);
            SubsystemManager.RegisterSubsystem(SubsystemKind.CommandPredictor, predictor);
        }

        /// <summary>
        /// Gets called when the binary module is unloaded.
        /// </summary>
        public void OnRemove(PSModuleInfo psModuleInfo)
        {
            SubsystemManager.UnregisterSubsystem(SubsystemKind.CommandPredictor, new Guid(Identifier));
        }
    }
}
