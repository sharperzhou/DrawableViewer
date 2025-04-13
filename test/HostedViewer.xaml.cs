using System;

namespace DrawableViewer.Test
{
    public partial class HostedViewer : IDisposable
    {
        public HostedViewer()
        {
            InitializeComponent();
        }

        public void Dispose()
        {
            SourceViewer?.Dispose();
            AnotherViewer?.Dispose();
        }
    }
}