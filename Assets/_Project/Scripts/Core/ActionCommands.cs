using System;
using UnityEngine;

public class ActionCommands : MonoBehaviour
{

    public static Action OnNewFileCommand;
    public static Action OnRefreshCommand;
    public static Action OnDeleteCommand;
    public static Action OnEmptyBinCommand;
    public static Action OnFormatCommand;
    public static Action OnGameOver;

    public static Action<BaseFile> OnFileEaten;

    public void ExecuteNewFile()
    {
        OnNewFileCommand?.Invoke();
        AudioManager.Instance.PlaySFX("Click");

    }

    public void ExecuteRefresh()
    {
        OnRefreshCommand?.Invoke();
        AudioManager.Instance.PlaySFX("Refresh");
    }

    public void ExecuteDelete()
    {
        OnDeleteCommand?.Invoke();
        AudioManager.Instance.PlaySFX("Delete");
    }
}
