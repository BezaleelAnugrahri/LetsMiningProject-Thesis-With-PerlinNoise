using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoUpdateData : ScriptableObject
{
    public event System.Action OnValuesUpdated;
    public bool autoUpdate;

#if UNITY_EDITOR//it means just usable inside unity editor

    protected virtual void OnValidate()
    {
        //get's called in a number of situation(s), when a value change in inspector it will be called
        if (autoUpdate)
        {
            UnityEditor.EditorApplication.update += NotifyOfUpdatedValues;
            //NotifyOfUpdatedValues();
        }
    }

    public void NotifyOfUpdatedValues()
    {
        UnityEditor.EditorApplication.update -= NotifyOfUpdatedValues;
        if (OnValuesUpdated != null)
        {
            OnValuesUpdated();
        }
    }

#endif

}