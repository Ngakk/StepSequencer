using UnityEditor;
using UnityEngine;

namespace StepSequencer.Editor
{
    public class SceneSaveHandler : AssetModificationProcessor
    {
        static string[] OnWillSaveAssets(string[] paths)
        {
            foreach (var path in paths)
            {
                if (path.EndsWith(".unity"))
                {
                    Debug.Log($"[SceneSaveHandler] Running sequencer setup for sequences in scene: {path}");
                    RunCustomPreSaveLogic();
                }
            }

            return paths;
        }

        static void RunCustomPreSaveLogic()
        {
            // Example: find all objects of a type and call a method
            foreach (var obj in GameObject.FindObjectsOfType<Sequencer>())
            {
                obj.RunStepSetup();
            }
        }
    }
}