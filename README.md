
# Habit Logger

 This is an application where you’ll register one habit. This habit can't be tracked by time (ex. hours of sleep), only by quantity (ex. number of water glasses a day)

## Given Requirements

- When the application starts, it should create a sqlite database, if one isn’t present.
- It should also create a table in the database, where the habit will be logged.
-  The app should show the user a menu of options.

-   The users should be able to insert, delete, update and view their logged habit.
- You should handle all possible errors so that the application never crashes.
- The application should only be terminated when the user inserts 0.
-   You can only interact with the database using raw SQL. You can’t use mappers such as Entity Framework.



## Features

- SQLite database connection

    - The program uses a SQLite db connection to store and read information
    - If no database exists, or the correct table does not exist they will be created on program start.

- A console based UI where users can navigate by key presses

- CRUD DB functions
    - From the main menu users can Create, Read, Update or Delete entries for whichever date they want, entered in mm-DD-yyyy format. Duplicate days will not be inputted.
    - Time and Dates inputted are checked to make sure they are in the correct and realistic format.
