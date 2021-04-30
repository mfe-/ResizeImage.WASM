using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
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
                item?.Stream?.Dispose();
                (item?.Tag as SixLabors.ImageSharp.Image)?.Dispose();
            }
            ImageFiles.Clear();
        }

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
                    var i = await SixLabors.ImageSharp.Image.LoadAsync(memoryStream);
                    ImageFile imageFile = new ImageFile(file.Name, memoryStream, i.Width, i.Height)
                    {
                        ContentType = file.ContentType,
                        Tag = i
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
        public string result = "";
        protected Task OnResizeButtonClick()
        {
            result = "asdf";
            return Task.CompletedTask;
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
