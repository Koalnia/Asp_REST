import http from 'k6/http';
import { check, sleep, group } from 'k6';
import { SharedArray } from 'k6/data';
import { randomIntBetween } from 'https://jslib.k6.io/k6-utils/1.2.0/index.js';

export const options = {
    stages: [
        { duration: '5s', target: 4 }, // Rozgrzewka: 4 użytkowników w 5 sekund
        { duration: '20s', target: 10 }, // Obciążenie: 10 użytkowników przez 20 sekund
        { duration: '5s', target: 0 }, // Redukcja: do 0 użytkowników w 5 sekund
    ],
    thresholds: {
        http_req_duration: ['p(95)<3000'], // 95% requestów musi być poniżej 3s
        http_req_failed: ['rate<0.1'], // Poziom błędów poniżej 10%
    },
};


const BASE_URL = 'http://localhost:5257'; // Zmieniono na endpoint API
let authToken = ''; // Zmienna do przechowywania JWT
let title = 'Oddam';

// Funkcja generująca losowe dane
function generateRandomPrice() {
    let randomNumber = Math.floor(Math.random() * 9999) + 1; // Losowa liczba od 1 do 9999
    return `${randomNumber} zł`; // Łączenie liczby z "zł"
}

const users = new SharedArray('users', function () {
    return [
        { email: 'admin1@pl', password: 'Hasloha11!' },
        { email: 'anna.kowalska@gmail.com', password: 'Password123!' },
        { email: 'jan.nowak@gmail.com', password: 'Qwerty456!' },
        { email: 't.zielinski@gmail.com', password: 'Pass1234!' }
    ];
});
const advertisementIds = [12, 13, 14, 15];

export default function () {
 
    const selectedUser = users[randomIntBetween(0, users.length - 1)];
    group('Logowanie', function () {

        // 1. Test logowania i pobrania tokenu JWT
        const loginUrl = `${BASE_URL}/auth/login`;
        const loginPayload = JSON.stringify({
            email: selectedUser.email,
            password: selectedUser.password,
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
        //console.log('Token JWT:', authToken);
        sleep(randomIntBetween(1, 2));
    });

        // Przygotowanie nagłówków z tokenem JWT
        const authHeaders = {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${authToken}`
        };
    group('Przeglądanie', function () {
        // 2. Test pobrania listy ogłoszeń
        const advertisementsResponse = http.get(`${BASE_URL}/advertisement`, {
            headers: authHeaders
        });

        check(advertisementsResponse, {
            'Pomyślne załadowanie listy ogłoszeń': (r) => r.status === 200
        });

        //console.log('Lista ogłoszeń:', advertisementsResponse); 
        sleep(randomIntBetween(1, 3));

        // 3. Test pobrania listy użytkowników
        const usersResponse = http.get(`${BASE_URL}/user`, {
            headers: authHeaders
        });

        check(usersResponse, {
            'Pomyślne załadowanie listy użytkowników': (r) => r.status === 200
        });

        //console.log('Lista użytkowników:', usersResponse); 
        sleep(randomIntBetween(1, 2));
    });
    group('Edycja ogłoszenia', function () {
        const advertisementId = advertisementIds[randomIntBetween(0, advertisementIds.length - 1)];

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
        sleep(randomIntBetween(2, 4));
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

        sleep(randomIntBetween(1, 3));


        const titleSearchResponse = http.get(
            `${BASE_URL}/advertisement/title?title=${title}`,
            { headers: authHeaders }
        );

        check(titleSearchResponse, {
            'Pomyślne listy ogłoszeń o tytułach': (r) => r.status === 200
        });

        //console.log('Pobrano ogłoszenia o tytułach :', titleSearchResponse);

        sleep(randomIntBetween(1, 3));
    });
}