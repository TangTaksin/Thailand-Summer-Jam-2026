using UnityEngine;

public class BaseFolder : MonoBehaviour
{
    protected virtual bool CanAcceptFile(BaseFile file) { return true; }

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
    protected virtual void ProcessFile(BaseFile file) { }
    protected virtual void RejectFile(BaseFile file) { }
}
