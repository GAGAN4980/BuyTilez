# 🏗️ BuyTilez - Floor Tile Marketplace Web Application  

## 📌 Overview  
**BuyTilez** is a modern **floor tile marketplace** built using **ASP.NET 8, C#, Razor, Bootstrap, Entity Framework, and SQL Server**, following the **MVC pattern**. It allows customers to browse and purchase floor tiles while enabling administrators to manage products, orders, and users efficiently.

---

## 🚀 Features  

### **For Customers**  
✅ Browse & filter tiles by color, size, material, and price  
✅ View detailed product descriptions with high-resolution images  
✅ Add tiles to the shopping cart and complete purchases securely  
✅ User authentication (Register, Login, Profile Management)  
✅ Responsive design using Bootstrap for an optimal experience across devices  

### **For Administrators**  
✅ Manage tile products (Add, Update, Delete)  
✅ Process and track customer orders  
✅ Manage user roles and access control  
✅ View sales reports and analytics  

---

## 🛠️ Tech Stack  

### **Frontend:**  
- **ASP.NET Razor Pages & MVC** – Dynamic UI components  
- **Bootstrap 5** – Ensures a modern and responsive UI  
- **jQuery & AJAX** – Improves interactivity  

### **Backend:**  
- **ASP.NET 8 (MVC Pattern)** – Modular & maintainable architecture  
- **C#** – Object-oriented programming for business logic  
- **Entity Framework Core** – ORM for seamless database operations  
- **SQL Server** – Robust relational database  

---

## 📦 Installation & Setup  

### **1️⃣ Prerequisites**  
- Install **.NET 8 SDK**  
- Install **SQL Server** & **SQL Server Management Studio (SSMS)**  
- Install **Visual Studio 2022+** (with ASP.NET & Web Development workload)  

### **2️⃣ Clone the Repository**
    git clone https://github.com/GAGAN4980/BuyTilez.git
    cd BuyTilez

### **3️⃣ Configure Database Connection**
    Open appsettings.json and update the SQL Server connection string:
    "ConnectionStrings": { 
        "DefaultConnection": "Server=YOUR_SERVER;Database=BuyTilezDB;Trusted_Connection=True;"
    }

### **4️⃣ Run Database Migrations**
    dotnet ef database update

### **5️⃣ Run the Application**
    dotnet run
Open https://localhost:5001 in the browser.

---

## 💡 Contributing
If you’d like to, your contributions are welcome:
1. Fork this repository
2. Create a feature branch: git checkout -b feature-branch-name
3. Commit your changes: git commit -m "Add feature"
4. Push to your fork: git push origin feature-branch-name
5. Open a Pull Request

---

## 📜 License
This project is licensed under the MIT License.
