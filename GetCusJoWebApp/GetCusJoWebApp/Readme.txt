This application is created in .net core 2.2
The code includes dabase migration. Steps mentioned in configuration section below explains how to execute migration and update database.

Endpoints:

# |Page				| Route				|  Description
1) Register user -> /account/register   => Use this page to register a user
2) Login ->			/account/login		=> Use this page to login to the application
3) Visitor Page ->	/visitor			=> anyone should be able to view this page
4) Member Page ->	/member				=> any user who is logged in should be able to view this page
5) Elite Page ->	/elite				=> any user who belongs to either 'elite' or 'admin' role should be able to view this page
6) Admin Page ->	/admin				=> admin home page, can be viewd only by admin role
7) Create Role ->	/role/create		=> Create a role by entering the role name on the text box and then click the create button
8) List Users and Roles  /list/usersroles => Lists all available users and roles side by side
9) Assign roles to user  /list/usersroles => Enter the user id in user text box and role names seperated by comma in Roles text box.

Configurations:

1) In SQL Server create a database named 'GetCusJo'
2) Open appsettings.json file, go to ConnectionStrings -> DefaultConnectionUpdate and update the connection string
3) Execute the migration scripts.
	-> Go to nuget package manager console and type "AddMigration InitialCreate". It adds the migration script
	-> Then type Update-Database

Typical flow to create users:

1) Comment the section [Authorize(Roles = "Admin")] on Controllers/AdminController.cs.
	This is required to allow the creation of admin User and Role. 
	Alternatively it can be done by inserting the records directly into the database.
	But for simplicity, lets do it from the UI by commenting the above mentioned portion and un comment it in a later step.
2) Go to Register user page and create a user "Admin" and enter password/ confirm password (eg: Pass@word1)
3) Go to create Role page and create a new role named "Admin"
4) Go to assign roles page and assign "Admin" role to "Admin" user. Remember, the user name should be the guid and role should be the role name (not id)
5) Now we can uncomment the section commented in step 1.
6) Go ahead and create the other users and roles ("Member" user, "Member" role", "Elite" user, "Elite" role)
7) Assign roles to corresponding users

Actions:

1) Visitor (unauthenticated user) will have access to only Visitor page and not to any other pages like member, visitor or admin
2) Member user will have access to only the Visitor page and Member page
3) Elite user will have access to Visitor, Member, Elite pages and not Admin pages
4) Admin user will have access to all the pages