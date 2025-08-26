# Larana: E-commerce Website with ASP.NET

Welcome to Larana, a fully functional e-commerce website project built with the ASP.NET framework. This application provides a complete online shopping experience, from browsing products to managing user accounts and administering the site.

## 🎥 Demo Video

For a quick overview of the project's features and user interface, please watch the demo video:

**[Watch the Demo on Google Drive](https://drive.google.com/file/d/1_0717y9q1uwDL-TAeTa4Djy7nNhAOyBJ)**

---

## ✨ Features

The website is divided into two main parts: a feature-rich storefront for customers and a powerful admin panel for site administrators.

### Customer-Facing Features:
-   **Homepage:** A welcoming landing page with featured products and categories.
-   **Shop:** Browse all available products with filtering and sorting capabilities.
-   **Product Detail Page:** View detailed information, images, and pricing for each product.
-   **Static Pages:**
    -   **About Us:** Learn more about the company.
    -   **Contact:** A form for users to send inquiries.
    -   **FAQ:** A section for frequently asked questions.
-   **Account Management:**
    -   User Registration and Login.
    -   View and manage personal profile information.
    -   Track order history.

### Administrative Features:
-   **Admin Panel:** A secure backend interface for managing the entire website.
-   **Product Management:** Add, edit, and delete products from the catalog.
-   **User Management:** View and manage customer accounts.
-   **Order Management:** Track and process customer orders.
-   **(Other admin features...)** *(You can add more specific details here about what the admin can do)*

---

## 💻 Tech Stack

This project is built using the following technologies:

-   **Backend:** ASP.NET (C#)
-   **Frontend:** HTML, CSS, JavaScript
-   **Database:** Microsoft SQL Server
-   **IDE:** Developed primarily in Visual Studio

---

## 🚀 Getting Started

To get a local copy up and running, follow these simple steps.

### Prerequisites

-   **Visual Studio** 2019 or later (with ASP.NET workload installed)
-   **Microsoft SQL Server** (and SQL Server Management Studio or a similar tool)

### Installation and Setup

1.  **Clone the repository:**
    ```bash
    git clone https://github.com/Oguzhan2022/E-commerce-Website.git
    ```

2.  **Open the solution in Visual Studio:**
    -   Navigate to the cloned directory and open the `Larana.sln` file.

3.  **Set up the database:**
    -   This project uses `.dacpac` files for database deployment. You need to publish them to your local SQL Server instance.
    -   Open your SQL Server management tool (like SSMS).
    -   Create two new empty databases (e.g., `LaranaDB` and `LaranaAccountDB`).
    -   Right-click on each database and select **Tasks -> Deploy Data-tier Application...**
    -   Browse for and select the corresponding `.dacpac` files from the project's root directory:
        -   `LaranaDB.dacpac`
        -   `Larana.Models.DbAccount.dacpac`
    -   Follow the wizard to complete the deployment.

4.  **Update the Connection Strings:**
    -   In the `Web.config` file within the `Larana` project, find the `<connectionStrings>` section.
    -   Update the connection strings to point to your local SQL Server instance and the databases you just created.

5.  **Build and Run the project:**
    -   Build the solution in Visual Studio to restore NuGet packages.
    -   Press `F5` or click the "Start" button to launch the website in your browser.

---

## 📂 Project Structure


```
├── Larana/                # The main ASP.NET project folder
│   ├── Controllers/       # Logic for handling user requests
│   ├── Models/            # Data models and database context
│   ├── Views/             # UI files (.cshtml)
│   ├── Content/           # CSS stylesheets
│   └── Scripts/           # JavaScript files
├── Larana.sln             # Visual Studio Solution file
├── LaranaDB.dacpac        # Database schema for the main application
└── Larana.Models.DbAccount.dacpac # Database schema for user accounts
```
