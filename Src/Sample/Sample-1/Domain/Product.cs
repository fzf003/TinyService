using System;
using System.Collections.Generic;
using System.Text;

namespace Sample_1.Domain
{
    public class Product
    {
        public int Id { get; private set; }
        /// <summary>
        /// 商品名称
        /// </summary>
       
        public string Name { get; private set; }
        /// <summary>
        /// 类别
        /// </summary>
       
        public string Category { get; private set; }
        /// <summary>
        /// 摘要
        /// </summary>
       
        public string Summary { get; private set; }
        /// <summary>
        /// 商品描述
        /// </summary>
      
        public string Description { get; private set; }
        /// <summary>
        /// 图片
        /// </summary>
        
        public string ImageFile { get; private set; }
        /// <summary>
        /// 商品价格
        /// </summary>
    

        public decimal? Price { get; private set; }
        /// <summary>
        /// 商品状态
        /// </summary>
     
        public ProductStatus Status { get; private set; }

        
        public DateTime CreateTime { get; private set; }
   

        private Product() { }

        private Product(int Id,string name, string category, string description, string imagefile, decimal? price)
        {
            this.Id = Id;
            this.Name = name;
            this.Summary = category;
            this.Category = category;
            this.Description = description;
            this.ImageFile = imagefile;
            this.Price = price;
            this.Status = ProductStatus.Draft;
            this.CreateTime = DateTime.Now;
        }

        public void ChangeName(string name)
        {
            this.Name = name;
            this.CreateTime = DateTime.Now;
        }

        /// <summary>
        /// 激活
        /// </summary>
        public void Activate()
        {
            this.Status = ProductStatus.Active;
            this.CreateTime = DateTime.Now;
        }

        /// <summary>
        /// 停售
        /// </summary>
        public void Discontinue()
        {
            this.Status = ProductStatus.Discontinued;
            this.CreateTime = DateTime.Now;
        }

        public static Product Create(int Id,string name, string category, string description, string image, decimal? price)
        {
            return new Product(Id,name, category, description, image, price);
        }
    }

    public enum ProductStatus
    {
        /// <summary>
        /// 草稿
        /// </summary>
        Draft,
        /// <summary>
        /// 已激活
        /// </summary>
        Active,
        /// <summary>
        /// 禁售
        /// </summary>
        Discontinued
    }
}
