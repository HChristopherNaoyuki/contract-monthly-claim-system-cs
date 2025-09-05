**Contract Monthly Claim System (CMMS) Documentation**

The Contract Monthly Claim System is a comprehensive web-based 
application developed using ASP.NET Core MVC framework. This 
system streamlines the monthly claim submission and approval 
process for independent contractor lecturers through an intuitive, 
role-based platform.

The system architecture follows the Model-View-Controller pattern 
with clear separation of concerns. Key components include authentication 
controllers, claim management modules, and responsive view templates. Three 
user roles are supported: Lecturers can submit claims with supporting documentation, 
Programme Coordinators verify submission details, and Academic Managers make final 
approval decisions.

Core functionalities include automated amount calculation based on hours worked and 
hourly rates, document upload capabilities with file validation, and real-time status 
tracking throughout the approval workflow. The system features a minimalist interface 
design with responsive layouts that adapt to desktop and mobile devices.

Technical implementation utilizes session-based authentication, client-side validation, 
and in-memory data storage for the prototype phase. The application is built with C# 7.0 
following Allman style coding conventions and includes comprehensive documentation for 
future development and potential production deployment.

Security measures include role-based access control, input validation, and protected 
session management. The system provides a solid foundation for expansion including database 
integration, enhanced reporting features, and additional administrative functionalities.