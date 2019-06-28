using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace WindowsControlApplication {
    public partial class MainPage : Form {

        public BufferedWaveProvider bwp; 

        private int RATE = 48000;
        private int BUFFER_SIZE = (int)Math.Pow(2, 11);
        private Queue<double> bassHistory = new Queue<double>();
        private Queue<double> lowMidHistory = new Queue<double>();
        private int historyMaxSize;

        private double secondsToBuffer = 1;

        private double bassMultiplier = 3;

        private double bassAverage = 0;
        private double lowMidAverage = 0;

        private int variance = 0;
        private int historyLength = 0;

        private int bassLowFreq = 30;
        private int bassHighFreq = 130;

        int lowMidLowFreq = 250;
        int lowMidHighFreq = 500;

        int hzPerItem;
        int bassItemsToSkip;
        int bassItemsToTake;
        int lowMidItemsToSkip;
        int lowMidItemsToTake;


        private SerialPort port = new SerialPort("COM5", 115200);

        public MainPage() {
            InitializeComponent();
            InitializeCommunication();
            RecordAudio();
        }

        void AudioDataAvailable(object sender, WaveInEventArgs e) {
            bwp.AddSamples(e.Buffer, 0, e.BytesRecorded);
        }

        private void RecordAudio() {

            WaveIn wi = new WaveIn();
            wi.DeviceNumber = 1;
            wi.WaveFormat = new NAudio.Wave.WaveFormat(RATE, 1);

            double bytesPerMs = (1 / (double)RATE) * 1000;
            wi.BufferMilliseconds =  (int)(BUFFER_SIZE * bytesPerMs);

            wi.DataAvailable += new EventHandler<WaveInEventArgs>(AudioDataAvailable);

            bwp = new BufferedWaveProvider(wi.WaveFormat);
            bwp.BufferLength = BUFFER_SIZE;
            bwp.DiscardOnBufferOverflow = true;
            try {
                wi.StartRecording();
            }
            catch {
                Console.WriteLine("Error reading audio input");
            }

            hzPerItem = (RATE / 2) / (BUFFER_SIZE / 2);
            historyMaxSize = (int)((RATE / (BUFFER_SIZE / 2)) * secondsToBuffer);

            bassItemsToSkip = bassLowFreq / hzPerItem;
            bassItemsToTake = (bassHighFreq - bassLowFreq) / hzPerItem;

            lowMidItemsToSkip = lowMidLowFreq / hzPerItem;
            lowMidItemsToTake = (lowMidHighFreq - lowMidLowFreq) / hzPerItem;
            Console.WriteLine(lowMidItemsToSkip);
            Console.WriteLine(lowMidItemsToTake);
            Console.WriteLine(historyMaxSize);
        }

        private void InitializeCommunication() {
            port.Open();
        }

        private void button1_Click(object sender, EventArgs e) {
            port.Write("s");
            UpdateTimer.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e) {
            port.Write("e");
            UpdateTimer.Enabled = false;

        }

        private void UpdateTimer_Tick(object sender, EventArgs e) {

            var audioBytes = new byte[BUFFER_SIZE];
            bwp.Read(audioBytes, 0, BUFFER_SIZE);

            int BYTES_PER_POINT = 2;
            int graphPointCount = audioBytes.Length / BYTES_PER_POINT;

            // create double arrays to hold the data we will graph
            double[] pcm = new double[graphPointCount];
            double[] fft = new double[graphPointCount];
            double[] fftReal = new double[graphPointCount / 2];

            // populate Xs and Ys with double data
            for (int i = 0; i < graphPointCount; i++) {
                //    // read the int16 from the two bytes
                Int16 val = BitConverter.ToInt16(audioBytes, i * 2);

                //    // store the value in Ys as a percent (+/- 100% = 200%)
                pcm[i] = (double)(val) / Math.Pow(2, 16) * 200.0;
            }

            // calculate the full FFT
            fft = FFT(pcm);

            // determine horizontal axis units for graphs
            double fftMaxFreq = RATE / 2;
            double hzPerValue = fftMaxFreq / graphPointCount;

            // just keep the real half (the other half imaginary)
            Array.Copy(fft, fftReal, fftReal.Length);

            double[] newfftReal = fftReal.Select(x => (x * x)).ToArray();

            double currentBassValues = newfftReal.Skip(bassItemsToSkip).Take(bassItemsToTake).Average();
            double currentLowMidValues = newfftReal.Skip(lowMidItemsToSkip).Take(lowMidItemsToTake).Average();

            if (currentBassValues > (bassMultiplier * bassAverage) && currentBassValues >= 1) {
                CalculateNumberOfLeds(currentBassValues, bassAverage);
            }

            //Keeping a 1s history of bass and low mids averages
            historyLength = bassHistory.Count();

            if (historyLength >= historyMaxSize) {
                bassHistory.Dequeue();
                lowMidHistory.Dequeue();
            }

            bassHistory.Enqueue(currentBassValues);
            lowMidHistory.Enqueue(currentLowMidValues);

            bassAverage = bassHistory.Average();
            lowMidAverage = lowMidHistory.Average();
        }

        private void CalculateNumberOfLeds(double currentBassValues, double bassAverage) {
            double jump = currentBassValues / bassAverage;
            if (jump < 10) {
                port.Write("2");
            }
            else if (jump >= 10 && jump <= 100) {
                port.Write("4");
            }
            else {
                port.Write("8");
            }
        }

        public double[] FFT(double[] data) {
            double[] fft = new double[data.Length];
            System.Numerics.Complex[] fftComplex = new System.Numerics.Complex[data.Length];
            for (int i = 0; i < data.Length; i++)
                fftComplex[i] = new System.Numerics.Complex(data[i], 0.0);
            Accord.Math.FourierTransform.FFT(fftComplex, Accord.Math.FourierTransform.Direction.Forward);
            for (int i = 0; i < data.Length; i++)
                fft[i] = fftComplex[i].Magnitude;
            return fft;
        }

        private void SlowButton_Click(object sender, EventArgs e) {
            port.Write("l");
        }

        private void MediumButton_Click(object sender, EventArgs e) {
            port.Write("m");
        }

        private void FastButton_Click(object sender, EventArgs e) {
            port.Write("f");
        }

        private void VFastButton_Click(object sender, EventArgs e) {
            port.Write("v");
        }

        private void SensitivityBar_Scroll(object sender, EventArgs e) {
            bassMultiplier = SensitivityBar.Value;
        }

    }
}
