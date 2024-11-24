# Floral Management System

A comprehensive application built in C# using MySQL for database management. This system manages a floral business, including customers, employees, inventory, orders, and statistical reporting.

## Features

### Customer Module
- Create an account and log in as a customer.
- View purchase history.
- Buy standard or customized bouquets.
- Manage customer profiles and payment history.

### Employee Module
- Login system for employees.
- Manage inventory.
- Update order status.

### Manager Module
- View statistics (best-selling bouquets, best customers, etc.).
- Manage stores and inventory.
- Generate reports in XML and JSON.

### Statistics and Reporting
- Identify the best customer of the year/month.
- Determine the most successful boutique.
- Generate the average price of orders.
- Export client and sales data to XML and JSON.

## Prerequisites

- **Environment:**
  - Windows OS (recommended for ease of setup).
  - Visual Studio with .NET framework installed.
- **Database:**
  - MySQL server installed and running.
- **Libraries:**
  - `MySql.Data`
  - `Newtonsoft.Json`

## Database Setup

1. Ensure the MySQL database `floral` is set up with the required schema.
2. Set the connection string in the code to match your MySQL credentials:
   ```csharp
   string connectionString = "SERVER=localhost;PORT=3306;DATABASE=floral;UID=root;PASSWORD=root;";
   ```

3. Use the provided SQL scripts to create the necessary tables.

## Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/username/repository-name.git
   ```

2. Open the project in Visual Studio.

3. Restore NuGet packages for dependencies.

4. Build and run the application.

## Usage

### Customer Actions
- **Create an account:** Follow prompts to provide name, email, and billing details.
- **View and buy bouquets:** Select from standard or customized bouquets.

### Employee Actions
- **Inventory management:** Add or view inventory for specific boutiques.
- **Update orders:** Modify the status and description of customer orders.

### Manager Actions
- **View statistics:** Access a variety of reports, such as:
  - Best-selling bouquets.
  - Most successful boutique.
  - Top customers (yearly/monthly).
- **Export data:** Generate XML and JSON files for client and sales data.

## Example Statistics Queries

### Best-Selling Bouquet
SQL Query:
```sql
SELECT nom_bouquet, COUNT(nom_bouquet) AS total_vendu
FROM Paiement
GROUP BY nom_bouquet
ORDER BY total_vendu DESC
LIMIT 1;
```

### Average Order Price
SQL Query:
```sql
SELECT AVG(prix) AS average_price FROM Paiement;
```

## XML and JSON Export

- **XML Export:** Generates an XML file for clients with purchase histories.
- **JSON Export:** Creates a JSON file listing customers who haven't ordered in the last 6 months.

## Contributing

1. Fork the repository.
2. Create a branch (`git checkout -b feature-name`).
3. Commit your changes (`git commit -m "Add new feature"`).
4. Push to the branch (`git push origin feature-name`).
5. Open a pull request.

## License

This project is licensed under the MIT License.

## Acknowledgements

- [MySql.Data](https://www.nuget.org/packages/MySql.Data/) for MySQL database connectivity.
- [Newtonsoft.Json](https://www.newtonsoft.com/json) for JSON processing in .NET.
