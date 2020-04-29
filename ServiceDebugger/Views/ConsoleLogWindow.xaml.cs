using System;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;

namespace ServiceDebugger.Views
{
    public partial class ConsoleLogWindow
    {
        public ConsoleLogWindow()
        {
            InitializeComponent();

            Loaded += (sender, e) =>
            {
                ShowConsoleOutput();
                Runner.ConsoleLogMessages.CollectionChanged += ModifiedCollection;
            };
            Closed += (sender, e) => Runner.ConsoleLogMessages.CollectionChanged -= ModifiedCollection;
        }

        private void ModifiedCollection(object s, NotifyCollectionChangedEventArgs a)
        {
            Dispatcher.BeginInvoke((Action) ShowConsoleOutput);
        }

        private void ShowConsoleOutput()
        {
            TbLog.Text = string.Join(string.Empty, Runner.ConsoleLogMessages.Reverse());
        }

        private void BtClearLog_OnClick(object sender, RoutedEventArgs e)
        {
            Runner.ConsoleLogMessages.Clear();
        }
    }
}