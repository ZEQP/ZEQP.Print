using Spire.Doc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;

namespace ZEQP.Print.Framework
{
    public class PrintDocument : IDisposable
    {
        public PrintModel Model { get; set; }
        public Document Doc { get; set; }
        public HttpClient Client { get; set; }
        public PrintDocument(PrintModel model)
        {
            this.Model = model;
            this.Doc = new Document(model.Template);
            this.Client = new HttpClient();
        }
        public void Print()
        {
            if (this.Model.FieldCotent.Count > 0)
                this.Doc.MailMerge.Execute(this.Model.FieldCotent.Keys.ToArray(), this.Model.FieldCotent.Values.ToArray());
            if (this.Model.ImageContent.Count > 0)
            {
                this.Doc.MailMerge.MergeImageField += MailMerge_MergeImageField;
                this.Doc.MailMerge.Execute(this.Model.ImageContent.Keys.ToArray(), this.Model.ImageContent.Values.Select(s => s.Value).ToArray());
            }
            if (this.Model.TableContent.Count > 0)
            {
                foreach (var item in this.Model.TableContent)
                {
                    var table = item.Value;
                    table.TableName = item.Key;
                    this.Doc.MailMerge.ExecuteWidthRegion(table);
                }
            }
            this.Doc.IsUpdateFields = true;
            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PrintDoc", $"{DateTime.Now.ToString("yyMMddHHmmssfff")}.docx");
            this.Doc.SaveToFile(fileName, FileFormat.Docx);
            using (var ms = new MemoryStream())
            {
                this.Doc.SaveToStream(ms, FileFormat.XPS);
                for (int i = 0; i < this.Model.Copies; i++)
                {
                    ms.Position = 0;
                    XpsPrintHelper.Print(ms, this.Model.PrintName, $"XPS_{i}_{DateTime.Now.ToString("yyMMddHHmmssfff")}", false);
                }
            }
        }

        private void MailMerge_MergeImageField(object sender, Spire.Doc.Reporting.MergeImageFieldEventArgs field)
        {
            var fieldName = field.FieldName;
            var imageModel = this.Model.ImageContent[fieldName];
            switch (imageModel.Type)
            {
                case ImageType.Local:
                    {
                        field.SetImage(imageModel.Value);
                        field.PictureSize = new SizeF(imageModel.Width, imageModel.Height);
                    };
                    break;
                case ImageType.Network:
                    {
                        var imageStream = this.Client.GetStreamAsync(imageModel.Value).Result;
                        field.SetImage(imageStream);
                        field.PictureSize = new SizeF(imageModel.Width, imageModel.Height);
                    }; break;
                case ImageType.BarCode:
                    {
                        var barImage = this.GenerateImage(BarcodeFormat.CODE_128, imageModel.Value, imageModel.Width, imageModel.Height);
                        field.Image = barImage;
                    }; break;
                case ImageType.QRCode:
                    {
                        var qrImage = this.GenerateImage(BarcodeFormat.QR_CODE, imageModel.Value, imageModel.Width, imageModel.Height);
                        field.Image = qrImage;
                    }; break;
                default: break;
            }
        }
        private Bitmap GenerateImage(BarcodeFormat format, string code, int width, int height)
        {
            var writer = new BarcodeWriter();
            writer.Format = format;
            EncodingOptions options = new EncodingOptions()
            {
                Width = width,
                Height = height,
                Margin = 2,
                PureBarcode = false
            };
            writer.Options = options;
            if (format == BarcodeFormat.QR_CODE)
            {
                var qrOption = new QrCodeEncodingOptions()
                {
                    DisableECI = true,
                    CharacterSet = "UTF-8",
                    Width = width,
                    Height = height,
                    Margin = 2
                };
                writer.Options = qrOption;
            }
            var codeimg = writer.Write(code);
            return codeimg;
        }
        public void Dispose()
        {
            this.Client.Dispose();
            this.Doc.Close();
            this.Doc.Dispose();
        }
    }
}
