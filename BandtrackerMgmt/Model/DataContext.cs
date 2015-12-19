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
            BindingOperations.EnableCollectionSynchronization(m_tasks, m_lock_tasks);
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

        // tasks
        public void TasksSet(List<ServerTask> p_tasks)
        {
            lock (m_lock_tasks)
            {
                m_tasks.Clear();

                foreach (var f_task in p_tasks)
                    m_tasks.Add(f_task);
            }
        }

        // properties
        public ObservableCollection<Band>       Bands { get { return m_bands; } }
        public ObservableCollection<ServerTask> Tasks { get { return m_tasks; } }

        // variables
        private ObservableCollection<Band>      m_bands = new ObservableCollection<Band>();
        private object                          m_lock_bands = new object();

        private ObservableCollection<ServerTask> m_tasks = new ObservableCollection<ServerTask>();
        private object                           m_lock_tasks = new object();
    }
}
