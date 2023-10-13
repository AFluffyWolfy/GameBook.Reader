using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using GBReader.LakeyeB.Presentations.Events;
using GBReader.LakeyeB.Presentations.ViewModel;

namespace GBReader.LakeyeB.Avalonia.Controls
{
    /// <summary>
    /// "Sous-vue" permettant de gérer l'affichage de tout les livres
    /// Cette vue est en principe liée à MainMenu
    /// </summary>
    public partial class BooksPanel : UserControl
    {
        private List<BookViewModel> _books = new();
        public BooksPanel()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Affiche les livre présent dans l'attribut 'books'.
        /// </summary>
        public void DisplayBooks()
        {
            ClearView();
            if (_books.Count == 0)
            {
                TextBlock noBooks = new()
                {
                    Text = "Pas de livres disponibles."
                };
                Books.Children.Add(noBooks);
                return;
            }
            
            foreach (var wrapPanel in _books.Select(DisplayBook))
            {
                Books.Children.Add(wrapPanel);
            }
        }
        /// <summary>
        /// Affiche les livre present dans l'attribut 'books'.
        /// La seule différence avec la méthode d'avant est le message renvoyé.
        /// </summary>
        public void DisplayBooksSearch()
        {
            ClearView();
            if (_books.Count == 0)
            {
                
                TextBlock noBooks = new()
                {
                    Text = "Aucun résultat."
                };
                Books.Children.Add(noBooks);
                return;
            }
            
            foreach (var wrapPanel in _books.Select(DisplayBook))
            {
                Books.Children.Add(wrapPanel);
            }
        }

        private void ClearView() => Books.Children.Clear();
        public void ReceiveBooks(List<BookViewModel> books) => _books = books ?? new List<BookViewModel>();

        /// <summary>
        /// S'occupe de créer l'affichage pour un livre, boutons avec Invoke d'events inclus.
        /// </summary>
        /// <param name="book"></param>
        /// <returns></returns>
        private WrapPanel DisplayBook(BookViewModel book)
        {
            WrapPanel wrapPanel = new()
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(20, 0, 0, 10),
            };

            WrapPanel authorArea = new();

            TextBlock author = new()
            {
                Text = book.Author
            };
            authorArea.Children.Add(author);

            TextBlock title = new()
            {
                Text = book.Title,
                TextWrapping = TextWrapping.Wrap
            };

            TextBlock isbn = new()
            {
                Text = book.Isbn,
                Name = "isbn",
                TextWrapping = TextWrapping.Wrap
            };

            TextBlock summary = new()
            {
                Text = book.Summary,
                Name = "summary",
                IsVisible = false,
                TextWrapping = TextWrapping.Wrap
            };

            Button buttonSummary = new();
            buttonSummary.Click += OnShowSummaryClick;
            buttonSummary.Content = "Afficher résumé";
            buttonSummary.FontFamily = "Arial Rounded MT";

            Button buttonRead = new();
            buttonRead.Click += OnReadBookClick;
            buttonRead.Content = "Lire ce livre";
            buttonRead.FontFamily = "Arial Rounded MT";

            wrapPanel.Children.Add(authorArea);
            wrapPanel.Children.Add(title);
            wrapPanel.Children.Add(isbn);
            wrapPanel.Children.Add(summary);
            wrapPanel.Children.Add(buttonSummary);
            wrapPanel.Children.Add(buttonRead);
            wrapPanel.Background = Brush.Parse("LightSkyBlue");
            return wrapPanel;
        }
        
        private void OnShowSummaryClick(object? sender, RoutedEventArgs args)
        {
            Button button = (Button)sender!;
            var parent = (WrapPanel)button.Parent!;
            foreach(var children in parent.Children)
            {
                if(children.Name == "summary")
                {
                    children.IsVisible= true;
                    button.IsVisible = false;
                }
            }
        }
        
        private void OnReadBookClick(object? sender, RoutedEventArgs args)
        {
            Button button = (Button)sender!;
            var parent = (WrapPanel)button.Parent!;
            foreach (var children in parent.Children)
            {
                TextBlock? block = children as TextBlock;
                if (children.Name == "isbn" && block != null)
                {
                    var bookReadArgs = new BookReadEventArgs(block.Text);
                    ReadBookRequested?.Invoke(this, bookReadArgs);
                }
            }
            
        }
        
        public event EventHandler<BookReadEventArgs>? ReadBookRequested;
    }
}
