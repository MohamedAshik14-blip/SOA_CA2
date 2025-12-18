College Events Management System

A full-stack College Events Management System built using ASP.NET Core Web API, Blazor WebAssembly, and Entity Framework Core, deployed on Fly.io.
The system allows students to register for events, check in/out, while administrators manage events, venues, categories, and users.

⸻

Live Deployment Links

Blazor Frontend:https://collegeeventsblazor.fly.dev
ASP.NET Core API:https://collegeeventsapi.fly.dev
Swagger API Docs:https://collegeeventsapi.fly.dev/swagger

⸻

User Roles

Student
	•	Login/Register
	•	View events
	•	Register / withdraw from events
	•	Check-in and check-out
  •	Filter/Search/Sort Events
  

Admin
	•	Create / Read/ update / delete:
	•	Events
	•	Venues
	•	Categories
	•	Students
	•	Reset student passwords
	•	View registrations
  •	Filter/Search/Sort Events/Students
  
⸻

Technologies Used

Backend
	•	ASP.NET Core 8 Web API
	•	Entity Framework Core
	•	JWT Authentication
	•	xUnit (Unit Testing)

Frontend
	•	Blazor WebAssembly
	•	Bootstrap 5
	•	Custom CSS

DevOps / Tools
	•	Fly.io (Deployment)
	•	Docker
	•	Postman (API testing)
	•	Swagger
	•	GitHub

⸻

Security Features
	•	JWT-based authentication
	•	Role-based authorization
	•	Secure password hashing
	•	Admin-only endpoints protected via [Authorize(Roles="Admin")]

⸻

References
	1.	IAmTimCorey – ASP.NET Core Web API Tutorial
https://www.youtube.com/watch?v=ZXdFisA_hOY
	2.	Patrick God – JWT Authentication in ASP.NET Core
https://www.youtube.com/watch?v=7nafaH9SddU
	3.	freeCodeCamp – Blazor WebAssembly Full Course
https://www.youtube.com/watch?v=4G_BzLxa9NQ
	4.	Nick Chapsas – Role-Based Authorization in ASP.NET Core
https://www.youtube.com/watch?v=4J6Rr8C0_0Q
	5.	Les Jackson – Entity Framework Core Relationships
https://www.youtube.com/watch?v=EFgZVf7GLM0
	6.	IAmTimCorey – Unit Testing in .NET
https://www.youtube.com/watch?v=HYrXogLj7vg
	7.	GitHub – Custom Authentication Handler for Testing
https://github.com/dotnet/AspNetCore.Docs/issues/19351
	8.	Valentin Despa – Postman Beginner to Advanced
https://www.youtube.com/watch?v=VywxIQ2ZXw4
	9.	Stack Overflow – What is HTTP 405 Method Not Allowed
https://stackoverflow.com/questions/15477856/what-is-http-405-method-not-allowed
	10.	Stack Overflow – Blazor CSS Not Loading
https://stackoverflow.com/questions/62440224/blazor-css-not-loading



