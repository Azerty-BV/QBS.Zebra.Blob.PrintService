using System;
using System.IO;
using Azure.Storage.Blobs;
using Azure;
using AzureBlobService.Services;

namespace AzureBlobService
{
    public class AzureStorage
    {
        private readonly BlobServiceClient blobClient;
        private readonly Log log;

        public AzureStorage(string accountName, string sasToken)
        {
            log = new Log();
            var cred = new AzureSasCredential(sasToken);
            Uri accountUri = new Uri($"https://{accountName}.blob.core.windows.net");

            blobClient = new BlobServiceClient(accountUri, cred);
        }

        public void DeleteAllFromContainer(string containerName)
        {
            int i = 0;
            var blobContainer = blobClient.GetBlobContainerClient(containerName);
            foreach (var item in blobContainer.GetBlobs())
            {
                string name = item.Name;
                blobContainer.DeleteBlobIfExists(name);
            }
            if (i > 0)
                log.Add(i.ToString() + " files deleted from Azure");
        }

        public void DownloadAll(string containerName, string downloadTo)
        {
            int i = 0;
            var blobContainer = blobClient.GetBlobContainerClient(containerName);
            foreach (var item in blobContainer.GetBlobs())
            {
                string name = item.Name;
                string path = downloadTo + @"\" + name;
                blobContainer.GetBlobClient(name).DownloadTo(path);
                //blockBlob.DownloadToFile(path, FileMode.OpenOrCreate);
                i++;
            }
            if (i > 0)
                log.Add(i.ToString() + " files downloaded from Azure");
        }

        public void PrintAll(string containerName, string printerName)
        {
            int i = 0;
            var blobContainer = blobClient.GetBlobContainerClient(containerName);
            var fileList = blobContainer.GetBlobs(prefix: $"{printerName}/");
            foreach (var item in fileList)
            {
                string name = item.Name.Replace($"{printerName}/", string.Empty);

                try
                {
                    var blockBlob = blobContainer.GetBlobClient(item.Name);
                    //byte[] zplFile = new byte[blockBlob.StreamWriteSizeInBytes];
                    //var fileSize = blockBlob.DownloadToByteArray(zplFile, 0);
                    var fileContent = blockBlob.DownloadContent().Value.Content;
                    log.Add("Sending " + name + " to printer " + printerName);
                    //PrintService.PrintZPL(zplFile, printerIP);
                    if (name.EndsWith(".zpl"))
                        RawPrinterService.SendStringToPrinter(printerName, fileContent.ToString(), name);
                    else if (name.EndsWith(".glabel"))
                        GLabelService.SendToPrinter(printerName, fileContent.ToString(), name);
                    else
                        RawPrinterService.SendStringToPrinter(printerName, fileContent.ToString(), name);
                    //RawPrinterService.SendToPrinter(name, fileContent.ToString(), printerName);
                    //blockBlob.DeleteIfExists();
                }
                catch (Exception ex)
                {
                    log.Add("Couldn't print file " + name + ". Exception: " + ex.Message);
                }
                i++;
            }
            log.Add(i.ToString() + " files printed from Azure");
        }

        public void UploadAll(string containerName, string localFolder)
        {
            int i = 0;
            var blobContainer = blobClient.GetBlobContainerClient(containerName);
            foreach (string file in Directory.EnumerateFiles(localFolder, "*.*", SearchOption.AllDirectories))
            {
                string cloudFileName = file.Substring(3).Replace('\\', '/').Replace('#', '_');
                /*CloudBlockBlob blob = blobContainer.GetBlockBlobReference(cloudFileName);
                try
                {
                    blob.Properties.ContentType = MimeMapping.GetMimeMapping(file);
                }
                catch (Exception e)
                {
                    log.Add("Couldn't upload file " + file + "(uploaded as " + cloudFileName + "). Exception: " + e.Message);
                }
                //if (!blob.Exists())
                //{
                try
                {
                    blob.UploadFromFile(file);
                    log.Add("It appears that file " + file + "(uploaded as " + cloudFileName + ") has been uploaded. No exception thrown. URI: " + blob.Uri);

                    blob.FetchAttributes();
                    if (blob.Properties.Length > 0)
                    {
                        log.Add("Blob length: " + blob.Properties.Length);
                        File.Delete(file);
                        i++;
                    }
                    else
                    {
                        log.Add("Couldn't upload file " + file + "(uploaded as " + cloudFileName + "). No exception thrown");
                    }
                }
                catch (Exception e)
                {
                    log.Add("Couldn't upload file " + file + "(uploaded as " + cloudFileName + "). Exception: " + e.Message);
                }
                //}*/
            }
            if (i > 0)
                log.Add(i.ToString() + " files uploaded to Azure");
        }
    }
}
 