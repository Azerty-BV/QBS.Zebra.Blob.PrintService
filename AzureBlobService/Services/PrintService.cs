﻿using System;
using System.Printing;

namespace AzureBlobService.Services
{
    public class PrintService
    {
        //public static void PrintZPL(byte[] value, string ip)
        //{
        //    Exception exception = null;
        //    // Instantiate connection for ZPL TCP port at given address
        //    var printerConn = new TcpConnection(ip, TcpConnection.DEFAULT_ZPL_TCP_PORT);

        //    try
        //    {
        //        // Open the connection - physical connection is established here.
        //        printerConn.Open();

        //        // Send the data to printer as a byte array.
        //        printerConn.Write(value);
        //    }
        //    catch (ConnectionException ex)
        //    {
        //        exception = ex;
        //    }
        //    finally
        //    {
        //        // Close the connection to release resources.
        //        printerConn.Close();
        //    }

        //    if (exception != null)
        //        throw exception;
        //}

        public static void PrintJob(byte[] value, string documentName, string printerName)
        {
            var localPrintServer = new LocalPrintServer();
            //var defaultPrintQueue = LocalPrintServer.GetDefaultPrintQueue();
            var printQueue = localPrintServer.GetPrintQueue(printerName);
            var printTicket = printQueue.DefaultPrintTicket;
            
            //printTicket.
            var qq = printQueue.QueuePrintProcessor.PropertiesCollection;

            // Call AddJob
            var printJob = printQueue.AddJob(documentName, printTicket);
            
            // Write a Byte buffer to the JobStream and close the stream
            var myStream = printJob.JobStream;
            myStream.Write(value, 0, value.Length);
            myStream.Close();
        }
    }
}
