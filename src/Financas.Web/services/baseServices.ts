import { IS_DESKTOP } from "@/utils/appMode";

const BASE_URL = IS_DESKTOP ? '' : process.env.NEXT_PUBLIC_API_URL


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


/**
 * Erros de negócio da API chegam como texto puro (BadRequest(string)); os de validação de modelo do
 * ASP.NET chegam como ProblemDetails em JSON. Devolve a mensagem legível de qualquer um dos dois.
 */
export async function readErrorAsync(response: Response, fallback: string): Promise<string> {
    const text = await response.text().catch(() => '');
    if (!text) return fallback;

    try {
        const data = JSON.parse(text);
        if (typeof data === 'string') return data;
        return data?.message || data?.title || fallback;
    } catch {
        return text;
    }
}
