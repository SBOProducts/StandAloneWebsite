using System.Collections.Generic;
using System.Net;
using System;

namespace Customer.Helpers
{
public class Importer
    {
        string ImagesDataURL = "http://broweronline.com/imager.aspx";
        string ProductDataURL = "http://broweronline.com/lister.aspx";

        public Importer()
        {

        }

        List<ImportedProduct> products = null;
        public List<ImportedProduct> GetProducts()
        {
            if (products != null)
                return products;

            products = new List<ImportedProduct>();
            
            using (WebClient wc = new WebClient())
            {
                string response = wc.DownloadString(ProductDataURL);
                response = response.Replace("|*|", "`");
                foreach (string part in response.Split('`'))
                {
                    if (!string.IsNullOrEmpty(part.Trim()))
                    {
                        ImportedProduct p = new ImportedProduct(part);
                        products.Add(p);
                        //Console.WriteLine(p.Title);
                    }
                }
            }
            return products;
        }

        List<ImportedImage> images = null;
        public List<ImportedImage> GetImages()
        {
            if (images != null)
                return images;

            images = new List<ImportedImage>();

            using (WebClient wc = new WebClient())
            {
                string response = wc.DownloadString(ImagesDataURL);
                response = response.Replace("|*|", "`");
                foreach (string part in response.Split('`'))
                {
                    if (!string.IsNullOrEmpty(part.Trim()))
                    {
                        ImportedImage img = new ImportedImage(part);
                        images.Add(img);
                    }
                }
            }
            return images;
        }
    }

    public class ImportedProduct
    {
        public ImportedProduct(string dataPart)
        {
            string[] parts = dataPart.Split('|');
            Category = parts[0];
            ProductId = Guid.Parse(parts[1]);
            Title = parts[2];
            Price = Decimal.Parse(parts[3]);
            Description = parts[4];
        }
        public string Category { get; set; }
        public Guid ProductId { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }



    }

    public class ImportedImage
    {
        public ImportedImage(string dataPart)
        {
            string[] parts = dataPart.Split('|');
            ImageId = Guid.Parse(parts[0]);
            ProductId = Guid.Parse(parts[1]);
            FileName = parts[2];
            Sequence = int.Parse(parts[3]);
        }
        public Guid ImageId { get; set; }
        public Guid ProductId { get; set; }
        public string FileName { get; set; }
        public int Sequence { get; set; }
        public string Url
        {
            get
            {
                return string.Format("http://broweronline.com/UserImages/Product/{0}/{1}", ProductId, FileName);
            }
        }
    }
}