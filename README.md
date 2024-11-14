# University Query Management System (UQMS) - Admin Dashboard

## Table of Contents
- [Introduction](#introduction)
- [Roles and Permissions](#roles-and-permissions)
  - [Admin](#admin)
  - [Staff](#staff)
- [Features](#features)
  - [Admin Features](#admin-features)
  - [Staff Features](#staff-features)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
  - [Running the Application](#running-the-application)
- [Technologies Used](#technologies-used)
- [Screenshots](#screenshots)
- [Contributing](#contributing)
- [License](#license)
- [Contact](#contact)

## Introduction
Welcome to the University Query Management System (UQMS) Admin Dashboard! This web-based application is designed to streamline the management of user accounts, queries, and system activities within a university setting. The Admin Dashboard provides comprehensive tools for administrators and staff to efficiently oversee and handle various aspects of the system, ensuring a smooth and effective user experience.

## Roles and Permissions
UQMS differentiates between two primary roles: Admin and Staff. Each role has distinct permissions and access levels to ensure secure and efficient management of the system.

### Admin
Admins possess the highest level of access within the UQMS. Their responsibilities include overseeing user accounts, monitoring system activities, and managing queries.

**Key Permissions:**
- **User Management:**
  - Create Users: Add new users (Students, Staff, Admins) to the system.
  - Read Users: View detailed information of all users.
  - Update Users: Modify user details, including roles and statuses.
  - Delete Users: Remove users from the system as necessary.
- **Query Management:**
  - View Queries: Access all submitted queries.
  - Assign Queries: Allocate queries to appropriate staff members.
  - Update Query Status: Mark queries as resolved or pending.
- **System Activity Monitoring:**
  - View Activity Logs: Monitor recent activities within the system, including user actions and query resolutions.
- **Team Management:**
  - Manage Teams: Oversee staff assignments and team compositions.
- **Statistics and Reporting:**
  - View Statistics: Access comprehensive statistics on user activity, query statuses, and more.
  - Generate Reports: Create detailed reports based on system data for administrative review.

### Staff
Staff members assist in managing queries and supporting the operational aspects of the UQMS. Their access is tailored to facilitate effective query handling and system support.

**Key Permissions:**
- **Query Management:**
  - View Assigned Queries: Access queries that have been assigned to them.
  - Update Query Status: Mark queries as resolved or pending based on their actions.
- **System Activity Monitoring:**
  - View Activity Logs: Monitor recent activities related to their assigned queries.
- **Statistics and Reporting:**
  - View Personal Statistics: Access statistics related to their performance and query handling.

*Note: Staff members do not have access to user management functionalities to maintain system security and integrity.*

## Features
### Admin Features
- **User Management**
  - Add New Users: Administrators can create new user accounts for students, staff, and other admins.
  - Edit User Details: Modify existing user information, including roles and permissions.
  - Delete Users: Remove user accounts from the system when necessary.
  - Search and Sort Users: Easily find and organize user accounts based on various criteria.
  
- **Query Statistics Display**
  - Total Users: View the total number of users registered in the system.
  - Active Users: Monitor the number of users currently active.
  - Total Queries: Track the total number of queries submitted.
  - Resolved Queries: Keep a count of queries that have been successfully addressed.
  - Pending Queries: Identify queries that are still awaiting resolution.

- **User Management Overview**
  - Role-Based User Counts: See the distribution of users across different roles (e.g., Students, Staff, Admins).

- **System Activity Logs**
  - Recent Activities: Monitor the latest actions performed within the system, including user logins, query submissions, and resolutions.

- **Team Overview Section**
  - Staff Assignments: View and manage the allocation of staff members to different query types and departments.

- **Query Heatmap**
  - Departmental Load: Visualize the distribution of queries across various departments to identify high-load areas.

### Staff Features
- **View Assigned Queries**
  - Access Queries: Staff members can view all queries that have been assigned to them for resolution.

- **Update Query Status**
  - Mark as Resolved/Pending: Change the status of queries based on their handling progress.

- **System Activity Logs**
  - Monitor Activities: Keep track of actions related to their assigned queries to ensure transparency and accountability.

- **Personal Statistics**
  - Performance Metrics: View statistics related to their query handling efficiency and workload.

## Getting Started
### Prerequisites
- .NET Core SDK: Ensure you have the latest version of the .NET Core SDK installed.
- Database: Set up the required database (e.g., SQL Server, PostgreSQL) as per the project configuration.
- Node.js & npm: For managing front-end dependencies, if applicable.

### Installation
1. Clone the Repository:
   ```bash
   git clone https://github.com/yourusername/UQMS-AdminDashboard.git
