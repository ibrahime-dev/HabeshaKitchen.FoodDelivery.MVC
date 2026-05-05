# 🍽️ Habesha Kitchen — Online Food Delivery System

## 📌 Overview

**Habesha Kitchen** is an event-driven online food delivery system built using **ASP.NET MVC 5 (.NET Framework 4.8)**. The platform allows customers to browse Ethiopian cuisine, place orders, and submit payment references, while restaurant owners manage menus and orders, and administrators oversee the system.

---

## 🚀 Features

### 👤 Guest (Unauthenticated User)

* Browse menu items by category (Tibs, Injera & Wot, Drinks)
* View food images, spice levels, and badges
* Register and log in
* Submit contact inquiries

### 🛒 Customer

* Add, update, and remove items from cart
* Real-time cart total calculation
* Checkout with Ethiopian payment methods:

  * Telebirr
  * CBE Birr
  * Dashen Bank
  * Amhara Bank
* Submit transaction reference for payment
* Track order status
* View order history
* Cancel eligible orders

### 🍳 Restaurant Owner

* Create and manage restaurant profile
* Add/edit/delete menu items with images
* Toggle item availability
* View and manage incoming orders
* Verify payment references manually
* Update order status:

  * Pending → Confirmed → Preparing → Delivered
* Reject invalid orders

### 🛠️ Administrator

* View system dashboard (users, orders, menu items)
* Manage user accounts (فعيل / deactivate)
* Create Restaurant Owner accounts
* View all system orders

---

## 🔄 Order Workflow

```
Customer Places Order → Pending
        ↓
Owner Verifies Payment
        ↓
   Confirmed
        ↓
   Preparing
        ↓
   Delivered
```

**Alternative flows:**

* ❌ Rejected (by Owner)
* ❌ Cancelled (by Customer before delivery)

---

## 💳 Payment Methods

| Method      | Account Number | Type         |
| ----------- | -------------- | ------------ |
| Telebirr    | 0912 345 678   | Mobile Money |
| CBE Birr    | 1000 4567 8901 | Bank         |
| Dashen Bank | 0234 5678 9012 | Bank         |
| Amhara Bank | 0345 6789 0123 | Bank         |

---

## 🧰 Technology Stack

| Layer          | Technology                         |
| -------------- | ---------------------------------- |
| Framework      | ASP.NET MVC 5 (.NET Framework 4.8) |
| Language       | C#                                 |
| Database       | SQL Server                         |
| ORM            | Entity Framework 6 (Code First)    |
| Authentication | ASP.NET Identity                   |
| Frontend       | Bootstrap 5, jQuery                |
| Validation     | jQuery Validate                    |
| Currency       | Ethiopian Birr (ETB)               |

---

## 🍲 Menu Categories

### Injera & Wot

* Doro Wot, Sega Wot, Shiro Wot, Misir Wot
* Gomen, Atkilt Wot, Special, Extra Injera

### Tibs

* Beef Tibs, Doro Tibs, Lamb Tibs
* Kitfo, Gored Gored, Dulet, Lebleb Tibs

### Drinks

* Ethiopian Coffee (Buna), Macchiato, Tea
* Soft Drinks, Fresh Juice
* Mango, Avocado, Spris Juice

---

## 👥 Actors

* **Guest:** Browses and registers
* **Customer:** Places and manages orders
* **Restaurant Owner:** Manages menu and orders
* **Administrator:** Controls system and users

---

## 🔐 Default Accounts

| Role             | Email                                                   | Password     |
| ---------------- | ------------------------------------------------------- | ------------ |
| Administrator    | [admin@fooddelivery.com](mailto:admin@fooddelivery.com) | Admin@123456 |
| Restaurant Owner | [owner@fooddelivery.com](mailto:owner@fooddelivery.com) | Owner@123456 |

---

## ⚙️ Setup Instructions

1. Clone the repository
2. Open in Visual Studio
3. Restore NuGet packages
4. Update connection string in `Web.config`
5. Run database migrations
6. Start the application

---

## 📌 Notes

* Payments are **manually verified** using transaction references
* Designed using **event-driven programming principles**
* Supports **role-based authentication and authorization**

---

## 📷 Future Improvements

* Online payment integration (API-based)
* Mobile app version
* Real-time order tracking (SignalR)
* Delivery personnel module

---

## 📄 License

This project is for educational purposes.

---

✨ *Habesha Kitchen brings Ethiopian flavors to your fingertips.*
