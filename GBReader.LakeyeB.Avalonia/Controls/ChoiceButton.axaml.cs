using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using GBReader.LakeyeB.Presentations.Events;

namespace GBReader.LakeyeB.Avalonia.Controls
{
    /// <summary>
    /// "Sous-vue" permettant d'afficher le bouton pour un choix avec les events qui y sont liés.
    /// </summary>
    public partial class ChoiceButton : UserControl
    {
        public event EventHandler<ChoiceSelectedEventArgs>? ChoiceSelected; 
        private int _pageTo;
        public ChoiceButton()
        {
            InitializeComponent();
        }

        public void InitializeChoice(string text, int pageTo)
        {
            Button.Content = text;
            _pageTo = pageTo;
            Button.Click += OnChoiceButtonClick;
        }

        private void OnChoiceButtonClick(object? sender, RoutedEventArgs e) => ChoiceSelected?.Invoke(this, new ChoiceSelectedEventArgs(_pageTo));
    }
}
