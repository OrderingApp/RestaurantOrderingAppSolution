import Keycloak from 'keycloak-js';

const API_BASE_URL =
    process.env.NEXT_PUBLIC_API_BASE_URL ?? 'http://localhost:5000/api';

export type AuthErrorCode =
    | 'missing_env'
    | 'network'
    | 'backend_unavailable'
    | 'invalid_pin'
    | 'unexpected';

export class AuthClientError extends Error {
    code: AuthErrorCode;
    status?: number;

    constructor(code: AuthErrorCode, message: string, status?: number) {
        super(message);
        this.code = code;
        this.status = status;
    }
}

// TODO(auth): Replace fallbacks with real env values from deployment config.
// Required vars:
// - NEXT_PUBLIC_KEYCLOAK_URL (depends on Keycloak distro, e.g. http://localhost:8080 or http://localhost:8080/auth)
// - NEXT_PUBLIC_KEYCLOAK_REALM (must match backend realm)
// - NEXT_PUBLIC_KEYCLOAK_CLIENT_ID (must be SPA/public client, not backend confidential API client)
const KEYCLOAK_URL =
    process.env.NEXT_PUBLIC_KEYCLOAK_URL ?? 'http://localhost:8080';
const KEYCLOAK_REALM = process.env.NEXT_PUBLIC_KEYCLOAK_REALM ?? 'OrderApp';
const KEYCLOAK_CLIENT_ID =
    process.env.NEXT_PUBLIC_KEYCLOAK_CLIENT_ID ?? 'order-api';

let keycloakClient: Keycloak | null = null;

const REQUIRED_AUTH_ENV_VARS = [
    'NEXT_PUBLIC_API_BASE_URL',
    'NEXT_PUBLIC_KEYCLOAK_URL',
    'NEXT_PUBLIC_KEYCLOAK_REALM',
    'NEXT_PUBLIC_KEYCLOAK_CLIENT_ID',
] as const;

export const getMissingAuthEnvVars = () =>
    REQUIRED_AUTH_ENV_VARS.filter((envVar) => !process.env[envVar]);

const toAuthClientError = (error: unknown) => {
    if (error instanceof AuthClientError) {
        return error;
    }

    if (error instanceof TypeError) {
        return new AuthClientError(
            'network',
            'Network error while calling auth endpoint'
        );
    }

    return new AuthClientError('unexpected', 'Unexpected auth client error');
};

const ensureAuthEnvConfiguration = () => {
    const missingEnvVars = getMissingAuthEnvVars();

    if (missingEnvVars.length) {
        throw new AuthClientError(
            'missing_env',
            `Missing required env vars: ${missingEnvVars.join(', ')}`
        );
    }
};

const ensureClientInitialized = async (client: Keycloak) => {
    if (client.didInitialize) {
        return;
    }

    await client.init({
        onLoad: 'check-sso',
        pkceMethod: 'S256',
        checkLoginIframe: false,
    });
};

export interface BackendSessionStatus {
    isDeviceAssigned: boolean;
    user?: {
        sub?: string;
        preferredUsername?: string;
        email?: string;
        givenName?: string;
        familyName?: string;
        issuer?: string;
        realm?: string;
    };
}

const getClient = () => {
    if (keycloakClient) {
        return keycloakClient;
    }

    keycloakClient = new Keycloak({
        url: KEYCLOAK_URL,
        realm: KEYCLOAK_REALM,
        clientId: KEYCLOAK_CLIENT_ID,
    });

    return keycloakClient;
};

const getCallbackPath = () => {
    if (typeof window === 'undefined') {
        return '/';
    }

    return `${window.location.origin}${window.location.pathname}`;
};

const removeAuthParamsFromUrl = () => {
    if (typeof window === 'undefined') {
        return;
    }

    const cleanUrl = `${window.location.pathname}${window.location.hash}`;
    window.history.replaceState({}, document.title, cleanUrl);
};

export const hasKeycloakAuthParams = () => {
    if (typeof window === 'undefined') {
        return false;
    }

    const params = new URLSearchParams(window.location.search);

    return params.has('code') && params.has('state');
};

const readSessionResponse = async (response: Response) => {
    if (response.status === 204) {
        return null;
    }

    return (await response.json()) as BackendSessionStatus;
};

export const fetchBackendSession = async () => {
    try {
        const response = await fetch(`${API_BASE_URL}/auth/session`, {
            credentials: 'include',
        });

        if (response.status === 401 || response.status === 403) {
            return null;
        }

        if (!response.ok) {
            throw new AuthClientError(
                'backend_unavailable',
                'Failed to fetch auth session',
                response.status
            );
        }

        return readSessionResponse(response);
    } catch (error) {
        throw toAuthClientError(error);
    }
};

export const clearBackendSession = async () => {
    try {
        const response = await fetch(`${API_BASE_URL}/auth/session`, {
            method: 'DELETE',
            credentials: 'include',
        });

        if (
            !response.ok &&
            response.status !== 401 &&
            response.status !== 403
        ) {
            throw new AuthClientError(
                'backend_unavailable',
                'Failed to clear auth session',
                response.status
            );
        }
    } catch (error) {
        throw toAuthClientError(error);
    }
};

export const startKeycloakLogin = async () => {
    try {
        ensureAuthEnvConfiguration();

        const client = getClient();

        await ensureClientInitialized(client);

        // TODO(auth): In Keycloak client settings, allow this redirect URI pattern:
        // http://localhost:3000/* (and production domain equivalents).
        // Also set Web Origins to http://localhost:3000 (and production origins).
        await client.login({
            redirectUri: getCallbackPath(),
        });
    } catch (error) {
        throw toAuthClientError(error);
    }
};

export const completeKeycloakLogin =
    async (): Promise<BackendSessionStatus | null> => {
        try {
            ensureAuthEnvConfiguration();

            const client = getClient();
            await ensureClientInitialized(client);
            const authenticated = client.authenticated;

            if (!authenticated || !client.token) {
                return null;
            }

            const response = await fetch(
                `${API_BASE_URL}/auth/keycloak/session`,
                {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        Authorization: `Bearer ${client.token}`,
                    },
                    credentials: 'include',
                }
            );

            if (!response.ok) {
                throw new AuthClientError(
                    'backend_unavailable',
                    'Failed to exchange keycloak token for app session',
                    response.status
                );
            }

            return readSessionResponse(response);
        } catch (error) {
            throw toAuthClientError(error);
        } finally {
            removeAuthParamsFromUrl();
        }
    };

export const loginWithPin = async (pin: string) => {
    try {
        const response = await fetch(`${API_BASE_URL}/auth/pin/session`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            credentials: 'include',
            body: JSON.stringify({ pin }),
        });

        if (response.status === 401 || response.status === 403) {
            throw new AuthClientError('invalid_pin', 'Invalid PIN');
        }

        if (!response.ok) {
            throw new AuthClientError(
                'backend_unavailable',
                'Failed to create PIN session',
                response.status
            );
        }

        return readSessionResponse(response);
    } catch (error) {
        throw toAuthClientError(error);
    }
};
