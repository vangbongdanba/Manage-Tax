# QuanLyBanHangThueVN 🇻🇳
> **Hệ thống Quản lý Bán hàng, Kho hàng và Tính thuế linh hoạt theo chính sách Việt Nam (Dự án Demo)**

[![.NET 8.0](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/)
[![Windows Forms](https://img.shields.io/badge/Platform-Windows%20Forms-brightgreen.svg)]()
[![EF Core](https://img.shields.io/badge/ORM-Entity%20Framework%20Core%208.0-purple.svg)]()
[![Database](https://img.shields.io/badge/Database-SQLite-lightgrey.svg)]()

---

## 📌 Giới thiệu dự án

**QuanLyBanHangThueVN** là ứng dụng desktop Windows Forms được xây dựng trên nền tảng **.NET 8.0**, phục vụ nhu cầu quản lý bán hàng (POS), nhập xuất tồn kho và đặc biệt là **tính toán - hoạch toán nghĩa vụ thuế** linh hoạt theo quy định pháp luật Việt Nam dành cho cả **Doanh nghiệp** và **Hộ kinh doanh cá thể**.

Dự án áp dụng mô hình kiến trúc phân lớp hiện đại (Layered Architecture), sử dụng **Generic Repository Pattern**, **Dependency Injection**, và **Entity Framework Core SQLite** giúp hệ thống vận hành nhẹ nhàng, độc lập không phụ thuộc vào máy chủ cơ sở dữ liệu cồng kềnh.

---

## ✨ Tính năng chính

### 1. Quản lý Hàng hóa & Danh mục
- Quản lý danh mục ngành hàng (ví dụ: Xe đạp điện, Xe điện 3 bánh, Phụ tùng...).
- Quản lý danh sách sản phẩm: mã sản phẩm, tên, giá nhập, giá bán, đơn vị tính, số lượng tồn kho.
- Gắn nhóm thuế GTGT áp dụng cho từng sản phẩm.

### 2. Quản lý Nhập kho (Inventory)
- Nhập thêm hàng hóa vào kho: cập nhật tức thì số lượng tồn thực tế.
- Lưu lại lịch sử nhập hàng, thời gian nhập, đơn giá và ghi chú.

### 3. Bán hàng tại quầy (POS - Point of Sale)
- Chọn sản phẩm, số lượng bán, chiết khấu.
- Tự động kiểm tra số lượng tồn kho trước khi tạo hóa đơn.
- Tự động tính toán thuế GTGT cho từng sản phẩm và tính tổng hóa đơn.
- Tạo mã hóa đơn tự động và cập nhật trừ hàng tồn kho theo giao dịch an toàn (Transaction).

### 4. Cơ chế tính thuế Việt Nam (Tax Engine)
Hệ thống được thiết kế mở để thích ứng với các thay đổi chính sách thuế:
- **Thuế GTGT (VAT):**
  - Quản lý các nhóm thuế chuẩn: 10%, 5%, 0%, không chịu thuế.
  - **Tự động áp dụng giảm thuế VAT xuống 8%** cho các mặt hàng đủ điều kiện căn cứ theo thời gian hiệu lực của các văn bản quy phạm pháp luật (ví dụ: Nghị quyết của Quốc hội như NQ 204/2025/QH15).
- **Thuế đối với Hộ kinh doanh cá thể:**
  - Tính thuế TNCN theo tỷ lệ % doanh thu đối với lĩnh vực phân phối, cung cấp hàng hóa (Thông tư 40/2021/TT-BTC & Thông tư 18/2026).
  - Giám sát và cảnh báo **ngưỡng doanh thu năm** (doanh thu không chịu thuế).
- **Linh hoạt cấu hình:** Dễ dàng thêm, cập nhật mức thuế, ngày hiệu lực và căn cứ pháp lý trong màn hình Cấu hình thuế.

### 5. Báo cáo & Thống kê
- **Báo cáo thuế theo kỳ:** Xem doanh thu, tổng thuế VAT và dự kiến thuế TNCN phát sinh theo tháng/năm.
- **Bảng điều khiển (Dashboard):**
  - Thống kê tổng số lượng sản phẩm đã bán, tổng doanh thu, lợi nhuận gộp tạm tính và tổng tồn kho.
  - Biểu đồ trực quan tích hợp thư viện **LiveChartsCore / SkiaSharp**.
  - Xếp hạng Top 5 sản phẩm bán chạy nhất.

---

## 🏗️ Kiến trúc & Công nghệ

### Công nghệ sử dụng
- **Ngôn ngữ:** C# 12 / .NET 8.0 (Windows Forms)
- **Cơ sở dữ liệu & ORM:** SQLite + Entity Framework Core 8.0
- **Dependency Injection:** `Microsoft.Extensions.DependencyInjection`
- **Biểu đồ:** `LiveChartsCore.SkiaSharpView.WinForms`

### Cấu trúc thư mục
```text
QuanLyBanHangThueVN/
├── Data/
│   ├── AppDbContext.cs          # DbContext Entity Framework Core
│   ├── DatabaseSeeder.cs        # Khởi tạo tự động dữ liệu mẫu
│   ├── IRepository.cs           # Generic Repository Interface
│   └── Repository.cs            # Generic Repository Implementation
├── Models/
│   ├── Category.cs              # Danh mục ngành hàng
│   ├── Product.cs               # Sản phẩm
│   ├── StockImport.cs           # Lịch sử nhập hàng
│   ├── TaxGroup.cs              # Nhóm thuế GTGT
│   ├── TaxConfiguration.cs     # Cấu hình luật & mức thuế
│   ├── Invoice.cs               # Hóa đơn bán hàng
│   └── InvoiceDetail.cs         # Chi tiết từng sản phẩm trên hóa đơn
├── Services/
│   ├── TaxService.cs            # Nghiệp vụ tính thuế GTGT, TNCN, kiểm tra ngưỡng
│   ├── InventoryService.cs      # Quản lý kho hàng & nhập hàng
│   ├── SalesService.cs          # Quản lý giỏ hàng & thanh toán hóa đơn
│   └── StatisticsService.cs     # Thống kê báo cáo & lợi nhuận
├── Forms/
│   ├── MainForm.cs              # Giao diện chính & menu điều hướng
│   ├── DashboardForm.cs         # Màn hình tổng quan & biểu đồ
│   ├── ProductForm.cs           # Quản lý danh sách sản phẩm
│   ├── AddProductForm.cs        # Thêm mới sản phẩm
│   ├── ImportForm.cs            # Màn hình nhập kho
│   ├── POSForm.cs               # Màn hình bán hàng POS
│   ├── ReportForm.cs            # Báo cáo thuế
│   └── TaxConfigForm.cs         # Cấu hình chính sách thuế
├── Program.cs                   # Điểm khởi chạy ứng dụng & cấu hình DI Container
└── QuanLyBanHangThueVN.csproj   # File cấu hình dự án .NET 8
```

---

## 🚀 Hướng dẫn cài đặt và chạy

### 1. Yêu cầu hệ thống
- Hệ điều hành: Windows 10 / Windows 11
- Cài đặt [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Visual Studio 2022 (khuyến nghị có workload *.NET Desktop Development*) hoặc Visual Studio Code / JetBrains Rider.

### 2. Các bước chạy dự án

1. **Clone repository về máy:**
   ```bash
   git clone https://github.com/vangbongdanba/Manage-Tax.git
   cd QuanLyBanHangThueVN
   ```

2. **Khôi phục các gói thư viện (Restore NuGet packages):**
   ```bash
   dotnet restore
   ```

3. **Chạy ứng dụng:**
   ```bash
   dotnet run
   ```

> 💡 **Lưu ý:** Trong lần khởi chạy đầu tiên, hệ thống sẽ tự động tạo file cơ sở dữ liệu SQLite (`QuanLyBanHang.db`) và nạp sẵn dữ liệu mẫu về danh mục, sản phẩm, nhóm thuế thông qua `DatabaseSeeder`.

---

## ⚠️ Lưu ý miễn trừ trách nhiệm (Disclaimer)
> Các số liệu tính toán thuế trong phần mềm phục vụ mục đích mô phỏng, quản lý nội bộ và ước tính. Nghĩa vụ thuế thực tế của doanh nghiệp hoặc hộ kinh doanh cần được xác định và kê khai theo đúng quy định pháp luật và hướng dẫn của cơ quan quản lý thuế tại thời điểm phát sinh.

---

## 📄 Bản quyền
Dự án được xây dựng với mục đích học tập và làm đồ án / demo giải pháp quản lý bán hàng kết hợp tính thuế tại Việt Nam.
