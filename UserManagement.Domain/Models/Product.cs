﻿using System;
using System.Collections.Generic;

namespace UserManagement.Domain.Models;

public partial class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int CategoryId { get; set; }

    public decimal Rate { get; set; }

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public bool? Available { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public decimal? Discount { get; set; }

    public string? ProductCode { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();

    public virtual ICollection<ProductCart> ProductCarts { get; set; } = new List<ProductCart>();

    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual User? UpdatedByNavigation { get; set; }

    public virtual ICollection<UserWishlist> UserWishlists { get; set; } = new List<UserWishlist>();
}
