import { NextRequest, NextResponse } from 'next/server';

async function proxyRequest(
  request: NextRequest,
  params: Promise<{ path: string[] }>,
  method: string
) {
  // Ignora erros de SSL autoassinado (certificado Kestrel/IIS local) em ambiente de desenvolvimento
  process.env.NODE_TLS_REJECT_UNAUTHORIZED = '0';
  try {
    const { path } = await params;
    const API_URL = process.env.API_URL;
    const WAGE_API_URL = process.env.WAGE_API_URL || 'http://localhost:5013';

    if (!API_URL) {
      return NextResponse.json(
        { error: 'API_URL não configurada' },
        { status: 500 }
      );
    }

    const pathString = path.join('/');
    const isWageRoute = path.some(p => p.toLowerCase() === 'wage');
    const baseUrl = isWageRoute ? WAGE_API_URL : API_URL;
    const url = `${baseUrl}/${pathString}`;

    const init: RequestInit = {
      method,
      headers: {
        'Content-Type': 'application/json',
      },
    };

    if (method !== 'GET' && method !== 'DELETE') {
      const body = await request.text();
      if (body) {
        init.body = body;
      }
    }

    const response = await fetch(url, init);

    if (!response.ok) {
      const error = await response.text();
      console.error('API Error:', error);
      return NextResponse.json(
        { error: 'API request failed', details: error, message: error },
        { status: response.status }
      );
    }

    const contentType = response.headers.get('content-type');
    if (contentType?.includes('application/json')) {
      const data = await response.json();
      return NextResponse.json(data, { status: response.status });
    }

    return new NextResponse(null, { status: response.status });
  } catch (error) {
    console.error('Proxy error:', error);
    return NextResponse.json(
      { error: 'Internal server error', details: String(error) },
      { status: 500 }
    );
  }
}

export async function GET(
  request: NextRequest,
  context: { params: Promise<{ path: string[] }> }
) {
  return proxyRequest(request, context.params, 'GET');
}

export async function POST(
  request: NextRequest,
  context: { params: Promise<{ path: string[] }> }
) {
  return proxyRequest(request, context.params, 'POST');
}

export async function PUT(
  request: NextRequest,
  context: { params: Promise<{ path: string[] }> }
) {
  return proxyRequest(request, context.params, 'PUT');
}

export async function DELETE(
  request: NextRequest,
  context: { params: Promise<{ path: string[] }> }
) {
  return proxyRequest(request, context.params, 'DELETE');
}
