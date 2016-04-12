using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using MahApps.Metro.Controls;
using NAudio.CoreAudioApi;
using NAudio.Mixer;

namespace VolumeControl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            Settings = new Settings();
            MicrophoneLevel = Settings.DefaultVolume;

            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
            TryGetVolumeControl();

        }

        public int MicrophoneLevel
        {
            get { return (int)GetValue(MicrophoneLevelProperty); }
            set { SetValue(MicrophoneLevelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MicrophoneLevel.  This enables animation, styling, binding, etc...
        public static DependencyProperty MicrophoneLevelProperty =
            DependencyProperty.Register("MicrophoneLevel", typeof(int), typeof(MainWindow), new PropertyMetadata(0));


        private List<UnsignedMixerControl> volumeControlList = new List<UnsignedMixerControl>();
        private Settings Settings;

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
            foreach (MMDevice device in enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.All))
            {
                Debug.WriteLine("{0}, {1}, {2}", device.FriendlyName, device.State, device.AudioEndpointVolume.VolumeRange);
                Debug.WriteLine("level is {0}", MicrophoneLevel);
                device.AudioEndpointVolume.MasterVolumeLevelScalar = MicrophoneLevel/100f;
            }
        }

        private void TryGetVolumeControl()
        {
            int waveInDeviceNumber = 0;
            var mixerLine = new MixerLine((IntPtr)waveInDeviceNumber, 0, MixerFlags.WaveIn);

            foreach (var control in mixerLine.Controls)
            {
                if (control.ControlType == MixerControlType.Volume)
                {
                    volumeControlList.Add(control as UnsignedMixerControl);
                    break;
                }
            }
        }

        private void MetroWindow_Activated(object sender, EventArgs e)
        {
            this.Topmost = true;
        }

        private void MetroWindow_Deactivated(object sender, EventArgs e)
        {
            this.Topmost = true;
            this.Activate();
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Settings.DefaultVolume = MicrophoneLevel;
            Settings.Save();
        }

    }
}
