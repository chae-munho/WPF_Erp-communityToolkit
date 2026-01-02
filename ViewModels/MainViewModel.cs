using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using WpfApp1.Models;
using WpfApp1.Services;

namespace WpfApp1.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly InventoryService _service = new();

       
        public ObservableCollection<Product> Products => _service.Products;
        public ObservableCollection<StockTransaction> Transactions => _service.Transactions;

        [ObservableProperty] private Product? selectedProduct;

        // 제품 추가 입력(문자열로 받고 Command에서 파싱 -> 바인딩 변환 에러 방지)
        /*
            예시   [ObservableProperty] private string newProductName = "";  -->
        public string NewProductName
        {
            get => newProductName;
            set => SetProperty(ref newProductName, value);
        }
         */
        [ObservableProperty] private string newProductName = "";
        [ObservableProperty] private string newProductQuantityText = "";
        [ObservableProperty] private string newProductPriceText = "";

        // 입출고 입력
        [ObservableProperty] private string stockQuantityText = "";

        [ObservableProperty] private string statusMessage = "";

        //변수가 아님 표현식 프로퍼티임
        public int TotalProductCount => Products.Count;
        public int TotalStockCount => Products.Sum(p => p.Quantity);
        public decimal TotalStockValue => Products.Sum(p => p.TotalValue);

        public string SelectedProductDisplay =>
            SelectedProduct == null
                ? "선택된 제품: 없음"
                : $"선택된 제품: {SelectedProduct.Name} (현재 재고: {SelectedProduct.Quantity})";

        public MainViewModel()
        {
            // 샘플 데이터(원하면 삭제 가능)
            _service.AddProduct(new Product { Id = 1, Name = "나사", Quantity = 100, Price = 50m });
            _service.AddProduct(new Product { Id = 2, Name = "볼트", Quantity = 80, Price = 120m });
            _service.AddProduct(new Product { Id = 3, Name = "너트", Quantity = 60, Price = 30m });



            
            //3개 제품의 값이 바뀌면 요약 갱신하도록 연결
            foreach (var p in Products)
                p.PropertyChanged += (_, __) => RefreshSummary();

            //제품이 추가 또는 삭제될때
            Products.CollectionChanged += (_, __) => RefreshSummary();

            //위에 두줄은 바뀔 떄마다 해주는거고 이건 처음 화면 표시용 즉 위에 두 줄은 예약을 한 상태임 바뀌면 적용하라 이런 식으로
            RefreshSummary();
        }

        //소스생성기가 SelectedProduct 프로퍼티를 자동으로 생성해줌 그리고 그 프로퍼티의 set안에서 OnSelectedProductChanged()를 자동으로 호출하도록 코드를 생성함
        partial void OnSelectedProductChanged(Product? value)
        {
            StatusMessage = "";
            OnPropertyChanged(nameof(SelectedProductDisplay));
        }

        private void RefreshSummary()
        {
            //nameof(TotalProductCount)는 문자열 "TotalProductCount"로 바뀌는 문법 오타 방지용
            //화면에 표시되는 요약 값들 상품 총 개수, 스톡 개수, 스톡 값, 선택된 상품들 다시 계산해서 UI가 새로 그리게 하라는 뜻
            OnPropertyChanged(nameof(TotalProductCount));
            OnPropertyChanged(nameof(TotalStockCount));
            OnPropertyChanged(nameof(TotalStockValue));
            OnPropertyChanged(nameof(SelectedProductDisplay));
        }

        [RelayCommand]
        private void AddProduct()
        {
            StatusMessage = "";


            var name = (NewProductName ?? "").Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                StatusMessage = "제품명은 필수입니다.";
                return;
            }

            if (!int.TryParse(NewProductQuantityText, NumberStyles.Integer, CultureInfo.InvariantCulture, out int qty))
            {
                StatusMessage = "초기수량은 정수로 입력하세요.";
                return;
            }

            if (qty < 0)
            {
                StatusMessage = "초기수량은 0 이상이어야 합니다.";
                return;
            }

            if (!decimal.TryParse(NewProductPriceText, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal price))
            {
                StatusMessage = "단가는 숫자로 입력하세요.";
                return;
            }

            if (price < 0)
            {
                StatusMessage = "단가는 0 이상이어야 합니다.";
                return;
            }

            int nextId = Products.Count == 0 ? 1 : Products.Max(p => p.Id) + 1;

            var product = new Product
            {
                Id = nextId,
                Name = name,
                Quantity = qty,
                Price = price
            };

            product.PropertyChanged += (_, __) => RefreshSummary();
            _service.AddProduct(product);

            // 입력 초기화
            NewProductName = "";
            NewProductQuantityText = "";
            NewProductPriceText = "";
            SelectedProduct = product;

            RefreshSummary();
        }

        [RelayCommand]
        private void DeleteProduct()
        {
            StatusMessage = "";

            if (SelectedProduct == null)
            {
                StatusMessage = "삭제할 제품을 선택하세요.";
                return;
            }

            var target = SelectedProduct;
            _service.RemoveProduct(target);
            SelectedProduct = null;

            RefreshSummary();
        }

        [RelayCommand]
        private void StockIn()
        {
            StatusMessage = "";

            if (SelectedProduct == null)
            {
                StatusMessage = "입고할 제품을 선택하세요.";
                return;
            }
            //사용자가 입력한 StockQuantityText 문자열을 정수로 바꿔보는데 성공사면 qty에 담고 실패하면 if안으로 
            if (!int.TryParse(StockQuantityText, NumberStyles.Integer, CultureInfo.InvariantCulture, out int qty))
            {
                StatusMessage = "수량은 정수로 입력하세요.";
                return;
            }

            try
            {
                _service.StockIn(SelectedProduct, qty);
                StockQuantityText = "";
                RefreshSummary();
            }
            catch (Exception ex)
            {
                StatusMessage = ex.Message;
            }
        }

        [RelayCommand]
        private void StockOut()
        {
            StatusMessage = "";

            if (SelectedProduct == null)
            {
                StatusMessage = "출고할 제품을 선택하세요.";
                return;
            }

            if (!int.TryParse(StockQuantityText, NumberStyles.Integer, CultureInfo.InvariantCulture, out int qty))
            {
                StatusMessage = "수량은 정수로 입력하세요.";
                return;
            }

            try
            {
                _service.StockOut(SelectedProduct, qty); // 여기서 재고 부족 체크
                StockQuantityText = "";
                RefreshSummary();
            }
            catch (Exception ex)
            {
                StatusMessage = ex.Message;
            }
        }
    }
}
