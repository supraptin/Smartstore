# Release Notes

## Smartstore 5.0.4

### New Features

- **easyCredit** plugin

### Improvements

- Added price settings for discount requirements to be validated in product lists.
- Faster loading of product lists that contain bundles with per-item pricing.
- MegaSearch: significantly more speed, especially with large amounts of data.

### Bugfixes

- #557 If the state is optional for addresses, none should be preselected when creating them.
- #608 Build DeleteGuestCustomers query with LINQ again.
- Fixed ArgumentException "The source argument contains duplicate keys" in case of menus with duplicate system names.
- MySQL: fixed migration failure when UTC_TIMESTAMP was used as default value for date columns.
- High data payload: 
	- Fixed InvalidOperationException "A second operation was started on this context instance before a previous operation completed" when opening category (and others) edit page.
	- Fixed many product tags blocks the loading of the product edit page due to initialization of the product tag selection box.
- Fixed discount coupon code could not be applied in some cases.
- PostFinance: fixed "The specified refund amount CHF needs to be rounded to a maximum of 2 decimal places".
- Fixed ArgumentNullException in ProcessImageQuery.Add if name is null.
- Fixed price adjustment of attributes was saved only with two decimal places.


## Smartstore 5.0.3

### New Features

- (DEV) New `WebhookEndpointAttribute` endpoint metadata. Suppresses creation of guest accounts for webhook calls.
- PayPal: 
	- Added a window to display PayPal account information for support issues 
	- Added setting for upper limit for Pay Upon Invoice 
	- Added option to turn off PayPal buttons on shopping cart page

### Improvements

- Allows to delete and filter assignments of customers to customer roles that were automatically created by a rule.

### Bugfixes

- Installation: changing database connection settings has no effect until app restart
- Fixed HTTP 400 `BadRequest` issue on saving AJAX grid changes
- Web API: 
  - Fixed wrong $metadata configuration of `System.String[]` as a complex type instead of `ICollection<string>`.
  - Fixed `InvalidOperationException` in `Microsoft.OData.Client` using MediaFiles and MediaFolders endpoints.
  - Fixed `InvalidOperationException` in `Microsoft.OData.Client` "An unexpected 'StartObject' node was found for property named 'Size' when reading from the JSON reader. A 'PrimitiveValue' node was expected.".
- Output Cache:
  - Invoking CookieManager view component throws because antiforgery token cannot be written
- Theming
  - Fixed top description displayed instead of bottom description on manufacturer page.
  - Instant search must not display default thumbs if ShowProductImagesInInstantSearch is disabled.
  - Fixed AOS init problem
  - Multiple file uploader instances in a single page did not work
  - Product box in listings must not close when entering the bottom action drop
- Selected tabs were no longer remembered across requests
- Fixed `NullReferenceException` when deleting a shopping cart item.
- Fixed export file count was always 0 in export profile list.
- Fixed `FileNotFoundException` when uploading an import file.
- PayPal: Fixed error that occurs when shipping fees were to high for initially authorized order amount 
- Fixed reward points calculation
- #602 Implemented server side validation of payment data
- #603 Fixed after payment validation failure the data entry form is resetted.
- Fixed CheckoutState change tracking issues
- Fixed IBAN radio button issue when using direct debit offline payment.
- Avoids *deals* label in product lists if the current date is not within the date range of an assigned discount.
- #612 Emails are sent with the email priority low
- Fixed problem with media display for variant attribute combinations 
- Billiger: fixed export profile configuration must not contain store scope selection


## Smartstore 5.0.2

### Breaking Changes

- (DEV) Product._ProductPictures_ renamed to _ProductMediaFiles_

### New Features

- Updated to **.NET 7**
- **Web API** plugin
- **Stripe Elements** plugin
- **BeezUp** (commercial plugin)
- **ElmarShopInfo** (commercial plugin)
- **Shopwahl** (commercial plugin)
- **CartApproval** (commercial plugin)
- New app setting: `DbDefaultSchema`
- (DEV) New action filter attribute `DisallowRobotAttribute`

### Improvements

- ~10 % faster app startup and TTFB
- ~10 % less RAM usage
- Significantly faster attribute combination scanning for large combination sets (1.000+)

### Bugfixes

- `LocaleStringResource` table could contain many dupe records.
- Rule sets were not applied to shipping methods in checkout.
- `ArgumentNullException` when deleting an image assignment on product edit page.
- Despite activated export profile option **per store** no records were exported to a separate file.
- Sometimes Page Builder reveal effects did not run on page load, only on windows resize.
- Product details showed wrong related products.
- Fixed wrong implementation of ByRegionTaxProvider
- Fixed product linkage of product detail ask question message
- Fixed password change issue with user records without username
- Settings couldn't be saved in several places (in migrated shop) 
- Fixed add required products automatically
- DbCache:
  - Fix "Collection was modified; enumeration operation may not execute"
  - Fix "Index was outside the bounds of the array"
