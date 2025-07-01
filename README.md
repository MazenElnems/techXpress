# TechXpress - E-commerce Platform

TechXpress is a full-featured e-commerce platform designed for selling electronics. It allows users to browse products, add them to a shopping cart, and complete purchases using an integrated payment gateway. The platform also includes an admin panel for managing products, categories, and orders.

## Features

### User Features
- Browse electronic products by category.
- Search and filter products.
- View product details, including specifications and reviews.
- Add products to the shopping cart.
- Secure checkout with integrated payment gateways.
- Order tracking and history.
- User authentication and profile management.

### Admin Features
- Manage product listings (add, update, delete).
- Organize products into categories.
- View and process customer orders.
- Manage user accounts and permissions.

## Tech Stack

- **Backend:** ASP.NET Core
- **Frontend:** Razor Pages / MVC
- **Database:** SQL Server
- **Authentication:** Identity Framework
- **Payment Gateway:** Stripe / PayPal integration
- **Hosting:** Azure

  ## Live Demo  
🌐 [Visit TechXpress](https://techxpress.tryasp.net/)  

### Steps to Run Locally
1. Clone the repository:
   ```sh
   git clone https://github.com/MazenElnems/TechXpress.git
   ```
2. Navigate to the project directory:
   ```sh
   cd TechXpress
   ```
3. Configure database connection in `appsettings.json`.
4. Apply database migrations:
   ```sh
   dotnet ef database update
   ```
5. Run the application:
   ```sh
   dotnet run
   ```
6. Open a browser and navigate to `http://localhost:[port]`.

## Contributing
Contributions are welcome! To contribute:
1. Fork the repository.
2. Create a feature branch (`git checkout -b feature-name`).
3. Commit your changes (`git commit -m 'Add feature'`).
4. Push to your branch (`git push origin feature-name`).
5. Open a pull request.


