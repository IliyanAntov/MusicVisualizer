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
            wi.BufferMilliseconds = (int)((double)BUFFER_SIZE / (double)RATE * 1000.0);
            wi.DataAvailable += new EventHandler<WaveInEventArgs>(AudioDataAvailable);
            bwp = new BufferedWaveProvider(wi.WaveFormat);
            bwp.BufferLength = BUFFER_SIZE * 2;
            bwp.DiscardOnBufferOverflow = true;
            try {
                wi.StartRecording();
            }
            catch {
                string msg = "Could not record from audio device!\n\n";
                msg += "Is your microphone plugged in?\n";
                msg += "Is it set as your default recording device?";
                Console.WriteLine(msg, "ERROR");
            }
            Console.WriteLine(WaveIn.DeviceCount);
            
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
            int frameSize = BUFFER_SIZE;
            var audioBytes = new byte[frameSize];
            bwp.Read(audioBytes, 0, frameSize);


            int BYTES_PER_POINT = 2;
            int graphPointCount = audioBytes.Length / BYTES_PER_POINT;

            // create double arrays to hold the data we will graph
            double[] pcm = new double[graphPointCount];
            double[] fft = new double[graphPointCount];
            double[] fftReal = new double[graphPointCount / 2];

            // populate Xs and Ys with double data
            for (int i = 0; i < graphPointCount; i++) {
                // read the int16 from the two bytes
                Int16 val = BitConverter.ToInt16(audioBytes, i * 2);

                // store the value in Ys as a percent (+/- 100% = 200%)
                pcm[i] = (double)(val) / Math.Pow(2, 16) * 200.0;
            }

            // calculate the full FFT
            fft = FFT(pcm);

            // determine horizontal axis units for graphs
            double pcmPointSpacingMs = RATE / 1000;
            double fftMaxFreq = RATE / 2;
            double fftPointSpacingHz = fftMaxFreq / graphPointCount;

            // just keep the real half (the other half imaginary)
            Array.Copy(fft, fftReal, fftReal.Length);

            ////Console.WriteLine(fft.Count());
            //for (int i = 0; i < fftReal.Count(); i++) {
            //    Console.Write((int)fftReal[i] + ", ");
            //}
            int[] newfftReal = fftReal.Select(x => (int)x).ToArray();
            if (newfftReal.Take(10).Where(x => x >= 7).Count() > 1){
                Random rnd = new Random();
                int a = rnd.Next(1, 100);
                Console.WriteLine(a);
                Console.WriteLine();
                //Console.WriteLine(newfftReal.First());
                //Console.WriteLine();
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
    }
}
