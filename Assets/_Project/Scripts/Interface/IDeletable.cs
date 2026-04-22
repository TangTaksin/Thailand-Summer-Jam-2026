public interface IDeletable
{
    bool CanDelete(out string reason);
    void Delete();

}