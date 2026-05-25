// API_URL não é acessível aqui (client-side)
// Use /api/proxy como base para requisições

export async function postAsync(url: string, data: any) {
    const response = await fetch(`/api/proxy/${url}`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(data),
    });
    return response;
}

export async function getAsync(url: string) {
    
    const response = await fetch(`/api/proxy/${url}`);
    return response;
}

export async function putAsync(url: string, data: unknown) {
    const response = await fetch(`/api/proxy/${url}`, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(data),
    });
    return response;
}

export async function deleteAsync(url: string) {
    const response = await fetch(`/api/proxy/${url}`, {
        method: 'DELETE',
        headers: {
            'Content-Type': 'application/json',
        },
    });
    return response;
}

