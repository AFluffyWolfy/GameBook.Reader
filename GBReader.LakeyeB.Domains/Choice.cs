namespace GBReader.LakeyeB.Domains
{
    /// <summary>
    /// Un choix est un numéro de page vers un autre numéro de page avec du texte lié à ce choix
    /// </summary>
    public class Choice
    {
        public string Text { get; }
        public int PageFrom { get; }
        public int PageTo { get; }

        public Choice(string text, int pageFrom, int pageTo)
        {
            Text = text;
            PageFrom = pageFrom;
            PageTo = pageTo;
        }
    }
}