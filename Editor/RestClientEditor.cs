using System.Collections.Generic;
using UnityEditor;

namespace RestClientService.Editor
{
    public class RestClientEditor
    {
        private const string LOGGING_SYMBOL = "REST_CLIENT_LOGGING";

#if !REST_CLIENT_LOGGING
        [MenuItem("Custom Tools/Rest Client/Enable Logging")]
        private static void EnableLogging()
        {
            PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, out string[] symbols);

            List<string> symbolsList = new List<string>(symbols.Length + 1) { LOGGING_SYMBOL };
            symbolsList.AddRange(symbols);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, symbolsList.ToArray());
        }
#else
        [MenuItem("Custom Tools/Rest Client/Disable Logging")]
        private static void DisableLogging()
        {
            PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, out string[] symbols);

            List<string> symbolsList = new List<string>(symbols.Length);
            symbolsList.AddRange(symbols);
            if (symbolsList.Remove(LOGGING_SYMBOL))
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, symbolsList.ToArray());
            }
        }
#endif
    }
}