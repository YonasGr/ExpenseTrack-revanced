# Interface and Operations Visuals

The Expense Tracker provides a clean and intuitive interface for managing daily expenses. Below are the visual and logical breakdowns of the application's key components and operations.

## 1. Main Interface Layout
The application features a single-window interface designed for efficiency:
- **Header**: A professional dark blue banner titled "EXPENSE TRACKER SYSTEM".
- **Input Section (Left)**: A dedicated panel for entering expense details, including Amount, Category, Description, and Date.
- **Action Controls**: Color-coded buttons for primary tasks:
    - **Add** (Green): Record a new expense.
    - **Update** (Blue): Modify an existing entry.
    - **Delete** (Orange): Remove an entry.
    - **Clear** (Gray): Reset the input fields.
- **Data Display (Right)**:
    - **Search**: A quick-access search bar to filter records.
    - **Grid**: A comprehensive list showing all recorded expenses with ID, Amount, Category, Description, and Date.
    - **Summary**: Real-time calculation of the total expenditure.

![Main Interface](screenshots/main_interface.png)

---

## 2. Database Operations (CRUD)

### 2.1 Add Operation
Allows users to record new financial transactions.
- **Process**: Enter details -> Click **Add**.
- **Outcome**: The transaction is saved and displayed in the grid.
![Add Operation](screenshots/add_operation.png)

### 2.2 Update Operation
Enables modification of previously recorded transactions.
- **Process**: Select record -> Modify details -> Click **Update**.
- **Outcome**: The record is updated in the local storage and the display.
![Update Operation](screenshots/update_operation.png)

### 2.3 Delete Operation
Removes unnecessary or incorrect records.
- **Process**: Select record -> Click **Delete** -> Confirm.
- **Outcome**: The record is permanently removed from the system.
![Delete Operation](screenshots/delete_operation.png)

### 2.4 Search and Filter
Quickly locate specific transactions.
- **Process**: Enter keyword/amount -> Click **Search**.
- **Outcome**: The grid displays only matching records.
![Search Operation](screenshots/search_operation.png)