- #577 PdfSettings.Enabled displayed twice and PdfSettings.LetterPageSizeEnabled was missing.
- Topics which are rendered as widgets were published to sitemap 
- Redirection problems with changing language & ContactUs page
- Multistore settings couldn't be saved
- File upload for a product attribute is no longer possible once another attribute has been changed.
- Fixes NullReferenceException when placing an order with an invalid email display name.
- Fixed link generation issue: `pathBase` is stripped when target endpoint requires culture code
- Fixed DbUpdateException when deleting a customer address via backend.
- Routing: non-slug, unlocalized system routes did not redirect to localized route
- UrlRewriter: fixed greedy matching (`/en/` should not match `/men/`)
- Fixed RuleSets could not be added or removed from a shipping method.
- Fixed wrong SKU in order XML export if the order contains multiple variants of the same product.
- Fixed payment fee was always displayed in primary currency in checkout.
- Several PayPal fixes

## Smartstore 5.0.1

### Breaking Changes

- (DEV) Product.**OldPrice** renamed to **ComparePrice**

### New Features

- Pricing & GDPR
  - Compliance with **Omnibus Directive**
    - Product reviews: display a **Verified Purchase** badge
    - Label crossed out compare prices with "Lowest" or "Lowest recent price"
  - Free configuration of compare **price labels**, e.g. "MSRP", "Regular", "Before", "Instead of", "Lowest" etc.
  - **Discount badges**, e.g. "Deal", "Limited offer", "Black Friday" etc.
  - **Offer countdown**, e.g. "Ends in 2 days, 3 hours"
  - New pricing settings
    - Always display retail price
    - Default compare price label
    - Default regular price label
    - Offer price replaces regular price
    - Always display retail price
    - Show offer countdown remaining hours
    - Show offer badge
    - Show offer badge in lists
    - Show saving badge in lists
    - Offer badge label
    - Offer badge style
    - Limited offer badge label
    - Limited offer badge style
    - Show price label in lists
    - Show retail price saving
- **EmailReminder** (commercial plugin)
- **DirectOrder** (commercial plugin)
- **Billiger.de** (commercial plugin)
- **Google Remarketing** (commercial plugin)
- **File Manager** (commercial plugin)
- **GiroCode** (commercial plugin)
- **IPayment** (commercial plugin)
- PayPal
	- Added **RatePay** widget
	- Added **Pay per invoice** payment method 
	- Added **PayPal onboarding** to module configuration (handles simple configuration via direct email login without the need to create an app on the PayPal developer page). 
- **cXmlPunchout** (commercial plugin)
- **OCI Punchout** (commercial plugin)
- **BizUrlMapper** (commercial plugin)
- Added **Barcode** encoding and generation infrastructure:
  - Can encode: EAN, QRCode, UPCA, UPCE, Aztec, Codabar, Code128, Code39, Code93, DataMatric, KixCode, PDF417, RoyalMail, TwoToFive
  - Can generate: Image (any type), SVG drawing
- MediaManager: display image **IPTC and EXIF metadata**
- MediaManager: added internal admin comment field
- (DEV) New TagHelpers
  - `sm-suppress-if-empty`: suppresses tag output if child content is empty (due to some conditional logic).
  - `sm-suppress-if-empty-zone`: suppresses parent tag output if a specified child zone is empty or whitespace.
- (DEV) Embedded/Inline mail attachments
- (DEV) Localized entity metadata: `ILocalizedEntityDescriptorProvider`, `ILocalizedEntityLoader`
- (DEV) New setting `SmtpServerTimeout` in *appsettings.json*

### Improvements

- Increased performance
  - Faster app startup
  - ~100 MB less memory usage after app start
- FLEX theme: pure CSS responsive tabs (tabs transform to accordion on screens smaller than md)
- Sticky image gallery in product detail
- (DEV) New methods: TabFactory `InsertBefore()`,`InsertAfter()`, `InsertBeforeAny()`, `InsertAfterAny()`, `InsertAt()`
- (DEV) New attribute for `tab` TagHelper: `sm-hide-if-empty`
- (DEV) New rendering extension method: `IHtmlContent.HasValue()`
- (DEV) New rendering extension method: `IHtmlHelper.RenderZoneAsync()`
- (DEV) DataGrid row editing: handle prefixed controls correctly (e.g. "CustomProperties")
- Additional fees are not allowed by PayPal, therefore removed the feature
- Added cacheable routes for *Google Analytics* widgets
- (DEV) Made MediaImporter more generic
- Removed preconfigured Google Fonts retrieval from Google servers from themes AlphaBlack & AlphaBlue  

