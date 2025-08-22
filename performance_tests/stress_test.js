import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
  stages: [
    { duration: "30s", target: 100 },   // ramp-up initiale à 100 VUs
    { duration: "1m", target: 500 },    // montée rapide à 500 VUs
    { duration: "1m", target: 1000 },   // palier à 1000 VUs
    { duration: "30s", target: 0 },     // ramp-down progressif
  ],
  thresholds: {
    http_req_duration: ["p(95)<1000"], // 95% des requêtes doivent répondre < 1000ms
    http_req_failed: ["rate<0.05"],    // tolérance de 5% d'erreurs
  },
};

export default function () {
  // Login
  let loginRes = http.post(
    'http://host.docker.internal:8000/gateway/auth/login',
    JSON.stringify({
      email: 'superadmin@gmail.com',
      password: 'Super123',
    }),
    { headers: { 'Content-Type': 'application/json' } }
  );

  check(loginRes, { 'login status 200': (r) => r.status === 200 });
  let token = loginRes.json('accessToken');

  if (!token) {
    console.error("❌ Token non reçu, arrêt du scénario");
    return;
  }

  let params = {
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json',
    },
  };

  // Get Users
  let usersRes = http.get('http://host.docker.internal:8000/gateway/auth/users', params);
  check(usersRes, {
    'get users status 200': (r) => r.status === 200,
    'users is array': (r) => Array.isArray(r.json()),
  });

  // Get Appointments
  let getAppointmentsRes = http.get('http://host.docker.internal:8000/gateway/appointments', params);
  check(getAppointmentsRes, {
    'get appointments status 200': (r) => r.status === 200,
    'appointments is array': (r) => Array.isArray(r.json()),
  });

  sleep(1);
}
