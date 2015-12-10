using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BandtrackerMgmt
{
    public class ViewModelMainWindow : ViewModelBase
    {
        ////////////////////////////////////////////////////////////////////////////////
        //
        // initalization
        //

        override public void Initialize()
        {
        }

        ////////////////////////////////////////////////////////////////////////////////
        //
        // properties
        //

        public string                                WindowTitle { get { return m_window_title; } set { SetField(ref m_window_title, value); } }
        public ObservableCollection<string>          Pages       { get { return m_pages; } }

        ////////////////////////////////////////////////////////////////////////////////
        //
        // variables.ViewModel
        //

        private string                                  m_window_title  = "BandTracker Management";
        private ObservableCollection<string>            m_pages = new ObservableCollection<string>();
    }
}
