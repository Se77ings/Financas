const BASE_URL = process.env.NEXT_PUBLIC_API_URL


export async function postAsync(url: string, data: any) {
    const response = await fetch(`${BASE_URL}/${url}`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(data),
    });
    return response;
}

export async function getAsync(url: string) {

    const response = await fetch(`${BASE_URL}/${url}`);
    return response;
}

export async function putAsync(url: string, data: unknown) {
    const response = await fetch(`${BASE_URL}/${url}`, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(data),
    });
    return response;
}

export async function deleteAsync(url: string) {
    const response = await fetch(`${BASE_URL}/${url}`, {
        method: 'DELETE',
        headers: {
            'Content-Type': 'application/json',
        },
    });
    return response;
}

