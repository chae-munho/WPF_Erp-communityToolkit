using System;
using System.Collections.ObjectModel;
using WpfApp1.Models;

namespace WpfApp1.Services
{
    /*
        ObservableCollection<T>는 WPF MVVM에서 리스트가 바뀌면 (추가 삭제 이동 초기화) UI가 자동으로 갱신되게 해주는 컬렉션
        List<T>처럼 보이지만 UI에 변경됐다는 신호를 보내는 기능이 핵심이다.
     */
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
