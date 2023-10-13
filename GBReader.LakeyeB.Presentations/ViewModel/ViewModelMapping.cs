using GBReader.LakeyeB.Domains;

namespace GBReader.LakeyeB.Presentations.ViewModel
{
    /// <summary>
    /// Cette classe me sers à éviter de faire des foreach dans le presenter
    /// pour transformer une liste de Domain en DomainViewModel
    /// Pas de pages car une page n'est qu'un int donnant un string, donc un dictionnaire
    /// </summary>
    public static class ViewModelMapping
    {
        public static List<BookViewModel> ToBookViewModel(List<Book> books)
        {
            List<BookViewModel> toViewModel = new();
            foreach (var book in books)
            {
                toViewModel.Add(new BookViewModel(book));
            }

            return toViewModel;
        }

        public static List<ChoiceViewModel> ToChoicesViewModel(List<Choice> choices)
        {
            List<ChoiceViewModel> toViewModel = new();
            foreach (var choice in choices)
            {
                toViewModel.Add(new ChoiceViewModel(choice));
            }
            return toViewModel;
        }

        public static List<UserSessionViewModel> ToSessionsViewModel(List<UserSession> sessions)
        {
            List<UserSessionViewModel> toViewModel = new();
            foreach (var session in sessions)
            {
                toViewModel.Add(new UserSessionViewModel(session));
            }

            return toViewModel;
        }
    }
}