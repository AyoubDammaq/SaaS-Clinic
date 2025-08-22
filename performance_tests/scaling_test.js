import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
  stages: [
    { duration: "30s", target: 20 },   // ramp-up à 20 VUs en 30s
    { duration: "1m", target: 50 },    // palier à 50 VUs pendant 1 min
    { duration: "1m", target: 100 },   // scaling à 100 VUs pendant 1 min
    { duration: "30s", target: 0 },    // ramp-down
  ],
  thresholds: {
    http_req_duration: ["p(95)<500"], // 95% des requêtes doivent répondre < 500ms
    http_req_failed: ["rate<0.01"],   // Moins de 1% d'erreurs tolérées
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
