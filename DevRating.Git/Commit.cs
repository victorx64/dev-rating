namespace DevRating.Git
{
    public interface Commit
    {
        string Sha();
        string Repository();
    }
}