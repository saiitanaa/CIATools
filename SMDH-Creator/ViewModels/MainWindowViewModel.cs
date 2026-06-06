using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SMDH_Creator.ViewModels
{
    public sealed class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly Window? owner;
        private SmdhFile currentSmdh = new();

        private string titleText = "";
        private string descriptionText = "";
        private string publisherText = "";
        private string statusText = "Ready";
        private int selectedTitleIndex = 1;

        private Bitmap? smallIconPreview;
        private Bitmap? bigIconPreview;

        public event PropertyChangedEventHandler? PropertyChanged;

        public MainWindowViewModel()
        {
            NewCommand = new AsyncRelayCommand(_ => NewAsync());
            OpenSmdhCommand = new AsyncRelayCommand(_ => OpenSmdhAsync());
            LoadIconCommand = new AsyncRelayCommand(_ => LoadIconAsync());
            SaveSmdhCommand = new AsyncRelayCommand(_ => SaveSmdhAsync());

            RefreshIconPreviews();
        }

        public MainWindowViewModel(Window owner) : this()
        {
            this.owner = owner;
        }

        public string TitleText
        {
            get => titleText;
            set => SetProperty(ref titleText, value);
        }

        public string DescriptionText
        {
            get => descriptionText;
            set => SetProperty(ref descriptionText, value);
        }

        public string PublisherText
        {
            get => publisherText;
            set => SetProperty(ref publisherText, value);
        }

        public string StatusText
        {
            get => statusText;
            set => SetProperty(ref statusText, value);
        }

        public int SelectedTitleIndex
        {
            get => selectedTitleIndex;
            set
            {
                if (value < 0 || value > 15)
                {
                    return;
                }

                if (value == selectedTitleIndex)
                {
                    return;
                }

                ApplyTextsToLanguage(selectedTitleIndex);

                selectedTitleIndex = value;
                OnPropertyChanged();

                LoadTextsFromLanguage(selectedTitleIndex);
            }
        }

        public Bitmap? SmallIconPreview
        {
            get => smallIconPreview;
            set => SetProperty(ref smallIconPreview, value);
        }

        public Bitmap? BigIconPreview
        {
            get => bigIconPreview;
            set => SetProperty(ref bigIconPreview, value);
        }

        public ICommand NewCommand { get; }
        public ICommand OpenSmdhCommand { get; }
        public ICommand LoadIconCommand { get; }
        public ICommand SaveSmdhCommand { get; }

        private Task NewAsync()
        {
            currentSmdh = new SmdhFile();

            TitleText = "";
            DescriptionText = "";
            PublisherText = "";
            selectedTitleIndex = 1;
            OnPropertyChanged(nameof(SelectedTitleIndex));

            RefreshIconPreviews();

            StatusText = "New SMDH.";
            return Task.CompletedTask;
        }

        private async Task OpenSmdhAsync()
        {
            if (owner is null)
            {
                StatusText = "Error: window not initialized.";
                return;
            }

            if (!owner.StorageProvider.CanOpen)
            {
                StatusText = "Error: file picker not available.";
                return;
            }

            var files = await owner.StorageProvider.OpenFilePickerAsync(
                new FilePickerOpenOptions
                {
                    Title = "Open SMDH file",
                    AllowMultiple = false,
                    FileTypeFilter = new[]
                    {
                        new FilePickerFileType("SMDH files")
                        {
                            Patterns = new[] { "*.smdh" }
                        },
                        FilePickerFileTypes.All
                    }
                });

            IStorageFile? file = files.FirstOrDefault();

            if (file is null)
            {
                StatusText = "Open cancelled.";
                return;
            }

            try
            {
                await using Stream stream = await file.OpenReadAsync();

                var smdh = new SmdhFile();
                smdh.Load(stream);

                if (!smdh.Valid)
                {
                    StatusText = "Invalid file: SMDH signature missing.";
                    return;
                }

                currentSmdh = smdh;

                LoadTextsFromLanguage(SelectedTitleIndex);
                RefreshIconPreviews();

                StatusText = "SMDH loaded.";
            }
            catch (Exception ex)
            {
                StatusText = $"Error opening: {ex.Message}";
            }
        }

        private async Task LoadIconAsync()
        {
            if (owner is null)
            {
                StatusText = "Error: window not initialized.";
                return;
            }

            if (!owner.StorageProvider.CanOpen)
            {
                StatusText = "Error: file picker not available.";
                return;
            }

            var files = await owner.StorageProvider.OpenFilePickerAsync(
                new FilePickerOpenOptions
                {
                    Title = "Load icon image",
                    AllowMultiple = false,
                    FileTypeFilter = new[]
                    {
                        new FilePickerFileType("Images")
                        {
                            Patterns = new[] { "*.png", "*.jpg", "*.jpeg", "*.bmp" },
                            MimeTypes = new[] { "image/png", "image/jpeg", "image/bmp" }
                        },
                        FilePickerFileTypes.All
                    }
                });

            IStorageFile? file = files.FirstOrDefault();

            if (file is null)
            {
                StatusText = "Icon loading cancelled.";
                return;
            }

            try
            {
                await using Stream stream = await file.OpenReadAsync();

                using var bitmap = new Bitmap(stream);

                currentSmdh.SetIconsFromBitmap(bitmap);
                RefreshIconPreviews();

                StatusText = "Icon loaded.";
            }
            catch (Exception ex)
            {
                StatusText = $"Error loading icon: {ex.Message}";
            }
        }

        private async Task SaveSmdhAsync()
        {
            if (owner is null)
            {
                StatusText = "Error: window not initialized.";
                return;
            }

            if (!owner.StorageProvider.CanSave)
            {
                StatusText = "Error: save not available.";
                return;
            }

            ApplyTextsToLanguage(SelectedTitleIndex);

            IStorageFile? file = await owner.StorageProvider.SaveFilePickerAsync(
                new FilePickerSaveOptions
                {
                    Title = "Save SMDH file",
                    SuggestedFileName = "icon.smdh",
                    DefaultExtension = "smdh",
                    ShowOverwritePrompt = true,
                    FileTypeChoices = new[]
                    {
                        new FilePickerFileType("SMDH files")
                        {
                            Patterns = new[] { "*.smdh" }
                        }
                    }
                });

            if (file is null)
            {
                StatusText = "Save cancelled.";
                return;
            }

            try
            {
                await using Stream stream = await file.OpenWriteAsync();
                currentSmdh.Save(stream);

                StatusText = "SMDH saved";
            }
            catch (Exception ex)
            {
                StatusText = $"Error saving: {ex.Message}";
            }
        }

        private void ApplyTextsToLanguage(int languageIndex)
        {
            currentSmdh.SetShortDescription(languageIndex, TitleText);
            currentSmdh.SetLongDescription(languageIndex, DescriptionText);
            currentSmdh.SetPublisher(languageIndex, PublisherText);
        }

        private void LoadTextsFromLanguage(int languageIndex)
        {
            TitleText = currentSmdh.GetShortDescription(languageIndex);
            DescriptionText = currentSmdh.GetLongDescription(languageIndex);
            PublisherText = currentSmdh.GetPublisher(languageIndex);
        }

        private void RefreshIconPreviews()
        {
            SmallIconPreview = currentSmdh.CreateSmallIconBitmap();
            BigIconPreview = currentSmdh.CreateBigIconBitmap();
        }

        private bool SetProperty<T>(
            ref T field,
            T value,
            [CallerMemberName] string? propertyName = null)
        {
            if (Equals(field, value))
            {
                return false;
            }

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private sealed class AsyncRelayCommand : ICommand
        {
            private readonly Func<object?, Task> execute;
            private bool isExecuting;

            public AsyncRelayCommand(Func<object?, Task> execute)
            {
                this.execute = execute;
            }

            public event EventHandler? CanExecuteChanged;

            public bool CanExecute(object? parameter)
            {
                return !isExecuting;
            }

            public async void Execute(object? parameter)
            {
                if (isExecuting)
                {
                    return;
                }

                try
                {
                    isExecuting = true;
                    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                    await execute(parameter);
                }
                finally
                {
                    isExecuting = false;
                    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private sealed class SmdhFile
        {
            private const uint MagicSmdh = 0x48444D53;

            private readonly SmdhHeader header = new();
            private readonly SmdhTitle[] titles = new SmdhTitle[16];
            private readonly SmdhSettings settings = new();
            private readonly byte[] reserved = new byte[0x08];

            private readonly ushort[] smallIconData = new ushort[24 * 24];
            private readonly ushort[] bigIconData = new ushort[48 * 48];

            private Rgb24[] smallIconPixels = CreateSolidPixels(24, 24, new Rgb24(40, 40, 40));
            private Rgb24[] bigIconPixels = CreateSolidPixels(48, 48, new Rgb24(40, 40, 40));

            private readonly byte[] tileOrder =
            {
                0, 1, 8, 9, 2, 3, 10, 11,
                16, 17, 24, 25, 18, 19, 26, 27,
                4, 5, 12, 13, 6, 7, 14, 15,
                20, 21, 28, 29, 22, 23, 30, 31,
                32, 33, 40, 41, 34, 35, 42, 43,
                48, 49, 56, 57, 50, 51, 58, 59,
                36, 37, 44, 45, 38, 39, 46, 47,
                52, 53, 60, 61, 54, 55, 62, 63
            };

            public SmdhFile()
            {
                for (int i = 0; i < titles.Length; i++)
                {
                    titles[i] = new SmdhTitle();
                }

                header.Magic = MagicSmdh;
                header.Version = 0;
                header.Reserved = 0;

                EncodeIcon(smallIconPixels, smallIconData, 24);
                EncodeIcon(bigIconPixels, bigIconData, 48);
            }

            public bool Valid => header.Magic == MagicSmdh;

            public string GetShortDescription(int index)
            {
                return DecodeText(titles[index].ShortDescription);
            }

            public void SetShortDescription(int index, string value)
            {
                EncodeText(value, titles[index].ShortDescription);
            }

            public string GetLongDescription(int index)
            {
                return DecodeText(titles[index].LongDescription);
            }

            public void SetLongDescription(int index, string value)
            {
                EncodeText(value, titles[index].LongDescription);
            }

            public string GetPublisher(int index)
            {
                return DecodeText(titles[index].Publisher);
            }

            public void SetPublisher(int index, string value)
            {
                EncodeText(value, titles[index].Publisher);
            }

            public void SetIconsFromBitmap(Bitmap source)
            {
                bigIconPixels = AvaloniaBitmapTools.ExtractRgb24(source, 48, 48);
                smallIconPixels = AvaloniaBitmapTools.ExtractRgb24(source, 24, 24);

                EncodeIcon(smallIconPixels, smallIconData, 24);
                EncodeIcon(bigIconPixels, bigIconData, 48);
            }

            public Bitmap CreateSmallIconBitmap()
            {
                return AvaloniaBitmapTools.CreateBitmapFromRgb24(smallIconPixels, 24, 24);
            }

            public Bitmap CreateBigIconBitmap()
            {
                return AvaloniaBitmapTools.CreateBitmapFromRgb24(bigIconPixels, 48, 48);
            }

            public void Load(Stream stream)
            {
                using var reader = new BinaryReader(stream);

                header.Magic = reader.ReadUInt32();
                header.Version = reader.ReadUInt16();
                header.Reserved = reader.ReadUInt16();

                if (!Valid)
                {
                    return;
                }

                for (int i = 0; i < titles.Length; i++)
                {
                    ReadU16Array(reader, titles[i].ShortDescription);
                    ReadU16Array(reader, titles[i].LongDescription);
                    ReadU16Array(reader, titles[i].Publisher);
                }

                ReadBytes(reader, settings.GameRatings);
                settings.RegionLock = reader.ReadUInt32();
                ReadBytes(reader, settings.MatchMakerId);
                settings.Flags = reader.ReadUInt32();
                settings.EulaVersion = reader.ReadUInt16();
                settings.Reserved = reader.ReadUInt16();
                settings.DefaultFrame = reader.ReadUInt32();
                settings.CecId = reader.ReadUInt32();

                ReadBytes(reader, reserved);

                ReadU16Array(reader, smallIconData);
                ReadU16Array(reader, bigIconData);

                smallIconPixels = DecodeIcon(smallIconData, 24);
                bigIconPixels = DecodeIcon(bigIconData, 48);
            }

            public void Save(Stream stream)
            {
                EncodeIcon(smallIconPixels, smallIconData, 24);
                EncodeIcon(bigIconPixels, bigIconData, 48);

                using var writer = new BinaryWriter(stream);

                header.Magic = MagicSmdh;

                writer.Write(header.Magic);
                writer.Write(header.Version);
                writer.Write(header.Reserved);

                for (int i = 0; i < titles.Length; i++)
                {
                    WriteU16Array(writer, titles[i].ShortDescription);
                    WriteU16Array(writer, titles[i].LongDescription);
                    WriteU16Array(writer, titles[i].Publisher);
                }

                writer.Write(settings.GameRatings);
                writer.Write(settings.RegionLock);
                writer.Write(settings.MatchMakerId);
                writer.Write(settings.Flags);
                writer.Write(settings.EulaVersion);
                writer.Write(settings.Reserved);
                writer.Write(settings.DefaultFrame);
                writer.Write(settings.CecId);

                writer.Write(reserved);

                WriteU16Array(writer, smallIconData);
                WriteU16Array(writer, bigIconData);
            }

            private Rgb24[] DecodeIcon(ushort[] source, int size)
            {
                var destination = new Rgb24[size * size];

                int i = 0;

                for (int tileY = 0; tileY < size; tileY += 8)
                {
                    for (int tileX = 0; tileX < size; tileX += 8)
                    {
                        for (int k = 0; k < 64; k++)
                        {
                            int x = tileOrder[k] & 0x07;
                            int y = tileOrder[k] >> 3;

                            destination[(tileY + y) * size + tileX + x] = DecodeRgb565(source[i]);
                            i++;
                        }
                    }
                }

                return destination;
            }

            private void EncodeIcon(Rgb24[] source, ushort[] destination, int size)
            {
                int i = 0;

                for (int tileY = 0; tileY < size; tileY += 8)
                {
                    for (int tileX = 0; tileX < size; tileX += 8)
                    {
                        for (int k = 0; k < 64; k++)
                        {
                            int x = tileOrder[k] & 0x07;
                            int y = tileOrder[k] >> 3;

                            destination[i] = EncodeRgb565(source[(tileY + y) * size + tileX + x]);
                            i++;
                        }
                    }
                }
            }

            private static Rgb24 DecodeRgb565(ushort color)
            {
                int r5 = (color >> 11) & 0x1F;
                int g6 = (color >> 5) & 0x3F;
                int b5 = color & 0x1F;

                byte r = (byte)((r5 << 3) | (r5 >> 2));
                byte g = (byte)((g6 << 2) | (g6 >> 4));
                byte b = (byte)((b5 << 3) | (b5 >> 2));

                return new Rgb24(r, g, b);
            }

            private static ushort EncodeRgb565(Rgb24 pixel)
            {
                int r = pixel.R >> 3;
                int g = pixel.G >> 2;
                int b = pixel.B >> 3;

                return (ushort)((r << 11) | (g << 5) | b);
            }

            private static string DecodeText(ushort[] source)
            {
                int length = 0;

                while (length < source.Length && source[length] != 0)
                {
                    length++;
                }

                char[] chars = new char[length];

                for (int i = 0; i < length; i++)
                {
                    chars[i] = (char)source[i];
                }

                return new string(chars);
            }

            private static void EncodeText(string text, ushort[] destination)
            {
                Array.Clear(destination, 0, destination.Length);

                int length = Math.Min(text.Length, destination.Length);

                for (int i = 0; i < length; i++)
                {
                    destination[i] = text[i];
                }
            }

            private static void ReadBytes(BinaryReader reader, byte[] destination)
            {
                int read = reader.Read(destination, 0, destination.Length);

                if (read != destination.Length)
                {
                    throw new EndOfStreamException("Incomplete SMDH file");
                }
            }

            private static void ReadU16Array(BinaryReader reader, ushort[] destination)
            {
                for (int i = 0; i < destination.Length; i++)
                {
                    destination[i] = reader.ReadUInt16();
                }
            }

            private static void WriteU16Array(BinaryWriter writer, ushort[] source)
            {
                for (int i = 0; i < source.Length; i++)
                {
                    writer.Write(source[i]);
                }
            }

            private static Rgb24[] CreateSolidPixels(int width, int height, Rgb24 color)
            {
                var pixels = new Rgb24[width * height];

                for (int i = 0; i < pixels.Length; i++)
                {
                    pixels[i] = color;
                }

                return pixels;
            }

            private sealed class SmdhHeader
            {
                public uint Magic;
                public ushort Version;
                public ushort Reserved;
            }

            private sealed class SmdhTitle
            {
                public ushort[] ShortDescription = new ushort[0x40];
                public ushort[] LongDescription = new ushort[0x80];
                public ushort[] Publisher = new ushort[0x40];
            }

            private sealed class SmdhSettings
            {
                public byte[] GameRatings = new byte[0x10];
                public uint RegionLock;
                public byte[] MatchMakerId = new byte[0x0C];
                public uint Flags;
                public ushort EulaVersion;
                public ushort Reserved;
                public uint DefaultFrame;
                public uint CecId;
            }
        }

        private readonly struct Rgb24
        {
            public readonly byte R;
            public readonly byte G;
            public readonly byte B;

            public Rgb24(byte r, byte g, byte b)
            {
                R = r;
                G = g;
                B = b;
            }
        }

        private static class AvaloniaBitmapTools
        {
            public static Rgb24[] ExtractRgb24(Bitmap source, int width, int height)
            {
                using Bitmap scaled = source.CreateScaledBitmap(
                    new PixelSize(width, height),
                    BitmapInterpolationMode.HighQuality);

                int stride = width * 4;
                byte[] bgra = new byte[stride * height];

                GCHandle handle = GCHandle.Alloc(bgra, GCHandleType.Pinned);

                try
                {
                    scaled.CopyPixels(
                        new PixelRect(0, 0, width, height),
                        handle.AddrOfPinnedObject(),
                        bgra.Length,
                        stride);
                }
                finally
                {
                    handle.Free();
                }

                return BgraToRgb24(bgra, stride, width, height);
            }

            public static Bitmap CreateBitmapFromRgb24(Rgb24[] pixels, int width, int height)
            {
                int stride = width * 4;
                byte[] bgra = new byte[stride * height];

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        Rgb24 pixel = pixels[y * width + x];
                        int offset = y * stride + x * 4;

                        bgra[offset + 0] = pixel.B;
                        bgra[offset + 1] = pixel.G;
                        bgra[offset + 2] = pixel.R;
                        bgra[offset + 3] = 255;
                    }
                }

                var bitmap = new WriteableBitmap(
                    new PixelSize(width, height),
                    new Vector(96, 96),
                    PixelFormat.Bgra8888,
                    AlphaFormat.Premul);

                using (ILockedFramebuffer framebuffer = bitmap.Lock())
                {
                    Marshal.Copy(bgra, 0, framebuffer.Address, bgra.Length);
                }

                return bitmap;
            }

            private static Rgb24[] BgraToRgb24(byte[] bgra, int stride, int width, int height)
            {
                var pixels = new Rgb24[width * height];

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int offset = y * stride + x * 4;

                        byte b = bgra[offset + 0];
                        byte g = bgra[offset + 1];
                        byte r = bgra[offset + 2];
                        byte a = bgra[offset + 3];

                        if (a > 0 && a < 255)
                        {
                            r = Unpremultiply(r, a);
                            g = Unpremultiply(g, a);
                            b = Unpremultiply(b, a);
                        }

                        pixels[y * width + x] = new Rgb24(r, g, b);
                    }
                }

                return pixels;
            }

            private static byte Unpremultiply(byte value, byte alpha)
            {
                int result = value * 255 / alpha;
                return (byte)Math.Clamp(result, 0, 255);
            }
        }
    }
}