using Gk_01.Observable;
using Gk_01.Views;
using System.Runtime.InteropServices;
using System.Threading.Channels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Gk_01.ViewModels
{
    public class HistogramViewModel : BaseViewModel
    {
        private static HistogramViewModel? _instance = null;
        private Image? _currentImage;
        private Image? _defaultImage;
        private HistogramWindow? _histogramWindow = null;

        private List<byte> RChannel = [];
        private List<byte> GChannel = [];
        private List<byte> BChannel = [];

        private List<byte> prevRChannel = [];
        private List<byte> prevGChannel = [];
        private List<byte> prevBChannel = [];

        public ICommand UndoHistogramCommand { get; set; }
        public ICommand ExpandHistogramCommand { get; set; }
        public ICommand EqualizeHistogramCommand { get; set; }
        private HistogramViewModel() 
        {
            UndoHistogramCommand = new RelayCommand(UndoHistogram);
            ExpandHistogramCommand = new RelayCommand(ExpandHistogram);
            EqualizeHistogramCommand = new RelayCommand(EqualizeHistogram);
        }

        private void EqualizeHistogram(object parameter)
        {
            prevRChannel = RChannel.ToList();
            prevGChannel = GChannel.ToList();
            prevBChannel = BChannel.ToList();

            // YCbCr - Y lumination brightness
            var yChannel = new List<byte>();
            var cbChannel = new List<byte>();
            var crChannel = new List<byte>();
            for (int i = 0; i < RChannel.Count; i++)
            {
                ConvertRGBToYCbCr(RChannel[i], GChannel[i], BChannel[i], out byte y, out byte cb, out byte cr);
                yChannel.Add(y);
                cbChannel.Add(cb);
                crChannel.Add(cr);
            }

            // Histogram equalization for brightness channel (Y)
            var equalizedYChannel = EqualizeChannel(yChannel);

            // RGB
            RChannel.Clear();
            GChannel.Clear();
            BChannel.Clear();
            for (int i = 0; i < equalizedYChannel.Count; i++)
            {
                ConvertYCbCrToRGB(equalizedYChannel[i], cbChannel[i], crChannel[i], out byte r, out byte g, out byte b);
                RChannel.Add(r);
                GChannel.Add(g);
                BChannel.Add(b);
            }

            // Update image and Histogram
            CreateHistograms(RChannel, GChannel, BChannel);
            _defaultImage = new Image
            {
                Source = (_currentImage!.Source as BitmapSource)?.Clone()
            };
            UpdateImage();
            _histogramWindow!.BackButton.IsEnabled = true;
        }

        private List<byte> EqualizeChannel(List<byte> channel)
        {
            List<byte> equalizedChannel = new List<byte>(channel.Count);

            // Brightness histogram 
            var histogram = new int[256];
            foreach (var value in channel)
                histogram[value]++;

            // CDF cumulative distribution
            int totalPixels = channel.Count;
            int cumulative = 0;
            var cdf = new int[256];
            for (int i = 0; i < 256; i++)
            {
                cumulative += histogram[i];
                cdf[i] = cumulative;
            }

            // Equalize histogram
            var minCDF = cdf.FirstOrDefault(value => value > 0);
            foreach (var value in channel)
            {
                byte newValue = (byte)Math.Round(((double)(cdf[value] - minCDF) / (totalPixels - minCDF)) * 255);
                equalizedChannel.Add(newValue);
            }

            return equalizedChannel;
        }


        private void ExpandHistogram(object parameter)
        {
            prevRChannel = RChannel.ToList();
            prevGChannel = GChannel.ToList();
            prevBChannel = BChannel.ToList();

            RChannel = ExpandChannel(RChannel);
            GChannel = ExpandChannel(GChannel);
            BChannel = ExpandChannel(BChannel);

            CreateHistograms(RChannel, GChannel, BChannel);

            _defaultImage = new Image
            {
                Source = (_currentImage!.Source as BitmapSource)?.Clone()
            };
            UpdateImage();
            _histogramWindow!.BackButton.IsEnabled = true;
        }

        private List<byte> ExpandChannel(List<byte> channel)
        {
            List<byte> expandedChannel = [];
            var minChannelValue = channel.Min();
            var maxChannelValue = channel.Max();

            foreach (var channelValue in channel)
            {
                var newValue = (byte)Math.Clamp(((channelValue - minChannelValue) / (double)(maxChannelValue - minChannelValue)) * 255, 0, 255);
                expandedChannel.Add(newValue);
            }
            return expandedChannel;
        }

        private void UpdateImage()
        {
            if (_currentImage == null) return;
            var bitmapSource = _currentImage!.Source as BitmapSource;
            var writeableBitmap = new WriteableBitmap(bitmapSource);

            writeableBitmap.Lock();
            int width = writeableBitmap.PixelWidth;
            int height = writeableBitmap.PixelHeight;
            int bytesPerPixel = (writeableBitmap.Format == PixelFormats.Bgr32) ? 4 : 3;
            int stride = writeableBitmap.BackBufferStride;
            byte[] pixelData = new byte[height * stride];

            var j = 0;
            for(var i=0; i<pixelData.Length; i+=bytesPerPixel)
            {
                if(i+2 < pixelData.Length)
                {
                    pixelData[i] = BChannel[j]; // B
                    pixelData[i+1] = GChannel[j]; // G
                    pixelData[i+2] = RChannel[j]; // R
                }
                if(bytesPerPixel == 4 && i+3 < pixelData.Length)
                {
                    pixelData[i + 3] = 255; // alpha
                }
                j++;
            }

            writeableBitmap.WritePixels(new Int32Rect(0, 0, width, height), pixelData, stride, 0);
            writeableBitmap.Unlock();
            _currentImage.Source = writeableBitmap;
        }

        private void UndoHistogram(object parameter)
        {
            if (_currentImage == null || _defaultImage == null) return;
            _currentImage.Source = (_defaultImage!.Source as BitmapSource)?.Clone();
            _defaultImage = null;

            _histogramWindow!.BackButton.IsEnabled = false;

            RChannel = prevRChannel.ToList();
            GChannel = prevGChannel.ToList();
            BChannel = prevBChannel.ToList();
            
            prevRChannel.Clear();
            prevGChannel.Clear();
            prevBChannel.Clear();

            CreateHistograms(RChannel, GChannel, BChannel);
        }

        public void ShowHistogram(object parameter, Image? currentImage)
        {
            if(_histogramWindow == null)
            {
                _histogramWindow = new HistogramWindow();
                _histogramWindow.DataContext = this;
                _histogramWindow.Closed += HistogramClosed;
            }
            _currentImage = currentImage;
            CreateView();
            _histogramWindow.Show();
        }

        private void HistogramClosed(object? sender, EventArgs e)
        {
            _histogramWindow = null;
            _currentImage = null;
        }

        private void CreateView()
        {
            if (_currentImage == null)
            {
                MessageBox.Show("Płótno nie zawiera żadnych obrazów",
                    "",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }
            int bytesPerPixel = 0;
            var channels = GetRGBChannels(_currentImage);
            RChannel = channels.RChannel;
            GChannel = channels.GChannel;
            BChannel = channels.BChannel;
            bytesPerPixel = channels.BytesPerPixel;

            // create histograms for RGB channels as tabs 
            CreateHistograms(RChannel, GChannel, BChannel);
        }


        private (List<byte> RChannel, List<byte> GChannel, List<byte> BChannel, int BytesPerPixel) GetRGBChannels(Image image)
        {
            var imageBitmapSource = _currentImage!.Source as BitmapSource;
            var imageWritableBitmap = new WriteableBitmap(imageBitmapSource);

            imageWritableBitmap.Lock();
            int width = imageWritableBitmap.PixelWidth;
            int height = imageWritableBitmap.PixelHeight;
            int bytesPerPixel = (imageWritableBitmap.Format == PixelFormats.Bgr32) ? 4 : 3;
            int stride = imageWritableBitmap.BackBufferStride;
            byte[] pixelData = new byte[height * stride];

            Marshal.Copy(imageWritableBitmap.BackBuffer, pixelData, 0, pixelData.Length);
            imageWritableBitmap.Unlock();

            List<byte> rChannel = [];
            List<byte> gChannel = [];
            List<byte> bChannel = [];

            for (var i = 0; i < pixelData.Length; i += bytesPerPixel)
            {
                if (i + 2 < pixelData.Length)
                {
                    bChannel.Add(pixelData[i]);
                    gChannel.Add(pixelData[i + 1]);
                    rChannel.Add(pixelData[i + 2]);
                }
            }
            return (rChannel, gChannel, bChannel, bytesPerPixel);
        }

        private void CreateHistograms(List<byte> rChannel, List<byte> gChannel, List<byte> bChannel)
        {
            // Histogram dla kanału czerwonego (R)
            var redChannelHistogram = _histogramWindow!.redChannelHistogram;
            CreateHistogramPlot(histogram: redChannelHistogram,
                    channel: rChannel,
                    color: ScottPlot.Colors.Red,
                    title: "Histogram kanału Czerwonego (R)",
                    xLabel: "Intensywność (0-255)",
                    yLabel: "Liczba pikseli");

            // Histogram dla kanału zielonego (G)
            var greenChannelHistogram = _histogramWindow!.greenChannelHistogram;
            CreateHistogramPlot(histogram: greenChannelHistogram,
                    channel: gChannel,
                    color: ScottPlot.Colors.Green,
                    title: "Histogram kanału Zielonego (G)",
                    xLabel: "Intensywność (0-255)",
                    yLabel: "Liczba pikseli");

            // Histogram dla kanału niebieskiego (B)
            var blueChannelHistogram = _histogramWindow!.blueChannelHistogram;
            CreateHistogramPlot(histogram: blueChannelHistogram,
                    channel: bChannel,
                    color: ScottPlot.Colors.Blue,
                    title: "Histogram kanału Niebieskiego (B)",
                    xLabel: "Intensywność (0-255)",
                    yLabel: "Liczba pikseli");
        }

        private void CreateHistogramPlot(ScottPlot.WPF.WpfPlot histogram, List<byte> channel, ScottPlot.Color color, string title, string xLabel, string yLabel)
        {
            histogram.Plot.Clear();
            var hist = ScottPlot.Statistics.Histogram.WithBinSize(1, channel.Select(x => (double)x).ToArray());
            var rPlot = histogram.Plot.Add.Bars(hist.Bins, hist.Counts);
            rPlot.Color = color;
            histogram.Plot.Title(title);
            histogram.Plot.XLabel(xLabel);
            histogram.Plot.YLabel(yLabel);
            histogram.Refresh();
        }

        private void ConvertRGBToYCbCr(byte r, byte g, byte b, out byte y, out byte cb, out byte cr)
        {
            y = (byte)Math.Clamp((0.299 * r + 0.587 * g + 0.114 * b), 0, 255);
            cb = (byte)Math.Clamp((128 - 0.168736 * r - 0.331264 * g + 0.5 * b), 0, 255);
            cr = (byte)Math.Clamp((128 + 0.5 * r - 0.418688 * g - 0.081312 * b), 0, 255);
        }

        private void ConvertYCbCrToRGB(byte y, byte cb, byte cr, out byte r, out byte g, out byte b)
        {
            double yVal = y;
            double cbVal = cb - 128;
            double crVal = cr - 128;

            r = (byte)Math.Clamp((yVal + 1.402 * crVal), 0, 255);
            g = (byte)Math.Clamp((yVal - 0.344136 * cbVal - 0.714136 * crVal), 0, 255);
            b = (byte)Math.Clamp((yVal + 1.772 * cbVal), 0, 255);
        }

        public static HistogramViewModel Instance
        {
            get
            {
                if (_instance == null) _instance = new HistogramViewModel();
                return _instance;
            }
        }
    }
}
