using LibGit2Sharp;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Git.HistoryAnalysis
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public SeriesCollection SeriesCollection { get; set; } = new SeriesCollection();
        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void Analisis(string path)
        {
            Repository repo = null;
            try
            {
                repo = new Repository(path);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return;
            }

            var comitters = new List<Committer>();
            foreach (var commit in repo.Commits)
            {
                if (!comitters.Any(x => x.Signature.Name == commit.Committer.Name))
                {
                    var comitter = new Committer(commit.Committer);
                    comitter.CommitsDate.Add(commit.Committer.When);
                    comitters.Add(comitter);
                    continue;
                }
                comitters.Find(x => x.Signature.Name == commit.Committer.Name).CommitsDate.Add(commit.Committer.When);
            }

            var dayConfig = Mappers.Xy<ChartModel>()
                           .X(dayModel => dayModel.DateTime.Ticks)
                           .Y(dayModel => dayModel.Value);

            SeriesCollection.Clear();
            SeriesCollection.Configuration = dayConfig;

            var i = 5;
            foreach (var committer in comitters)
            {
                var series = new LineSeries();
                var totalDays = committer.CommitsDate.First() - committer.CommitsDate.Last();
                series.Title = $"{committer.Signature.Name} - total days = {totalDays} - commits = {committer.CommitsDate.Count}";
                var values = new ChartValues<ChartModel>();
                foreach (var dateTimeOffset in committer.CommitsDate)
                {
                    values.Add(new ChartModel(dateTimeOffset.DateTime, i));
                }
                series.Values = values;
                i += 5;

                SeriesCollection.Add(series);
            }

            YFormatter = value => new DateTime((long)value).ToString("yyyy-MM:dd HH:mm:ss");
            Chart.Series = SeriesCollection;
            AxisX.LabelFormatter = YFormatter;
        }

        private void SelectRepoForAnalysis_Click(object sender, RoutedEventArgs e)
        {
            var openFileDlg = new System.Windows.Forms.FolderBrowserDialog();
            var result = openFileDlg.ShowDialog();
            if (result.ToString() != string.Empty)
            {
                Analisis(openFileDlg.SelectedPath);
            }
        }
    }
}
