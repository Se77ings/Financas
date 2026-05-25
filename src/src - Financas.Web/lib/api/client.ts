/**
 * Em desenvolvimento com next.config.ts rewrites ativos,
 * usamos path relativo ('/api/...') que é interceptado pelo proxy do Next.js.
 * Em produção, defina NEXT_PUBLIC_API_URL para apontar direto à API .NET.
 */
const API_BASE_URL =
  typeof process !== 'undefined' && process.env.NEXT_PUBLIC_API_URL
    ? process.env.NEXT_PUBLIC_API_URL
    : '';

export class ApiError extends Error {
  constructor(
    public status: number,
    message: string
  ) {
    super(message);
    this.name = 'ApiError';
  }
}

async function handleResponse<T>(res: Response): Promise<T> {
  if (!res.ok) {
    const text = await res.text().catch(() => res.statusText);
    throw new ApiError(res.status, text || `HTTP ${res.status}`);
  }
  const text = await res.text();
  return text ? (JSON.parse(text) as T) : (undefined as unknown as T);
}

export const apiClient = {
  get: async <T>(path: string): Promise<T> => {
    const res = await fetch(`${API_BASE_URL}${path}`, {
      headers: { 'Content-Type': 'application/json' },
    });
    return handleResponse<T>(res);
  },

  post: async <T>(path: string, body: unknown): Promise<T> => {
    const res = await fetch(`${API_BASE_URL}${path}`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(body),
    });
    return handleResponse<T>(res);
  },

  put: async <T>(path: string, body: unknown): Promise<T> => {
    const res = await fetch(`${API_BASE_URL}${path}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(body),
    });
    return handleResponse<T>(res);
  },

  delete: async <T>(path: string): Promise<T> => {
    const res = await fetch(`${API_BASE_URL}${path}`, {
      method: 'DELETE',
      headers: { 'Content-Type': 'application/json' },
    });
    return handleResponse<T>(res);
  },
};
