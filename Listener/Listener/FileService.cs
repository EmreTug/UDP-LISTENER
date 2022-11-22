using Listener.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Listener
{
    public static class FileService
    {
        public static void Listen(string args)
        {
            
            try
            {
                // Create a new FileSystemWatcher and set its properties.  
                FileSystemWatcher watcher = new FileSystemWatcher();
                watcher.Path = args;
                // Watch both files and subdirectories.  
                watcher.IncludeSubdirectories = true;
                // Watch for all changes specified in the NotifyFilters  
                //enumeration.  
                watcher.NotifyFilter = NotifyFilters.Attributes |
                NotifyFilters.CreationTime |//
                NotifyFilters.DirectoryName |
                NotifyFilters.FileName |
                NotifyFilters.LastAccess |
                NotifyFilters.LastWrite |
                NotifyFilters.Security |
                NotifyFilters.Size;
                // Watch all files.  
                watcher.Filter = "*value.json*";
                // Add event handlers.  
                watcher.Changed += new FileSystemEventHandler(OnChanged);
               

                watcher.EnableRaisingEvents = true;
                
            }
            catch (IOException e)
            {
                Console.WriteLine("A Exception Occurred :" + e);
            }
            catch (Exception oe)
            {
                Console.WriteLine("An Exception Occurred :" + oe);
            }
        }
        public static void OnChanged(object source, FileSystemEventArgs e)
        {
            Thread.Sleep(200);

            using (StreamReader r = new StreamReader("C:\\Users\\Msi\\Desktop\\value.json"))
            {
                string json = r.ReadToEnd();
                StaticClass.Values = JsonConvert.DeserializeObject<ValueModel>(json);
                r.Close();
            }
        }
      
    }
}
