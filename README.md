# 🍽️ OrderingApp

System obsługi kelnerskiej (Waiter Assistance System) stworzony w **C# .NET** oraz **Next.js (React + TypeScript)**.  
Aplikacja umożliwia kelnerom szybkie przyjmowanie zamówień, przypisywanie ich do stolików oraz zarządzanie nimi w czasie rzeczywistym.  
Kucharze otrzymują zamówienia w formie wydruków z drukarki sieciowej.

---

## 🧩 Technologie

### 🖥️ Backend
- **ASP.NET Core (.NET 8)**
- **Entity Framework Core**
- **AutoMapper**
- **Keycloak** – autoryzacja i uwierzytelnianie
- **xUnit**, **Moq**, **FluentAssertions** – testy jednostkowe
- **Clean Architecture** z podziałem na warstwy: `Domain`, `Application`, `Infrastructure`, `API`

### 💻 Frontend
- **Next.js** – framework do Reacta (SSG/SSR)
- **TypeScript** – typowanie i bezpieczeństwo kodu
- **Tailwind CSS** – szybkie i nowoczesne stylowanie
- **React Hook Form** – obsługa formularzy
- **Zod** – walidacja danych formularzy
- **TanStack Query (React Query)** – obsługa zapytań HTTP i cache
- **Framer Motion** – animacje interfejsu
- **Storybook** – dokumentacja komponentów UI
- **Vitest + Cypress** – testy jednostkowe i e2e
- **ESLint + Prettier** – lintowanie i formatowanie kodu

---

## 🧠 Opis działania

OrderingApp umożliwia:
- tworzenie i zarządzanie zamówieniami przypisanymi do stolików 🍷  
- dodawanie pozycji z menu oraz składników 🥗  
- przeglądanie historii zamówień i rachunków 💰  
- wysyłanie zamówień do kuchni przez sieć Wi-Fi 🖨️  
- szybkie rozliczanie pojedynczych osób przy stole 💳  

Kucharze nie korzystają z aplikacji — otrzymują wydruki zamówień automatycznie.

---

## 🧱 Architektura projektu

### Backend (C# .NET)
- `Domain` – encje i logika biznesowa  
- `Infrastructure.Database` – konfiguracja EF Core i seedy  
- `Application` – logika aplikacyjna, DTO, mapowanie  
- `API` – REST API obsługujące komunikację z frontendem  

### Frontend (Next.js)
- `pages` – routy i SSR/SSG  
- `components` – komponenty UI  
- `hooks` – niestandardowe hooki  
- `lib` – konfiguracje i utils  
- `styles` – globalne style i motywy  

---

## 🧪 Testy

### Backend:
- Testy jednostkowe dla serwisów (`AreaService`, `MenuItemService`, `OrderService`, itd.)
- Technologie: **xUnit**, **Moq**, **FluentAssertions**

### Frontend:
- **Vitest** – testy jednostkowe komponentów  
- **Cypress** – testy end-to-end UI  

---

👥 Zespół projektowy
| Imię i nazwisko     | Rola                                                                               | Opis                                                                                                   |
| ------------------- | ---------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------ |
| **Jakub Owczarek**  | 💼 Business Owner / 🧠 Pomysłodawca / 👨‍💻 Backend Developer / 👥 Project Manager | Twórca koncepcji systemu, zarządzanie zespołem i spotkaniami, architektura backendu i logika biznesowa |
| **Patryk Sąsiadek** | 💻 Frontend Developer                                                              | Tworzenie interfejsu użytkownika, implementacja logiki frontendu i komunikacji z API                   |
| **Konrad Depowski** | 💻 Frontend Developer                                                              | Współtworzenie warstwy wizualnej, komponenty Next.js i integracja z Tailwind CSS                       |
| **Łukasz Owczarek** | 🎨 Grafik / UI Designer                                                            | Projektowanie layoutów, kolorystyki, ikonografii i elementów wizualnych aplikacji                      |


