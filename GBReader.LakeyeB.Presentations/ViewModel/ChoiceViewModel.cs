using GBReader.LakeyeB.Domains;

namespace GBReader.LakeyeB.Presentations.ViewModel
{
    public class ChoiceViewModel
    {
        public string Text { get; }
        public int PageFrom { get; }
        public int PageTo { get; }

        public ChoiceViewModel(Choice choice)
        {
            Text = choice.Text;
            PageFrom = choice.PageFrom;
            PageTo = choice.PageTo;
        }
    }
}