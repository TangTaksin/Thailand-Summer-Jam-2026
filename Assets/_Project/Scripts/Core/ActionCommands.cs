using System;
using UnityEngine;

public class ActionCommands : MonoBehaviour
{

    public static Action OnNewFileCommand;
    public static Action OnRefreshCommand;
    public static Action OnDeleteCommand;

    public void ExecuteNewFile()
    {
        OnNewFileCommand?.Invoke();
    }

    public void ExecuteRefresh()
    {
        OnRefreshCommand?.Invoke();
    }

    public void ExecuteDelete()
    {
        OnDeleteCommand?.Invoke();
    }
}
