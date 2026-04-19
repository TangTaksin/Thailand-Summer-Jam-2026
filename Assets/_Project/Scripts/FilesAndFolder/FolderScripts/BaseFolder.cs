using UnityEngine;

public class BaseFolder : MonoBehaviour
{

    public virtual void ReceiveFile(BaseFile droppedFile)
    {
        if (CanAcceptFile(droppedFile))
        {
            ProcessFile(droppedFile);
        }
        else
        {
            RejectFile(droppedFile);
        }
    }

    protected virtual bool CanAcceptFile(BaseFile file) { return true; }
    protected virtual void ProcessFile(BaseFile file) { }
    protected virtual void RejectFile(BaseFile file) { }
}
