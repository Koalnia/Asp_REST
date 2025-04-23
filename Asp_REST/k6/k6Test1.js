import http from 'k6/http';
import { check, sleep } from 'k6';


const BASE_URL = 'http://localhost:5257'; // Zmieniono na endpoint API
let authToken = ''; // Zmienna do przechowywania JWT
let advertisementId = 15;
let title = 'Oddam';

// Funkcja generująca losowe dane
function generateRandomPrice() {
    let randomNumber = Math.floor(Math.random() * 9999) + 1; // Losowa liczba od 1 do 9999
    return `${randomNumber} zł`; // Łączenie liczby z "zł"
}


export default function () {
    // Dane testowe
    const email = 'admin1@pl'; // Stały email do testów
    const password = 'Hasloha11!'; // Stałe hasło do testów

    // 1. Test logowania i pobrania tokenu JWT
    const loginUrl = `${BASE_URL}/auth/login`;
    const loginPayload = JSON.stringify({
        email: email,
        password: password
    });

    const loginParams = {
        headers: {
            'Content-Type': 'application/json',
        },
    };

    const loginResponse = http.post(loginUrl, loginPayload, loginParams);

    check(loginResponse, {
        'Logowanie zakończone sukcesem': (r) => r.status === 200,
        'Token JWT otrzymany': (r) => r.json('token') !== undefined,
    });

    //console.log('LoginResponse:', loginResponse);

    // Pobranie tokenu JWT z odpowiedzi
    if (loginResponse.status === 200) {
        try {
            authToken = loginResponse.json('token');
        } catch (e) {
            console.log('Błąd podczas parsowania tokenu JWT:', e);
        }
    }
    console.log('Token JWT:', authToken);

    // Przygotowanie nagłówków z tokenem JWT
    const authHeaders = {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${authToken}`
    };

    // 2. Test pobrania listy ogłoszeń
    const advertisementsResponse = http.get(`${BASE_URL}/advertisement`, {
        headers: authHeaders
    });

    check(advertisementsResponse, {
        'Pomyślne załadowanie listy ogłoszeń': (r) => r.status === 200
    });

    //console.log('Lista ogłoszeń:', advertisementsResponse); 

    // 3. Test pobrania listy użytkowników
    const usersResponse = http.get(`${BASE_URL}/user`, {
        headers: authHeaders
    });

    check(usersResponse, {
        'Pomyślne załadowanie listy użytkowników': (r) => r.status === 200
    });

    //console.log('Lista użytkowników:', usersResponse); 

    // 4. Test edycji ogłoszenia
    const editAdvertisementPayload = JSON.stringify({
        title: 'Oddam regały na książki',
        description: 'Dwa takie same regały z IKEA, 5 półek, wymiary 200x80x30 cm. Stan bardzo dobry.',
        price: generateRandomPrice(),
        duration: 'Brak'
    });

    const editResponse = http.patch(
        `${BASE_URL}/advertisement/${advertisementId}`,
        editAdvertisementPayload,
        { headers: authHeaders }
    );

    check(editResponse, {
        'Pomyślna edycja ogłoszenia': (r) => r.status === 200
    });

    //console.log('Ogłoszenie po edycji :', editResponse); 

    // 5. Test pobrania pojedynczego ogłoszenia
    const singleAdvertisementResponse = http.get(
        `${BASE_URL}/advertisement/${advertisementId}`,
        { headers: authHeaders }
    );

    check(singleAdvertisementResponse, {
        'Pomyślne pobranie pojedynczego ogłoszenia': (r) => r.status === 200
    });

    //console.log('Pobrane ogłoszenie  :', singleAdvertisementResponse); 


    // 6. Test wyszukania ogłoszeń po nazwie
    const titleSearchResponse = http.get(
        `${BASE_URL}/advertisement/title?title=${title}`,
        { headers: authHeaders }
    );

    check(titleSearchResponse, {
        'Pomyślne listy ogłoszeń o tytułach': (r) => r.status === 200
    });

    //console.log('Pobrano ogłoszenia o tytułach :', titleSearchResponse);

    sleep(1);
}