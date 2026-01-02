using System;
using System.Collections.ObjectModel;
using WpfApp1.Models;

namespace WpfApp1.Services
{
    public class InventoryService
    {
        public ObservableCollection<Product> Products { get; } = new();
        public ObservableCollection<StockTransaction> Transactions { get; } = new();

        public void AddProduct(Product product) => Products.Add(product);

        public void RemoveProduct(Product product) => Products.Remove(product);

        public void StockIn(Product product, int qty)
        {
            if (qty <= 0) throw new ArgumentException("입고 수량은 1 이상이어야 합니다.");
            product.Quantity += qty;

            Transactions.Insert(0, new StockTransaction
            {
                Time = DateTime.Now,
                ProductName = product.Name,
                Type = "입고",
                Quantity = qty
            });
        }

        public void StockOut(Product product, int qty)
        {
            if (qty <= 0) throw new ArgumentException("출고 수량은 1 이상이어야 합니다.");
            if (product.Quantity < qty) throw new InvalidOperationException("재고가 부족합니다.");

            product.Quantity -= qty;

            Transactions.Insert(0, new StockTransaction
            {
                Time = DateTime.Now,
                ProductName = product.Name,
                Type = "출고",
                Quantity = qty
            });
        }
    }
}
