using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multichat
{
    public class FileSendReceive
    {
        public string FileName { get; set; }

        public string TypeName { get; set; }

        public List<byte> DataFile { get; set; }

        public FileSendReceive(string filename, string typename, List<byte> data)
        {
            this.FileName = filename;
            this.TypeName = typename;
            this.DataFile = data;
        }

        public FileSendReceive(string filename, List<byte> data)
        {
            this.FileName = filename;
            this.TypeName = getType(filename);
            this.DataFile = data;
        }

        public void Remove()
        {
            this.FileName = string.Empty;
            this.TypeName = string.Empty;
            this.DataFile = new List<byte>();
        }

        private string getType(string s)
        {
            List<string> subType = s.Split('.').ToList();
            return subType[subType.Count - 1];
        }
    }
}
