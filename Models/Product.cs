using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;

namespace WpfApp1.Models
{
    public partial class Product : ObservableObject
    {
        /*
         Community Toolkit의 [ObservableProperty]는 자동으로 프로퍼티를 생성
         private int quantity; -> public int Quantity { get; set; }
         private decimal price; -> public decimal Price { get; set; }
        그래서 Quantity * Price; 이렇게 사용할 수 있었음
         */

        [ObservableProperty] private int id;
        [ObservableProperty] private string name = string.Empty;
        [ObservableProperty][NotifyPropertyChangedFor(nameof(TotalValue))] private int quantity;
        [ObservableProperty][NotifyPropertyChangedFor(nameof(TotalValue))] private decimal price;

        public decimal TotalValue => Quantity * Price;
    }
}
