using System;
using UnityEngine;

public class RefreshCommand : MonoBehaviour
{   
    public static Action OnRefreshCommand;
    public void ExecuteRefresh()
    {
        OnRefreshCommand?.Invoke();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1)) // Right mouse button pressed
        {
            ExecuteRefresh();
        }
    }

}
