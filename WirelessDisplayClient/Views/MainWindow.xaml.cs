using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using WirelessDisplayClient.ViewModels;

namespace WirelessDisplayClient.Views
{
    public class MainWindow : Window
    {
        private bool  _readyToCloseWindow = false;

        public MainWindow()
        {
            InitializeComponent();

            //this.AttachDevTools();

            // Don't like this hack, but I couldn't get this work in XAML.
            this.Closing += async (o,e) => { 
                if ( ! _readyToCloseWindow )
                {
                    e.Cancel = true;
                    await ( (MainWindowViewModel) DataContext).OnWindowClose();
                    // Now we are ready to terminate
                    _readyToCloseWindow = true;
                    this.Close(); // reaises another Closing-event.
                }
            };
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}