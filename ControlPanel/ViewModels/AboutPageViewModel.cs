using ControlPanel.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlPanel.ViewModels
{
    public class AboutPageViewModel : BinableBase
    {
        private string pageTitle;

        public string PageTitle
        {
            get { return pageTitle; }
            set { SetValue(ref pageTitle, value); }
        }

        public AboutPageViewModel()
        {
            PageTitle = "AboutPage";
        }
    }
}
