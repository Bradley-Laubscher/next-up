# 🎮 Next Up

Next Up is a .NET 9 MVC web application designed to help gamers track new releases, upcoming titles, and maintain a personalized game backlog. It features integration with the IGDB and Steam APIs to provide real-time game data, discounts, and expansion notifications.

## 🌐 Live Demo

Deployed on [Render](https://next-up-8qce.onrender.com/). Note that free-tier apps can spin down when idle, but GitHub Actions are used to keep the app warm during business hours.

---

## 🚀 Features

- User authentication via ASP.NET Core Identity
- Add, view, and remove games from a personal backlog ("My List")
- View new releases and upcoming games from IGDB
- View Steam discount info and upcoming expansions for tracked games
- Scheduled ping system using GitHub Actions to prevent cold starts

---

## 🛠️ Tech Stack

- **Framework:** .NET 9 (ASP.NET Core MVC + Razor Pages)
- **Database:** PostgreSQL (via Npgsql)
- **ORM:** Entity Framework Core
- **Auth:** ASP.NET Core Identity
- **Frontend:** Razor Views + Bootstrap (optional)
- **APIs:** IGDB, Steam Store
- **Hosting:** Render
- **Background Jobs:** GitHub Actions (pinging for warm-up)

---