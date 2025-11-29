We are implementing the Admin Dashboard page of a multi-tenant cafe menu application (ASP.NET Core MVC, Clean Architecture, EF Core).

Requirements for the Admin Dashboard:

1) Product count by category widget:
   - Create an endpoint on DashboardController that returns JSON with product counts per category for the current tenant.
   - The data should be loaded from the existing IProductRepository / DashboardService layer.
   - On the view (Areas/Admin/Views/Dashboard/Index.cshtml), render a modern card widget that shows a table: CategoryName + ProductCount.
   - Add a small “Refresh” button to reload the widget via JavaScript (fetch + DOM update).

2) Currency widget:
   - Create an endpoint on DashboardController that returns the latest FX rates for USD, EUR, TRY as JSON.
   - Use our existing ICurrencyService, which internally calls the URL http://any.kur.com/kurlar web/rest service to fetch currency information.
   - In the view, create a second card that shows USD/TRY, EUR/TRY and TRY values.
   - Use JavaScript to call the endpoint every 10 seconds and update the numbers on the page.
   - Show a “last updated” timestamp under or above the widget.

3) General constraints:
   - Follow the existing Clean Architecture structure: Controllers in Web, business logic in Application Services, data access in Repositories.
   - Do not add comments.
   - Use the existing Admin layout and CSS utility classes (content-card, page-header, table-modern, etc.) to keep the UI consistent.
   - Keep all strings and labels on the UI in Turkish (e.g. “Kategoriye Göre Ürün Sayısı”, “Güncel Kur Bilgisi”).

Please:
- Generate or update DashboardController to add the necessary actions.
- Generate or update DashboardService if needed to expose product count data.
- Update the Dashboard/Index.cshtml view to include both widgets and the JavaScript for periodic refresh.
