//  A script to stress your API

import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
    stages: [
        { duration: '15s', target: 3000 },
    ]
};

export default function () {
    const res = http.get('<empty>');
    check(res, { 'status was 200': (r) => r.status == 200 });
    sleep(1);
}
