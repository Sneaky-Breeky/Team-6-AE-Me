import { API_BASE_URL } from "./apiURL.js"

const AUTH_URL = `${API_BASE_URL}/api/auth`;
// const LOCAL_AUTH_URL = `${LOCAL_AUTH_URL}/api/auth`;
/**
 * Login user
 * @param {string} email 
 * @param {string} password 
 * @returns {Promise<Response>}
 */
export async function loginUser(email, password) {
    const response = await fetch(`${AUTH_URL}/login`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({ email, password })
    });

    if (!response.ok) {
        // Handle errors and parse JSON safely
        const errorData = await response.json().catch(() => ({ error: "Unknown error" }));
        return errorData;
    }

    return response.json();
}

export async function fetchUsers() {
    try {
        const response = await fetch(`${AUTH_URL}/fetchusers`, {
            method: "GET",
            headers: {
                "Content-Type": "application/json"
            }
        });

        console.log(`${AUTH_URL}/fetchusers`);

        if (!response.ok) {
            // Try to parse error response JSON, but catch errors safely
            const errorText = await response.text();
            try {
                const errorData = JSON.parse(errorText);
                return { error: errorData.message || "Unknown error" };
            } catch {
                return { error: `HTTP Error ${response.status}: ${errorText}` };
            }
        }

        // Try parsing JSON safely
        return await response.json();
    } catch (error) {
        console.error("Network or fetch error:", error);
        return { error: "Network error or server unreachable", message : error.message };
    }
}


export async function addUser(values) {
    try {
        const response = await fetch(`${AUTH_URL}/addUser`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(values)
        });

        if (!response.ok) {
            // Attempt to extract error details
            const errorData = await response.json().catch(() => ({ error: "Unknown error" }));
            throw new Error(errorData.error || "Failed to add user.");
        }

        return await response.json();
    } catch (err) {
        console.error("Error adding user:", err);
        throw err;
    }
}

export async function deleteUser(email) {
    try {
        const response = await fetch(`${AUTH_URL}/deleteuser/${email}`, {
            method: "DELETE",
            headers: { "Content-Type": "application/json" }
        });

        if (!response.ok) {
            const errorData = await response.json().catch(() => ({ error: "Unknown error" }));
            throw new Error(errorData.error || "Failed to delete user.");
        }

        return await response.json();
    } catch (err) {
        console.error("Error deleting user:", err);
        throw err;
    }
}