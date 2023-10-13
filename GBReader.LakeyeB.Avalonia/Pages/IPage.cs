using System;

namespace GBReader.LakeyeB.Avalonia.Pages
{
    public interface IPage
    {
        public void OnEnter();
        
        /// <summary>
        /// Par "on quit", comprendre lors de la fermeture de l'application et non lorsque l'on quitte la vue
        /// </summary>
        public void OnQuit();
    }
}