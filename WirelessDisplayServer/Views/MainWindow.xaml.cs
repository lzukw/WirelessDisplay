using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using WirelessDisplayServer.ViewModels;

namespace WirelessDisplayServer.Views
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            //this.AttachDevTools();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            this.Closed += (o,e) => {((MainWindowViewModel) DataContext).OnWindowClosed(); };
        }
    }
}