using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace BandtrackerMgmt
{
    using FilterTypeEntry   = KeyValuePair<ViewModelBands.FilterType, string>;
    using FilterStatusEntry = KeyValuePair<int, string>;

    public class ViewModelBands : ViewModelBase
    {
        // initalization
        public ViewModelBands()
        {
            m_cmd_refresh           = new SimpleCommand(Refresh);
            m_cmd_cancel            = new SimpleCommand(CancelRefresh, false);
            m_cmd_musicbrainz_url   = new ParamCommand<IList>(MusicBrainzUrl);
            m_cmd_discogs_data      = new ParamCommand<IList>(DiscogsData);
            m_cmd_band_release      = new ParamCommand<IList>(BandRelease);
            m_cmd_band_revoke       = new ParamCommand<IList>(BandRevoke);

            m_cmd_browser_musicbrainz   = new SimpleCommand(BrowserMusicBrainz);
            m_cmd_browser_discogs       = new SimpleCommand(BrowserDiscogs);
        }

        override public void Initialize()
        {
            FilterCurrent       = m_filter_types.FirstOrDefault();
            FilterStatusCurrent = m_filter_status.FirstOrDefault();

            // load the initial filter
            Refresh();
        }

        // commands
        public void Refresh()
        {
            // build condition object for band query
            var f_cond = new BandTrackerClient.BandListConditions
            {
                namePattern      = !String.IsNullOrEmpty(m_filter_name) ? m_filter_name : null,
                withoutBiography = m_filter_current.Key == FilterType.ftBandsNoBio,
                withoutDiscogsId = m_filter_current.Key == FilterType.ftBandsNoDiscogs,
            };

            if (m_filter_status_current.Key != -100)
                f_cond.recordStatus = m_filter_status_current.Key;

            if (m_filter_current.Key == FilterType.ftBandsComplete)
            {
                f_cond.withBiography = true;
                f_cond.withDiscogsId = true;
                f_cond.namePattern   = ".+";
            }

            // indicate refresh is starting
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
                    var f_bands = await BandTrackerClient.Instance.BandList(BANDS_PER_PAGE, f_skip, f_cond, f_cancel_token);
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

        public void MusicBrainzUrl(IList p_selection)
        {
            Task.Run(async () =>
            {
                await BandTrackerClient.Instance.TaskCreateMusicBrainzUrl(band_selection_mbid_list(p_selection));
            });
        }

        public void BandRelease (IList p_selection)
        {
            Task.Run(async () =>
            {
                var f_server_update = BandTrackerClient.Instance.BandUpdateStatus(band_selection_mbid_list(p_selection), Band.Status.Released);

                // update the local records as well
                foreach (Band f_band in p_selection) 
                {
                    f_band.recordStatus = Band.Status.Released;
                }

                await f_server_update;
            });

        }

        public void BandRevoke (IList p_selection)
        {
            Task.Run(async () =>
            {
                var f_server_update = BandTrackerClient.Instance.BandUpdateStatus(band_selection_mbid_list(p_selection), Band.Status.Revoked);

                // update the local records as well
                foreach (Band f_band in p_selection) 
                {
                    f_band.recordStatus = Band.Status.Revoked;
                }

                await f_server_update;
            });
        }

        public void DiscogsData (IList p_selection)
        {
            Task.Run(async () =>
            {
                await BandTrackerClient.Instance.TaskCreateDiscogsBandInfo(band_selection_mbid_list(p_selection));
            });
        }

        public void BrowserMusicBrainz()
        {
            if (m_selected_band != null)
            {
                System.Diagnostics.Process.Start("https://musicbrainz.org/artist/" + m_selected_band.MBID);
            }
        }

        public void BrowserDiscogs()
        {
            if (m_selected_band != null)
            {
                System.Diagnostics.Process.Start("http://www.discogs.com/artist/" + m_selected_band.discogsId);
            }
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

        private void ui_selected_band_changed()
        {
            App.Current.Dispatcher.Invoke(async () =>
            {
                var f_img_data = await BandTrackerClient.Instance.BandImage(m_selected_band.MBID);

                var f_img = new BitmapImage();
                f_img.BeginInit();
                if (f_img_data != null) 
                { 
                    f_img.CacheOption = BitmapCacheOption.OnLoad;
                    f_img.StreamSource = new MemoryStream(f_img_data);
                }
                else
                {
                    f_img.UriSource = new Uri("/Resources/no_image.png", UriKind.Relative);
                }
                f_img.EndInit();


                BandImage = f_img;
            });
        }

        private List<string> band_selection_mbid_list(IList p_bands)
        {
            var f_mbids = new List<string>();

            foreach (Band f_band in p_bands)
            {
                f_mbids.Add(f_band.MBID);
            }

            return f_mbids;
        }

        // nested types
        public enum FilterType
        {
            ftBandsAll,
            ftBandsComplete,
            ftBandsNoBio,
            ftBandsNoDiscogs
        };

        const int BANDS_PER_PAGE = 100;

        // properties
        public static string Id { get { return "Bands"; } }
        public string PageTitle { get { return m_page_title; } set { SetField(ref m_page_title, value); } }

        public ObservableCollection<Band>   Bands          { get { return DataCentral.Context.Bands;  } }
        public Band                         SelectedBand   { get { return m_selected_band; }    set { if (SetField(ref m_selected_band, value)) ui_selected_band_changed(); } }
        public BitmapImage                  BandImage      { get { return m_band_image; }       set { SetField(ref m_band_image, value); } }
        public List<FilterTypeEntry>        FilterTypes    { get { return m_filter_types; } }
        public FilterTypeEntry              FilterCurrent  { get { return m_filter_current; }   set { if (SetField(ref m_filter_current, value)) Refresh(); } }
        public string                       FilterName     { get { return m_filter_name; }      set { if (SetField(ref m_filter_name, value)) Refresh(); } }
        public List<FilterStatusEntry>      FilterStatus   { get { return m_filter_status; } }
        public FilterStatusEntry            FilterStatusCurrent { get { return m_filter_status_current; }  set { if (SetField(ref m_filter_status_current, value)) Refresh(); } }

        public SimpleCommand                CommandRefresh          { get { return m_cmd_refresh; } }
        public SimpleCommand                CommandCancel           { get { return m_cmd_cancel; } }
        public ParamCommand<IList>          CommandMusicBrainzUrl   { get { return m_cmd_musicbrainz_url; } }
        public ParamCommand<IList>          CommandDiscogsData      { get { return m_cmd_discogs_data; } }
        public ParamCommand<IList>          CommandBandRelease      { get { return m_cmd_band_release; } }
        public ParamCommand<IList>          CommandBandRevoke       { get { return m_cmd_band_revoke; } }

        public SimpleCommand                CommandBrowserMusicBrainz   { get { return m_cmd_browser_musicbrainz; } }
        public SimpleCommand                CommandBrowserDiscogs       { get { return m_cmd_browser_discogs; } }

        // variables
        private string                m_page_title = "Bands";
        private Band                  m_selected_band = null;
        private BitmapImage           m_band_image = null;

        private string                m_filter_name;
        private FilterTypeEntry       m_filter_current;
        private List<FilterTypeEntry> m_filter_types = new List<FilterTypeEntry> {
            new FilterTypeEntry(FilterType.ftBandsAll,          "All Bands"),
            new FilterTypeEntry(FilterType.ftBandsComplete,     "Complete Bands"),
            new FilterTypeEntry(FilterType.ftBandsNoBio,        "Bands without biography"),
            new FilterTypeEntry(FilterType.ftBandsNoDiscogs,    "Bands without discogsId")
        };
        private FilterStatusEntry       m_filter_status_current;
        private List<FilterStatusEntry> m_filter_status = new List<FilterStatusEntry> {
            new FilterStatusEntry(-100, "All bands"),
            new FilterStatusEntry( 1, "Only released bands"),
            new FilterStatusEntry( 0, "Only new bands"),
            new FilterStatusEntry(-1, "Only revoked bands")
        };

        private SimpleCommand           m_cmd_refresh;
        private SimpleCommand           m_cmd_cancel;
        private ParamCommand<IList>     m_cmd_musicbrainz_url;
        private ParamCommand<IList>     m_cmd_discogs_data;
        private ParamCommand<IList>     m_cmd_band_release;
        private ParamCommand<IList>     m_cmd_band_revoke;

        private SimpleCommand           m_cmd_browser_musicbrainz;
        private SimpleCommand           m_cmd_browser_discogs;

        private CancellationTokenSource m_refresh_cancellation;
    }
}
