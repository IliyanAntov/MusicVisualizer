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

        private int soundCardSampleRate = 48000; // Sound card sample rate

        private int bufferSize = (int)Math.Pow(2, 11); // Sound buffer size

        private int audioDevice = 1; // Audio device to record from
        private int audioChannels = 1; // Audio channels

        private Queue<double> bassHistory = new Queue<double>(); // Used for keeping a history of the bass range for better beat detection
        private Queue<double> lowMidHistory = new Queue<double>(); // Used for keeping a history of the low midrange
        private int historyMaxSize; // Used for calculating bassHistory and lowMidHistory sizes

        private double historySizeInSeconds = 1; // Determines the history queue's size in seconds

        private double bassMultiplier = 3; // Determines how high the jump has to be to detect a bass (in times)

        private double bassAverage = 0; // Used for storing the average of the values stored in bassHistory
        private double lowMidAverage = 0; // Used for storing the average of the low midrange values stored in lowMidHistory

        private int variance = 0; // (TODO) Used for altering the jump requirement for detecting a beat based on the variance of the song

        private int bassLowFreq = 30; // Determines the lowest frequency of the bass range (in Hz)
        private int bassHighFreq = 130; // Determines the highest frequency of the bass range (in Hz)

        int lowMidLowFreq = 250; // Determines the lowest frequency of the low midrange (in Hz)
        int lowMidHighFreq = 500; // Determines the highest frequency of the low midrange (in Hz)

        int hzPerItem; // Used for calculating what the frequency of each item in the buffer is (The difference in Hz between 2 items in the buffer)
        int bassItemsToSkip; // Stores the number of items that need to be skipped from the buffer to reach the bass range
        int bassItemsToTake; // Stores the number of items that need to be taken from the buffer to get the desired bass range values 
        int lowMidItemsToSkip; // Stores the number of items that need to be skipped from the buffer to reach the low midrange
        int lowMidItemsToTake; // Stores the number of items that need to be taken from the buffer to get the desired low midrange values 

        private SerialPort port = new SerialPort("COM5", 115200); // Defines the port at which the Arduino is plugged in

        public MainPage() { // Initializes the program on startup
            
            // Windows forms initialization
            InitializeComponent();

            // Serial communication initialization
            InitializeCommunication();

            // Audio recording initialization
            RecordAudio();
        }

        private void RecordAudio() { // Initializes the audio recording

            // Makes an instance of the WaveIn class using the given audio device, sample rate and audio channels
            WaveIn wi = new WaveIn();
            wi.DeviceNumber = audioDevice;
            wi.WaveFormat = new NAudio.Wave.WaveFormat(soundCardSampleRate, audioChannels);

            // Calculates the milliseconds needed to record 1 byte with the given sound card sample rate
            double msPerByte = (1 / (double)soundCardSampleRate) * 1000;

            // Sets up the buffer milliseconds of the WaveIn class to fill the desired buffer
            wi.BufferMilliseconds =  (int)(bufferSize * msPerByte);

            // Wries avaliable data to the buffer
            wi.DataAvailable += new EventHandler<WaveInEventArgs>(AudioDataAvailable);

            // Makes an instance of the BufferedWaveProvider class using the given buffer size and wave format
            bwp = new BufferedWaveProvider(wi.WaveFormat);
            bwp.BufferLength = bufferSize;

            // Sets up the buffer to discard old items when new data is avaliable
            bwp.DiscardOnBufferOverflow = true;

            // Starts the audio recording from the desired audio device
            try {
                wi.StartRecording();
            }
            catch {
                Console.WriteLine("Error reading audio input");
            }

            // Calculates the difference in Hz between 2 items in the buffer
            hzPerItem = (soundCardSampleRate / 2) / (bufferSize / 2);

            // Calculates the maximum items to be stored in history
            historyMaxSize = (int)((soundCardSampleRate / (bufferSize / 2)) * historySizeInSeconds);

            // Calculates the number of items that need to be skipped from the buffer to reach the bass range
            bassItemsToSkip = bassLowFreq / hzPerItem;

            // Calculates the number of items that need to be taken from the buffer to get the desired bass range values 
            bassItemsToTake = (bassHighFreq - bassLowFreq) / hzPerItem;

            // Calculates the number of items that need to be skipped from the buffer to reach the low midrange
            lowMidItemsToSkip = lowMidLowFreq / hzPerItem;

            // Calculates the number of items that need to be taken from the buffer to get the desired low midrange values 
            lowMidItemsToTake = (lowMidHighFreq - lowMidLowFreq) / hzPerItem;
        }

        void AudioDataAvailable(object sender, WaveInEventArgs e) { // Executes when audio data is avaliable

            // Writes the incoming audio data to the buffer
            bwp.AddSamples(e.Buffer, 0, e.BytesRecorded);
        }

        private void InitializeCommunication() { // Initializes the serial communication with the microcontroller

            // Opens the serial port
            port.Open();
        }

        private void StartDefaultButton_Click(object sender, EventArgs e) { // Executes when the "Start default" button is pressed

            // Writes the default visualization start flag to the serial port
            port.Write("a");

            // Enables the update timer, allowing beat detection to begin
            UpdateTimer.Enabled = true;
        }

        private void StartAlternateButton_Click(object sender, EventArgs e) { // Executes when the "Start alternate" button is pressed

            // Writes the alternate visualization start flag to the serial port
            port.Write("b");

            // Enables the update timer, allowing beat detection to begin
            UpdateTimer.Enabled = true;
        }

        private void StartLightShiftButton_Click(object sender, EventArgs e) {

            // Writes the light shift visualization start flag to the serial port
            port.Write("c");

            // Enables the update timer, allowing beat detection to begin
            UpdateTimer.Enabled = true;
        }

        private void StopButton_Click(object sender, EventArgs e) { // Executes when the "Stop" button is pressed

            // Writes the stop flag to the serial port
            port.Write("e");

            // Disables the update timer, thus stopping beat detection
            UpdateTimer.Enabled = false;

        }

        private void UpdateTimer_Tick(object sender, EventArgs e) { // Executes every time the update timer ticks

            // Analyzes the incoming audio and detects beats
            AnalyzeAudio();
        }

        private void AnalyzeAudio() { // Analyzes the incoming audio and detects beats

            // Reads audio bytes from the buffer and stores them in a local array
            var audioBytes = new byte[bufferSize];
            bwp.Read(audioBytes, 0, bufferSize);

            // Calculates the compressed array size
            int compressedArraySize = audioBytes.Length / 2;

            // Initializes the 3 arrays that are going to be used for calculations
            double[] compressedArray = new double[compressedArraySize];
            double[] fft = new double[compressedArraySize];
            double[] fftReal = new double[compressedArraySize / 2];

            // Populates the compressed array with data
            for (int i = 0; i < compressedArraySize; i++) {

                // Converts 2 consecutive bytes to 1 Int16 for easier calculation
                Int16 val = BitConverter.ToInt16(audioBytes, i * 2);

                // Converts the value in the Int16 to percent (+/- 100%)
                compressedArray[i] = (double)(val) / Math.Pow(2, 16) * 200.0;
            }

            // Calculates the Fast Fourier Transform
            fft = FFT(compressedArray);

            // Copies the real part of the FFT to the fftReal array
            Array.Copy(fft, fftReal, fftReal.Length);

            // Raises the values to the second power so that low values will be closer to 0 and high values will be higher, thus making the difference bigger and easier to work with
            fftReal = fftReal.Select(x => (x * x)).ToArray();

            // Calculates the current bass value from the last FFT
            double currentBassValues = fftReal.Skip(bassItemsToSkip).Take(bassItemsToTake).Average();

            // Calculates the current low midrange value from the last FFT
            double currentLowMidValues = fftReal.Skip(lowMidItemsToSkip).Take(lowMidItemsToTake).Average();

            // Detects beats by comparing the current bass value to the history average
            if (currentBassValues > (bassMultiplier * bassAverage) && currentBassValues >= 1) {

                // Calculates the beat strength using the current bass value and the average and sends the result to the serial port
                CalculateNumberOfLeds(currentBassValues, bassAverage);
            }

            // Removes the oldest entry in each queue if it is full
            if (bassHistory.Count() >= historyMaxSize) {
                bassHistory.Dequeue();
                lowMidHistory.Dequeue();
            }

            // Saves the current bass value to the bass history
            bassHistory.Enqueue(currentBassValues);

            // Saves the current low midrange value to the low midrange history
            lowMidHistory.Enqueue(currentLowMidValues);

            // Calculates the bass average from history
            bassAverage = bassHistory.Average();

            // Calculates the low midrange average from history
            lowMidAverage = lowMidHistory.Average();
        }

        private void CalculateNumberOfLeds(double currentBassValues, double bassAverage) { // Calculates the beat strength and sends it to the serial port

            // Calculates how much stronger the beat was from the averge
            double jump = currentBassValues / bassAverage;

            // Low jump
            if (jump < 10) {
                port.Write("2");
            }

            // Medium jump
            else if (jump >= 10 && jump <= 100) {
                port.Write("4");
            }
            else {

                // High jump
                port.Write("8");
            }
        }

        public double[] FFT(double[] data) { // Performs a FFT on the given data

            // Used to store the result
            double[] fft = new double[data.Length];

            // Used to store the complex of the FFT
            System.Numerics.Complex[] fftComplex = new System.Numerics.Complex[data.Length];

            // Calculates the complex for every item in the array
            for (int i = 0; i < data.Length; i++) {
                fftComplex[i] = new System.Numerics.Complex(data[i], 0.0);
            }

            // Performs a FFT
            Accord.Math.FourierTransform.FFT(fftComplex, Accord.Math.FourierTransform.Direction.Forward);

            // Copies the magnitude of every item to the resulting array
            for (int i = 0; i < data.Length; i++) {
                fft[i] = fftComplex[i].Magnitude;
            }

            // Returns the result of the FFT
            return fft;
        }

        private void SlowButton_Click(object sender, EventArgs e) { // Executes when the "Slow" button is pressed

            // Writes the slow speed flag to the serial port
            port.Write("l");
        }

        private void MediumButton_Click(object sender, EventArgs e) { // Executes when the "Medium" button is pressed

            // Writes the medium speed flag to the serial port
            port.Write("m");
        }

        private void FastButton_Click(object sender, EventArgs e) { // Executes when the "Fast" button is pressed

            // Writes the fast speed flag to the serial port
            port.Write("f");
        }

        private void VeryFastButton_Click(object sender, EventArgs e) { // Executes when the "Very fast" button is pressed

            // Writes the very fast speed flag to the serial port
            port.Write("v");
        }

        private void SensitivityBar_Scroll(object sender, EventArgs e) { // Executes when the "Sensitivity" bar is moved

            // Adjusts the bassMultiplier to the new value (Higher value == Higher jump needed before a beat is detected)
            bassMultiplier = 12 - SensitivityBar.Value;
        }

        private void ShiftSpeed_Scroll(object sender, EventArgs e) { // Executes when the "Shift speed" bar is moved

            // Writes the new sensitivity value to the serial port
            port.Write(ShiftSpeed.Value.ToString());
        }
    }
}
