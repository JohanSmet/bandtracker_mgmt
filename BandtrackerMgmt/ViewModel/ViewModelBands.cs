using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BandtrackerMgmt
{
    using FilterTypeEntry = KeyValuePair<ViewModelBands.FilterType, string>;

    public class ViewModelBands : ViewModelBase
    {
        // initalization
        public ViewModelBands()
        {
            m_cmd_refresh  = new SimpleCommand(Refresh);
            m_cmd_cancel   = new SimpleCommand(CancelRefresh, false);
        }

        override public void Initialize()
        {
            FilterCurrent = m_filter_types.FirstOrDefault();

            // load the initial filter
            Refresh();
        }

        // commands
        public void Refresh()
        {
            var f_nobio     = m_filter_current.Key == FilterType.ftBandsNoBio;
            var f_nodiscogs = m_filter_current.Key == FilterType.ftBandsNoDiscogs;

            ui_refresh_running(true);

            var f_cancel_token = m_refresh_cancellation.Token;

            Task.Run(async () =>
            {
                DataCentral.Context.BandsClear();

                // don't load everything at once
                bool f_keep_running = true;
                int f_skip = 0;

                while (f_keep_running && !f_cancel_token.IsCancellationRequested)
                {
                    var f_bands = await BandTrackerClient.Instance.BandList(BANDS_PER_PAGE, f_skip, m_filter_name, f_nobio, f_nodiscogs, f_cancel_token);
                    DataCentral.Context.BandsMergeList(f_bands);

                    f_keep_running = f_bands.Count > 0;
                    f_skip += BANDS_PER_PAGE;
                }

                // signal completion (on the main thread)
                App.Current.Dispatcher.Invoke(() => { ui_refresh_running(false); });
            }, f_cancel_token);
        }

        public void CancelRefresh()
        {
            m_refresh_cancellation.Cancel();
        }

        // helper functions
        private void ui_refresh_running(bool p_running)
        {
            m_cmd_refresh.CanExecute = !p_running;
            m_cmd_cancel.CanExecute = p_running;

            if (p_running)
                m_refresh_cancellation = new CancellationTokenSource();
            else
                m_refresh_cancellation.Dispose();
        }

        // nested types
        public enum FilterType
        {
            ftBandsAll,
            ftBandsNoBio,
            ftBandsNoDiscogs
        };

        const int BANDS_PER_PAGE = 100;

        // properties
        public static string Id { get { return "Bands"; } }
        public string PageTitle { get { return m_page_title; } set { SetField(ref m_page_title, value); } }

        public ObservableCollection<Band>   Bands          { get { return DataCentral.Context.Bands;  } }
        public List<FilterTypeEntry>        FilterTypes    { get { return m_filter_types; } }
        public FilterTypeEntry              FilterCurrent  { get { return m_filter_current; }   set { if (SetField(ref m_filter_current, value)) Refresh(); } }
        public string                       FilterName     { get { return m_filter_name; }      set { if (SetField(ref m_filter_name, value)) Refresh(); } }

        public SimpleCommand CommandRefresh  { get { return m_cmd_refresh; } }
        public SimpleCommand CommandCancel   { get { return m_cmd_cancel; } }

        // variables
        private string                m_page_title = "Bands";

        private string                m_filter_name;
        private FilterTypeEntry       m_filter_current;
        private List<FilterTypeEntry> m_filter_types = new List<FilterTypeEntry> {
            new FilterTypeEntry(FilterType.ftBandsAll,   "All Bands"),
            new FilterTypeEntry(FilterType.ftBandsNoBio, "Bands without biography")
        };

        private SimpleCommand   m_cmd_refresh;
        private SimpleCommand   m_cmd_cancel;

        private CancellationTokenSource m_refresh_cancellation;
    }
}
