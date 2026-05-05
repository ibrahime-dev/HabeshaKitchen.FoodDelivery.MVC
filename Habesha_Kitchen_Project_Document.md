# Group Project: Event-Driven Programming
## Project Title: Habesha Kitchen — Online Food Delivery System

---

## 1. Functional Requirements

---

### 1.1 Guest (Unauthenticated User)

| Event | System Response |
|---|---|
| User visits the home page | System displays all available menu items grouped by category with images, spice levels, and badges |
| User filters menu by category | System shows only items matching the selected category (Tibs, Injera & Wot, Drinks) |
| User clicks Register | System presents a registration form and creates a Customer account on submission |
| User clicks Login | System authenticates credentials and redirects to the appropriate dashboard based on role |
| User submits the Contact form | System validates input and displays a success confirmation message |

---

### 1.2 Customer

| Event | System Response |
|---|---|
| Customer clicks "Add to Cart" on a menu item | System adds the item to the customer's persistent cart (creates cart if none exists) |
| Customer updates item quantity in cart | System recalculates subtotals and total live in real time without page reload |
| Customer removes an item from cart | System deletes the cart item and refreshes the cart view |
| Customer clears the cart | System removes all items from the cart |
| Customer proceeds to checkout | System displays cart summary, delivery address form, and payment options |
| Customer selects a payment method | System displays the account name, account number, and exact amount to send for the selected method (Telebirr, CBE Birr, Dashen Bank, Amhara Bank) |
| Customer enters transaction reference and places order | System creates the Order, records the payment method and transaction reference, clears the cart, and redirects to confirmation |
| Customer views order confirmation | System displays order ID, items, total amount in ETB, delivery address, payment method, and status |
| Customer views order history | System lists all past orders with status, date, and total in ETB |
| Customer cancels a Pending or Confirmed order | System updates order status to Cancelled |
| Customer attempts to cancel a Delivered or Rejected order | System rejects the cancellation and displays an error message |
| Customer logs out | System ends the session and redirects to the home page |

---

### 1.3 Restaurant Owner

| Event | System Response |
|---|---|
| Owner logs in for the first time (no restaurant) | System redirects to the Create Restaurant form |
| Owner submits the Create Restaurant form | System registers the restaurant profile linked to the owner's account |
| Owner edits restaurant details | System validates input and saves updated name, description, cuisine category, and phone number |
| Owner opens the Menu Management page | System lists all menu items ordered by category with prices in ETB |
| Owner creates a new menu item | System validates the form, saves the item with optional image upload, and adds it to the menu |
| Owner uploads an image for a menu item | System saves the file to the server and stores the image URL; image displays correctly on the home page |
| Owner edits a menu item | System updates the item details; replaces image only if a new one is provided |
| Owner deletes a menu item with no order history | System permanently removes the item |
| Owner deletes a menu item linked to existing orders | System marks the item as unavailable instead of deleting it |
| Owner toggles item availability | System flips the availability flag and reflects the change on the public menu |
| Owner views the Dashboard | System displays all incoming orders with customer name, date, total in ETB, payment method, transaction reference, and current status |
| Owner verifies transaction reference | Owner checks the reference number against their Telebirr/CBE app to confirm payment |
| Owner filters orders by status | System returns only orders matching the selected status |
| Owner advances an order to the next stage | System validates the transition and updates the status (Pending → Confirmed → Preparing → Delivered) |
| Owner rejects a Pending order | System sets the order status to Rejected |
| Owner attempts an invalid status transition | System blocks the change and displays an error message |

---

### 1.4 Administrator

| Event | System Response |
|---|---|
| Admin logs in | System redirects to the Admin Dashboard |
| Admin views the Dashboard | System displays total users, total orders, and total menu items |
| Admin views the Users list | System lists all users with email, full name, role, and active status |
| Admin toggles a user's active status | System activates or deactivates the user account |
| Admin creates a new Restaurant Owner account | System registers the user, assigns the Restaurant_Owner role, and confirms creation |
| Admin views all orders | System lists every order with customer email, date, total in ETB, and status |

---

## 2. Actor Identification

---

### Actor 1: Guest (Unauthenticated Visitor)
- **Role:** Public user with read-only access
- **Interactions:**
  - Browses the restaurant menu with category filtering
  - Views menu item images, spice levels, and popular/bestseller badges
  - Registers for a new Customer account
  - Logs in to an existing account

---

### Actor 2: Customer
- **Role:** Registered user who places food orders
- **Interactions:**
  - Manages a personal shopping cart with live price calculation
  - Selects Ethiopian payment method and views account details to send payment
  - Enters transaction reference number to confirm payment
  - Tracks order status from Pending through to Delivered
  - Cancels orders that have not yet been delivered
  - Views full order history with ETB pricing

---

### Actor 3: Restaurant Owner
- **Role:** Manages the restaurant, its menu, and incoming orders
- **Interactions:**
  - Creates and edits the restaurant profile
  - Adds, edits, deletes, and toggles availability of menu items with image upload
  - Views incoming orders with payment method and transaction reference
  - Manually verifies customer payments via Telebirr/CBE/Dashen/Amhara Bank
  - Progresses orders: Pending → Confirmed → Preparing → Delivered
  - Rejects unverified or invalid orders

---

### Actor 4: Administrator
- **Role:** System-level manager with full oversight
- **Interactions:**
  - Monitors system-wide statistics
  - Views and manages all user accounts
  - Activates or deactivates user accounts
  - Creates new Restaurant Owner accounts
  - Views all orders across the system

---

## 3. Order Status Event Flow

```
[Customer Places Order + Transaction Ref] → Pending
        ↓                                      ↓
[Owner Verifies Payment]              [Owner Rejects] → Rejected (terminal)
        ↓
   Confirmed
        ↓                    ↓
  [Owner Prepares]    [Customer Cancels] → Cancelled (terminal)
        ↓
   Preparing
        ↓
  [Owner Delivers]
        ↓
   Delivered (terminal)
```

---

## 4. Payment Methods Supported

| Method | Account Number | Type |
|---|---|---|
| Telebirr | 0912 345 678 | Mobile Money |
| CBE Birr | 1000 4567 8901 | Bank |
| Dashen Bank | 0234 5678 9012 | Bank |
| Amhara Bank | 0345 6789 0123 | Bank |

---

## 5. Technology Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET MVC 5 (.NET Framework 4.8) |
| Language | C# |
| Database | SQL Server (Entity Framework 6, Code First) |
| Authentication | ASP.NET Identity (cookie-based, role-based) |
| Frontend | Bootstrap 5, jQuery, jQuery Validate |
| ORM | Entity Framework 6 with migrations |
| Currency | Ethiopian Birr (ETB) |
| Payment | Manual bank transfer with transaction reference verification |

---

## 6. Menu Categories

| Category | Items |
|---|---|
| Injera & Wot | Extra Injera, Doro Wot, Sega Wot, Shiro Wot, Misir Wot, Gomen, Atkilt Wot, Special |
| Tibs | Beef Tibs, Kitfo, Doro Tibs, Lamb Tibs, Lamb Tibs Awaze, Gored Gored, Dulet, Lebleb Tibs |
| Drinks | Ethiopian Coffee (Buna), Macchiato, Tea (Shai), Soft Drinks, Fresh Juice, Avocado Juice, Mango Juice, Spris Juice |

---

## 7. Default Accounts

| Role | Email | Password |
|---|---|---|
| Administrator | admin@fooddelivery.com | Admin@123456 |
| Restaurant Owner | owner@fooddelivery.com | Owner@123456 |