### Bugfixes

- `LocalFile` did not implement `CreateFileAsync()` correctly, which led to PackageInstaller, PageBuilder thumbnail cache and PublicFolderPublisher throwing `NotImplementedException`
- Media legacy url redirection did not work: `TemplateMatcher` does not evaluate inline constraints anymore
- *MediaManager* always displayed current date instead of file's last updated date
- *MegaMenu*: fixed badge styling issues
- Fixed "Unknown schema or invalid link expression 'mailto:...'
- Memory cache: parallel key enumeration sometimes failed
- Fixed *Google Analytics* number formatting issues
- Several fixes for laying Emails on a local directory
- Removed payment fee configuration from PayPal plugin
- Fixed Drag&Drop of images for HTML-Editor
- Fixed saving of emails on disk
- After installation of modules with custom Sass imports: bundle disk cache was not invalidated
- #539 Fixed flickering on hovering over product image on product detail page
- #552 <meta itemprop="availability"..> should not be rendered twice
- Fixed theme preview tool display 
- Fixed creating of SeoSlugs with special chars for installation 


## Smartstore 5.0.0

Smartstore 5 is a port of Smartstore 4 - which is based on the classic .NET Framework 4.7.2 - to the new ASP.NET Core 6 platform.

### Highlights

- Smartstore 5 is now **cross-platform** and supports **Linux** and **macOS** alongside **Microsoft Windows**. This means that Smartstore can be run on almost any hosting server, whether dedicated, cloud or shared.
- In addition to **Microsoft SQL Server**, Smartstore now supports **MySQL**. **PostgreSQL** is in planning and will follow soon.
- Smartstore 5 is one of the **fastest out-of-the-box e-commerce solutions in the world**! A small store with less than 1,000 items and a few dozen categories can achieve an average *Time to First Byte* (TTFB) of far below 100 milliseconds... even without output cache or other performance measures.
  - Compared to Smartstore 4, 10x faster in some areas
  - Significantly less memory consumption (approx. 50%)
  - Even low-cost (cloud) hosting delivers high performance
- Powerful **DataGrid** in the backend
  - Developed in-house, no more 3rd party libaries with annoying license restrictions
  - Intuitive, feature-rich and flexible
  - Supports row selection, multi-column sorting, column reordering, column toggling, paging etc.
  - Grid state is persisted in browser's local storage and restored on page refresh
  - Search filter expressions: run complex search queries, e.g. `(*jacket or *shirt) and !leather*`
  - (DEV) `datagrid` TagHelper which lets you control every aspect of the grid
- Frontend & backend **facelifting**
- Create, manage and restore **database backups** in the backend
- More **external authentication** providers: Google, Microsoft, LinkedIn coming soon
- Advanced settings for **image processing**: compression, quality, response caching etc.

### Breaking or Significant Changes

- **Blog**, **News**, **Forum** and **Polls** are now external commercial plugins
- No support for Microsoft SQL Server Compact Edition (SQLCE) anymore
- Payment providers need to be reconfigured (API Key etc.)

### Project Status

- Except for the **WebApi** plugin, the open source Community Edition has been fully ported (WebApi will follow soon)
- Already ported commercial plugins: 
  - Azure
  - BMEcat
  - Common Export Providers
  - Content Slider
  - ETracker
  - GDPR
  - Guenstiger
  - Media Manager
  - Mega Menu
  - Mega Search
  - Mega Search Plus
  - OpenTrans
  - Order Number Formatter
  - Output Cache
  - PageBuilder
  - PdfExport
  - PostFinance
  - Redis
  - SearchLog
  - Skrill
  - Sofortueberweisung
  - TinyImage
  - TrustedShops
  - UrlRewriter
  - *Other commercial plugins developed by Smartstore will follow soon*
- Obsolete plugins that will not be ported: 
  - AccardaKar
  - BizImporter
  - EasyCredit
  - NewsImporter
  - LeGuide Shopwahl

### Development

- No proprietary Unit of Work & Repository Pattern anymore: gave up `IDbContext` and `IRepository<T>` in favor of `DbContext` and `DbSet<T>`
- Less and more lightweight service classes by removing all generic CRUD stuff
- Much easier plugin development
- Async function calls all the way through
- Database schema did not change and therefore remains backward compatible. Still we STRONGLY recommend to create a backup before upgrading
- Extremely powerful widget system
- Large TagHelper library with 50+ custom helpers
- On-demand deployment of native libraries via NuGet
- Custom entities in plugin projects can now define navigation properties to entities in the application core