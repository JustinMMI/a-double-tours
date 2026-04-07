using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public class AutoSaveOnFocus
{
    static AutoSaveOnFocus()
    {
        // On s'abonne à l'événement de changement d'état de l'application
        EditorApplication.delayCall += () =>
        {
            Application.focusChanged += OnFocusChanged;
        };
    }

    private static void OnFocusChanged(bool focus)
    {
        // Si focus est "false", ça veut dire que tu as quitté Unity (Alt-Tab)
        if (!focus)
        {
            // On sauvegarde la scène active
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                Debug.Log("<color=green>Auto-Save : Scène sauvegardée car Unity a perdu le focus.</color>");
            }

            // On sauvegarde aussi les assets (ProjectSettings, Prefabs, etc.)
            AssetDatabase.SaveAssets();
        }
    }
}