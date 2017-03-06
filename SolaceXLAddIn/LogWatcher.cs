// Copyright 2017 Dishant Langayan
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// Author:
//  Dishant Langayan <dishant.langayan@solace.com>
//
using System.IO;

namespace Solace.Labs.Excel.SolaceXLAddIn
{
    internal class LogWatcher : FileSystemWatcher
    {
        // The name of the file to monitor
        internal string FileName { get; set; }

        // The FileStream for reading the text from the file
        FileStream Stream;
        // The StreamReader for reading the text from the FileStream
        StreamReader Reader;

        // Constructor for the LogWatcher class
        public LogWatcher(string fileName)
        {
            //Subscribe to the Changed event of the base FileSystemWatcher class
            this.Changed += OnChanged;

            //Set the filename of the file to watch
            this.FileName = fileName;

            //Create the FileStream and StreamReader object for the file
            Stream = new FileStream
                (fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            Reader = new StreamReader(Stream);

            //Set the position of the stream to the end of the file
            //Stream.Position = Stream.Length;
        }

        //Occurs when the file is changed
        public void OnChanged(object source, FileSystemEventArgs e)
        {
            //Read the new text from the file
            string contents = Reader.ReadToEnd();

            //Fire the TextChanged event
            LogWatcherEventArgs args = new LogWatcherEventArgs(contents);
            TextChanged(this, args);
        }

        public delegate void LogWatcherEventHandler(object sender, LogWatcherEventArgs e);

        //Event that is fired when the file is changed
        public event LogWatcherEventHandler TextChanged;
    }
}
