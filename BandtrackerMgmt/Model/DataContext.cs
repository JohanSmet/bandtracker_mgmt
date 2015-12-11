using System;
using System.Windows.Data;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BandtrackerMgmt
{
    public class DataContext
    {
        public DataContext()
        {
            BindingOperations.EnableCollectionSynchronization(m_bands, m_lock_bands);
        }

        // bands
        public void BandsClear()
        {
            lock (m_lock_bands)
            {
                m_bands.Clear();
            }
        }

        public void BandsMergeList(List<Band> p_bands)
        {
            lock (m_lock_bands)
            {
                foreach (var f_band in p_bands)
                    m_bands.Add(f_band);
            }
        }

        // properties
        public ObservableCollection<Band>   Bands { get { return m_bands; } }

        // variables
        private ObservableCollection<Band>  m_bands = new ObservableCollection<Band>();
        private object                      m_lock_bands = new object();
    }
}
