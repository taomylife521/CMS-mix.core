﻿using Mix.Heart.Entities;
using Mix.Services.Ecommerce.Lib.Enums;

namespace Mix.Services.Ecommerce.Lib.Entities.Mix
{
    public class OrderItem : EntityBase<int>
    {
        public OrderItemType? ItemType { get; set; }
        public string? Sku { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public string? ReferenceUrl { get; set; }
        public string? Currency { get; set; }
        public int PostId { get; set; }
        public double OriginalPrice { get; set; }
        public double Discount { get; set; }
        public double? Price { get; set; }
        public bool? IsActive { get; set; }
        public double? Percent { get; set; }
        public int Quantity { get; set; }
        public double Total { get; set; }
        public int OrderDetailId { get; set; }
        public int TenantId { get; set; }
    }
}
