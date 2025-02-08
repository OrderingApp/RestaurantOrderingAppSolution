export enum API {
    URL = "http://localhost:5000/api",
    Tables = 'Table',
}

const apiClient = async <T>(endpoint: Exclude<API, "URL">): Promise<T> => {
    const res = await fetch(`${API.URL}/${endpoint}`);

    if (!res.ok)
        throw new Error(`Failed to fetch ${endpoint}: ${res.statusText}`);

    return await res.json();
}

export default apiClient;