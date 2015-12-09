using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BandtrackerMgmt
{
    public class ViewModelBands : ViewModelBase
    {
        // initalization
        override public void Initialize()
        {

        }

        // properties
        public static string Id { get { return "Bands"; } }
        public string PageTitle { get { return m_page_title; } set { SetField(ref m_page_title, value); } }

        // variables
        private string m_page_title = "Bands";
    }
}
