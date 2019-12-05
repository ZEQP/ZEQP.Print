using Spire.Doc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ZEQP.Print.Framework
{
    public class PrintDocument : IDisposable
    {
        public PrintModel Model { get; set; }
        public Document Doc { get; set; }
        public PrintDocument(PrintModel model)
        {
            this.Model = model;
            this.Doc = new Document(model.Template);
        }
        public void Print()
        {
            this.Doc.MailMerge.Execute(this.Model.DicCotent.Keys.ToArray(), this.Model.DicCotent.Values.ToArray());
            this.Doc.IsUpdateFields = true;
            using (var ms = new MemoryStream())
            {
                this.Doc.SaveToStream(ms, FileFormat.XPS);
                ms.Position = 0;
                XpsPrintHelper.Print(ms, this.Model.PrintName, $"XPS_{DateTime.Now.ToString("yyMMddHHmmssfff")}", false);
            }
        }

        public void Dispose()
        {
            this.Doc.Close();
            this.Doc.Dispose();
        }
    }
}
