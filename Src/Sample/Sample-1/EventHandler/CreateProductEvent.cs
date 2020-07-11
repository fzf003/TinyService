using Sample_1.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using TinyService.Cqrs.Events;

namespace Sample_1.EventHandler
{
    public class CreateProductEvent : IEvent
    {
        public int Id { get; }

        public string Name { get; }
     
        /// <summary>
        /// 类别
        /// </summary>

        public string Category { get;  }
        /// <summary>
        /// 摘要
        /// </summary>

        public string Summary { get;  }
        /// <summary>
        /// 商品描述
        /// </summary>

        public string Description { get;  }
        /// <summary>
        /// 图片
        /// </summary>

        public string ImageFile { get;  }
        /// <summary>
        /// 商品价格
        /// </summary>


        public decimal? Price { get;  }
        /// <summary>
        /// 商品状态
        /// </summary>

        public ProductStatus Status { get; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; }

        public DateTime UpdateTime { get; }
        public CreateProductEvent(int id,string name, string category, string summary, string description, string imageFile, decimal? price = 0.0m, ProductStatus status = default)
        {
            Id = id;
            Name = name;
            Category = category;
            Summary = summary;
            Description = description;
            ImageFile = imageFile;
            Price = price;
            Status = status;
            CreateTime = DateTime.Now;
            UpdateTime = DateTime.Now;
        }
    }
}
