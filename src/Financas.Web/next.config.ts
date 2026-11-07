import type { NextConfig } from 'next';

const desktopConfig: NextConfig = {
  output: 'export',
  trailingSlash: true,
  images: { unoptimized: true },
};

const serverConfig: NextConfig = {
  async rewrites() {
    return [
      {
        source: '/api/:path*',
        destination: 'http://localhost:1337/api/:path*',
      },
    ];
  },
};

export default process.env.NEXT_PUBLIC_APP_MODE === 'desktop' ? desktopConfig : serverConfig;