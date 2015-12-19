using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BandtrackerMgmt
{
    public class ViewModelTasks : ViewModelBase
    {
        // initalization
        public ViewModelTasks()
        {
            m_cmd_refresh = new SimpleCommand(Refresh);
        }

        override public void Initialize()
        {
            // load the initial filter
            Refresh();
        }

        // commands
        public void Refresh()
        {
            ui_refresh_running(true);

            Task.Run(async () => {
                var f_tasks = await BandTrackerClient.Instance.TaskList("any");
                DataCentral.Context.TasksSet(f_tasks);

                // signal completion (on the main thread)
                App.Current.Dispatcher.Invoke(() => { ui_refresh_running(false); });
            });
        }

        // helper functions
        private void ui_refresh_running(bool p_running)
        {
            m_cmd_refresh.CanExecute = !p_running;
        }

        // properties
        public static string    Id { get { return "Tasks"; } }
        public string           PageTitle { get { return m_page_title; } set { SetField(ref m_page_title, value); } }

        public ObservableCollection<ServerTask> Tasks { get { return DataCentral.Context.Tasks; } }

        public SimpleCommand    CommandRefresh { get { return m_cmd_refresh; } }

        // variables
        private string          m_page_title = "Tasks";

        private SimpleCommand   m_cmd_refresh;
    }

}
