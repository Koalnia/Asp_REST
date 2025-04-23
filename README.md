# Asp_REST
Aplikacja Asp_REST, będąca projektem aplikacji serwerowej, implementującej API zgodne z architekturą REST we frameworku Asp.Net Core.
Jest to przykład wykorzystania frameworku Asp.Net Core jako frameworku tworzenia backendu aplikacji internetowych, napisany na potrzeby pracy dyplomowej.
Backend aplikacji napisany jest w Asp.Net Core, a wybrana baza danych dla projektu to PostgreSQL.
Aplikacja tworzy serwis ogłoszeń, gdzie użytkownicy mogą zamieszczać ogłoszenia o chęci zakupu, sprzedaży lub darmowego oddania określonych przedmiotów lub usług. 
Zawiera dwie klasy - klasę User opisującą użytkowników aplikacji oraz klasę Advertisement opisującą ogłoszenia aplikacji w relacji  jeden-do-wielu (ang. One-to-Many), gdzie:
•	jeden użytkownik (User) może mieć wiele ogłoszeń (Advertisement);
•	każde ogłoszenie (Advertisement) należy do jednego użytkownika (User).
Klasa użytkowników przechowuje identyfikator użytkownika (ang. id), jego imię i nazwisko, numer telefonu, adres email oraz hasło logowania, przechowywane w postaci hasz. 
Każde ogłoszenie przechowuje jego identyfikator (id), tytuł, opis, koszt, czas trwania oraz termin jego utworzenia. 
Użytkownicy dzielą się na dwie role - zwykłych użytkowników o roli USER oraz administratorów o roli ADMIN. Zwykli użytkownicy mogą edytować dane tylko swojego konta oraz swoich ogłoszeń (oraz je usuwać). 
Administrator ma dostęp do edycji oraz usuwania wszystkich kont użytkowników oraz ogłoszeń. Nie może on jednak usunąć własnego konta. 
W celu weryfikacji, każdy nowo utworzony użytkownik musi zweryfikować się poprzez kod wysłany na jego adres email. Ta funkcjonalność wymaga połączenia projektu z pocztą email programisty. 
Autoryzacja i autentykacja w aplikacji przeprowadzana jest przez tokeny JWT. 
Autoryzacja i autentykacja w aplikacji przeprowadzana jest przez ciasteczka HTTP. 
Aplikacja wczytuje domyślne wartości do bazy danych, zawierające trzech użytkowników administratorów, siedmiu zwykłych użytkowników oraz trzydzieści ogłoszeń. 
Aplikacja ustawiona jest, żeby sama usuwała i generowała od nowa strukturę bazy danych oraz jej dane przy każdym uruchomieniu. 
W celu zarządzania pakietami projektów Asp.Net Core używa narzędzia NuGet.
Aplikacja zawiera także testy wydajnościowe napisane w technologii k6.
#ENG
Asp_REST
The Asp_REST application is a server application project implementing an API compliant with the REST architecture in the Asp.Net Core framework.
It serves as an example of using the Asp.Net Core framework as a backend for web applications, written for a diploma thesis.
The application's backend is written in Asp.Net Core, and the selected database for the project is PostgreSQL.

The application creates an advertisement service where users can post ads about buying, selling, or giving away specific items or services for free.
It contains two classes - the User class describing application users and the Advertisement class describing application advertisements in a One-to-Many relationship, where:
- one user (User) can have many advertisements (Advertisement);
- each advertisement (Advertisement) belongs to one user (User).

The User class stores the user's identifier (id), their first and last name, phone number, email address, and login password stored as a hash.
Each advertisement stores its identifier (id), title, description, cost, duration, and creation date.

Users are divided into two roles - regular users with the USER role and administrators with the ADMIN role. Regular users can only edit data for their own account and advertisements (and delete them).
An administrator has access to edit and delete all user accounts and advertisements. However, they cannot delete their own account.

For verification purposes, each newly created user must verify themselves through a code sent to their email address. This functionality requires connecting the project to the developer's email account.
Authorization and authentication in the application are conducted through JWT tokens.
Authorization and authentication in the application are conducted through HTTP cookies.

The application loads default values into the database, containing three administrator users, seven regular users, and thirty advertisements.
The application is set to automatically delete and regenerate the database structure and its data at each startup.
To manage Asp.Net Core project packages, it uses the NuGet tool.

The application also includes performance tests written using k6 technology.
