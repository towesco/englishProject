namespace englishProject.Infrastructure
{
    public enum Alert : int
    {
        info, warning
    }

    internal enum CommentIssue
    {
        comment = 1, issue = 2
    }

    public enum Modul
    {
        WordModul = 1,
        PictureWordModul = 2
    }

    public enum ModulSubLevel
    {
        Temel = 1, İleri = 2, Mükemmel = 3
    }

    public enum Kind
    {
        English = 1,
        German = 2
    }
}