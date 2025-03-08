﻿namespace PresentationLayer.ActionRequests
{
    public class CreateProductActionRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public IFormFile Image { get; set; }
    }
}
