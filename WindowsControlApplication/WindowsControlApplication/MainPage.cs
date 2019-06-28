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

        private double bassAverage = 0;
        private double lowMidAverage = 0;

        private int variance = 0;
        private int historyLength = 0;

        private int bassLowFreq = 20;
        private int bassHighFreq = 130;

        int lowMidLowFreq = 300;
        int lowMidHighFreq = 750;

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
            historyMaxSize = RATE / (BUFFER_SIZE / 2);

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
            port.Write("b");
        }

        private void button2_Click(object sender, EventArgs e) {
            port.Write("e");

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
                //    //Console.WriteLine(pcm[i]);
            }

            // calculate the full FFT
            fft = FFT(pcm);

            // determine horizontal axis units for graphs
            double fftMaxFreq = RATE / 2;
            double hzPerValue = fftMaxFreq / graphPointCount;

            // just keep the real half (the other half imaginary)
            Array.Copy(fft, fftReal, fftReal.Length);
            //Console.WriteLine(fftMaxFreq + " " + fftReal.Count() + " " + fftPointSpacingHz);

            //int[] newfftReal = fftReal.Select(x => (int)(x * x)).ToArray();
            double[] newfftReal = fftReal.Select(x => (x)).ToArray();

            double currentBassValues = newfftReal.Skip(bassItemsToSkip).Take(bassItemsToTake).Average();
            double currentLowMidValues = newfftReal.Skip(lowMidItemsToSkip).Take(lowMidItemsToTake).Average();
            
            if (currentBassValues > (5 * bassAverage)) {
                Console.WriteLine(currentBassValues);
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

            //Console.WriteLine(newfftReal.Average());
            //Console.WriteLine(fftReal.Skip(2).Take(3).Max());
            //if (fftReal.Skip(2).Take(3).Where(x => x >= 20).Count() > 1) {
            //    Console.WriteLine(fftReal.Skip(2).Take(3).Max());
            //    Console.WriteLine();
            //}
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
    }
}
