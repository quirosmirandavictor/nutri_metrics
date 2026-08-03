import { httpClient } from "../../../lib/httpClient";
export async function login(payload) {
    const { data } = await httpClient.post("/Auth/login", payload);
    return data;
}
export async function register(payload) {
    const { data } = await httpClient.post("/Auth/register", payload);
    return data;
}
