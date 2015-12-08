using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BandtrackerMgmt
{
    class ViewModelMainWindow : ViewModelBase
    {

        // properties
        public string WindowTitle { get { return m_window_title; } set { SetField(ref m_window_title, value); } }

        // variables.ViewModel
        private string m_window_title = "BandTracker Management";
    }
}
