# Inventory management

An `inventory management` project with angular and .net core apis.

## When have I started it?

I have built this project in **early 2024** with `.net 8 ` and `angular 17`. I had created separate github repositories for front-end and backend. Now, I have moved them to a single repository. I am also upgrading them to latest version (as of Aug,2025), I am planning to add few more features like login.

## Tech Stack

- **Backend:** Asp.net core web api (9.0)
- **Database:** Microsoft SQL Server
- **ORM:** Dapper
- **Frontend:** Angular 20
- **UI:** Angular material (UI component library)
- Ngrx componnent store (state management)

## How to run the project in dev environment

- Make sure to install dotnet 9 sdk,latest node js and angular cli.
- **Clone this project:** Open terminal and execute ``
- `cd `
- With command `code .`, your project will be opened in VS Code.
- You need to run both projects separately:

### A. Backend

1. To work with this project, you must execute this [script](./database/db.sql) in your sql server database.
2. Open `appsettings.json` and configure the connection string according to your database. `server= {my_server_name}` to `{your_server_name}`. At this stage, you must know how to configure the connection string.
3. Open `InventoryMgt.Api` in the integrated terminal.
4. Execute the command `dotnet run`, to run this project.
5. Keep this application running in that terminal.

### B. Front-end

1. Open the another integrated terminal (keep the terminal open, where your backend project is running).
2. Execute the command `npm i` to install all the dependencies.
3. Execute `ng serve --open` to run and open this project in browser.

## Screenshots

--- 

🙂 Github star is appreciated.
