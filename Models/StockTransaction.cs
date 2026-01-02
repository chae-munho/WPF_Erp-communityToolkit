using System;

namespace WpfApp1.Models
{
    //입출고 내역이라 대부분 생성 후 값이 바뀌지 않는 데이터라서 굳이 ObservableProperty가 없어도 UI가 잘 갱신됨
    //컬렉션 변경 알림(ObservableCollection)만으로 충분함
    public class StockTransaction
    {
        public DateTime Time { get; set; } = DateTime.Now;
        public string ProductName { get; set; } = "";
        public string Type { get; set; } = "";   // "입고" / "출고"
        public int Quantity { get; set; }        // 입고/출고 수량(양수로 기록)
    }
}
