using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wpf.Ui.Controls;
using Xceed.Wpf.AvalonDock.Layout;

namespace WpfApp1.Page.ViewModels
{
    public interface IScnacbarService
    {
        void Show();
    }

    public class SnackbarService : IScnacbarService
    {
        public void Show()
        {
            Snackbar bar = new Snackbar();
            bar.Message = "Test";
            bar.Show();
        }
    }

}
