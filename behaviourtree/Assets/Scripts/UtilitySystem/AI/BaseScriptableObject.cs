using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
[InitializeOnLoad]
public abstract class BaseScriptableObject : ScriptableObject
{
    [SerializeField] private bool autoReset = true;
    public BaseScriptableObject()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }
    private void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            AssetDatabase.SaveAssets();
        }
        if (state == PlayModeStateChange.EnteredEditMode && autoReset)
        {
            Resources.UnloadAsset(this);
        }
    }
}
#endif