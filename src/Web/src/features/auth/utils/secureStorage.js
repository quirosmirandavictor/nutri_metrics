/**
 * Encrypted session persistence.
 *
 * IMPORTANT (read before touching this file):
 * This is NOT a replacement for an httpOnly cookie. It is "obfuscation with
 * real encryption" intended to raise the cost of passive exfiltration
 * (storage dumps, logs, extensions), not to withstand XSS with JavaScript
 * execution, because the decryption key must remain accessible in the same
 * origin so the app can use it. The real XSS mitigation is the backend
 * (httpOnly cookies) and frontend CSP.
 *
 * In addition to encryption, the app never uses this storage as a source of
 * truth in real time: it is read only once when the app mounts
 * (restoreSession), and after that the token lives in memory (AuthContext).
 */
// Deliberately non-descriptive key names: they avoid trivial grep for
// "token"/"auth" in storage. This is not real security, just noise reduction.
const CIPHER_KEY_NAME = "__nm_a";
const KEY_MATERIAL_NAME = "__nm_b";
function bufferToBase64(buffer) {
    return btoa(String.fromCharCode(...new Uint8Array(buffer)));
}
function base64ToBuffer(base64) {
    const binary = atob(base64);
    const bytes = new Uint8Array(binary.length);
    for (let i = 0; i < binary.length; i++) {
        bytes[i] = binary.charCodeAt(i);
    }
    return bytes.buffer;
}
async function generateKey() {
    return crypto.subtle.generateKey({ name: "AES-GCM", length: 256 }, true, [
        "encrypt",
        "decrypt"
    ]);
}
async function exportKey(key) {
    const raw = await crypto.subtle.exportKey("raw", key);
    return bufferToBase64(raw);
}
async function importKey(base64Key) {
    return crypto.subtle.importKey("raw", base64ToBuffer(base64Key), { name: "AES-GCM", length: 256 }, false, ["decrypt"]);
}
export async function persistSession(session) {
    const key = await generateKey();
    const iv = crypto.getRandomValues(new Uint8Array(12));
    const plaintext = new TextEncoder().encode(JSON.stringify(session));
    const ciphertext = await crypto.subtle.encrypt({ name: "AES-GCM", iv }, key, plaintext);
    const payload = {
        iv: bufferToBase64(iv.buffer),
        data: bufferToBase64(ciphertext)
    };
    sessionStorage.setItem(CIPHER_KEY_NAME, JSON.stringify(payload));
    sessionStorage.setItem(KEY_MATERIAL_NAME, await exportKey(key));
}
export async function restoreSession() {
    const rawPayload = sessionStorage.getItem(CIPHER_KEY_NAME);
    const rawKey = sessionStorage.getItem(KEY_MATERIAL_NAME);
    if (!rawPayload || !rawKey) {
        return null;
    }
    try {
        const { iv, data } = JSON.parse(rawPayload);
        const key = await importKey(rawKey);
        const plaintext = await crypto.subtle.decrypt({ name: "AES-GCM", iv: base64ToBuffer(iv) }, key, base64ToBuffer(data));
        const session = JSON.parse(new TextDecoder().decode(plaintext));
        if (session.expiresAt <= Date.now()) {
            clearSession();
            return null;
        }
        return session;
    }
    catch {
        // Corrupt payload or invalid key: do not risk it, clear and re-login.
        clearSession();
        return null;
    }
}
export function clearSession() {
    sessionStorage.removeItem(CIPHER_KEY_NAME);
    sessionStorage.removeItem(KEY_MATERIAL_NAME);
}
