using Sample_1.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using TinyService.Cqrs.Commands;
namespace Sample_1.CommandHandler
{
    public class CreateProductCommand: ICommand
    {
        public CreateProductCommand(string name, string category, string summary, string description, string imageFile, decimal? price, ProductStatus status)
        {
            Name = name;
            Category = category;
            Summary = summary;
            Description = description;
            ImageFile = imageFile;
            Price = price;
            Status = status;
        }

        public string Name { get; }

        /// <summary>
        /// 类别
        /// </summary>

        public string Category { get; }
        /// <summary>
        /// 摘要
        /// </summary>

        public string Summary { get; }
        /// <summary>
        /// 商品描述
        /// </summary>

        public string Description { get; }
        /// <summary>
        /// 图片
        /// </summary>

        public string ImageFile { get; }
        /// <summary>
        /// 商品价格
        /// </summary>


        public decimal? Price { get; }
        /// <summary>
        /// 商品状态
        /// </summary>

        public ProductStatus Status { get; }

        
    }
}
