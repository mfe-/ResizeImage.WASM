using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ResizeImage.WASM.Pages
{
    public class IndexComponent : ComponentBase, IDisposable, INotifyPropertyChanged
    {
        protected long maxFileSize = 5 * 1000 * 100;
        protected int maxAllowedFiles = 3;
        protected bool isLoading;
        protected string exceptionMessage;

        public void Dispose()
        {
            UnloadFiles();
        }

        private void UnloadFiles()
        {
            foreach (var item in ImageFiles)
            {
                item.Stream?.Dispose();
                item.Stream = null;
                if(item.Tag is Image img)
                {
                    img.Dispose();
                }
            }
            ImageFiles.Clear();
        }

        /// <summary>
        /// Gets the Settings object
        /// </summary>
        [Inject]
        public ResizeSettings Settings
        {
            get;
            protected set;
        }


        protected int RadioOptions;

        public string WidthCustom { get; set; }
        public string HeightCustom { get; set; }

        public string WidthPercent { get; set; }
        public string HeightPercent { get; set; }

        protected async Task OpenFilePicker(InputFileChangeEventArgs e)
        {
            isLoading = true;
            UnloadFiles();
            exceptionMessage = string.Empty;

            try
            {
                ImageFiles = new ObservableCollection<ImageFile>();
                foreach (var file in e.GetMultipleFiles(maxAllowedFiles))
                {
                    Stream s = file.OpenReadStream(maxFileSize);
                    MemoryStream memoryStream = new MemoryStream();
                    await s.CopyToAsync(memoryStream);
                    s.Close();
                    s.Dispose();

                    memoryStream.Position = 0;
                    var image = await Image.LoadAsync(memoryStream);
                    ImageFile imageFile = new ImageFile(file.Name, memoryStream, image.Width, image.Height)
                    {
                        ContentType = file.ContentType,
                        Tag = image,
                        FileInfo = new FileInfo(file.Name)
                    };

                    ImageFiles.Add(imageFile);
                }
            }
            catch (Exception ex)
            {
                exceptionMessage = ex.Message;
            }

            isLoading = false;
        }
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }
        protected async Task OnResizeButtonClick()
        {
            try
            {
                await ResizeImages();
            }
            catch (Exception e)
            {
                exceptionMessage = e.Message;
            }
        }

        protected async Task OnCancelButtonClick()
        {
            try
            {
                UnloadFiles();
            }
            catch (Exception e)
            {
                exceptionMessage = e.Message;
            }
        }

        protected string output;
        public async Task ResizeImages()
        {
            Resizing = true;
            //if no file is selected open file picker 
            if (ImageFiles == null || ImageFiles.Count == 0)
            {
                //todo open filepicker
            }
            String suggestedFileName = String.Empty;
            foreach (ImageFile currentImage in ImageFiles)
            {
                output = "1";

                currentImage.NewHeight = 100;
                currentImage.NewWidth = 100;

                suggestedFileName = GenerateResizedFileName(currentImage, currentImage.NewWidth, currentImage.NewHeight);

                output = "2";
                if (currentImage.Tag is Image img)
                {
                    output = "3";
                    using (var ms = new MemoryStream())
                    {
                        img.Mutate((x) => x.AutoOrient().Resize(currentImage.NewWidth, currentImage.NewHeight));
                        output = "4";
                        await img.SaveAsync(ms,
                                new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder() { Quality = 90 });
                        output = "5";
                        var bytes = ms.ToArray();
                        await SaveAs(JSRuntime, suggestedFileName, bytes);
                        output = "6";
                    }
                }

            }
        }
        public virtual string GenerateResizedFileName(ImageFile storeage, int? width, int? height)
        {

            if (!width.HasValue)
            {
                width = storeage.Width;
            }
            if (!height.HasValue)
            {
                height = storeage.Height;
            }
            if (storeage != null)
            {
                string suggestedfilename = $"{storeage.FileInfo.Name.Replace(storeage.FileInfo.Extension, String.Empty)}-{width}x{height}{storeage.FileInfo.Extension}";
                return suggestedfilename;
            }
            else
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// Get or sets the flag which indicates whether a resize operation is currently processed
        /// </summary>
        /// <remarks>Also this flag is set to true when the user selected (open) images for resizing</remarks>
        private bool _Resizing;
        public bool Resizing
        {
            get { return _Resizing; }
            protected set
            {
                SetProperty(ref _Resizing, value, nameof(Resizing));
            }
        }

        public int Width { get; set; }

        public int Height { get; set; }

        /// <summary>
        /// Get or sets the list of files which should be resized
        /// </summary>
        private ObservableCollection<ImageFile> _ImageFiles = new ObservableCollection<ImageFile>();
        public ObservableCollection<ImageFile> ImageFiles
        {
            get { return _ImageFiles; }
            set
            {
                //SetProperty(ref _ImageFiles, value, nameof(ImageFiles));
                if (ImageFiles != null)
                {
                    ImageFiles.CollectionChanged += ImageFiles_CollectionChanged;
                }
                //ApplyPreviewDimensions();
                //OnPropertyChanged(nameof(ShowOpenFilePicker));
                //OnPropertyChanged(nameof(SingleFile));
            }
        }
        public bool ShowOpenFilePicker
        {
            get { return ImageFiles != null && ImageFiles.Count == 0; }
        }
        private void ImageFiles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //OnPropertyChanged(nameof(ShowOpenFilePicker));
            //OnPropertyChanged(nameof(CanOverwriteFiles));
            //OnPropertyChanged(nameof(SingleFile));
            if (!_ImageFiles.Any())
            {
                OnPropertyChanged(nameof(ImageFiles));
            }
            //ApplyPreviewDimensions();
        }

        public async static Task SaveAs(IJSRuntime js, string filename, byte[] data)
        {
            await js.InvokeAsync<object>(
                "saveAsFile",
                filename,
                Convert.ToBase64String(data));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected virtual bool SetProperty<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
