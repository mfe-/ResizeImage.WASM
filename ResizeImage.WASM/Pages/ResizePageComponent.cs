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
    public class ResizePageComponent : ComponentBase, IDisposable, INotifyPropertyChanged
    {
        protected long maxFileSize = 3670016;//2^10*3.5
        protected int maxAllowedFiles = 3;
        protected bool isLoading;
        protected string exceptionMessage;

        public ResizePageComponent()
        {
            WidthCustom = "1024";
            HeightCustom = "768";
            WidthPercent = "50";
            HeightPercent = "50";
        }
        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            PropertyChanged += ResizePageViewModel_PropertyChanged;
        }

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
                if (item.Tag is Image img)
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
            get; //todo store in localstorage and 
            protected set;
        }


        private int _RadioOptions;
        public int RadioOptions
        {
            get { return _RadioOptions; }
            set
            {
                SetProperty(ref _RadioOptions, value, nameof(RadioOptions));
                ApplyPreviewDimensions();
            }
        }


        private string _WidthCustom;
        public string WidthCustom
        {
            get { return _WidthCustom; }
            set { SetProperty(ref _WidthCustom, value, nameof(WidthCustom)); }
        }

        private string _HeightCustom;
        public string HeightCustom
        {
            get { return _HeightCustom; }
            set { SetProperty(ref _HeightCustom, value, nameof(HeightCustom)); }
        }

        private string _HeightPercent;
        public string HeightPercent
        {
            get { return _HeightPercent; }
            set { SetProperty(ref _HeightPercent, value, nameof(HeightPercent)); }
        }

        private string _WidthPercent;
        public string WidthPercent
        {
            get { return _WidthPercent; }
            set { SetProperty(ref _WidthPercent, value, nameof(WidthPercent)); }
        }

        private bool _MaintainAspectRatioWidth;
        public bool MaintainAspectRatioWidth
        {
            get { return _MaintainAspectRatioWidth; }
            set
            {
                SetProperty(ref _MaintainAspectRatioWidth, value, nameof(MaintainAspectRatioWidth));
                if (MaintainAspectRatioWidth)
                {
                    MaintainAspectRatioHeight = false;
                    RadioOptions = 3;
                }
                HeightCustomDisabled = MaintainAspectRatioWidth;
                MaintainAspectRatio = _MaintainAspectRatioWidth || _MaintainAspectRatioHeight;
            }
        }

        private bool _MaintainAspectRatioHeight;
        public bool MaintainAspectRatioHeight
        {
            get { return _MaintainAspectRatioHeight; }
            set
            {
                SetProperty(ref _MaintainAspectRatioHeight, value, nameof(MaintainAspectRatioHeight));
                if (MaintainAspectRatioHeight)
                {
                    MaintainAspectRatioWidth = false;
                    RadioOptions = 3;
                }
                WidthCustomDisabled = MaintainAspectRatioHeight;
                MaintainAspectRatio = _MaintainAspectRatioWidth || _MaintainAspectRatioHeight;
            }
        }

        private bool _MaintainAspectRatio;
        public bool MaintainAspectRatio
        {
            get { return _MaintainAspectRatio; }
            set
            {
                SetProperty(ref _MaintainAspectRatio, value, nameof(MaintainAspectRatio));
                PercentDisabled = _MaintainAspectRatio;
                ApplyPreviewDimensions();
            }
        }

        protected bool PercentDisabled { get; set; } = false;

        protected bool HeightCustomDisabled { get; set; }

        protected bool WidthCustomDisabled { get; set; }


        public bool SizePercentChecked => RadioOptions == 4;

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
                ApplyPreviewDimensions();
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

        protected void OnCancelButtonClick()
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
            try
            {
                Resizing = true;
                //if no file is selected open file picker 
                if (ImageFiles == null || ImageFiles.Count == 0)
                {
                    await JSRuntime.InvokeVoidAsync("openFilepicker");
                }
                String suggestedFileName = String.Empty;
                foreach (ImageFile currentImage in ImageFiles)
                {
                    output = "1";

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
            finally
            {
                Resizing = false;
            }

        }
        public void ApplyPreviewDimensions()
        {
            if (ImageFiles is IEnumerable<ImageFile> imagefiles)
            {
                foreach (ImageFile currentImage in imagefiles)
                {
                    int newWidth;
                    int newHeight;

                    int height = 0;
                    int.TryParse(HeightCustom, out height);
                    int width = 0;
                    int.TryParse(WidthCustom, out width);

                    if (MaintainAspectRatio)
                    {
                        if (MaintainAspectRatioHeight)
                        {
                            newWidth = (int)(((double)currentImage.Width / (double)currentImage.Height) * (double)height);
                            newHeight = height;
                        }
                        else
                        {
                            newWidth = width;
                            newHeight = (int)(((double)currentImage.Height / (double)currentImage.Width) * (double)width);
                        }
                    }
                    else if (SizePercentChecked)
                    {
                        int heightpercent = 0;
                        int.TryParse(HeightPercent, out heightpercent);
                        int widthpercent = 0;
                        int.TryParse(WidthPercent, out widthpercent);
                        newWidth = currentImage.Width * widthpercent / 100;
                        newHeight = currentImage.Height * heightpercent / 100;
                    }
                    else
                    {
                        newWidth = width;
                        newHeight = height;
                    }
                    currentImage.NewWidth = newWidth;
                    currentImage.NewHeight = newHeight;
                }
                StateHasChanged();
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


        /// <summary>
        /// Get or sets the list of files which should be resized
        /// </summary>
        private ObservableCollection<ImageFile> _ImageFiles = new ObservableCollection<ImageFile>();
        public ObservableCollection<ImageFile> ImageFiles
        {
            get { return _ImageFiles; }
            set
            {
                SetProperty(ref _ImageFiles, value, nameof(ImageFiles));
                ApplyPreviewDimensions();
                //OnPropertyChanged(nameof(ShowOpenFilePicker));
                //OnPropertyChanged(nameof(SingleFile));
            }
        }
        public bool ShowOpenFilePicker
        {
            get { return ImageFiles != null && ImageFiles.Count == 0; }
        }

        public async static Task SaveAs(IJSRuntime js, string filename, byte[] data)
        {
            await js.InvokeAsync<object>(
                "saveAsFile",
                filename,
                Convert.ToBase64String(data));
        }

        private void ResizePageViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            try
            {
                PropertyChanged -= ResizePageViewModel_PropertyChanged;
                if (
                      e.PropertyName.Equals(nameof(SizePercentChecked)) ||
                      e.PropertyName.Equals(nameof(WidthCustom)) ||
                      e.PropertyName.Equals(nameof(HeightCustom)) ||
                      e.PropertyName.Equals(nameof(WidthPercent)) ||
                      e.PropertyName.Equals(nameof(HeightPercent)
                      ))
                {
                    ApplyPreviewDimensions();
                }
            }
            catch (Exception ex)
            {
                exceptionMessage = ex.ToString();
            }
            finally
            {
                PropertyChanged -= ResizePageViewModel_PropertyChanged;
                PropertyChanged += ResizePageViewModel_PropertyChanged;
            }
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
