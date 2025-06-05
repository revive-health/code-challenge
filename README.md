## 🧪 Member Medication API – Coding Exercise

### 📋 Overview

This codebase provides a starting point for a basic API to manage members and their prescribed medications. You’ll build upon it by implementing a few real-world features to demonstrate your understanding of:

* RESTful API design
* DTO usage and validation
* Query/filter logic
* Status code correctness
* Working with relationships in Entity Framework

The API is built with **.NET 9**, **Entity Framework Core**, and uses a **SQL Server container** (via `docker-compose`). Swagger is enabled for easy local testing.

---

### 🚀 Setup Instructions

1. Make sure you have **Docker**, **.NET 9 SDK**, and an editor like **Visual Studio** or **VS Code** installed.
2. Run the following to start the database:

```bash
docker-compose up -d
```

3. Start the API:

```bash
dotnet run --project src/CodeChallenge.Api/CodeChallenge.Api.csproj
```

4. Open the API docs at `http://localhost:5062/api-doc` (or the port you see in the console).

---

### ⏱️ Your Task (2 Hours)

Complete the following features:

---

#### 🧾 1. Add Notes to Medications

Add a `Notes` field to the `Medication` model to store any extra prescription information.

* Accept `Notes` as part of the POST `/members/{id}/medications` request.
* Ensure it is stored and returned in responses.

---

#### 📅 2. Filter Medications by Date Range

Enhance the endpoint:

```
GET /members/{memberId}/medications
```

Add optional query parameters:

* `fromDate` (inclusive)
* `toDate` (inclusive)

Only return medications where `PrescribedDate` is within the given range.

---

#### 🧠 3. Member Summary Endpoint

Create a new endpoint:

```
GET /members/{id}/summary
```

Return:

```json
{
  "fullName": "John Doe",
  "age": 42,
  "medicationCount": 3
}
```

> Age should be calculated from `DateOfBirth`.

---

### ✅ Bonus (if you have time)

Implement the following additional feature:

#### 🔍 Search Members by Medication Name

Create an endpoint:

```
GET /medications/search?name=aspirin
```

Return a list of members who have been prescribed a medication with the given name (case-insensitive).

---

### ✅ Requirements

* Follow RESTful conventions.
* Return proper HTTP status codes.
* Use DTOs for request bodies where appropriate.
* Use async/await throughout.
* Aim for clean, readable code.
