using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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
            m_cmd_next     = new SimpleCommand(Next);
            m_cmd_previous = new SimpleCommand(Previous);

            m_skip = 0;
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
            var f_nobio = m_filter_current.Key == FilterType.ftBandsNoBio;

            new Task(async () =>
            {
                var f_bands = BandTrackerClient.Instance.BandList(BANDS_PER_PAGE, m_skip, m_filter_name, f_nobio);
                DataCentral.Context.BandsClear();
                DataCentral.Context.BandsMergeList(await f_bands);
            }).Start();
        }

        public void Next()
        {
            if (Bands.Count >= BANDS_PER_PAGE)
            {
                m_skip += BANDS_PER_PAGE;
                Refresh();
            }
        }

        public void Previous()
        {
            if (m_skip <= 0)
                return;

            m_skip = Math.Max(0, m_skip - BANDS_PER_PAGE);
            Refresh();
        }

        // nested types
        public enum FilterType
        {
            ftBandsAll,
            ftBandsNoBio
        };

        const int BANDS_PER_PAGE = 50;

        // properties
        public static string Id { get { return "Bands"; } }
        public string PageTitle { get { return m_page_title; } set { SetField(ref m_page_title, value); } }

        public ObservableCollection<Band>   Bands          { get { return DataCentral.Context.Bands;  } }
        public List<FilterTypeEntry>        FilterTypes    { get { return m_filter_types; } }
        public FilterTypeEntry              FilterCurrent  { get { return m_filter_current; }   set { if (SetField(ref m_filter_current, value)) Refresh(); } }
        public string                       FilterName     { get { return m_filter_name; }      set { if (SetField(ref m_filter_name, value)) Refresh(); } }

        public SimpleCommand CommandRefresh  { get { return m_cmd_refresh; } }
        public SimpleCommand CommandNext     { get { return m_cmd_next; } }
        public SimpleCommand CommandPrevious { get { return m_cmd_previous; } }

        // variables
        private string                m_page_title = "Bands";

        private string                m_filter_name;
        private FilterTypeEntry       m_filter_current;
        private List<FilterTypeEntry> m_filter_types = new List<FilterTypeEntry> {
            new FilterTypeEntry(FilterType.ftBandsAll,   "All Bands"),
            new FilterTypeEntry(FilterType.ftBandsNoBio, "Bands without biography")
        };

        private SimpleCommand   m_cmd_refresh;
        private SimpleCommand   m_cmd_next;
        private SimpleCommand   m_cmd_previous;

        private int m_skip;

    }
}
