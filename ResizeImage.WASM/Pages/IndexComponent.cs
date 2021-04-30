using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ResizeImage.WASM.Pages
{
    public class IndexComponent : ComponentBase, IDisposable
    {
        protected Dictionary<IBrowserFile, MemoryStream> loadedFiles = new Dictionary<IBrowserFile, MemoryStream>();
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
            foreach (var item in loadedFiles)
            {
                item.Value.Dispose();
            }
            loadedFiles.Clear();
        }

        protected async Task LoadFiles(InputFileChangeEventArgs e)
        {
            isLoading = true;
            UnloadFiles();
            exceptionMessage = string.Empty;

            try
            {
                foreach (var file in e.GetMultipleFiles(maxAllowedFiles))
                {
                    Stream s = file.OpenReadStream(maxFileSize);
                    MemoryStream memoryStream = new MemoryStream();
                    await s.CopyToAsync(memoryStream);
                    s.Close();
                    s.Dispose();
                    loadedFiles.Add(file, memoryStream);
                }
            }
            catch (Exception ex)
            {
                exceptionMessage = ex.Message;
            }

            isLoading = false;
        }
    }
}
