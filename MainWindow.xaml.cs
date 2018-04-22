using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Collections.ObjectModel;

using simpleaudioNET;
using iteratorNET;

namespace volumemixer
{
    public class VolumeViewModel
    {
        public VolumeViewModel(simpleaudioNET.Device device)
        {
            this.device = device;
            this.session = null;
        }

        public VolumeViewModel(simpleaudioNET.Session session)
        {
            this.device = null;
            this.session = session;
        }

        public string Name
        {
            get
            {
                if (this.device != null)
                    return device.FriendlyName;
                else
                    return session.DisplayName;
            }
        }

        public double Volume
        {
            get
            {
                if (this.device != null)
                    return denormalize(device.Volume);
                else
                    return denormalize(session.Volume);
            }
            set
            {
                if (this.device != null)
                    device.Volume = normalize(value);
                else
                    session.Volume = normalize(value);
            }
        }

        public int SliderMax
        {
            get { return SLIDER_MAX; }
        }

        public int SliderMin
        {
            get { return SLIDER_MIN; }
        }

        private simpleaudioNET.Session session;
        private simpleaudioNET.Device device;

        private static readonly int SLIDER_MAX = 100;
        private static readonly int SLIDER_MIN = 0;

        private float normalize(double formVolume)
        {
            double min = SLIDER_MIN;
            double max = SLIDER_MAX;
            return (float)((formVolume - min) / (max - min));
        }

        private double denormalize(float volume)
        {
            double min = SLIDER_MIN;
            double max = SLIDER_MAX;
            return (double)((volume * (max - min)) + min);
        }
    }

    public class MainViewModel
    {
        public MainViewModel()
        {
            setUpSessions();
        }

        public VolumeViewModel Device
        {
            get { return device; }
        }

        public ObservableCollection<VolumeViewModel> Sessions
        {
            get { return sessions; }
        }

        //

        private VolumeViewModel device;
        private ObservableCollection<VolumeViewModel> sessions;
        private void setUpSessions()
        {
            sessions = new ObservableCollection<VolumeViewModel>();

            simpleaudioNET.Interface audio = new simpleaudioNET.Interface();
            simpleaudioNET.Device defaultdevice = audio.getDefaultDevice();
            device = new VolumeViewModel(defaultdevice);

            iteratorNET.Iterator<Session> it = defaultdevice.sessionIterator();
            while (it.hasNext())
            {
                sessions.Add(new VolumeViewModel(it.next()));
            }
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainViewModel();
        }
    }
}
